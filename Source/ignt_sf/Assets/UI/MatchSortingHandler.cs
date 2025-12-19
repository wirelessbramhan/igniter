using System;
using System.Collections;
using ignt.sports.cricket.core;
using UnityEngine;

namespace ignt.sports.cricket.UI
{
    public class MatchSortingHandler : MonoBehaviour
    {
        public LeagueUIDataSO dataSO;
        public LeagueCard LeagueCardDefault;
        [SerializeField]
        private bool enableSort = false;

        private float highestTime = 0;
        void OnEnable()
        {
            GameStateManager.OnStateEnter += SpawnLeagues;
        }

        void OnDisable()
        {
            GameStateManager.OnStateEnter -= SpawnLeagues;
        }

        private void SpawnLeagues(GState state)
        {
            if (state == GState.home)
            {
                int count = 0;

                foreach (var item in dataSO.DummyLeagues)
                {
                    LeagueCard card = Instantiate(LeagueCardDefault, this.transform);
                    card.SetData(item.TeamA, item.TeamB, item.MatchName, item.SecondsLeft, item.PrizeAmount);
                    card.gameObject.name = "Card" + count;
                    card.PopCard(count * 0.1f);
                    count++;
                }
            }

            enableSort = true;
            StartCoroutine(SortCards());
        }

        private IEnumerator SortCards()
        {
            var cards = GetComponentsInChildren<LeagueCard>();

            for (int i = 0; i < cards.Length - 1; i++)
            {
                if (cards[i].secondsLeft > cards[i + 1].secondsLeft)
                {
                    SwapCards(cards[i], cards[i + 1]);
                }

                yield return new WaitForSeconds(1.0f);
            }
        }

        private void SwapCards(LeagueCard prev, LeagueCard next)
        {
            int temp = prev.transform.GetSiblingIndex();

            prev.transform.SetSiblingIndex(next.transform.GetSiblingIndex());
            next.transform.SetSiblingIndex(temp);
        }
    }
}
