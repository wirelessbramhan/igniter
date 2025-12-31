using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gpp.CommonUI.Legal
{
    public interface IGppLegalUI
    {
        /// <summary>
        /// 약관 ID와 약관 정보가 전달됩니다.
        /// Passes the policy ID and policy information.
        /// </summary>
        public void SetPolicies(Dictionary<string, GppLegalModel> policies, Action onViewDetail);
        
        /// <summary>
        /// 모든 약관이 동의된 경우 호출해야 하는 Delegate가 전달됩니다.
        /// Passes the delegate to be called when all policies are agreed to.
        /// </summary>
        public void SetSubmitDelegate(Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmit);
        
        /// <summary>
        /// 다른 계정으로 로그인 버튼이 선택된 경우 호출되어야 하는 Delegate가 전달됩니다.
        /// Passes the delegate to be called when the login with another account button is selected.
        /// </summary>
        public void SetLoginAnotherAccountDelegate(Action<GameObject> onLoginAnotherAccount);
    }
}