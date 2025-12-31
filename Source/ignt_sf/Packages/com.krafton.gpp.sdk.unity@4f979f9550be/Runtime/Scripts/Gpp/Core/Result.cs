using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Gpp.Core
{
    public enum ErrorCode
    {
        None = 0,

        //HTTP Status
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        ProxyAuthenticationRequired = 407,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        RequestEntityTooLarge = 413,
        RequestUriTooLong = 414,
        UnsupportedMediaType = 415,
        RequestedRangeNotSatisfiable = 416,
        ExpectationFailed = 417,
        UnprocessableEntity = 422,
        HttpTooManyRequests = 429,
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HttpVersionNotSupported = 505,

        PaymentNotSupportCountry = 1004,

        EntitlementAlreadyExist = 31179,

        ErrorFromException = 14001,
        InvalidRequest = 14003,
        IsNotLoggedIn = 14006,
        MessageFieldTypeNotSupported = 14015,
        MessageFormatInvalid = 14016,
        MessageFieldDoesNotExist = 14017,
        MessageFieldConversionFailed = 14018,
        MessageCannotBeSent = 14019,
        MessageTypeNotSupported = 14020,

        UserUnderage = 10130,
        SteamServerError = 1074005,
        MandatoryPoliciesNotAccepted = 1075020,
        RepayRequired = 1105020,
        ReceiptDoNotConsume = 39177,
        AccountBan = 10134,
        KraftonAccountDeletion = 7286,
        KraftonAccountDisabled = 20013,
        KCNAlreadyAllAppIdExist = 60001,
        KraftonAccountMerge = 2045001,
        KraftonMaintenance = 10020029,
        GppAccountDeletion = 7233,
        VersionMismatch = 20003,
        KraftonClaimFailed = 91001,
        KraftonAccountAlreadyMerged = 10146,
        PlatformAlreadyLinkedWithAnotherAccount = 10173,
        DuplicatedEmailFound = 10200,
        SetMainGameId = 2042011,
        KCNCodeNotExist = 220005,
        SdkPlatformTokenNotFound = 3000010,
        SdkRefreshTokenNotFound = 3000011,
        SdkNotAcceptedLegal = 30000017,
        SdkGuestPopupCancel = 3000026,
        SdkTestModeNotVerifyReceiptAndConsume = 999901,
        SdkTestModeNotConsume = 999902,
        GetStoreCountryCodeFailed = 999903,
        SteamPurchaseFailed = 39150,
        SteamStartPurchaseFailed = 39151,
        SteamVerifyPurchaseFailed = 39152,
        AppleIdMismatch = 13217,
        IAMNotRegisteredEmailUser = 10186,

        #region SDK ErrorCodes (반드시 아래에 추가할 것)

        NetworkError = 7000,                // 네트워크 에러
        NamespaceNotFound = 7001,           // Namespace 없음
        ClientIdNotFound = 7002,            // ClientId 없음
        NotInitialized = 7003,              // 초기화 안됨
        NotLoggedIn = 7004,                 // 로그인되지 않음
        AlreadyLoggedIn = 7005,             // 이미 로그인됨
        AuthWebsocketConnectFailed = 7006,  // 인증 웹소켓 연결 실패
        SteamNotInitialized = 7007,         // 스팀 초기화 안됨
        SteamOffline = 7008,                // 스팀 오프라인
        SteamNotAvailable = 7009,           // 스팀 사용 불가
        InvalidSteamToken = 7010,           // 스팀 토큰 유효하지 않음
        InvalidWebUrl = 7011,               // Web Url이 유효하지 않음
        UnsupportedBrowser = 7012,          // 지원되지 않는 브라우저
        UserCancelled = 7013,               // 유저가 취소함
        UserBanned = 7014,                  // 유저 계정 제재 상태
        MissingRequestedTerms = 7015,       // 요청한 약관이 없음
        EmptyTermsTag = 7016,               // 약관 태그 비어있음
        KidLoginError = 7017,               // KID 로그인 에러
        AlreadyLinked = 7018,               // 이미 연동됨
        UnderMaintenance = 7019,            // 전체 점검 상태
        LoadTermsFailed = 7020,             // 약관 로드 실패
        LoadExternalUrlFailed = 7021,       // 외부 URL 로드 실패
        NotSupportPlatform = 7022,          // 지원되지 않는 플랫폼
        SelectLoginCancelled = 7023,        // 로그인 선택 취소
        IapInitFailed = 7024,               // IAP 초기화 실패
        NotIapInit = 7025,                  // IAP 초기화되지 않음
        StoreProductQueryFailed = 7026,     // 스토어 상품 조회 실패
        PurchaseReservationFailed = 7027,   // 구매 예약 실패
        JapanPurchaseLimitExceeded = 7028,  // 일본 구매 한도 초과
        PurchaseValidationFailed = 7029,    // 구매 검증 실패
        PurchaseConsumeFailed = 7030,       // 구매 컨슘 실패
        GetUnconsumedReceiptsFailed = 7031, // 미소비 영수증 조회 실패
        PurchaseFailed = 7032,              // 구매 실패
        GameServerMaintenance = 7033,       // 게임 서버 점검 상태
        PushSettingError = 7034,            // 푸시 설정 값 오류
        EmptySurveyKey = 7035,              // 설문조사 키 비어있음
        EpicNotAvailable = 7036,            // Epic 로그인 사용 불가
        EpicLoginCanceled = 7037,           // Epic 로그인 취소
        SteamGetDLCListFailed = 7038,       // 스팀 DLC 리스트 조회 실패
        KCNNoCodeToInput = 7039,            // KCN 입력 가능 코드 없음
        AppTransactionVerifyFailed = 7040,  // [Mac] AppTransaction 조회 실패
        AppleSignInFailed = 7041,           // Mac AppleSignIn 실패
        PCLoginFailed = 7042,               // PC 로그인 실패
        IDPLoginCancelled = 7043,           // IDP를 통한 로그인 실패
        AuthSessionExpired = 7044,          // 인증 세션 만료
        GameServerNotFound = 7045,          // 게임서버id가 존재하지 않음
        KCNCodeAlreadySubmitted = 7046,     // 이미 KCN 코드가 등록되었음
        PS5LoginFailed = 7047,              // PS5 로그인 실패        
        EmptyKcnCode = 7048,                // KCN 코드가 비어있는 경우
        NotRegisteredEmailUser = 100001022, // 이메일 정보가 등록되지 않은 유저
        OfferIdEmpty = 100033000,           // OfferId가 없는 경우
        OfferIdMismatch = 100033001,        // 캐쉬된 Offer 정보 중 매칭되는 OfferId가 없는 경우     
        NotSupportCountry = 100033002,      // KPS 지원하지 않는 국가 코드를 전송한 경우 (미국만 가능)
        GPPSDKInitFailed = 7049,            // GPP SDK Init 실패
        GPPConfigNotSet = 7050,             // GPP Config가 설정 안됨
        AlreadyLoginProgress = 7051,        // 이미 GPP Login 진행중
        NotSupportIdP = 7052,               // 지원하지 않는 ID Provider
        NotFullKID = 7053                   // Full KID가 아닌 상태
        #endregion
    }

    public class ErrorCodeConverter : JsonConverter<ErrorCode>
    {
        public override void WriteJson(JsonWriter writer, ErrorCode value, JsonSerializer serializer)
        {
            var enumName = Enum.GetName(typeof(ErrorCode), value);
            writer.WriteValue($"{(int)value} ({enumName})");
        }

        public override ErrorCode ReadJson(JsonReader reader, Type objectType, ErrorCode existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class Error
    {
        [DataMember(EmitDefaultValue = true)]
        [JsonConverter(typeof(ErrorCodeConverter))]
        public readonly ErrorCode Code;

        [DataMember(EmitDefaultValue = true)]
        public readonly string Message;

        [DataMember(EmitDefaultValue = true)]
        internal readonly object MessageVariables;

        [DataMember(EmitDefaultValue = true)]
        internal readonly Error InnerError;

        [DataMember(EmitDefaultValue = true)]
        internal readonly string RedirectUri;

        [DataMember(EmitDefaultValue = true)]
        internal readonly string Country;

        [DataMember(EmitDefaultValue = true)]
        internal readonly string WsUri;

        [DataMember(EmitDefaultValue = true)]
        internal readonly int ExpiresIn = 0;

        [IgnoreDataMember]
        public Object value;

        public Error(ErrorCode code, string message = null, object messageVariables = null, string redirectUri = null, string country = null, string wsUri = null, Error innerError = null, int expires_in = 0)
        {
            Code = code;
            Message = string.IsNullOrEmpty(message) ? GetDefaultErrorMessage() : message;
            InnerError = innerError;
            MessageVariables = messageVariables;
            RedirectUri = redirectUri;
            Country = country;
            WsUri = wsUri;
            if(expires_in == 0)
                expires_in = 600;
            ExpiresIn = expires_in;
        }

        private string GetDefaultErrorMessage()
        {
            switch (Code)
            {
                case ErrorCode.None:
                    return "This error code doesn't make sense and should not happen at all.";

                //HTTP Status Codes
                case ErrorCode.BadRequest:
                    return "The request could not be understood by the server due to malformed syntax.";
                case ErrorCode.Unauthorized:
                    return "The request requires user authentication.";
                case ErrorCode.PaymentRequired:
                    return "The request requires a payment.";
                case ErrorCode.Forbidden:
                    return "The server understood the request, but is refusing to fulfill it.";
                case ErrorCode.NotFound:
                    return "The server has not found anything matching the Request-URI.";
                case ErrorCode.MethodNotAllowed:
                    return
                        "The method specified in the Request-Line is not allowed for the resource identified by the " +
                        "Request-URI.";
                case ErrorCode.NotAcceptable:
                    return "The resource identified by the request can not generate content according to the accept " +
                           "headers sent in the request.";
                case ErrorCode.ProxyAuthenticationRequired:
                    return "The request requires user authentication via proxy.";
                case ErrorCode.RequestTimeout:
                    return "The client did not produce a request within the time that the server was prepared to wait.";
                case ErrorCode.Conflict:
                    return
                        "The request could not be completed due to a conflict with the current state of the resource.";
                case ErrorCode.Gone:
                    return "The requested resource is no longer available at the server and no forwarding address is " +
                           "known.";
                case ErrorCode.LengthRequired:
                    return "The server refuses to accept the request without a defined Content-Length.";
                case ErrorCode.PreconditionFailed:
                    return
                        "The precondition given in one or more of the request-header fields evaluated to false when " +
                        "it was tested on the server.";
                case ErrorCode.RequestEntityTooLarge:
                    return "The request entity is larger than the server is willing or able to process.";
                case ErrorCode.RequestUriTooLong:
                    return "The Request-URI is longer than the server is willing to interpret.";
                case ErrorCode.UnsupportedMediaType:
                    return "The entity of the request is in a format not supported by the requested resource for the " +
                           "requested method.";
                case ErrorCode.RequestedRangeNotSatisfiable:
                    return
                        "The request included a Range request-header field but none of the range-specifier values in " +
                        "this field overlap the current extent of the selected resource, and the request did not include" +
                        " an If-Range request-header field.";
                case ErrorCode.ExpectationFailed:
                    return "The expectation given in an Expect request-header field could not be met by this server.";
                case ErrorCode.UnprocessableEntity:
                    return "Entity can not be processed.";
                case ErrorCode.InternalServerError:
                    return "Unexpected condition encountered which prevented the server from fulfilling the request.";
                case ErrorCode.NotImplemented:
                    return "The server does not support the functionality required to fulfill the request.";
                case ErrorCode.BadGateway:
                    return "The gateway or proxy received an invalid response from the upstream server.";
                case ErrorCode.ServiceUnavailable:
                    return "The server is currently unable to handle the request due to a temporary overloading or " +
                           "maintenance of the server.";
                case ErrorCode.GatewayTimeout:
                    return "The gateway or proxy, did not receive a timely response from the upstream server.";
                case ErrorCode.HttpVersionNotSupported:
                    return
                        "The server does not support the HTTP protocol version that was used in the request message.";

                //Client side error codes
                case ErrorCode.IsNotLoggedIn:
                    return "User is not logged in.";
                case ErrorCode.NetworkError:
                    return "There is no response.";
                case ErrorCode.MessageFieldTypeNotSupported:
                    return "Serialization for expected field type is not supported.";
                case ErrorCode.MessageFormatInvalid:
                    return "Message is not well formed.";
                case ErrorCode.MessageFieldDoesNotExist:
                    return "Expected message field cannot be found.";
                case ErrorCode.MessageFieldConversionFailed:
                    return "Message field value cannot be converted to expected field type.";
                case ErrorCode.MessageCannotBeSent:
                    return "Sending message to server failed.";

                // SDK Errors
                case ErrorCode.NotInitialized:
                    return "GppSdk is not initialize.";

                case ErrorCode.AlreadyLoggedIn:
                    return "GppSdk is already loggedIn.";

                case ErrorCode.NotLoggedIn:
                    return "User is not logged in.";

                case ErrorCode.MissingRequestedTerms:
                    return "Agreement document can not found";

                case ErrorCode.EmptyTermsTag:
                    return "Tag must not be null or empty.";

                case ErrorCode.UserCancelled:
                    return "User Cancelled.";

                case ErrorCode.AlreadyLinked:
                    return "User is already linked with krafton id.";

                case ErrorCode.SdkRefreshTokenNotFound:
                    return "RefreshToken must not be null or empty.";

                case ErrorCode.SdkPlatformTokenNotFound:
                    return "PlatformToken must not be null or empty.";

                case ErrorCode.UnderMaintenance:
                    return "Service is undergoing maintenance.";

                case ErrorCode.LoadTermsFailed:
                    return "Fail to load legal documents from server.";

                case ErrorCode.SdkNotAcceptedLegal:
                    return "The user hasn't accepted the terms.";

                case ErrorCode.LoadExternalUrlFailed:
                    return "Cannot retrieve the Helpdesk URL.";

                case ErrorCode.InvalidWebUrl:
                    return "WebView url must not be null or empty.";

                case ErrorCode.NotSupportPlatform:
                    return "Not supported platform type.";

                case ErrorCode.NotIapInit:
                    return "IAP module is not initialized.";

                case ErrorCode.JapanPurchaseLimitExceeded:
                    return "This is the age limit.";

                case ErrorCode.PurchaseValidationFailed:
                    return "Invalid payment.";

                case ErrorCode.AuthWebsocketConnectFailed:
                    return "Can not connect auth websocket.";

                case ErrorCode.UnsupportedBrowser:
                    return "Not support browser. Check default browser.";
                
                case ErrorCode.NotRegisteredEmailUser:
                    return "This user has not registered an email";

                case ErrorCode.NotSupportIdP:
                    return "This IDP is not supported on this platform.";

                case ErrorCode.NotFullKID:
                    return "This action need full KID.";

                default:
                    return "Unknown error: " + Code.ToString("G");
            }
        }

        public new string ToString()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(this, settings);
        }
    }

    public interface IResult
    {
        Error Error { get; }
        bool IsError { get; }
    }

    public class Result<T> : IResult
    {
        public Error Error { get; }
        public T Value { get; }

        public bool IsError => Error != null;

        public static Result<T> CreateOk(T value)
        {
            return new Result<T>(null, value);
        }

        public static Result<T> CreateError(ErrorCode errorCode, string errorMessage = null, object messageVariables = null)
        {
            return new Result<T>(new Error(errorCode, errorMessage, messageVariables), default);
        }

        public static Result<T> CreateError(object value, ErrorCode errorCode, string errorMessage = null, object messageVariables = null)
        {
            var error = new Error(errorCode, errorMessage, messageVariables);
            error.value = value;
            return new Result<T>(error, default);
        }

        public static Result<T> CreateError(Error error)
        {
            return new Result<T>(error, default);
        }

        private Result(Error error, T value)
        {
            Error = error;
            Value = value;
        }

        public override string ToString()
        {
            return Error.ToString();
        }
    }

    public class Result : IResult
    {
        public Error Error { get; }

        public bool IsError => Error != null;

        public static Result CreateOk()
        {
            return new Result(null);
        }

        public static Result CreateError(ErrorCode errorCode, string errorMessage = null, object messageVariables = null)
        {
            return new Result(new Error(errorCode, errorMessage, messageVariables));
        }

        public static Result CreateError(Error error)
        {
            return new Result(error);
        }

        private Result(Error error)
        {
            Error = error;
        }
    }
}