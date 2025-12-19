using System;
using System.Collections.Generic;
using ignt.sports.cricket.core;
using UnityEngine;

namespace com.krafton.fantasysports
{
    public class PlayerCardHandler : MonoBehaviour
    {
        public PlayerDataSO dataSO;
        public PlayerCard cardPrefab;
        [SerializeField]
        protected List<PlayerCard> AllCards;

        void OnEnable()
        {
            GameStateManager.OnStateEnter += StartSpawn;
        }

        void OnDisable()
        {
            GameStateManager.OnStateEnter -= StartSpawn;
        }

        private void StartSpawn(GState state)
        {
            if (state == GState.create)
            {
                SpawnPlayerCards();
            }
        }

        public void SpawnPlayerCards()
        {
            for (int i = 0; i < dataSO.PlayerDatas.Count; i++)
            {
                PlayerCard newCard = Instantiate(cardPrefab, transform);
                newCard.SetData(dataSO.PlayerDatas[i]);

                AllCards.Add(newCard);
            }
        }
    }
}
