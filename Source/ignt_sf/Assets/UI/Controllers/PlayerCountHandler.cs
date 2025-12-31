using com.krafton.fantasysports.UI;
using TMPro;
using UnityEngine;

namespace com.krafton.fantasysports
{
    public class PlayerCountHandler : MonoBehaviour
    {
        public UserTeamSO dataSO;
        public TextMeshProUGUI textMesh;

        void Update()
        {
            textMesh.text = dataSO.PlayerCount + "/11";
        }
    }
}
