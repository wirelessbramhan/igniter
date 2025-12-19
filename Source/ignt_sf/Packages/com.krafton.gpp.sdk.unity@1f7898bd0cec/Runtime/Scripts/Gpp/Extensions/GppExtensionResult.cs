using System.Runtime.Serialization;

namespace Gpp.Extensions
{
    [DataContract]
    internal class GppExtensionResult
    {
        [DataMember(Name = "responseCode")]
        public int Code { get; set; }

        [DataMember(Name = "responseMessage")]
        public string Message { get; set; }

        [DataMember(Name = "responseObject")]
        public object Value { get; set; }

        public bool IsError => Code != 0;

        public override string ToString()
        {
            return $"[{Code}] {Message}";
        }

        public GppExtensionResult()
        {
            Value = null;
        }
    }

    [DataContract]
    internal class GppExtensionResult<T> : GppExtensionResult
    {
        [DataMember(Name = "responseObject")]
        public new T Value
        {
            get => (T)base.Value;
            set => base.Value = value;
        }

        public GppExtensionResult()
        {
            Value = default;
        }
    }
}