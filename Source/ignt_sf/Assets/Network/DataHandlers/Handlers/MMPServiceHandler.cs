using UnityEngine;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "MMPServiceHandler", menuName = "NetworkManager/MMPServiceHandler")]
    public class MMPServiceHandler : ScriptableObject
    {
        
        public MMPServiceSO MMPServiceSo;
        
        public void InitializeService()
        {
            MMPServiceSo.InitializeService();
        }

        public void TrackEvent()
        {
            MMPServiceSo.TrackEvent();
        }

        public void TrackRevenue<T>(T revenueInfo)
        {
            MMPServiceSo.TrackRevenue(revenueInfo);
        }
    }
}