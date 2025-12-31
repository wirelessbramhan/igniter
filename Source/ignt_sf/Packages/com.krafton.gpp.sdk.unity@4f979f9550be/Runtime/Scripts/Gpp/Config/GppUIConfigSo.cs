using System;
using Gpp.CommonUI;
using TMPro;
using UnityEngine;

[Serializable]
public class GppUIConfigSo : ScriptableObject
{
    [Header("Font (Only GppCommonUI)")]
    public TMP_FontAsset font;

    [Header("Common")]
    public GameObject maintenanceUI;
    public GameObject modalUI;
    public GameObject toastUI;
    public GameObject repayUI;

    [Header("Mobile")]
    public GameObject mobileLoginUI;
    public GameObject mobileLegalUI;
    public GameObject mobileJapanPaymentUI;
    public GameObject mobileGpgPopupUI;

    [Header("PC")]
    public GameObject pcLoginUI;
    public GameObject pcLegalUI;
    public GameObject pcAccountCheckUI;
    public GameObject pcAuthWaitingUI;

    public static GppUIConfigSo LoadFromFile()
    {
        return Resources.Load<GppUIConfigSo>("GppSDK/GppUIConfig");
    }

    public static GameObject GetCustomUIPrefab(UIType type)
    {
        GppUIConfigSo uiConfigSo = LoadFromFile();

        if (uiConfigSo == null)
        {
            return null;
        }

        return type switch
        {
            // Common
            UIType.Maintenance => uiConfigSo.maintenanceUI,
            UIType.Modal => uiConfigSo.modalUI,
            UIType.Toast => uiConfigSo.toastUI,
            UIType.RepayRequired => uiConfigSo.repayUI,

            // Mobile
            UIType.MobileLogin => uiConfigSo.mobileLoginUI,
            UIType.MobileLegal => uiConfigSo.mobileLegalUI,
            UIType.MobileJapanPayment => uiConfigSo.mobileJapanPaymentUI,
            UIType.MobileGPGSPopup => uiConfigSo.mobileGpgPopupUI,

            // PC
            UIType.PcLogin => uiConfigSo.pcLoginUI,
            UIType.PcLegal => uiConfigSo.pcLegalUI,
            UIType.PcAccountCheck => uiConfigSo.pcAccountCheckUI,
            UIType.PcAuthWaiting => uiConfigSo.pcAuthWaitingUI,

            _ => null
        };
    }
}