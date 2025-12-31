using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Log;
using Gpp.Models;

namespace Gpp.Network
{
    internal static class HttpErrorParser
    {
        public static Result TryParse(this IHttpResponse response)
        {
            var error = ParseError(response);
            return error != null ? Result.CreateError(error) : Result.CreateOk();
        }

        public static Result<T> TryParseJson<T>(this IHttpResponse response)
        {
            var error = ParseError(response);
            if (error != null) return Result<T>.CreateError(error);
            
            try
            {
                if (response.BodyBytes == null || response.BodyBytes.Length == 0) return Result<T>.CreateOk(default);

                return Result<T>.CreateOk(response.BodyBytes.ToObject<T>());
            }
            catch (Exception e)
            {
                return Result<T>.CreateError(ErrorCode.ErrorFromException, e.Message, response);
            }
        }

        private static Error ParseError(IHttpResponse response)
        {
            if (response == null) return new Error(ErrorCode.NetworkError, "There is no response.");

            if (response.Code is >= 200 and < 300) return null;

            if (response.Code is < 400 or >= 600) return ParseDefaultError(response);

            if (response.BodyBytes == null) return new Error((ErrorCode)response.Code);

            try
            {
                return ParseServiceError(response);
            }
            catch (Exception)
            {
                return new Error((ErrorCode)response.Code);
            }
        }

        private static Error ParseServiceError(IHttpResponse response)
        {
            var error = response.BodyBytes.ToObject<ServiceError>();
            if (error.numericErrorCode != 0)
            {
                return new Error((ErrorCode)error.numericErrorCode, error.errorMessage, error.messageVariables, error.redirect_uri);
            }

            if (error.errorCode != 0)
            {
                return new Error((ErrorCode)error.errorCode, error.errorMessage, error.messageVariables, error.redirect_uri);
            }

            if (error.code != 0)
            {
                return new Error((ErrorCode)error.code, error.message);
            }

            switch (error.error_code)
            {
                case (int)ErrorCode.SteamServerError:
                    return new Error(ErrorCode.SteamServerError, error.message, response, error.redirect_uri, error.country, error.ws_uri);
                case (int)ErrorCode.AccountBan:
                    return new Error(ErrorCode.AccountBan, error.message, response, error.redirect_uri, error.country, error.ws_uri, null, error.expires_in);
                case (int)ErrorCode.GppAccountDeletion:
                    return new Error(ErrorCode.GppAccountDeletion, error.message, response, error.redirect_uri, error.country, error.ws_uri, null, error.expires_in);
                case (int)ErrorCode.KraftonAccountDeletion:
                    return new Error(ErrorCode.KraftonAccountDeletion, error.message, response, error.redirect_uri, error.country, error.ws_uri, null, error.expires_in);
                case (int)ErrorCode.KraftonAccountDisabled:
                    return new Error(ErrorCode.KraftonAccountDisabled, error.message, response, error.redirect_uri, error.country, error.ws_uri, null, error.expires_in);
                case (int)ErrorCode.KraftonAccountMerge:
                    return new Error(ErrorCode.KraftonAccountMerge, error.message, response, error.redirect_uri, error.country, error.ws_uri);
                case (int)ErrorCode.KraftonMaintenance:
                    return new Error(ErrorCode.KraftonMaintenance, error.message, response, error.redirect_uri, error.country, error.ws_uri);
                case (int)ErrorCode.KraftonClaimFailed:
                    return new Error(ErrorCode.KraftonClaimFailed, error.message, response, error.redirect_uri, error.country, error.ws_uri);
                case (int)ErrorCode.VersionMismatch:
                    return new Error(ErrorCode.VersionMismatch, error.message, response, error.redirect_uri, error.country, error.ws_uri);
                case (int)ErrorCode.MandatoryPoliciesNotAccepted:
                    return new Error(ErrorCode.MandatoryPoliciesNotAccepted, error.message, response, error.redirect_uri, error.country, error.ws_uri);
                case (int)ErrorCode.UserUnderage:
                    return new Error(ErrorCode.UserUnderage, error.message, response, error.redirect_uri, error.country, error.ws_uri, null, error.expires_in);
                case (int)ErrorCode.RepayRequired:
                    return new Error(ErrorCode.RepayRequired, error.message, response);
            }

            if (error.error_code != 0)
            {
                return new Error((ErrorCode)error.error_code, error.message, error.required_policies, error.redirect_uri);
            }

            if (error.error != null)
            {
                string message = error.error;

                if (error.error_description != null) message += ": " + error.error_description;

                return new Error((ErrorCode)response.Code, message);
            }

            return new Error((ErrorCode)response.Code);
        }

        private static Error ParseDefaultError(IHttpResponse response)
        {
            if (response.BodyBytes == null) return new Error((ErrorCode)response.Code);

            string body = Encoding.UTF8.GetString(response.BodyBytes);

            return new Error((ErrorCode)response.Code, "Unknown error: " + body);
        }

        [DataContract]
        private class ServiceError
        {
            [DataMember]
            public long code { get; set; }

            [DataMember]
            public long error_code { get; set; }

            [DataMember]
            public long errorCode { get; set; }

            [DataMember]
            public int numericErrorCode { get; set; }

            [DataMember]
            public string errorMessage { get; set; }

            [DataMember]
            public string message { get; set; }

            [DataMember]
            public object messageVariables { get; set; }

            [DataMember]
            public string error { get; set; }

            [DataMember]
            public string error_description { get; set; }

            [DataMember]
            public RequiredPolicy[] required_policies { get; set; }

            [DataMember]
            public string redirect_uri { get; set; }

            [DataMember]
            public string ws_uri { get; set; }

            [DataMember]
            public string country { get; set; }

            [DataMember]
            public int expires_in { get; set; }
        }
    }
}