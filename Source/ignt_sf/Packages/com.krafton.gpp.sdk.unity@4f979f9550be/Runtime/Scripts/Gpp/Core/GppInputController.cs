
using System;
using Gpp.Constants;
using Gpp.Extensions.Steam;

namespace Gpp
{
    internal class GppInputController
    {
        private InputControllerType inputControllerType;
        internal Action<InputControllerType, InputControllerType> OnInputControllerChanged;
        internal GppInputController()
        {

#if UNITY_EDITOR
            inputControllerType = InputControllerType.KeyBoardMouse;
#elif UNITY_ANDROID || UNITY_IOS
            inputControllerType = InputControllerType.Touch;
#elif UNITY_PS5
            inputControllerType = InputControllerType.DualSense;
#elif UNITY_GAMECORE_SCARLETT
            inputControllerType = InputControllerType.XboxController;
#elif UNITY_STANDALONE
            inputControllerType = InputControllerType.KeyBoardMouse;
            if(Environment.GetEnvironmentVariable("SteamDeck") == "1")
                inputControllerType = InputControllerType.XboxController;
#else
            inputControllerType = InputControllerType.KeyBoardMouse;
#endif
        }

        internal InputControllerType GetControllerType()
        {
            return inputControllerType;
        }

        internal void SetControllerType(InputControllerType controllerType)
        {
            var prevControllerType = inputControllerType;
            inputControllerType = controllerType;
            OnInputControllerChanged?.Invoke(prevControllerType, controllerType);
        }

        internal bool IsGamePad()
        {
            return inputControllerType == InputControllerType.XboxController
                || inputControllerType == InputControllerType.DualSense
                || inputControllerType == InputControllerType.ProCon;
        }
    }
}
