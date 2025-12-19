using System;
using System.Collections.Generic;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Models;
using UnityEngine;
using static Gpp.Models.GppClientLogModels;

namespace Gpp.Telemetry
{
    internal static class GppTelemetry
    {
        private const string EntryStepKey = "entry_step";
        private const string KcnCodeSubmitEventCode = "event_code";
        private static string beforeEntryStep = "null";

        public static bool IsFunnelImmediateSend
        {
            private get;
            set;
        } = true;

        public static void SendUserEntry(string step, Dictionary<string, object> metadata = null)
        {
            if (!GppSDK.IsInitialized)
            {
                return;
            }
            if (metadata == null)
                metadata = new Dictionary<string, object>();

            string loginFlowType = GppSDK.GetSession().GetLoginFlowType() == Core.LoginSession.LoginFlowType.none ? null : GppSDK.GetSession().GetLoginFlowType().ToString();
            metadata.Add("login_flow_type", loginFlowType);
            
            var payload = CreatePayload(EntryStepKey, step, metadata);
            
            if (IsFunnelImmediateSend)
            {
                GppSDK.SendImmediatelyGppTelemetry(GppClientLogModels.LogType.UserEntry, payload);
            }
            else
            {
                GppSDK.SendGppTelemetry(GppClientLogModels.LogType.UserEntry, payload);
            }
            
        }

        public static void SendKcnSubmitProcess(string code, Dictionary<string, object> metadata = null)
        {
            if (!GppSDK.IsInitialized)
            {
                return;
            }
            if (GppSDK.GetSession().cachedTokenData != null)
            {
                if (metadata == null)
                    metadata = new Dictionary<string, object>();
                metadata.Add("krafton_id", GppSDK.GetSession().cachedTokenData.KraftonId);
            }

            var payload = CreatePayload(KcnCodeSubmitEventCode, code, metadata);
            GppSDK.SendGppTelemetry(GppClientLogModels.LogType.KcnCodeSubmit, payload);
        }

        public static void SendKpsPaymentChannelSelect(String checkoutId, PublicOffer offer, String paymentChannel = "KPS")
        {
            if (!GppSDK.IsInitialized || offer == null || string.IsNullOrEmpty(checkoutId))
            {
                return;
            }
            Dictionary<string, object> payload = new Dictionary<string, object>();

            if (GppSDK.GetSession().cachedTokenData != null)
            {
                payload ??= new Dictionary<string, object>();
                payload.Add("kos_user_id", GppSDK.GetSession().UserId);
                payload.Add("krafton_id", GppSDK.GetSession().cachedTokenData?.KraftonId);
            }
            payload.Add("payment_channel", paymentChannel);
            payload.Add("checkout_id", checkoutId);
            payload.Add("offer_id", offer.OfferId);
            payload.Add("currency", offer.Price.Currency);
            GppSDK.SendGppTelemetry(GppClientLogModels.LogType.KpsChannelSelect, payload);
        }

#if UNITY_STANDALONE_WIN
        public static void SendDeviceInfo(PCHardware hardware)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            Dictionary<string, object> devicePayload = new Dictionary<string, object>();
            devicePayload.Add("pc", hardware);
            payload.Add("device_type", "PC");
            payload.Add("device_info", devicePayload);
            GppSDK.SendImmediatelyGppTelemetry(GppClientLogModels.LogType.DeviceInfo, payload);
        }
#elif UNITY_ANDROID
        public static void SendDeviceInfo(AndroidHardware hardware)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            Dictionary<string, object> devicePayload = new Dictionary<string, object>();
            devicePayload.Add("android", hardware);
            payload.Add("device_type", "Android");
            payload.Add("device_info", devicePayload);
            GppSDK.SendImmediatelyGppTelemetry(GppClientLogModels.LogType.DeviceInfo, payload);
        }
#elif UNITY_IOS
        public static void SendDeviceInfo(IOSHardware hardware)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            Dictionary<string, object> devicePayload = new Dictionary<string, object>();
            devicePayload.Add("ios", hardware);
            payload.Add("device_type", "iOS");
            payload.Add("device_info", devicePayload);
            GppSDK.SendImmediatelyGppTelemetry(GppClientLogModels.LogType.DeviceInfo, payload);
        }
#elif UNITY_STANDALONE_OSX
        public static void SendDeviceInfo(MacHardware hardware)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            Dictionary<string, object> devicePayload = new Dictionary<string, object>();
            devicePayload.Add("pc", hardware);
            payload.Add("device_type", "PC");
            payload.Add("device_info", devicePayload);
            GppSDK.SendImmediatelyGppTelemetry(GppClientLogModels.LogType.DeviceInfo, payload);
        }

#elif UNITY_GAMECORE_SCARLETT
        public static void SendDeviceInfo(XboxHardware hardware)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            Dictionary<string, object> devicePayload = new Dictionary<string, object>();
            devicePayload.Add("xbox", hardware);
            payload.Add("device_type", "xbox");
            payload.Add("device_info", devicePayload);
            GppSDK.SendImmediatelyGppTelemetry(GppClientLogModels.LogType.DeviceInfo, payload);
        }
#elif UNITY_PS5
        public static void SendDeviceInfo(Ps5Hardware hardware)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            Dictionary<string, object> devicePayload = new Dictionary<string, object>();
            devicePayload.Add("ps5", hardware);
            payload.Add("device_type", "ps5");
            payload.Add("device_info", devicePayload);
            GppSDK.SendImmediatelyGppTelemetry(GppClientLogModels.LogType.DeviceInfo, payload);
        }
#endif

        public static void SendDeleteAccount(string step, Dictionary<string, object> metadata = null)
        {
            if (!GppSDK.IsInitialized)
            {
                return;
            }
            if (metadata == null)
                metadata = new Dictionary<string, object>();

            if (step == EntryStep.TryAccountDeletionRequest)
                beforeEntryStep = null;

            metadata.Add("before_entry_step", beforeEntryStep);
            metadata.Add("kos_user_id", GppSDK.GetSession().UserId);
            metadata.Add("is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless);
            metadata.Add("krafton_id", GppSDK.GetSession().cachedTokenData?.KraftonId);
            metadata.Add("platform_id", GppSDK.GetSession().cachedTokenData?.PlatformUserId);
            metadata.Add("platform", GppSDK.GetSession().cachedTokenData?.PlatformType);

            var payload = CreatePayload(EntryStepKey, step, metadata);
            GppSDK.SendImmediatelyGppTelemetry(GppClientLogModels.LogType.DeleteAccount, payload);
            beforeEntryStep = step;
        }

        public static void SendSurvey(string eventName, string surveyMonkeyId)
        {
            if (!GppSDK.IsInitialized)
            {
                return;
            }

            var metadata = new Dictionary<string, object>
            {
                { "survey_id", surveyMonkeyId },
                { "kos_user_id", GppSDK.GetSession().UserId },
                { "krafton_id", GppSDK.GetSession().cachedTokenData?.KraftonId },
            };

            var payload = CreatePayload("event_name", eventName, metadata);
            GppSDK.SendImmediatelyGppTelemetry(GppClientLogModels.LogType.Survey, payload);
        }

        private static Dictionary<string, object> CreatePayload(string key, string value, Dictionary<string, object> metadata)
        {
            var payload = new Dictionary<string, object>
            {
                { key, value }
            };

            if (metadata == null) return payload;
            foreach (var kvp in metadata)
            {
                payload[kvp.Key.ToLower()] = kvp.Value;
            }

            return payload;
        }
    }
}