using UnityEngine;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "MMPService", menuName = "NetworkManager/Services/MMPServices/MMPService")]
    
    public class MMPServiceSO : ScriptableObject, IMMPService
    {
        public virtual void InitializeService()
        {
            //throw new System.NotImplementedException();
        }

        public virtual void TrackEvent()
        {
            //throw new System.NotImplementedException();
        }

        public virtual void TrackRevenue<T>(T revenueInfo)
        {
            //throw new System.NotImplementedException();
        }
    }
}