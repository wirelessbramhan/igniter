using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using Gpp.Core;
using Gpp.Datadog;
using Gpp.Extension;
using Gpp.Log;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Gpp.Network
{
    public class GppHttpClient
    {
        private enum RequestState
        {
            Invalid = 0,
            Running,
            Paused,
            Resumed,
            Stopped
        }

        private Uri _baseUri;
        private uint _totalTimeoutMs;
        private uint _initialDelayMs;
        private uint _maxDelayMs;
        private string _clientId;
        private string _clientSecret;
        private string _accessToken;
        private bool _isBanDetected;
        private bool _isRequestingNewAccessToken;

        private readonly IDictionary<string, string> _pathParams = new Dictionary<string, string>();

        public event Action<IHttpRequest> ServerErrorOccured;
        public event Action<IHttpRequest> NetworkErrorOccured;
        public event Action<string, Action<string>> BearerAuthRejected;
        public event Action<string> UnauthorizedOccured;

        public GppHttpClient()
        {
            SetRetryParameters();
        }

        public void SetCredentials(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public void SetImplicitPathParams(IDictionary<string, string> pathParams)
        {
            foreach (var param in pathParams)
            {
                _pathParams[param.Key] = param.Value;
            }
        }

        public void ClearImplicitPathParams()
        {
            _pathParams.Clear();
        }

        public void ClearCookies()
        {
            if (_baseUri != null)
            {
                UnityWebRequest.ClearCookieCache(_baseUri);
            }
        }

        public void SetBaseUri(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public void OnBearerAuthRejected(Action<string> callback)
        {
            PauseBearerAuthRequest();
            if (IsRequestingNewAccessToken()) return;

            if (BearerAuthRejected == null)
            {
                callback?.Invoke(null);
            }
            else
            {
                SetRequestingNewAccessToken(true);
                BearerAuthRejected?.Invoke(_accessToken, result =>
                    {
                        SetRequestingNewAccessToken(false);
                        if (result != null)
                        {
                            ResumeBearerAuthRequest();
                        }

                        callback?.Invoke(result);
                    }
                );
            }
        }

        public void SetImplicitBearerToken(string accessToken)
        {
            _accessToken = accessToken;
        }

        public void SetRetryParameters(uint totalTimeoutMs = 60000, uint initialDelayMs = 1000, uint maxDelayMs = 30000)
        {
            _totalTimeoutMs = totalTimeoutMs;
            _initialDelayMs = initialDelayMs;
            _maxDelayMs = maxDelayMs;
        }

        public IEnumerator SendRequest(IHttpRequest request, Action<IHttpResponse, Error> callback)
        {
            var rand = new Random();
            uint retryDelayMs = _initialDelayMs;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            ApplyImplicitAuthorization(request);
            ApplyImplicitPathParams(request);

            if (!TryResolveUri(request))
            {
                callback?.Invoke(null, new Error(ErrorCode.InvalidRequest, "Invalid uri format: ", request.Url));
                yield break;
            }

            IHttpResponse httpResponse = null;
            Error error = null;

            RequestState state;
            if (IsBearerAuthRequestPaused() && request.AuthType == HttpAuth.Bearer)
            {
                state = RequestState.Paused;
            }
            else
            {
                state = RequestState.Running;
            }

            do
            {
                if (state == RequestState.Paused)
                {
                    if (IsBearerAuthRequestPaused())
                    {
                        OnBearerAuthRejected(result => { });
                        yield return new WaitWhile(() => IsBearerAuthRequestPaused() && IsRequestingNewAccessToken());

                        if (IsBearerAuthRequestPaused())
                        {
                        }
                    }

                    state = RequestState.Resumed;
                    stopwatch.Restart();
                    if (IsBearerAuthRequestPaused())
                    {
                        request.Headers.Remove("Authorization");
                        ApplyImplicitAuthorization(request);
                    }
                }

                yield return Send(request, (rsp, err) => (httpResponse, error) = (rsp, err), 10000);

                switch ((HttpStatusCode)httpResponse.Code)
                {
                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.BadGateway:
                    case HttpStatusCode.ServiceUnavailable:
                    case HttpStatusCode.GatewayTimeout:
                    case HttpStatusCode.RequestTimeout:
                        ServerErrorOccured?.Invoke(request);
                        int jitterMs = rand.Next((int)(-retryDelayMs / 4), (int)(retryDelayMs / 4));
                        yield return new WaitForSeconds((retryDelayMs + jitterMs) / 1000f);
                        retryDelayMs = Math.Min(retryDelayMs * 2, _maxDelayMs);
                        break;
                    case HttpStatusCode.Unauthorized:
                        if (state == RequestState.Resumed || request.AuthType != HttpAuth.Bearer)
                        {
                            state = RequestState.Stopped;
                            callback?.Invoke(httpResponse, error);
                            UnauthorizedOccured?.Invoke(_accessToken);
                            ResumeBearerAuthRequest();
                            SetRequestingNewAccessToken(false);
                            yield break;
                        }

                        state = RequestState.Paused;
                        OnBearerAuthRejected(result =>
                        {
                            GppLog.Log($"Refresh access token completed: {result}");
                        });
                        yield return new WaitWhile(() => IsBearerAuthRequestPaused() && IsRequestingNewAccessToken());
                        state = RequestState.Stopped;
                        callback?.Invoke(httpResponse, error);
                        ResumeBearerAuthRequest();
                        SetRequestingNewAccessToken(false);
                        yield break;

                    default:

                        if (error is { Code: ErrorCode.NetworkError })
                        {
                            state = RequestState.Stopped;
                            NetworkErrorOccured?.Invoke(request);
                            callback?.Invoke(httpResponse, error);
                            ResumeBearerAuthRequest();
                            SetRequestingNewAccessToken(false);
                            yield break;
                        }

                        state = RequestState.Stopped;
                        callback?.Invoke(httpResponse, null);
                        ResumeBearerAuthRequest();
                        SetRequestingNewAccessToken(false);
                        yield break;
                }

                if (request is HttpRequestBuilder.HttpRequestPrototype)
                {
                    var requestPrototype = (HttpRequestBuilder.HttpRequestPrototype)request;
                    if (!requestPrototype.EnableRetry)
                    {
                        break;
                    }
                }

            } while (stopwatch.Elapsed < TimeSpan.FromMilliseconds(_totalTimeoutMs) || IsBearerAuthRequestPaused());
            callback?.Invoke(httpResponse, null);
            ResumeBearerAuthRequest();
            SetRequestingNewAccessToken(false);
        }

        private bool TryResolveUri(IHttpRequest request)
        {
            if (string.IsNullOrEmpty(request.Url)) return false;

            if (request.Url.Contains("{") || request.Url.Contains("}")) return false;

            if (!Uri.TryCreate(_baseUri, request.Url, out Uri uri)) return false;

            if (uri.Scheme != "https" && uri.Scheme != "http") return false;

            request.Url = uri.AbsoluteUri;

            return true;
        }

        private void ApplyImplicitPathParams(IHttpRequest request)
        {
            if (request == null) return;

            if (_pathParams == null || _pathParams.Count == 0) return;

            string formattedUrl = request.Url;

            foreach (var param in _pathParams)
            {
                formattedUrl = formattedUrl.Replace("{" + param.Key + "}", Uri.EscapeDataString(param.Value));
            }

            request.Url = formattedUrl;
        }

        private void ApplyImplicitAuthorization(IHttpRequest request)
        {
            if (request == null) return;

            const string authHeaderKey = "Authorization";

            if (request.Headers.ContainsKey(authHeaderKey)) return;

            switch (request.AuthType)
            {
                case HttpAuth.Basic:
                    if (string.IsNullOrEmpty(_clientId)) return;

                    string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                    request.Headers[authHeaderKey] = "Basic " + base64;
                    break;
                case HttpAuth.Bearer:
                    if (string.IsNullOrEmpty(_accessToken)) return;
                    request.Headers[authHeaderKey] = "Bearer " + _accessToken;
                    break;
            }
        }

        private bool IsBearerAuthRequestPaused()
        {
            return _isBanDetected;
        }

        private void PauseBearerAuthRequest()
        {
            _isBanDetected = true;
        }

        private void ResumeBearerAuthRequest()
        {
            _isBanDetected = false;
        }

        private bool IsRequestingNewAccessToken()
        {
            return _isRequestingNewAccessToken;
        }

        private void SetRequestingNewAccessToken(bool isRequesting)
        {
            _isRequestingNewAccessToken = isRequesting;
        }

        public IEnumerator Send(IHttpRequest request, Action<IHttpResponse, Error> callback, int timeoutMs)
        {
            using UnityWebRequest unityWebRequest = CreateUnityWebRequest(request);
            unityWebRequest.timeout = timeoutMs / 1000;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            GppLog.LogHttpReq(request, unityWebRequest);
            yield return unityWebRequest.SendWebRequest();
            stopwatch.Stop();
            var response = GetHttpResponse(unityWebRequest);

            Error error = null;
            if (Application.internetReachability == NetworkReachability.NotReachable && response.Code == (long)HttpStatusCode.BadRequest)
            {
                error = new Error(ErrorCode.NetworkError);
            }

            if (GppSDK.GetSession() != null && GppSDK.GetSession().RemoteSdkConfig != null)
            {
                int httpResponseSlowTime = GppSDK.GetSession().RemoteSdkConfig.HttpResponseSlowTime;
                if (httpResponseSlowTime != 0)
                {
                    if (stopwatch.ElapsedMilliseconds > httpResponseSlowTime * 1000)
                    {
                        var stackTrace = new StackTrace();
                        var timerLog = new GppResponseTimeLog
                        {
                            UrlPath = unityWebRequest.uri.PathAndQuery,
                            ResponseCode = response.Code,
                            HttpResponseTime = stopwatch.ElapsedMilliseconds * 0.001f,
                            Function = stackTrace.GetFrame(0).GetMethod().ToString()
                        };

                        var remoteLog = new GppClientRemoteLog()
                        {
                            ErrorCode = response.Code.ToString(),
                            Log = timerLog.ToString()
                        };

                        DatadogManager.ReportLog(remoteLog);
                    }
                }
            }

            GppLog.LogHttpRes(unityWebRequest);
            callback?.Invoke(response, error);
        }

        private UnityWebRequest CreateUnityWebRequest(IHttpRequest request)
        {
            var uri = new Uri(request.Url);
            var unityWebRequest = new UnityWebRequest(uri, request.Method);

            if (request.Headers.TryGetValue("Authorization", out string value))
            {
                unityWebRequest.SetRequestHeader("Authorization", value);
            }

            foreach (var headerPair in request.Headers.Where(x => x.Key != "Authorization"))
            {
                unityWebRequest.SetRequestHeader(headerPair.Key, headerPair.Value);
            }

            if (request.BodyBytes != null)
            {
                unityWebRequest.uploadHandler = new UploadHandlerRaw(request.BodyBytes);
            }

            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

            if (request is HttpRequestBuilder.HttpRequestPrototype)
            {
                var requestPrototype = (HttpRequestBuilder.HttpRequestPrototype)request;
                unityWebRequest.redirectLimit = requestPrototype.RedirectCount;
            }

            return unityWebRequest;
        }

        private IHttpResponse GetHttpResponse(UnityWebRequest request)
        {
            if (request.result is UnityWebRequest.Result.ConnectionError)
            {
                long code = (long)HttpStatusCode.BadRequest;

                if (request.error.Contains("timeout"))
                {
                    code = (long)HttpStatusCode.RequestTimeout;
                }

                var responseAdapter = new UnityHttpResponseAdapter()
                {
                    Url = request.url,
                    Code = code,
                    Headers = null,
                    BodyBytes = null
                };

                return responseAdapter;
            }

            return new UnityHttpResponseAdapter
            {
                Url = request.url,
                Code = request.responseCode,
                BodyBytes = request.downloadHandler.data,
                Headers = request.GetResponseHeaders()
            };
        }

        private class UnityHttpResponseAdapter : IHttpResponse
        {
            private IHttpResponse m_HttpResponseImplementation;
            public string Url { get; set; }
            public long Code { get; set; }
            public byte[] BodyBytes { get; set; }

            public IDictionary<string, string> Headers { get; set; }
        }
    }
}