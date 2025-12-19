using System;
using System.Collections.Generic;
using Gpp.Core;

namespace Gpp.Extensions.Ps5
{
    internal interface IGppPs5
    {
        public void Init(Action<bool> callback);
        public bool CanUse();
        public void GetToken(ResultCallback<string> callback);
        public string GetCountryCode();
    }
}
