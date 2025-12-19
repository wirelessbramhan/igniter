using ignt.sports.cricket.core;
using UnityEngine;


namespace ignt.sports.cricket.network
{
    public class ServiceManager : MonoBehaviour
    {
        [SerializeField] private GlobalConfigRegistrySO registrySo;
        

        public KosSO KOSInitialization;
        

        public AdServiceHandler AdsService;


        public MMPServiceHandler MMmpService;
        

        public  AuthHandler AuthService;

        private void Awake()
        {
            Environment.Config = registrySo;
            
            InitializeKosService();
            InitializeMMpService();
            InitializeAdsService();
            InitializeAuthService();
        }
        
        private void InitializeKosService()
        {
            //KOSInitialization = registrySo.KOSInitialization as KosSO;
            if (KOSInitialization == null)
            {
                Debug.LogWarning("KOSInitialization is missing or not of type KosSO.");
                return;
            }
            KOSInitialization.InitializeService();
        }

        private void InitializeAdsService()
        {
            //AdsService = ScriptableObject.CreateInstance<AdServiceHandler>();
            AdsService.AdsServiceSo = registrySo.AdsService;
            if (AdsService.AdsServiceSo == null)
            {
                Debug.LogWarning("AdsService is missing or not of type AdServiceHandler.");
                return;
            }

            AdsService.InitializeService();
            
        }

        private void InitializeMMpService()
        {
            //MMmpService = ScriptableObject.CreateInstance<MMPServiceHandler>();
            MMmpService.MMPServiceSo = registrySo.MmpService;
            if (MMmpService.MMPServiceSo == null)
            {
                Debug.LogWarning("MMmpService is missing or not of type MMPServiceHandler.");
                return;
            }
            MMmpService.InitializeService();
        }

        private void InitializeAuthService()
        {
            //AuthService = ScriptableObject.CreateInstance<AuthHandler>();
            AuthService.AuthenticationSo = registrySo.AuthService;
            if (AuthService == null)
            {
                Debug.LogWarning("AuthService is missing or not of type AuthHandler.");
                return;
            }
            AuthService.InitializeService();
        }
        
    }
}