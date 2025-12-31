using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Gpp.Constants;
using Gpp.Log;

namespace Gpp.Models
{
    [DataContract]
    public class AuthorizeResult
    {
        [DataMember(Name = "code")]
        public string Code;

        [DataMember(Name = "redirect_to")]
        public string RedirectTo;
    }

    [DataContract]
    public class WebViewAuthResult
    {
        [DataMember(Name = "code")]
        public string Code;

        [DataMember(Name = "error")]
        public string Error;

        [DataMember(Name = "errorDescription")]
        public string ErrorDescription;

        public bool IsError => !string.IsNullOrEmpty(Error);
    }

    [DataContract]
    public class KidInfoResult
    {
        [DataMember(Name = "game_user")]
        public GameUser GameUser;

        [DataMember(Name = "kid_info")]
        public KidInfo KidInfo;
    }

    [DataContract]
    public class KidIdTokenResult
    {
        [DataMember(Name = "expires_in")]
        public int ExpiresIn;

        [DataMember(Name = "id_token")]
        public string IdToken;

        [DataMember(Name = "krafton_id")]
        public string KraftonId;
    }

    [DataContract]
    public class GameUser
    {
        [DataMember(Name = "deletion_status")]
        public bool DeletionStatus;

        [DataMember(Name = "linked_at")]
        public string LinkedAt;

        [DataMember(Name = "namespace")]
        public string Namespace;

        [DataMember(Name = "user_id")]
        public string UserId;
    }

    [DataContract]
    public class KidInfo
    {
        [DataMember(Name = "email")]
        public string Email;

        [DataMember(Name = "krafton_id")]
        public string KraftonId;

        [DataMember(Name = "krafton_tag")]
        public string KraftonTag;
    }

    [DataContract]
    public class PcConsoleAuthResult
    {
        [DataMember(Name = "code")]
        public string Code;

        [DataMember(Name = "email")]
        public string Email;

        [DataMember(Name = "ktag")]
        public string KTag;

        [DataMember(Name = "state")]
        public string State;

        [DataMember(Name = "device_code")]
        public string DeviceCode;

        [DataMember(Name = "error_code")]
        public int ErrorCode = -999;

        [DataMember(Name = "expires_in")]
        public int ExpiresIn = 0;

        [DataMember(Name = "redirect_uri")]
        public string RedirectUri;

        [DataMember(Name = "user_code")]
        public string UserCode;

        [DataMember(Name = "verification_uri")]
        public string VerificationUri;

        [DataMember(Name = "ws_uri")]
        public string WebSocketUri;
    }

    [DataContract]
    public class CheckEligibilityResult
    {
        [DataMember(Name = "age_status")]
        public string AgeStatus;

        [DataMember(Name = "country")]
        public string Country;

        [DataMember(Name = "device_code")]
        public string DeviceCode;

        [DataMember(Name = "error_code")]
        public int ErrorCode = -999;

        [DataMember(Name = "expires_in")]
        public int ExpiresIn = 0;

        [DataMember(Name = "ktag")]
        public string Ktag;

        [DataMember(Name = "redirect_uri")]
        public string RedirectUri;

        [DataMember(Name = "user_code")]
        public string UserCode;

        [DataMember(Name = "verification_uri")]
        public string VerificationUri;

        [DataMember(Name = "verified")]
        public bool Verified;

        [DataMember(Name = "ws_uri")]
        public string WebSocketUri;
    }

    [DataContract]
    public class MultipleGameUserResult
    {
        [DataMember(Name = "error_code")]
        public int ErrorCode { get; set; }

        [DataMember(Name = "error_description")]
        public string ErrorDescription { get; set; }

        [DataMember(Name = "error_body")]
        public Dictionary<string, MultipleGameUserInfo> ErrorBody { get; set; }

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
    }

    [DataContract]
    public class MultipleGameUserInfo
    {
        [DataMember(Name = "created_at")]
        public long CreatedAt { get; set; }

        [DataMember(Name = "last_login_at")]
        public long LastLoginAt { get; set; }

        [DataMember(Name = "display_name")]
        public string DisplayName { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "is_full_kid")]
        public bool IsFullKid { get; set; }

        [DataMember(Name = "platform_id")]
        public string PlatformId { get; set; }

        [DataMember(Name = "main_game_user_id")]
        public bool MainGameUserId { get; set; }

        [DataMember(Name = "banned")]
        public bool Banned { get; set; }

        [DataMember(Name = "deletion_status")]
        public bool DeletionStatus { get; set; }
    }

    internal class AuthWebSocketResult
    {
        public AuthMsgType Message { get; private set; }
        public string Code { get; private set; }
        public string State { get; private set; }

        public AuthWebSocketResult(string wsMsg)
        {
            Message = AuthMsgType.NONE;
            Code = "";
            State = "";

            try
            {
                var result = wsMsg.Split("::");

                if (result.Length > 0)
                {
                    if (Enum.TryParse(result[0], out AuthMsgType message))
                    {
                        Message = message;
                    }
                }

                if (result.Length > 1)
                {
                    Code = result[1];
                }

                if (result.Length > 2)
                {
                    State = result[2];
                }
            }
            catch (Exception ex)
            {
                GppLog.LogWarning($"Error processing websocket message: {ex.Message}");
            }
        }
    }
}