using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Gpp.Extensions.Mac
{
    // var authorizationCode: String = ""
    // var user: String = ""
    [DataContract]
    public class AppleSignInResult
    {
        [DataMember(Name = "responseCode")]
        public int Code;

        [DataMember(Name = "responseMessage")]
        public string Message;

        [DataMember(Name = "responseObject")]
        public string ResponseObject;
    }

    [DataContract]
    public class AppleSignInResultObject
    {
        [DataMember(Name = "authorizationCode")]
        public string AuthorizationCode;
        [DataMember(Name = "user")]
        public string User;
    }
}

