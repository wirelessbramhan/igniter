using System.Collections.Generic;
using com.krafton.fantasysports.core;
using UnityEngine;

namespace ignt.sports.cricket.UI
{
    [System.Serializable]
    public class PlayerIconData
    {
        [HideInInspector] public string Name;
        public Sprite Icon;
        public PlayerType PlayerType;
    }
    [CreateAssetMenu(menuName = "UI/Data Holder/Icon Data", fileName = "IconData_New", order = 6)]
    public class IconDataSO : ScriptableObject
    {
        public List<PlayerIconData> PlayerIcons;

        void OnValidate()
        {
            if (PlayerIcons != null && PlayerIcons.Count > 0)
            {
                foreach (var iconData in PlayerIcons)
                {
                    iconData.Name = iconData.PlayerType.ToString() + "_icon";
                }
            }
        }

        public Sprite GetPlayerIcon(PlayerType playerType)
        {
            Sprite icon = null;
            for (int i = 0; i < PlayerIcons.Count; i++)
            {
                if (PlayerIcons[i].PlayerType == playerType)
                {
                    icon = PlayerIcons[i].Icon;
                }
            }
            return icon;
        }
    }
}
