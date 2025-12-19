using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gpp.Auth
{
    public class GppCheckEligibilityResult
    {
        public readonly bool IsError;
        public readonly string ErrorMessage;
        public readonly GppUser User;
        public readonly bool Verified;
        public readonly string AgeStatus;

        public GppCheckEligibilityResult(bool isError, string errorMessage, bool verified, string ageStatus)
        {
            this.IsError = isError;
            this.ErrorMessage = errorMessage;
            this.User = new GppUser(GppSDK.GetSession().cachedTokenData);
            this.Verified = verified;
            this.AgeStatus = ageStatus;
        }
    }
}

