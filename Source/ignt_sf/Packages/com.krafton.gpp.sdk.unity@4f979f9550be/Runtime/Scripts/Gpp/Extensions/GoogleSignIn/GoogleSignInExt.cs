using System.Collections;
using System.Collections.Generic;
using Gpp.Utils;
using UnityEngine;

namespace Gpp.Extensions.GoogleSignIn
{
    internal class GoogleSignInExt : GppExtension<IGppGoogleSignIn, GoogleSignInExt>
    {
        protected override bool IsSupportPlatform()
        {
            return PlatformUtil.IsAndroid();
        }

        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.googlesignin";
        }

        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.GoogleSignIn.GppGoogleSignIn";
        }
    }
}
