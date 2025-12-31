using System.Runtime.Serialization;

namespace Gpp.Extensions.GooglePlayGames.Models
{
    [DataContract]
    public class GpgResponse
    {
        [DataMember(Name = "responseCode")]
        public int Code;
        
        [DataMember(Name = "responseMessage")]
        public string Message;
        
        [DataMember(Name = "responseObject")]
        public GpgResponseObject ResponseObject;

        public GpgResponse()
        {
            
        }

        public GpgResponse(int code, string message, GpgResponseObject obj)
        {
            Code = code;
            Message = message;
            ResponseObject = obj;
        }
    }

    [DataContract]
    public class GpgResponseObject
    {
        [DataMember(Name = "accessToken")]
        public string AccessToken;
        
        [DataMember(Name = "playerId")]
        public string PlayerId;
    }
}