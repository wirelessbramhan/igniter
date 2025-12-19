using System.Collections;
using System.Collections.Generic;
using Gpp.Api;
using Gpp.Core;
using Gpp.Models;
using UnityEngine;

namespace Gpp.Api
{
    internal partial class KCNApi : GppApi
    {
        protected override string GetServiceName()
        {
            return "kcn";
        }


        internal void GetLastCreatorCode(ResultCallback<KCNResult> callback)
        {
            Run(RequestGetLastCreatorCode(callback));
        }

        internal void SetCreatorCode(string code, ResultCallback<KCNResult> callback)
        {
            Run(RequestSetCreatorCode(code, null, callback));
        }

        internal void SetCreatorCode(string code, string[] associatedPurchases, ResultCallback<KCNResult> callback)
        {
            Run(RequestSetCreatorCode(code, associatedPurchases, callback));
        }

        internal void DeleteCreatorCode(ResultCallback callback)
        {
            Run(RequestDeleteCreatorCode(callback));
        }
    }
}

