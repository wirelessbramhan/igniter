using System;
using System.Collections.Generic;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Log;
using Gpp.Utils;
using UnityEngine;

namespace Gpp.CommonUI.Login
{
    public class GppLoginUI : GppCommonUI, IGppLoginUI
    {
        private Action<GameObject, PlatformType> _onClickLoginButtonCallback;
        private Action<GameObject> _onClickCloseButtonCallback;

        private Dictionary<PlatformType, GameObject> _lastLoginBalloons;
        private PlatformType[] _unusedPlatformTypes;

        public void SetOnClickLoginListener(Action<GameObject, PlatformType> onLogin)
        {
            _onClickLoginButtonCallback = onLogin;
        }

        public void SetOnClickCloseListener(Action<GameObject> onClose)
        {
            _onClickCloseButtonCallback = onClose;
        }

        protected override void OnChangedOrientation(ScreenOrientation orientation)
        {
            base.OnChangedOrientation(orientation);
            InitLastLoginBalloon();
            ShowLastLoginBalloon();
        }

        protected override void Start()
        {
            base.Start();
            InitLastLoginBalloon();
            ShowLastLoginBalloon();
            UpdatePlatformButtons();
        }

        private void InitLastLoginBalloon()
        {
            GppLoginButton[] buttons = CurrentLayout.GetComponentsInChildren<GppLoginButton>();
            _lastLoginBalloons = new Dictionary<PlatformType, GameObject>();
            foreach (GppLoginButton button in buttons)
            {
                GameObject balloon = GppUtil.FindChildWithName(button.gameObject, "LastLoginBalloon");
                _lastLoginBalloons.Add(button.platformType, balloon);
            }
        }

        private void ShowLastLoginBalloon()
        {
            if (!GppTokenProvider.TryGetLastLoginType(out PlatformType type))
            {
                return;
            }

            if (type == PlatformType.None)
            {
                GppLog.Log("Can not draw LastLoginBalloon. platform type is none.");
                return;
            }

            if (type.IsGuest())
            {
                type = PlatformType.Guest;
            }

            GppLog.Log($"LastLoginPlatformType: {type}");

            if (_lastLoginBalloons.TryGetValue(type, out GameObject balloon))
            {
                balloon.SetActive(true);
            }
        }

        private void UpdatePlatformButtons()
        {
            if (_unusedPlatformTypes != null)
            {
                foreach (var platformType in _unusedPlatformTypes)
                {
                    GppUtil.FindChildWithName(gameObject, platformType.ToString()).SetActive(false);
                }
            }
        }

        public void SetUnusedPlatformType(PlatformType[] platformTypes)
        {
            _unusedPlatformTypes = platformTypes;
        }

        public void OnClickLogin(PlatformType type)
        {
            _onClickLoginButtonCallback?.Invoke(gameObject, type);
        }

        public void OnClickClose()
        {
            _onClickCloseButtonCallback?.Invoke(gameObject);
        }

        protected override void Update()
        {
            base.Update();
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            var commonUI = FindFirstObjectByType<GppCommonUI>();
            if (commonUI != null && commonUI.GetInstanceID() == GetInstanceID())
            {
                OnClickClose();
            }
        }
    }
}