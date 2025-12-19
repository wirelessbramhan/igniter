using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gpp.CommonUI.Legal.Console
{
    public interface IGppConsoleLegalUI
    {
        /// <summary>
        /// ��� ID�� ��� ������ ���޵˴ϴ�.
        /// Passes the policy ID and policy information.
        /// </summary>
        public void SetPolicies(Dictionary<string, GppLegalModel> policies, Action<GppLegalModel> onViewDetail);

        /// <summary>
        /// ��� ����� ���ǵ� ��� ȣ���ؾ� �ϴ� Delegate�� ���޵˴ϴ�.
        /// Passes the delegate to be called when all policies are agreed to.
        /// </summary>
        public void SetSubmitDelegate(Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmit);

        /// <summary>
        /// Passes the delegate to be called when user close legal ui.
        /// </summary>
        public void SetCloseDelegate(Action<GameObject> onClose);
    }
}
