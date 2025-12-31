using UnityEngine;
using Gpp.Models;
using RTLTMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace Gpp.CommonUI.Login
{
    public class GppSelectMainGameUserIdItem : MonoBehaviour, ISelectHandler
    {
        public RTLTextMeshPro userInfoText;
        Action<RectTransform> _onSelected;

        public void Init(string userId, MultipleGameUserInfo userInfo, Action<RectTransform> onSelected = null)
        {
            _onSelected = onSelected;
            userInfoText.text =
                $"UserId : {userId}\n" +
                $"CreatedAt : {userInfo.CreatedAt}\n" +
                $"LastLoginAt : {userInfo.LastLoginAt}\n" +
                $"DisplayName : {userInfo.DisplayName}\n" +
                $"UserName : {userInfo.Username}\n" +
                $"PlatformId : {userInfo.PlatformId}\n" +
                $"IsFullKid : {userInfo.IsFullKid}\n" +
                $"IsMainGameUserId : {userInfo.MainGameUserId}\n" +
                $"Banned : {userInfo.Banned}\n" +
                $"DeletionStatus : {userInfo.DeletionStatus}";
        }


        public void OnSelect(BaseEventData eventData)
        {
            if (_onSelected != null)
                _onSelected.Invoke(GetComponent<RectTransform>());
        }
    }
}