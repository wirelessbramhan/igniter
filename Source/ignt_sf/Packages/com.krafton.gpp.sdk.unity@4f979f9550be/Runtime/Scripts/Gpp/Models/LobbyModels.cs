using System.Runtime.Serialization;

namespace Gpp.Models
{
    public enum MessageType
    {
        unknown,
        connectNotif,
        disconnectNotif,
        messageNotif,
        refreshTokenRequest,
        accountDeletionNotif,
        disconnectByDuplicatedLoggedInNotif,
        surveyRefreshNotif
    }

    [DataContract]
    public class DisconnectNotify
    {
        [DataMember]
        public string message;
    }

    [DataContract]
    public class RefreshSurvey
    {
        [DataMember]
        public string payload;
    }

    [DataContract]
    public class LobbySessionId
    {
        [DataMember]
        public string lobbySessionID;
    }

    [DataContract]
    public class Notification
    {
        [DataMember]
        public string id;
        [DataMember]
        public string from;
        [DataMember]
        public string to;
        [DataMember]
        public string topic;
        [DataMember]
        public string payload;
        [DataMember]
        public string sentAt;
    }

    [DataContract]
    public class InGameNotificationPayload
    {
        [DataMember]
        public string title { get; set; }
        
        [DataMember]
        public string message { get; set; }
        
        [DataMember]
        public string displayPosition { get; set; }
        
        [DataMember]
        public float displayDurationMillis { get; set; }
    }

    [DataContract]
    public class RefreshAccessTokenRequest
    {
        [DataMember]
        public string token;
    }
}