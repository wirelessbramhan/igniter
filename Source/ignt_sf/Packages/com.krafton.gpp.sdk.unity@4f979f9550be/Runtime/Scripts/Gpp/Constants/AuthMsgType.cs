namespace Gpp.Constants
{
    public enum AuthMsgType
    {
        NONE,
        MSG_PING,
        MSG_PONG,
        MSG_DEVICE_AUTH_GRANT_COMPLETE,
        MSG_DEVICE_AUTH_GRANT_USER_CODE_CONSUMED,
        MSG_DEVICE_AUTH_HAS_ISSUE,
        MSG_USER_STATUS_CHECK_STARTED,
        MSG_USER_STATUS_HAS_ISSUE,
        MSG_USER_STATUS_RESOLVED,
    }
}