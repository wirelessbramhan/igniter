using System;
using System.Collections.Generic;
using Gpp.Core;

namespace Gpp.Extensions.Xbox
{
    internal interface IGppXbox
    {
        public void Init(Action<int> callback);
        public bool CanUse();
        public void GetToken(ResultCallback<string> callback);
        public string DeviceId();
        public string OSVersion();
    }
}
