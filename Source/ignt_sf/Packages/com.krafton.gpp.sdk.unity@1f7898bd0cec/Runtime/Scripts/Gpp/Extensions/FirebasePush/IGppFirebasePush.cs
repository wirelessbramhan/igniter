using Gpp.Core;
using Gpp.Constants;

namespace Gpp.Extensions.FirebasePush
{
    internal interface IGppFirebasePush
    {
        public void GetPermission(ResultCallback<PermissionState> callback);
        public void GetPushToken(ResultCallback<string> callback);
        public void DeletePushToken(ResultCallback callback = null);
        public void GetPushPermissionGranted(ResultCallback<PermissionState> callback);
        public void OpenNotificationSetting();
    }
}