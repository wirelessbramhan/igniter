namespace Gpp.WebView
{
    public enum GppWebViewResultCode
    {
        RESULT_NETWORK_ERROR = 100,
        RESULT_URL_OK = 200,
        RESULT_HTTP_ERROR = 400,
        RESULT_SERVER_ERROR = 500,
        RESULT_NOT_SUPPORT_BROWSER = 998,
        RESULT_USER_DISMISS = 999,
        RESULT_LOAD_FAILED = 2001,
        RESULT_AUTH_FAILED = 3001,
        RESULT_SSL_ERROR = 4001,
        RESULT_UNKNOWN_ERROR = -1,
    }
    
    public class GppWebViewResponse
    {
        public GppWebViewResultCode ResultCode { get; }
        public string Response { get; }

        public GppWebViewResponse(GppWebViewResultCode resultCode, string response)
        {
            ResultCode = resultCode;
            Response = response;
        }
    }
}