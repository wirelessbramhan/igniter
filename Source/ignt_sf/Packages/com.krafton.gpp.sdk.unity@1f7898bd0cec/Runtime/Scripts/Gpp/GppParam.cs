using Gpp.Auth;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Models;

namespace Gpp
{
    public class LoginRequestParam
    {
        public ResultCallback<GppUser> callback;
        public PlatformType platformType;
        public bool isHeadless = false;
        public bool tryLink = false;
        public CustomProviderIdParam customProviderIdParam;
    }

    public class SelectMainGameUserIdParam
    {
        public string accessToken;
        public string userId;
        public MultipleGameUserInfo userInfo;
        public bool isMain;
    }

    public class CustomProviderIdParam
    {
        public string credential;
        public string platformId;
    }
}