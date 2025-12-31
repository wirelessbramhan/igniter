using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gpp.CommonUI.Legal.PcLegal
{
    public interface IGppPcLegalUI
    {
        /// <summary>
        /// 약관 ID와 약관 정보가 전달됩니다.
        /// Passes the policy ID and policy information.
        /// </summary>
        public void SetPolicies(Dictionary<string, GppLegalModel> policies, Action<GppLegalModel> onViewDetail);
        
        /// <summary>
        /// 모든 약관이 동의된 경우 호출해야 하는 Delegate가 전달됩니다.
        /// Passes the delegate to be called when all policies are agreed to.
        /// </summary>
        public void SetSubmitDelegate(Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmit);

        /// <summary>
        /// Passes the delegate to be called when user close legal ui.
        /// </summary>
        public void SetCloseDelegate(Action<GameObject> onClose);
    }
}