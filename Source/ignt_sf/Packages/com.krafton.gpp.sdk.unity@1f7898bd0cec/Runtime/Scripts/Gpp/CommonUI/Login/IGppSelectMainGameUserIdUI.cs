using System;
using System.Collections;
using System.Collections.Generic;
using Gpp.Auth;
using Gpp.Constants;
using Gpp.Models;
using UnityEngine;

namespace Gpp.CommonUI.Login
{
    public interface IGppSelectMainGameUserIdUI
    {
        /// <summary>
        /// 대표 계정 선택이 필요한 경우 계정 정보 리스트와 계정 선택 또는 취소 시 동작을 등록합니다.
        /// </summary>
        public void Init(MultipleGameUserResult result, Action<SelectMainGameUserIdParam, GameObject> onSelect, Action<GameObject> closeEvent);
    }
}