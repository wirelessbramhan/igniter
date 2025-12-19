using System.Collections.Generic;
using ignt.sports.cricket.core;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports
{
    public class PlayerSelectHandler : MonoBehaviour
    {
        public FloatVariableSO PlayerCredits;
        public List<Image> SelectionBars;
        protected int SelectionCount = 0;

        public void AddPlayer(float creditValue)
        {
            SelectionBars[SelectionCount].color = Color.green;
            PlayerCredits.Value -= creditValue;
        }
    }
}
