using System;
using System.Collections;
using System.Collections.Generic;
using Gpp.Extensions.GoogleSignIn.Models;
using UnityEngine;

namespace Gpp.Extensions.GoogleSignIn
{
    public interface IGppGoogleSignIn
    {
        public void Login(Action<GoogleSignInResponse> callback);
    }
}

