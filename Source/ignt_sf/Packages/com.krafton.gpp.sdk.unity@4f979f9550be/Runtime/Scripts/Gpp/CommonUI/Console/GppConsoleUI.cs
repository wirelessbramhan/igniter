using System.Collections.Generic;
using Gpp.Constants;
using Gpp.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gpp.CommonUI.Console
{
    public class GppConsoleUI : MonoBehaviour
    {
        private class IconContainer
        {
            internal readonly Sprite MoveIcon;
            internal readonly Sprite BtnIconA;
            internal readonly Sprite BtnIconB;
            internal readonly Sprite BtnIconX;
            internal readonly Sprite BtnIconY;

            internal IconContainer(string moveIcon, string btnIconA, string btnIconB, string btnIconX, string btnIconY)
            {
                MoveIcon = Resources.Load<Sprite>($"GppConsoleUIImage/{moveIcon}");
                BtnIconA = Resources.Load<Sprite>($"GppConsoleUIImage/{btnIconA}");
                BtnIconB = Resources.Load<Sprite>($"GppConsoleUIImage/{btnIconB}");
                BtnIconX = Resources.Load<Sprite>($"GppConsoleUIImage/{btnIconX}");
                BtnIconY = Resources.Load<Sprite>($"GppConsoleUIImage/{btnIconY}");
            }
        }
        private GameObject prevSelected = null;
        private Dictionary<InputControllerType, IconContainer> iconDictionary;
        protected virtual void Start()
        {
            prevSelected = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);

            iconDictionary = new Dictionary<InputControllerType, IconContainer>()
            {
                [InputControllerType.XboxController] = new IconContainer("Xbox_L_Stick_Default", "Xbox_Button_A_Default", "Xbox_Button_B_Default", "Xbox_Button_X_Default", "Xbox_Button_Y_Default"),
                //[InputControllerType.DualSense] = new IconContainer("PS_D_Pad", "PS_Button_Cross", "PS_Button_Circle", "PS_Button_Square", "PS_Button_Triangle"),
                [InputControllerType.DualSense] = new IconContainer("Xbox_L_Stick_Default", "PS_Button_Cross", "PS_Button_Circle", "PS_Button_Square", "PS_Button_Triangle"), // 기획에서 스틱 아이콘으로 통일해서 제공하기로 했습니다.
                [InputControllerType.ProCon] = new IconContainer("Xbox_L_Stick_Default", "Xbox_Button_A_Default", "Xbox_Button_B_Default", "Xbox_Button_X_Default", "Xbox_Button_Y_Default")
            };

            //ui ������ ����Ʈ�� xbox�� ps5�� ���� ���� 
            //32, 62
            var inputControllerType = GppSDK.GetInputController().GetControllerType();

            float moveIconHeight = 62.0f;
            // 기획에서 스틱 아이콘으로 통일해서 제공하기로 했습니다.
            // if (inputControllerType == InputControllerType.DualSense)
            //     moveIconHeight = 32.0f;

            SetButtonIcon("MoveIcon", iconDictionary[inputControllerType].MoveIcon, 32.0f, moveIconHeight);
            SetButtonIcon("BtnIconA", iconDictionary[inputControllerType].BtnIconA);
            SetButtonIcon("BtnIconB", iconDictionary[inputControllerType].BtnIconB);
            SetButtonIcon("BtnIconX", iconDictionary[inputControllerType].BtnIconX);
            SetButtonIcon("BtnIconY", iconDictionary[inputControllerType].BtnIconY);

            GppSDK.GetInputController().OnInputControllerChanged += UpdateController;
        }

        protected virtual void OnDestroy()
        {
            // 추후 콘솔 UI 개선 작업을 위해서 남겨둠
            GppSDK.GetInputController().OnInputControllerChanged -= UpdateController;
        }

        private void SetButtonIcon(string objectName, Sprite icon, float x = 0.0f, float y = 0.0f)
        {
            var target = GppUtil.FindChildWithName(gameObject, objectName);
            if (target == null)
                return;

            Image imageComponent = target.GetComponentInChildren<Image>();
            if (imageComponent == null)
                return;

            if (x != 0.0f || y != 0.0f)
                imageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
            if (icon == null)
            {
                return;
            }
            imageComponent.sprite = icon;
        }

        private void UpdateController(InputControllerType prevController, InputControllerType currentController)
        {
            if (iconDictionary.ContainsKey(currentController))
            {
                float moveIconHeight = 62.0f;
                // 기획에서 스틱 아이콘으로 통일해서 제공하기로 했습니다.
                // if (currentController == InputControllerType.DualSense)
                //     moveIconHeight = 32.0f;

                SetButtonIcon("MoveIcon", iconDictionary[currentController].MoveIcon, 32.0f, moveIconHeight);
                SetButtonIcon("BtnIconA", iconDictionary[currentController].BtnIconA);
                SetButtonIcon("BtnIconB", iconDictionary[currentController].BtnIconB);
                SetButtonIcon("BtnIconX", iconDictionary[currentController].BtnIconX);
                SetButtonIcon("BtnIconY", iconDictionary[currentController].BtnIconY);
            }
            else
            {
                ChangeToPcUI();
            }
        }

        protected virtual UIType PcUIType()
        {
            return UIType.None;
        }

        protected virtual void ChangeToPcUI()
        {

        }
    }
}