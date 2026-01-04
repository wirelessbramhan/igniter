using System;
using System.Collections.Generic;
using com.krafton.fantasysports.core;
using ignt.sports.cricket.core;
using TMPro;
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
        [SerializeField]
        private RectTransform PlayerCounter, MatchCard, TypeBtnPanel;
        [SerializeField]
        private TextMeshProUGUI FirstStageText, SecondStageText, SecondHeaderText;
        [SerializeField] private bool hasChosenPlayers = false;
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
            hasChosenPlayers = false;

            for (int i = 0; i < PlayerData.DummyData.Count; i++)
            {
                PlayerCard newCard = Instantiate(cardPrefab, SpawnTarget);
                newCard.InitCard(PlayerData.DummyData[i]);

                AllCards.Add(newCard);
            }
        }

        [ContextMenu("Change Stage")]
        public void LockSelection()
        {
            for (int i = 0; i < AllCards.Count; i++)
            {
                AllCards[i].ChangeStage();
            }

            hasChosenPlayers = true;
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

            //Disable header panels
            PlayerCounter.gameObject.SetActive(!hasChosenPlayers);
            MatchCard.gameObject.SetActive(!hasChosenPlayers);
            TypeBtnPanel.gameObject.SetActive(!hasChosenPlayers);

            //Toggle text
            FirstStageText.gameObject.SetActive(!hasChosenPlayers);
            SecondStageText.gameObject.SetActive(hasChosenPlayers);
            SecondHeaderText.gameObject.SetActive(hasChosenPlayers);
        }
    }
}
