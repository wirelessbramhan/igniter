using System;
using System.Collections.Generic;
using com.krafton.fantasysports.core;
using ignt.sports.cricket.core;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports
{
    public class PlayerCardHandler : MonoBehaviour
    {
        public DataHandlerSO UserDataHandler;
        public PlayerDataSO PlayerData;
        [SerializeField]
        private PlayerCard cardPrefab;
        [SerializeField]
        private RectTransform SpawnTarget;
        [SerializeField]
        private List<PlayerCard> AllCards;
        [SerializeField]
        private List<Image> PlayerBars;
        [SerializeField]
        private Button nextStageButton, saveBtn;

        void Awake()
        {
            StartSpawn(GState.teamCreate);
        }

        private void StartSpawn(GState state)
        {
            if (state == GState.teamCreate)
            {
                SpawnPlayerCards();
                UserDataHandler.CreateTeam();
            }
        }

        private void SpawnPlayerCards()
        {
            for (int i = 0; i < PlayerData.DummyData.Count; i++)
            {
                PlayerCard newCard = Instantiate(cardPrefab, SpawnTarget);
                newCard.InitCard(PlayerData.DummyData[i]);

                AllCards.Add(newCard);
            }
        }

        [ContextMenu("Change Cards")]
        public void ChangeCards()
        {
            for (int i = 0; i < AllCards.Count; i++)
            {
                AllCards[i].ChangeStage();
            }
        }

        void Update()
        {
            if (UserDataHandler.CurrentTeam != null)
            {
                for (int i = 0; i < PlayerBars.Count; i++)
                {
                    if (i >= UserDataHandler.CurrentTeam.PlayerCount)
                    {
                        PlayerBars[i].color = Color.white;
                    }

                    else
                    {
                        PlayerBars[i].color = Color.green;
                    }
                }
            }

            nextStageButton.interactable = (UserDataHandler.CurrentTeam.PlayerCount == 11);
            saveBtn.interactable = (UserDataHandler.CurrentTeam.ViceCapt != null && UserDataHandler.CurrentTeam.Capt != null);
        }
    }
}
