using UnityEngine;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "AdjustSO", menuName = "NetworkManager/Services/MMPServices/Adjust")]
    public class AdjustSO : MMPServiceSO
    {
        public override void InitializeService()
        {
            // AdjustConfig adjustConfig = new AdjustConfig("tvi3aju8311c", AdjustEnvironment.Sandbox);
            // Adjust.InitSdk(adjustConfig);
            
        }

        public override void TrackEvent()
        {
            //throw new System.NotImplementedException();
        }

        //public override void TrackRevenue<T>(T adInfo)
        //{
        //    if (adInfo is MaxSdkBase.AdInfo info)
        //    {
        //        Debug.Log("Tracking AdRevenue from Applovin");

        //        var adjustAdRevenue = new AdjustAdRevenue("applovin_max_sdk");

        //        adjustAdRevenue.SetRevenue(info.Revenue, "USD");
        //        adjustAdRevenue.AdRevenueNetwork = info.NetworkName;
        //        adjustAdRevenue.AdRevenueUnit = info.AdUnitIdentifier;
        //        adjustAdRevenue.AdRevenuePlacement = info.Placement;
        //        Adjust.TrackAdRevenue(adjustAdRevenue);
        //    }
        //    else
        //    {
        //        Debug.Log("Invalid AdInfo");
        //    }
        //}
    }
}