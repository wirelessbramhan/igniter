using Gpp;
using Gpp.CommonUI;
using Gpp.Constants;
using UnityEngine;

public abstract class GppCommonPcUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GppSDK.GetInputController().OnInputControllerChanged += UpdateController;
    }

    void OnDestroy()
    {
        GppSDK.GetInputController().OnInputControllerChanged -= UpdateController;
    }

    protected abstract UIType ConsoleUIType();
    protected abstract void UpdateController(InputControllerType prevController, InputControllerType currentController);
}
