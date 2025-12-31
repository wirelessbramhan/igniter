using Gpp.Localization;
using Gpp.Models;
using Gpp.Utils;

namespace Gpp.CommonUI.Legal
{
    public class GppLegalModel
    {
        public string PolicyTitle { get; private set; }
        public bool IsMandatory { get; private set; }
        public string WebUrl { get; private set; }
        public string PolicyId { get; private set; }
        public string PolicyVersionId { get; private set; }
        public string LocalizedPolicyVersionId { get; private set; }
        public bool IsAccepted { get; set; }
        public PushNotificationIntegration PushNotificationIntegration { get; private set; }
        
        private GppLegalModel() { }
        
        public class Builder
        {
            private GppLegalModel model;

            public Builder()
            {
                model = new GppLegalModel();
            }

            public Builder SetPolicyTitle(bool isMandatory, string title)
            {
                if (string.IsNullOrEmpty(title))
                {
                    return this;
                }
                string mandatoryTextKey = isMandatory ? LocalizationKey.Mandatory : LocalizationKey.Optional;
                string mandatoryText = LocalizationManager.Localise(mandatoryTextKey, GppSDK.GetSession().LanguageCode);
                if (PlatformUtil.IsPc())
                {
                    model.PolicyTitle = $"<b>{mandatoryText}</b> {title}";   
                }
                else
                {
                    model.PolicyTitle = GppUtil.IsRtlLang() ? $"{title} [{mandatoryText}]" : $"[{mandatoryText}] {title}";
                }
                return this;
            }

            public Builder SetIsMandatory(bool mandatory)
            {
                model.IsMandatory = mandatory;
                return this;
            }

            public Builder SetWebUrl(string url)
            {
                model.WebUrl = url;
                return this;
            }

            public Builder SetPolicyId(string id)
            {
                model.PolicyId = id;
                return this;
            }

            public Builder SetPolicyVersionId(string versionId)
            {
                model.PolicyVersionId = versionId;
                return this;
            }

            public Builder SetLocalizedPolicyVersionId(string localizedVersionId)
            {
                model.LocalizedPolicyVersionId = localizedVersionId;
                return this;
            }

            public Builder SetPushNotificationIntegration(PushNotificationIntegration pushNotificationIntegration)
            {
                model.PushNotificationIntegration = pushNotificationIntegration;
                return this;
            }

            public Builder SetIsAccepted(bool accepted)
            {
                model.IsAccepted = accepted;
                return this;
            }

            public GppLegalModel Build()
            {
                return model;
            }
        }
    }

}