using System;
using System.Runtime.Serialization;

namespace Gpp.Config.Extensions
{
    [DataContract]
    public class ExtensionConfig : IEquatable<ExtensionConfig>
    {
        [DataMember(Name = "google_analytics", EmitDefaultValue = false)]
        public GoogleAnalyticsConfig GoogleAnalytics { get; set; }

        [DataMember(Name = "firebase_push", EmitDefaultValue = false)]
        public FirebasePushConfig FirebasePush { get; set; }
        
        [DataMember(Name = "braze", EmitDefaultValue = false)]
        public BrazeConfig Braze { get; set; }

        [DataMember(Name = "google_play_games", EmitDefaultValue = false)]
        public GooglePlayGamesConfig GooglePlayGames { get; set; }

        [DataMember(Name = "steam", EmitDefaultValue = false)]
        public SteamConfig Steam { get; set; }

        [DataMember(Name = "xbox", EmitDefaultValue = false)]
        public XboxConfig Xbox { get; set; }

        public bool Equals(ExtensionConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            bool isEqual = true;
            if (GoogleAnalytics != null && other.GoogleAnalytics != null)
            {
                isEqual = GoogleAnalytics.Equals(other.GoogleAnalytics);
            }
            
            if (FirebasePush != null && other.FirebasePush != null)
            {
                isEqual = isEqual && FirebasePush.Equals(other.FirebasePush);
            }

            if (Braze != null && other.Braze != null)
            {
                isEqual = isEqual && Braze.Equals(other.Braze);
            }

            if (GooglePlayGames != null && other.GooglePlayGames != null)
            {
                isEqual = isEqual && GooglePlayGames.Equals(other.GooglePlayGames);
            }

            if (Steam != null && other.Steam != null)
            {
                isEqual = isEqual && Steam.Equals(other.Steam);
            }

            if(Xbox != null && other.Xbox != null)
            {
                isEqual = isEqual && Xbox.Equals(other.Xbox);
            }

            return isEqual;
        }
    }
}