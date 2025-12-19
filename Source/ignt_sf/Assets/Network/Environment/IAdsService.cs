namespace ignt.sports.cricket.network
{
    public interface  IAdsService : IService
    {
        public void ShowInterstitial();
        public void ShowRewardedAd();

        public bool ToggleBannerVisibility();

        public bool ToggleMRecVisibility();
        
        public void ShowMediationDebugger();
    }
}
