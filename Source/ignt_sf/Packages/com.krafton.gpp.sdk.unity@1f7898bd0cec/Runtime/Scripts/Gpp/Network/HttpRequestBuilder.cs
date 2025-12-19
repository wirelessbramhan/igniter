using System;
using System.Collections.Generic;
using System.Text;
using Gpp.Api;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Gpp.Network
{
    public class HttpRequestBuilder
    {
#if UNITY_ANDROID
        private static readonly string operatingSystem = "Android";
#elif UNITY_IOS
        private static readonly string operatingSystem = "IOS";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private static readonly string operatingSystem = PlatformUtil.IsSteamDeck() ? "SteamOS" : "Windows";
#elif UNITY_STANDALONE_OSX
        private static readonly string operatingSystem = "Mac";
#elif UNITY_GAMECORE_SCARLETT
        private static readonly string operatingSystem = "Xbox";
#elif UNITY_PS5
        private static readonly string operatingSystem = "PS5";
#else
        private static readonly string operatingSystem = "Windows";
#endif

        private readonly StringBuilder formBuilder = new StringBuilder(1024);
        private readonly StringBuilder queryBuilder = new StringBuilder(256);
        private readonly StringBuilder urlBuilder = new StringBuilder(256);
        private HttpRequestPrototype result;

        private int redirectCount = -1;
        private bool ignoreRedirectError;
        // https://wiki.krafton.com/display/GPP/HTTP+Headers 참고
        private static HttpRequestBuilder CreatePrototype(string method, string url)
        {
            var session = GppSDK.GetSession();
            var builder = new HttpRequestBuilder
            {
                result = new HttpRequestPrototype
                {
                    Method = method,
                    Headers =
                    {
                        ["x-gpp-app-id"] = $"{GppSDK.GetConfig()?.Namespace ?? ""};{GppSDK.GetOptions().AppVersion}",
                        ["x-gpp-platform"] = GetPlatform(),
                        ["x-gpp-sdk-version"] = GppSDK.SdkVersion,
                        ["x-gpp-country"] = session.CountryCode,
                        ["x-gpp-ua"] = $"{GppSDK.SdkVersion};{session.LanguageCode};{session.CountryCode};unity;{GppUtil.GetDevice()};{operatingSystem};{GppUtil.GetOSDetail()};{SystemInfo.deviceModel}",
                        ["x-gpp-device-id"] = GppDeviceProvider.GetGppDeviceId(),
                        ["x-gpp-analytics-id"] = session.AnalyticsId,
                        ["x-gpp-store"] = GppSDK.GetOptions().Store.ToString(),
                        ["x-gpp-model"] = SystemInfo.deviceModel,
                        ["x-gpp-lang-code-device"] = PreciseLocale.GetLanguage(),
                        ["x-gpp-country-code-device"] = session.GetSystemCountry(),
                        ["x-gpp-request-id"] = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
                    },
                    EnableRetry = true
                }
            };

            builder.urlBuilder.Append(url);
            return builder;
        }

        private static string GetPlatform()
        {
            string device = SystemInfo.deviceType.ToString();
#if UNITY_ANDROID
            device = "AOS";
#elif UNITY_IOS
            device = "IOS";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            device = "Windows";
#elif UNITY_STANDALONE_OSX
            device = "Mac";
#elif UNITY_PS5
            device = "PS5";
#endif
            return device;
        }

        public HttpRequestBuilder SetRedirect(int numberOfRedirects)
        {
            redirectCount = numberOfRedirects;
            return this;
        }

        public HttpRequestBuilder SetIgnoreRedirectError(bool ignore)
        {
            ignoreRedirectError = ignore;
            return this;
        }

        public static HttpRequestBuilder CreateGet(string url)
        {
            return CreatePrototype("GET", url);
        }

        public static HttpRequestBuilder CreatePost(string url)
        {
            return CreatePrototype("POST", url);
        }

        public static HttpRequestBuilder CreatePut(string url)
        {
            return CreatePrototype("PUT", url);
        }

        public static HttpRequestBuilder CreatePatch(string url)
        {
            return CreatePrototype("PATCH", url);
        }

        public static HttpRequestBuilder CreateDelete(string url)
        {
            return CreatePrototype("DELETE", url);
        }

        public HttpRequestBuilder WithPathParam(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                throw new Exception($"Path parameter with key={key} is null or empty.");
            }

            urlBuilder.Replace("{" + key + "}", Uri.EscapeDataString(value));

            return this;
        }

        public HttpRequestBuilder WithPathParams(IDictionary<string, string> pathParams)
        {
            foreach (var param in pathParams)
            {
                WithPathParam(param.Key, param.Value);
            }

            return this;
        }

        public HttpRequestBuilder WithQueryParam(string key, string value)
        {
            Assert.IsNotNull(key, "query key is null");
            Assert.IsNotNull(value, $"query value is null for key {key}");

            if (queryBuilder.Length > 0)
            {
                queryBuilder.Append("&");
            }

            queryBuilder.Append($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value ?? string.Empty)}");

            return this;
        }

        public HttpRequestBuilder WithQueryParam(string key, ICollection<string> values)
        {
            foreach (string value in values)
            {
                WithQueryParam(Uri.EscapeDataString(key), Uri.EscapeDataString(value));
            }

            return this;
        }

        public HttpRequestBuilder WithQueries(Dictionary<string, string> queryMap)
        {
            foreach (var queryPair in queryMap)
            {
                WithQueryParam(queryPair.Key, queryPair.Value);
            }

            return this;
        }

        public HttpRequestBuilder WithQueries<T>(T queryObject)
        {
            if (queryBuilder.Length > 0)
            {
                queryBuilder.Append("&");
            }

            queryBuilder.Append(queryObject.ToForm());

            return this;
        }

        public HttpRequestBuilder WithBasicAuth()
        {
            result.AuthType = HttpAuth.Basic;

            return this;
        }

        public HttpRequestBuilder WithBasicAuth(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username and password for Basic Authorization shouldn't be empty or null");
            }

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            result.Headers["Authorization"] = "Basic " + credentials;
            result.AuthType = HttpAuth.Basic;

            return this;
        }

        public HttpRequestBuilder WithBasicAuth(string clientID)
        {
            if (string.IsNullOrEmpty(clientID))
            {
                throw new ArgumentException("ClientID for Basic Authorization shouldn't be empty or null");
            }

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientID));
            if (credentials.EndsWith("="))
            {
                credentials = credentials.Remove(credentials.Length - 1);
            }

            result.Headers["Authorization"] = "Basic " + credentials;
            result.AuthType = HttpAuth.Basic;

            return this;
        }

        public HttpRequestBuilder WithBearerAuth()
        {
            result.AuthType = HttpAuth.Bearer;

            return this;
        }

        public HttpRequestBuilder WithBearerAuth(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("token for Bearer Authorization shouldn't be empty or null");
            }

            result.Headers["Authorization"] = "Bearer " + token;
            result.AuthType = HttpAuth.Bearer;

            return this;
        }

        public HttpRequestBuilder WithContentType(MediaType mediaType)
        {
            result.Headers["Content-Type"] = mediaType.ToString();

            return this;
        }

        public HttpRequestBuilder WithContentType(string rawMediaType)
        {
            result.Headers["Content-Type"] = rawMediaType;

            return this;
        }

        public HttpRequestBuilder Accepts(MediaType mediaType)
        {
            result.Headers["Accept"] = mediaType.ToString();

            return this;
        }

        public HttpRequestBuilder WithFormParam(string key, string value)
        {
            Assert.IsNotNull(key, "form key is null");
            Assert.IsNotNull(value, $"form value is null for key {key}");

            if (string.IsNullOrEmpty(value)) return this;

            if (formBuilder.Length > 0)
            {
                formBuilder.Append("&");
            }

            formBuilder.Append($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");

            return this;
        }

        public HttpRequestBuilder WithBody(string body)
        {
            if (!result.Headers.ContainsKey("Content-Type"))
            {
                result.Headers.Add("Content-Type", MediaType.TextPlain.ToString());
            }

            result.BodyBytes = Encoding.UTF8.GetBytes(body);

            return this;
        }

        public HttpRequestBuilder WithBody(byte[] body)
        {
            if (!result.Headers.ContainsKey("Content-Type"))
            {
                result.Headers.Add("Content-Type", MediaType.ApplicationOctetStream.ToString());
            }

            result.BodyBytes = body;

            return this;
        }

        public HttpRequestBuilder WithBody(FormDataContent body)
        {
            if (!result.Headers.ContainsKey("Content-Type"))
            {
                result.Headers.Add("Content-Type", body.GetMediaType());
            }

            result.BodyBytes = body.Get();

            return this;
        }

        public HttpRequestBuilder WithFormBody<T>(T body)
        {
            if (!result.Headers.ContainsKey("Content-Type"))
            {
                result.Headers.Add("Content-Type", MediaType.ApplicationForm.ToString());
            }

            result.BodyBytes = Encoding.UTF8.GetBytes(body.ToForm());

            return this;
        }

        public HttpRequestBuilder WithJsonBody<T>(T body)
        {
            if (!result.Headers.ContainsKey("Content-Type"))
            {
                result.Headers.Add("Content-Type", MediaType.ApplicationJson.ToString());
            }

            result.BodyBytes = body.ToUtf8Json();

            return this;
        }

        public HttpRequestBuilder SetCookieHeader(string key, string value)
        {
            StringBuilder sb = new StringBuilder();
            string cookieValue = $"{key}={value}";
            if (result.Headers.ContainsKey("Cookie"))
            {
                sb.Append(result.Headers["Cookie"]).Append(";");
            }

            sb.Append(cookieValue);
            result.Headers.Add("Cookie", sb.ToString());
            return this;
        }

        public HttpRequestBuilder SetCustomHeader(string key, string value)
        {
            result.Headers.Add(key, value);
            return this;
        }

        public HttpRequestBuilder SetDisableRetry()
        {
            result.EnableRetry = false;
            return this;
        }

        public IHttpRequest GetResult()
        {
            if (queryBuilder.Length > 0)
            {
                urlBuilder.Append("?");
                urlBuilder.Append(queryBuilder);
            }

            if (formBuilder.Length > 0)
            {
                result.Headers["Content-Type"] = MediaType.ApplicationForm.ToString();
                result.BodyBytes = Encoding.UTF8.GetBytes(formBuilder.ToString());
            }

            result.Url = urlBuilder.ToString();
            result.RedirectCount = redirectCount;
            result.IgnoreRedirectError = ignoreRedirectError;

            return result;
        }

        public class HttpRequestPrototype : IHttpRequest
        {
            public string Method { get; set; }
            public string Url { get; set; }

            public HttpAuth AuthType { get; set; }
            public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
            public byte[] BodyBytes { get; set; }

            public int RedirectCount { get; set; }

            public bool IgnoreRedirectError { get; set; }

            public bool EnableRetry { get; set; }
        }
    }
}