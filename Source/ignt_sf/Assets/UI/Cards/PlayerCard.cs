using com.krafton.fantasysports.core;
using ignt.sports.cricket.core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports
{
    public class PlayerCard : MonoBehaviour
    {
        [SerializeField]
        private PlayerData playerData;
        public Image PlayerIcon;
        public PlayerType PlayerType;
        public FloatVariableSO PlayerCredits;
        public float creditValue;
        public TextMeshProUGUI creditsText, pointsText, playerName, lastMatchStatus;
        [SerializeField]
        private bool isChosen;
        [SerializeField]
        private RectTransform FirstStagePanel, SecondStagePanel;
        [SerializeField]
        private Button CaptBtn, ViceBtn;
        public DataHandlerSO DataHandler;

        public void InitCard(PlayerData playerdata)
        {
            playerdata.IsCapt = false;
            playerdata.IsVice = false;
            playerdata.IsChosen = false;

            this.playerData = playerdata;
            SetData(playerData);
        }

        private void SetData(PlayerData data)
        {
            playerName.text = data.Name;
            PlayerType = data.Type;
            creditValue = data.CreditCost;

            creditsText.text = creditValue.ToString();
            pointsText.text = data.Pointsvalue.ToString();
            lastMatchStatus.gameObject.SetActive(data.HasPlayedLastMatch);

            FirstStagePanel.gameObject.SetActive(true);
            SecondStagePanel.gameObject.SetActive(false);
        }

        private void DeductCost()
        {
            PlayerCredits.Value -= creditValue;
        }

        private void AddCost()
        {
            PlayerCredits.Value += creditValue;
        }

        public void SetPlayerChosen(bool isChosen)
        {
            this.isChosen = isChosen;
            playerData.IsChosen = isChosen;

            if (isChosen)
            {
                DataHandler.AddPlayer(playerData);
                DeductCost();
            }

            else
            {
                DataHandler.RemovePlayer(playerData);
                AddCost();
            }
        }

        public void ChangeStage()
        {
            if (!isChosen)
            {
                gameObject.SetActive(false);
            }

            else
            {
                FirstStagePanel.gameObject.SetActive(!isChosen);
                SecondStagePanel.gameObject.SetActive(isChosen);
            }
        }

        public void SetCapt()
        {
            if (isChosen)
            {
                playerData.IsCapt = true;
                DataHandler.SetCapt(this.playerData);

                DataHandler.UpdateTeam(playerData);
            }
        }

        public void SetVice()
        {
            if (isChosen)
            {
                playerData.IsVice = true;
                DataHandler.SetVice(this.playerData);

                DataHandler.UpdateTeam(playerData);
            }
        }

        void Update()
        {
            CaptBtn.interactable = !(DataHandler.CurrentTeam.ViceCapt == this.playerData);
            ViceBtn.interactable = !(DataHandler.CurrentTeam.Capt == this.playerData);
        }
    }
}
