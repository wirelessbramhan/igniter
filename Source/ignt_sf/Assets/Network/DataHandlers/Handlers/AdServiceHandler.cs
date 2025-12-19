using UnityEngine;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "AdServiceHandler", menuName = "NetworkManager/AdServiceHandler")]
    public class AdServiceHandler : ScriptableObject
    {

        public AdsServiceSO AdsServiceSo;


        public void InitializeService()
        {
            AdsServiceSo.InitializeService();
        }

        public void ShowInterstitial()
        {
            AdsServiceSo.ShowInterstitial();
        }

        public void ShowRewardedAd()
        {
            AdsServiceSo.ShowRewardedAd();
        }

        public bool ToggleBannerVisibility()
        {
            return AdsServiceSo.ToggleBannerVisibility();
        }

        public bool ToggleMRecVisibility()
        {
            return AdsServiceSo.ToggleMRecVisibility();
        }

        public void ShowMediationDebugger()
        {
            AdsServiceSo.ShowMediationDebugger();
        }
    }
}