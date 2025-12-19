using System;
using System.Runtime.Serialization;

namespace Gpp.Models
{
    [DataContract]
    public class KCNResult
    {
        [DataMember(Name = "associated_purchases")]
        public string[] AssociatedPurchases;

        [DataMember(Name = "code")]
        public string Code;

        [DataMember(Name = "expire_at")]
        public string ExpireAt;

        [DataMember(Name = "expired_after")]
        public int ExpiredAfter;
    }

    [DataContract]
    public class CreatorCodeResult
    {
        public string Code;
        public bool IsExpired;
        public int ExpireAfter;
        public DateTime ExpireAt;
    }
}
