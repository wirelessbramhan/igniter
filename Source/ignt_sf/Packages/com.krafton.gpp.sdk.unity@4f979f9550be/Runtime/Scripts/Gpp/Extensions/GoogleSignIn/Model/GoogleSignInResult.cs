using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Gpp.Extensions.GoogleSignIn.Models
{
    public class GoogleSignInResponse
    {
        [DataMember(Name = "responseCode")]
        public int Code;

        [DataMember(Name = "responseMessage")]
        public string Message;

        [DataMember(Name = "responseObject")]
        public GoogleSignInResponseObject ResponseObject;
    }


    [DataContract]
    public class GoogleSignInResponseObject
    {
        [DataMember(Name = "idToken")]
        public string IdToken;
    }
}

