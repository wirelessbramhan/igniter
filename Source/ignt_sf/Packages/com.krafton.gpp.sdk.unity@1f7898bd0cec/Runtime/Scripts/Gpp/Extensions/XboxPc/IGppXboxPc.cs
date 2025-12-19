using System;
using System.Collections.Generic;
using Gpp.Core;

namespace Gpp.Extensions.XboxPc
{
    internal interface IGppXboxPc
    {
        public void Init(Action<int> callback);
        public bool CanUse();
        public void GetToken(ResultCallback<string> callback);
    }
}