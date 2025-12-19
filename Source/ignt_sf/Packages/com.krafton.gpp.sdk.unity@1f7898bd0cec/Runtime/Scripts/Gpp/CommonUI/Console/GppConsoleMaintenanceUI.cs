using System;
using System.Collections.Generic;
using System.Globalization;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Localization;
using Gpp.Models;
using Gpp.Telemetry;
using Gpp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

namespace Gpp.CommonUI.Console
{
    public class GppConsoleMaintenanceUI : GppConsoleUI, IGppConsoleMaintenanceUI
    {
        public GameObject CurrentLayout;
        public GameObject detailButton;
        public GameObject moveIcon;
        private Models.Maintenance _maintenance;
        private Action<GameObject> _onConfirm;

        [SerializeField]
        private RectTransform[] contentSizeFitTransform;
        [SerializeField]
        private ScrollRect scrollRect;
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        private float scrollSpeed = 0.8f;
        [SerializeField][Range(0.05f, 0.5f)] private float deadzone = 0.1f;

        private const string TypeNone = "NONE";
        private const string TypeManual = "MANUAL";
        private const string TypeSystem = "SYSTEM";

        private void Update()
        {
            if (Gamepad.current != null)
            {
                if (Gamepad.current.buttonSouth.wasPressedThisFrame) // A ��ư
                {
                    GppSyncContext.RunOnUnityThread(() =>
                    {
                        _onConfirm?.Invoke(gameObject);
                        _onConfirm = null;
                    });
                }

                if (Gamepad.current.buttonNorth.wasPressedThisFrame && detailButton.activeInHierarchy) // Y ��ư
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ViewMaintenanceDetails);
                    Application.OpenURL(_maintenance.ExternalUrl);
                }

                Vector2 stickInput = Vector2.zero;
                stickInput = Gamepad.current.leftStick.ReadValue();
                if (Mathf.Abs(stickInput.y) > deadzone)
                {
                    float newPosition = scrollRect.verticalNormalizedPosition + stickInput.y * scrollSpeed * Time.unscaledDeltaTime;
                    scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newPosition);
                }

                float contentHeight = content.GetComponent<RectTransform>().sizeDelta.y;
                moveIcon.SetActive(contentHeight > 380);
            }
        }

        public void SetMaintenanceData(Models.Maintenance maintenance)
        {
            _maintenance = maintenance;
            BindingMaintenanceData();
        }

        public void SetOnClickConfirmListener(Action<GameObject> onConfirm)
        {
            _onConfirm = onConfirm;
        }

        private void BindingMaintenanceData()
        {
            SetTitle();
            SetStartDate();
            SetEndDate();
            SetRemainTime();
            SetLink();
            SetDescription();

            for (int i = 0; i < contentSizeFitTransform.Length; i++)
            {
                RefreshLayout(contentSizeFitTransform[i]);
            }
        }

        private void SetDescription()
        {
            var detail = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "Description");
            detail.text = _maintenance.Detail;
        }

        private void SetLink()
        {
            if (string.IsNullOrEmpty(_maintenance.ExternalUrl))
            {
                detailButton.SetActive(false);
            }
        }

        private void SetRemainTime()
        {
            var remainTimeObj = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "RemainTime");
            if (!_maintenance.ShowRemainingTime)
            {
                remainTimeObj.gameObject.SetActive(false);
                return;
            }

            string date;
            switch (_maintenance.DisplayPeriodType)
            {
                case TypeSystem:
                    date = _maintenance.EndDate;
                    break;
                case TypeManual:
                    date = _maintenance.DisplayEndDate;
                    break;
                default:
                    remainTimeObj.gameObject.SetActive(false);
                    return;
            }
            DateTime endDate = DateTime.Parse(date).ToLocalTime();
            DateTime currentDate = DateTime.Parse(_maintenance.CurrentTime).ToLocalTime();
            var remainSeconds = (endDate - currentDate).TotalSeconds;
            var remainTime = ConvertSecondsToRemainTime((long)remainSeconds);
            remainTimeObj.text = remainTime;
        }

        private void SetStartDate()
        {
            var startDateObj = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "StartDate");
            string date;
            switch (_maintenance.DisplayPeriodType)
            {
                case TypeSystem:
                    if (string.IsNullOrEmpty(_maintenance.StartDate))
                    {
                        startDateObj.gameObject.SetActive(false);
                        return;
                    }

                    date = _maintenance.StartDate;
                    break;
                case TypeManual:
                    if (string.IsNullOrEmpty(_maintenance.DisplayStartDate))
                    {
                        startDateObj.gameObject.SetActive(false);
                        return;
                    }
                    date = _maintenance.DisplayStartDate;
                    break;
                default:
                    startDateObj.gameObject.SetActive(false);
                    return;
            }

            string startDateStr = $"{LocalizationKey.MaintenanceStart.Localise()} : {FormatTime(date)}";
            startDateObj.text = startDateStr;
        }

        private void SetEndDate()
        {
            var endDateObj = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "EndDate");
            string date;
            switch (_maintenance.DisplayPeriodType)
            {
                case TypeSystem:
                    if (string.IsNullOrEmpty(_maintenance.EndDate))
                    {
                        endDateObj.text = $"{LocalizationKey.MaintenanceEnd.Localise()} : {LocalizationKey.MaintenanceUndecided.Localise()}";
                        return;
                    }

                    date = _maintenance.EndDate;
                    break;
                case TypeManual:
                    if (string.IsNullOrEmpty(_maintenance.DisplayEndDate))
                    {
                        endDateObj.text = $"{LocalizationKey.MaintenanceEnd.Localise()} : {LocalizationKey.MaintenanceUndecided.Localise()}";
                        return;
                    }
                    date = _maintenance.DisplayEndDate;
                    break;
                default:
                    endDateObj.text = $"{LocalizationKey.MaintenanceEnd.Localise()} : {LocalizationKey.MaintenanceUndecided.Localise()}";
                    return;
            }

            string endDateStr = $"{LocalizationKey.MaintenanceEnd.Localise()} : {FormatTime(date)}";
            endDateObj.text = endDateStr;
        }

        private void SetTitle()
        {
            var title = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "Title");
            title.text = _maintenance.Title;
        }

        private static string ConvertSecondsToRemainTime(long seconds)
        {
            var timeDic = new Dictionary<string, string>();
            string result = $"{LocalizationKey.MaintenanceRemainingTime.Localise()} : ";
            const int secondsPerMinute = 60;
            const int secondsPerHour = 3600;
            const int secondsPerDay = 86400;

            long days = seconds / secondsPerDay;
            long hours = seconds % secondsPerDay / secondsPerHour;
            long minutes = seconds % secondsPerHour / secondsPerMinute;
            // long secs = seconds % secondsPerMinute;

            if (seconds <= 0)
            {
                timeDic.Add("{n}", "0");
                result += $"{LocalizationKey.MaintenanceRemainSeconds.Localise(null, timeDic)} ";
            }
            else if (days > 0)
            {
                timeDic.Add("{d}", days.ToString());
                timeDic.Add("{h}", hours.ToString());
                result += $"{LocalizationKey.MaintenanceRemainDays.Localise(null, timeDic)} ";
            }
            else if (hours > 0)
            {
                timeDic.Add("{n}", hours.ToString());
                result += $"{LocalizationKey.MaintenanceRemainHours.Localise(null, timeDic)} ";
            }
            else if (minutes > 0)
            {
                timeDic.Add("{n}", minutes.ToString());
                result += $"{LocalizationKey.MaintenanceRemainMinutes.Localise(null, timeDic)} ";
            }
            else
            {
                timeDic.Add("{n}", "1");
                result += $"{LocalizationKey.MaintenanceRemainSeconds.Localise(null, timeDic)} ";
            }

            return result.Trim();
        }

        private static string FormatTime(string isoTime)
        {
            DateTime utcDateTime = DateTime.Parse(isoTime, null, DateTimeStyles.AdjustToUniversal);
            DateTime localDateTime = utcDateTime.ToLocalTime();
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            string offset = localTimeZone.BaseUtcOffset.Hours >= 0
                ? $"UTC+{localTimeZone.BaseUtcOffset.Hours:D2}:{localTimeZone.BaseUtcOffset.Minutes:D2}"
                : $"UTC{localTimeZone.BaseUtcOffset.Hours:D2}:{localTimeZone.BaseUtcOffset.Minutes:D2}";
            return localDateTime.ToString("MM/dd HH:mm").Replace("-", "/") + $" ({offset})";
        }
        private void RefreshLayout(RectTransform target)
        {
            StartCoroutine(ForceRebuildNextFrame(target));
        }

        private IEnumerator ForceRebuildNextFrame(RectTransform target)
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        }
    }
}