using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gpp.Models
{
    [DataContract]
    public class UserProfile
    {
        [DataMember]
        public string userId { get; set; }
        [DataMember(Name = "namespace")]
        public string Namespace { get; set; }
        [DataMember]
        public string firstName { get; set; }
        [DataMember]
        public string lastName { get; set; }
        [DataMember]
        public string avatarSmallUrl { get; set; }
        [DataMember]
        public string avatarUrl { get; set; }
        [DataMember]
        public string avatarLargeUrl { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string language { get; set; }
        [DataMember]
        public string timeZone { get; set; }
        [DataMember]
        public string dateOfBirth { get; set; }
        [DataMember]
        public Dictionary<string, object> customAttributes { get; set; }
        [DataMember]
        public bool pushEnable { get; set; }
        [DataMember]
        public bool pushNightEnable { get; set; }
    }

    [DataContract]
    public class UserNightPushStatus
    {
        [DataMember]
        public bool pushNightEnable { get; set; }
    }

    [DataContract]
    public class UserPushStatus
    {
        [DataMember]
        public bool pushEnable { get; set; }
    }

    [DataContract]
    public class AccountDeletionConfig
    {
        [DataMember(Name = "namespace")]
        public string Namespace { get; set; }

        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }

        [DataMember(Name = "gracePeriodInMins")]
        public int PeriodInMins { get; set; }
    }

    [DataContract]
    public class Maintenance
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "namespace")]
        public string Namespace { get; set; }

        [DataMember(Name = "targetClients")]
        public List<string> TargetClients { get; set; }

        [DataMember(Name = "startDate")]
        public string StartDate { get; set; }

        [DataMember(Name = "endDate")]
        public string EndDate { get; set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "createdAt")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        public string UpdatedAt { get; set; }

        [DataMember(Name = "externalUrl")]
        public string ExternalUrl { get; set; }

        [DataMember(Name = "maintenanceTypeName")]
        public string MaintenanceTypeName { get; set; }

        [DataMember(Name = "maintenanceTitle")]
        public string Title { get; set; }

        [DataMember(Name = "maintenanceDetail")]
        public string Detail { get; set; }

        [DataMember(Name = "displayPeriodType")]
        public string DisplayPeriodType { get; set; }

        [DataMember(Name = "showRemainingTime")]
        public bool ShowRemainingTime { get; set; }

        [DataMember(Name = "displayStartDate")]
        public string DisplayStartDate { get; set; }

        [DataMember(Name = "displayEndDate")]
        public string DisplayEndDate { get; set; }

        [DataMember(Name = "currentTime")]
        public string CurrentTime { get; set; }

        [DataMember(Name = "gameServerIds")]
        public List<string> GameServerIds { get; set; }
    }

    [DataContract]
    public class MaintenanceResult
    {
        public bool IsMaintenance { get; set; }
        public Maintenance Maintenance { get; set; }
    }

    [DataContract]
    public class GameServerMaintenanceResult
    {
        [DataMember(Name = "scope")]
        public string Scope { get; set; }
        [DataMember(Name = "maintenances")]
        public List<GameServerMaintenance> Maintenances { get; set; }
        [DataMember(Name = "whiteList")]
        public bool WhiteList { get; set; }
    }

    [DataContract]
    public class GameServerMaintenance
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "underMaintenance")]
        public bool UnderMaintenance { get; set; }
        [DataMember(Name = "maintenance")]
        public Maintenance Maintenance { get; set; }
    }

    [DataContract]
    public class EventCenterUrl
    {
        [DataMember(Name = "namespace")]
        public string Namespace { get; set; }
        [DataMember(Name = "eventId")]
        public string EventId { get; set; }
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}