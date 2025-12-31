using System.Collections.Generic;
using Gpp.Core;
using Gpp.Models;

namespace Gpp.Telemetry
{
    public static class FunnelLogManager
    {
        private static readonly Dictionary<string, string> StartEventType = new()
        {
            { Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.REPAY_USER_ENTRY },
            { Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.START_PURCHASE },
            { Funnel.Purchase.Type.PURCHASE_RECOVERY_FUNNEL, Funnel.Purchase.PurchaseRecovery.TRIGGER_PURCHASE_RECOVERY }
        };

        private static readonly Dictionary<string, string> FunnelIds = new();
        private static readonly Dictionary<string, FunnelLogInfo> FunnelLogInfos = new();

        public static string GetFunnelId(string funnelType, string step)
        {
            if (!IsFirstLogEvent(funnelType, step))
            {
                return FunnelIds.GetValueOrDefault(funnelType);
            }
            
            DeletePrevInfo(funnelType);
                
            var funnelId = CreateFunnelId();
            FunnelIds.Add(funnelType, funnelId);

            CreateFunnelLogInfo(funnelId);
                
            return funnelId;

        }
        
        public static FunnelLogInfo GetFunnelLogInfo(string funnelId)
        {
            return FunnelLogInfos.GetValueOrDefault(funnelId);
        }

        private static void CreateFunnelLogInfo(string funnelId)
        {
            var funnelLogInfo = new FunnelLogInfo();
            SetFunnelLogInfo(funnelId, funnelLogInfo);
        }

        private static void SetFunnelLogInfo(string funnelId, FunnelLogInfo funnelLogInfo)
        {
            FunnelLogInfos[funnelId] = funnelLogInfo;
        }

        private static void DeleteFunnelLogInfo(string funnelId)
        {
            FunnelLogInfos.Remove(funnelId);
        }

        private static bool IsFirstLogEvent(string funnelType, string step)
        {
            return StartEventType.TryGetValue(funnelType, out var value) && value.Equals(step);
        }

        private static void DeletePrevInfo(string funnelType)
        {
            if (FunnelIds.TryGetValue(funnelType, out var prevFunnelId) is false || string.IsNullOrEmpty(prevFunnelId))
            {
                return;
            }
            
            DeleteFunnelLogInfo(prevFunnelId);
            FunnelIds.Remove(funnelType);
        }

        private static string CreateFunnelId()
        {
            return GppTokenProvider.CreateGuestUserId();
        }
    }
}