using System.Collections.Generic;
using Gpp.Core;
using Gpp.Models;

namespace Gpp.Api.Basic
{
    internal partial class BasicApi : GppApi
    {
        protected override string GetServiceName()
        {
            return "basic";
        }

        internal void GetPushStatus(ResultCallback<UserPushStatus> callback)
        {
            Run(RequestGetPushStatus(callback));
        }

        internal void GetNightPushStatus(ResultCallback<UserNightPushStatus> callback)
        {
            Run(RequestGetNightPushStatus(callback));
        }

        internal void SetPushStatus(bool isEnable, ResultCallback<UserPushStatus> callback)
        {
            Run(RequestPatchPushStatus(isEnable, callback));
        }

        internal void SetNightPushStatus(bool status, ResultCallback<UserNightPushStatus> callback)
        {
            Run(RequestPatchNightPushStatus(status, callback));
        }

        internal void GetMaintenance(ResultCallback<Maintenance> callback)
        {
            Run(RequestGetMaintenance(callback));
        }

        internal void GetGameServerMaintenance(string gameServerId, ResultCallback<Maintenance> callback)
        {
            Run(RequestGetGameServerMaintenance(gameServerId, callback));
        }

        internal void GetGameServersMaintenance(ResultCallback<GameServerMaintenanceResult> callback)
        {
            Run(RequestGetGameServersMaintenance(callback));
        }

        internal void UpdateUserProfile(ResultCallback callback = null)
        {
            Run(RequestPatchUserProfile(callback));
        }

        internal void GetAccountDeletionConfig(ResultCallback<AccountDeletionConfig> callback)
        {
            Run(RequestGetAccountDeletionConfig(callback));
        }

        internal void GetEventCenterUrl(string eventId, ResultCallback<EventCenterUrl> callback, Dictionary<string, object> additionalData)
        {
            Run(RequestGetEventCenterUrl(eventId, callback, additionalData));
        }
    }
}