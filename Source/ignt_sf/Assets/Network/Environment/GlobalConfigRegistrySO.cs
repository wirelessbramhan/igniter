using UnityEngine;
using ignt.sports.cricket.core;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(menuName = "Config/GlobalConfigRegistry")]
    public class GlobalConfigRegistrySO : ScriptableObject
    {
        public ScriptableObject KOSInitialization;
        
        // public EnvironmentType EnvironmentType;
        
        public AdsServiceSO AdsService;

        public MMPServiceSO MmpService;
        
        public  AuthenticationSO AuthService;
        

    }
}

