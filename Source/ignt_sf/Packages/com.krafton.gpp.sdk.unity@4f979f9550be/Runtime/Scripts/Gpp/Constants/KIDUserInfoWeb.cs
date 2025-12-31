namespace Gpp.Constants
{    
    public class KIDUserInfoUri
    {
        public string UriPath { get; }

        private KIDUserInfoUri(string uriPath = null)
        {
            UriPath = uriPath;
        }

        public static readonly KIDUserInfoUri None = null;
        public static readonly KIDUserInfoUri PersonalInfo = new KIDUserInfoUri("personal-info");
        public static readonly KIDUserInfoUri ConnectionsAccounts = new KIDUserInfoUri("connections-accounts");
        public static readonly KIDUserInfoUri ConnectionsGames = new KIDUserInfoUri("connections-games");
        public static readonly KIDUserInfoUri ConnectionsCode = new KIDUserInfoUri("connections-code");
        public static readonly KIDUserInfoUri AccountSecurity = new KIDUserInfoUri("account-security");
        public static readonly KIDUserInfoUri CreatorHistory = new KIDUserInfoUri("creator-history");
        public static readonly KIDUserInfoUri CreatorDonation = new KIDUserInfoUri("creator-donation");
        public static readonly KIDUserInfoUri CreatorStatus = new KIDUserInfoUri("creator-status");

        public override string ToString() => UriPath;
    }
}