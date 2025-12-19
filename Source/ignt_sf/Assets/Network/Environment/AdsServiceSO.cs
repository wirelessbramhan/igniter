using UnityEngine;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "Applovin", menuName = "NetworkManager/Services/Ads/AdService")]
    public class AdsServiceSO : ScriptableObject, IAdsService
    {
        public virtual void InitializeService()
        {
            //throw new System.NotImplementedException();
        }

        public virtual void ShowInterstitial()
        {
            //throw new System.NotImplementedException();
        }

        public virtual void ShowRewardedAd()
        {
            //throw new System.NotImplementedException();
        }

        public virtual bool ToggleBannerVisibility()
        {
            //throw new System.NotImplementedException();
            return false;
        }

        public virtual bool ToggleMRecVisibility()
        {
            //throw new System.NotImplementedException();
            return false;
        }

        public virtual void ShowMediationDebugger()
        {
           // throw new System.NotImplementedException();
        }
    }
}