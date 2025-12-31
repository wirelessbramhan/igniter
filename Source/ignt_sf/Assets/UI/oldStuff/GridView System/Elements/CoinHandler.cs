using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using ignt.sports.cricket.core;
using TMPro;

namespace com.krafton.fantasysports
{
    public class CoinHandler : MonoBehaviour
    {
        [SerializeField]
        private Image coinIcon;
        [SerializeField]
        private RectTransform StartCoinPos, EndCoinPos;
        [SerializeField] private int NumberOfSteps = 10;
        [SerializeField] private FloatVariableSO playerCoins;
        [SerializeField] private TextMeshProUGUI coinText;
        public void AddCredits(int amount)
        {
            StartCoroutine(TweenCoins(NumberOfSteps, amount));
        }

        private IEnumerator TweenCoins(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                // Instantiate at the collection point
                Image coin = Instantiate(coinIcon, StartCoinPos.transform.parent);
                coin.transform.position = StartCoinPos.position;

                // Tween the coin to the target UI position
                coin.transform.DOMove(EndCoinPos.position, 0.2f)
                    .SetEase(Ease.OutBack) // Smooth movement with a little bounce
                    .OnComplete(() =>
                    {
                        // Destroy coin after animation
                        Destroy(coin.gameObject); 
                    });

                yield return new WaitForSeconds(0.1f);
            }            
        }

        private IEnumerator TweenCoins(int steps, int amount)
        {
            int value = amount/steps;

            for (int i = 0; i < steps; i++)
            {
                playerCoins.Value += value;
                // Instantiate at the collection point
                Image coin = Instantiate(coinIcon, StartCoinPos.transform.parent);
                coin.transform.position = StartCoinPos.position;

                // Tween the coin to the target UI position
                coin.transform.DOMove(EndCoinPos.position, 0.4f)
                    .SetEase(Ease.InOutBounce) // Smooth movement with a little bounce
                    .OnComplete(() =>
                    {
                        // Destroy coin after animation
                        Destroy(coin.gameObject); 
                    });

                yield return new WaitForSeconds(0.1f);
            }            
        }

        void Update()
        {
            coinText.text = playerCoins.Value.ToString();
        }
    }
}
