using System.Collections;
using ignt.sports.cricket.core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports.UI
{
    public class CoinCounter : ViewElement
    {
        public FloatVariableSO UserCoins;

        // void OnEnable()
        // {
        //     GameStateManager.OnStateEnter += AddCoins;
        // }

        // void OnDisable()
        // {
        //     GameStateManager.OnStateEnter -= AddCoins;
        // }

        // private IEnumerator AddCoinsDelayed(float delay, int steps = 5)
        // {
        //     if (btnImage && UserCoins)
        //     {
        //         for (int i = 0; i < steps; i++)
        //         {
        //             btnImage.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
        //             UserCoins.Value += 100;

        //             yield return new WaitForSeconds(delay);

        //             btnImage.transform.localScale = Vector3.one;
        //         }
        //     }
        // }

        // private void AddCoins(GState state)
        // {
        //     if (state == GState.home)
        //     {
        //         StartCoroutine(AddCoinsDelayed(0.2f));
        //     }
        // }

        // void Update()
        // {
        //     btnText.text = UserCoins.Value.ToString();
        // }
    }
}
