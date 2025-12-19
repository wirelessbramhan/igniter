using TMPro;
using UnityEngine;

namespace Gpp.Core
{
    public class GppFontCustomizer : MonoBehaviour
    {
        private void Start()
        {
            GppUIConfigSo uiConfigSo = Resources.Load<GppUIConfigSo>("GppSDK/GppUIConfig");
            if (uiConfigSo == null || uiConfigSo.font == null)
            {
                return;
            }
            TextMeshProUGUI[] textMeshProUguIs = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI tmp in textMeshProUguIs)
            {
                tmp.font = uiConfigSo.font;
            }
        }
    }
}