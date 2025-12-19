using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Gpp.Log
{
    [DataContract]
    internal class GppResponseTimeLog
    {
        [DataMember]
        internal double HttpResponseTime;
        [DataMember]
        internal long ResponseCode;
        [DataMember]
        internal string UrlPath;
        [DataMember]
        internal string Function;

        public override string ToString()
        {
            return $"HttpResponseTime : {HttpResponseTime} seconds, ResponseCode : {ResponseCode}, UrlPath : {UrlPath}";
        }
    }
}