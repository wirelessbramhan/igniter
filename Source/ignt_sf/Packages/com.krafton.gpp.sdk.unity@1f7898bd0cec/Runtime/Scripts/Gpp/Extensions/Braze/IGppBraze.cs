using Gpp.Config.Extensions;

namespace Gpp.Extensions.Braze
{
    internal interface IGppBraze
    {
        public void Init();
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        public void SaveBrazeConfig(BrazeConfig config);  
#endif
    }
}