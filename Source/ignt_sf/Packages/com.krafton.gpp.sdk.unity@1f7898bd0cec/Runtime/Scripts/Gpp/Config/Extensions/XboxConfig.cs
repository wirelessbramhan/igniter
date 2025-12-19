using System;
using System.Runtime.Serialization;

namespace Gpp.Config.Extensions
{
    [DataContract]
    public class XboxConfig : IEquatable<XboxConfig>
    {
        [DataMember(Name = "scid")]
        public string Scid { get; set; } = "";
        public bool Equals(XboxConfig other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Scid == other.Scid;
        }
    }
}