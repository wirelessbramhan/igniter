using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gpp.Models
{
    #region enum

    public enum LegalPolicyType
    {
        EMPTY = -1,
        LEGAL_DOCUMENT_TYPE,
        MARKETING_PREFERENCE_TYPE,
        ECOMMERCE_TYPE
    }

    [DataContract]
    public enum PushNotificationIntegration
    {
        NONE = -1,
        IN_APP_MARKETING_CONSENT,
        NIGHT_TIME_IN_APP_MARKETING_CONSENT
    }

    #endregion enum

    public class LegalLocalizedPolicy
    {
        public bool isMandatory { get; set; }
        public LocalizedPolicyVersionObject localizedPolicyVersionObject { get; set; }
        public PushNotificationIntegration pushNotificationIntegration { get; set; }
    }

    [DataContract]
    public class LocalizedPolicyVersionObject
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string createdAt { get; set; }
        [DataMember]
        public string updatedAt { get; set; }
        [DataMember]
        public string localeCode { get; set; }
        [DataMember]
        public string contentType { get; set; }
        [DataMember]
        public string attachmentLocation { get; set; }
        [DataMember]
        public string attachmentChecksum { get; set; }
        [DataMember]
        public string attachmentVersionIdentifier { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string publishedDate { get; set; }
        [DataMember]
        public bool isDefaultSelection { get; set; }
    }

    [DataContract]
    public class PolicyVersionWithLocalizedVersionObject
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string createdAt { get; set; }
        [DataMember]
        public string updatedAt { get; set; }
        [DataMember]
        public string displayVersion { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string publishedDate { get; set; }
        [DataMember]
        public LocalizedPolicyVersionObject[] localizedPolicyVersions { get; set; }
        [DataMember]
        public bool isCommitted { get; set; }
        [DataMember]
        public bool isCrucial { get; set; }
        [DataMember]
        public bool isInEffect { get; set; }
    }

    [DataContract]
    public class PublicPolicy
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string createdAt { get; set; }
        [DataMember]
        public string updatedAt { get; set; }
        [DataMember]
        public string readableId { get; set; }
        [DataMember]
        public string policyName { get; set; }
        [DataMember]
        public string policyType { get; set; }
        [DataMember(Name = "namespace")]
        public string Namespace { get; set; }
        [DataMember]
        public string countryCode { get; set; }
        [DataMember]
        public string countryGroupCode { get; set; }
        [DataMember]
        public string[] baseUrls { get; set; }
        [DataMember]
        public bool shouldNotifyOnUpdate { get; set; }
        [DataMember]
        public PolicyVersionWithLocalizedVersionObject[] policyVersions { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public bool isMandatory { get; set; }
        [DataMember]
        public bool isDefaultOpted { get; set; }
        [DataMember]
        public bool isDefaultSelection { get; set; }
        [DataMember]
        public string basePolicyId { get; set; }
        [DataMember]
        public bool isAccepted { get; set; }
        [DataMember]
        public PushNotificationIntegration pushNotificationIntegration { get; set; }
    }

    [DataContract]
    public class AcceptLegalRequest
    {
        [DataMember]
        public string localizedPolicyVersionId { get; set; }
        [DataMember]
        public string policyVersionId { get; set; }
        [DataMember]
        public string policyId { get; set; }
        [DataMember]
        public bool isAccepted { get; set; }
    }

    [DataContract]
    public class AcceptLegalResponse
    {
        [DataMember]
        public bool proceed { get; set; }
    }

    public class LocalizedPolicyVersion
    {
        [DataMember]
        public string attachmentChecksum { get; set; }
        [DataMember]
        public string attachmentLocation { get; set; }
        [DataMember]
        public string attachmentVersionIdentifier { get; set; }
        [DataMember]
        public string contentType { get; set; }
        [DataMember]
        public DateTime createdAt { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public bool isDefaultSelection { get; set; }
        [DataMember]
        public string localeCode { get; set; }
        [DataMember]
        public DateTime publishedDate { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string updatedAt { get; set; }
        [DataMember]
        public PolicyVersionWithLocalizedVersionObject policyVersion { get; set; }
        [DataMember]
        public PublicPolicy policy { get; set; }
    }

    public class PolicyVersion
    {
        [DataMember]
        public DateTime createdAt { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string displayVersion { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public bool isCommitted { get; set; }
        [DataMember]
        public bool isInEffect { get; set; }
        [DataMember]
        public List<LocalizedPolicyVersion> localizedPolicyVersions { get; set; }
        [DataMember]
        public DateTime publishedDate { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public DateTime updatedAt { get; set; }
    }

    public class RequiredPolicy
    {
        [DataMember]
        public string basePolicyId { get; set; }
        [DataMember]
        public List<string> baseUrls { get; set; }
        [DataMember]
        public string countryCode { get; set; }
        [DataMember]
        public DateTime createdAt { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public bool isDefaultOpted { get; set; }
        [DataMember]
        public bool isDefaultSelection { get; set; }
        [DataMember]
        public bool isMandatory { get; set; }
        [DataMember]
        public string @namespace { get; set; }
        [DataMember]
        public string policyName { get; set; }
        [DataMember]
        public string policyType { get; set; }
        [DataMember]
        public List<PolicyVersion> policyVersions { get; set; }
        [DataMember]
        public bool shouldNotifyOnUpdate { get; set; }
        [DataMember]
        public List<object> tags { get; set; }
        [DataMember]
        public DateTime updatedAt { get; set; }
    }
}