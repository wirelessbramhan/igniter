using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Gpp.Auth;
using Gpp.Constants;
using Gpp.Extension;
using Gpp.Log;
using Gpp.Models;
using Gpp.Utils;
using UnityEngine;

namespace Gpp.Core
{
    internal static class GppTokenProvider
    {
        private static string TokenPath => Path.Combine(Application.persistentDataPath, GppSDK.GetConfig().ClientId + "-t");
        private static string GuestUserIdPath => Path.Combine(Application.persistentDataPath, GppSDK.GetConfig().ClientId + "-g");
        private static string LastLoginInfoPath => Path.Combine(Application.persistentDataPath, GppSDK.GetConfig().ClientId + "-l");
        private static string UserIdPath => Path.Combine(Application.persistentDataPath, GppSDK.GetConfig().ClientId + "-u");
        private static string LastLoginPlatformUserIdPath => Path.Combine(Application.persistentDataPath, GppSDK.GetConfig().ClientId + "-p");
        private static string LastLoginMethodPath => Path.Combine(Application.persistentDataPath, GppSDK.GetConfig().ClientId + "-m");

        private static string EncryptString(string plainText)
        {
            return string.IsNullOrEmpty(plainText) ? string.Empty : GPPSecurityHelper.EncryptString(plainText, SystemInfo.deviceUniqueIdentifier);
        }

        private static string DecryptString(string encryptString, string version = "")
        {
            if (string.IsNullOrEmpty(encryptString))
            {
                return string.Empty;
            }
            
            return version switch
            {
                "1_0" => GPPSecurityHelper.DecryptString(encryptString, SystemInfo.deviceUniqueIdentifier),
                _ => GppUtil.XorString(GppSDK.GetConfig().Namespace, Convert.FromBase64String(encryptString).ToObject<string>())
            };
        }
        
        internal static void SaveRefreshTokenData(TokenData tokenData)
        {
            if (tokenData == null)
            {
                GppLog.LogWarning("Failed to save refresh token data. token data must not be null.");
                return;
            }
            
            var newTokenData = new RefreshTokenData
            {
                refresh_token = tokenData.RefreshToken,
                expiration_date = tokenData.RefreshTokenExpiresIn + GppUtil.UnixTimeNow()
            };
            
            try
            {
                var formatter = new BinaryFormatter();
                using var stream = new FileStream(string.Concat(TokenPath, GPPSecurityHelper.VERSION), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                formatter.Serialize(stream, EncryptString(newTokenData.ToJsonString()));
            }
            catch (Exception ex)
            {
                GppLog.LogWarning(ex);
            }
        }

        internal static void DeleteTokenData()
        {
            var securityVersion = GetSecurityVersion(TokenPath);
            var filePath = string.Concat(TokenPath, securityVersion);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        
        private static string GetSecurityVersion(string filePath)
        {   
            if (File.Exists(string.Concat(filePath, "1_0")))
            {
                return "1_0";
            }
            
            return string.Empty;
        }
        
        internal static bool TryGetRefreshTokenData(out RefreshTokenData tokenData)
        {
            tokenData = null;

            var securityVersion = GetSecurityVersion(TokenPath);
            var filePath = string.Concat(TokenPath, securityVersion);
            
            if (File.Exists(filePath) is false)
            {
                GppLog.Log("RefreshToken file does not exist.");
                return false;
            }
            
            try
            {
                using var dataStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                if (dataStream.Length is 0)
                {
                    GppLog.Log("dataStream for RefreshToken is empty.");
                    return false;
                }
                
                var formatter = new BinaryFormatter();
                
                if (securityVersion.Equals("1_0"))
                {
                    var data = (string)formatter.Deserialize(dataStream);
                    if (string.IsNullOrEmpty(data))
                    {
                        return false;
                    }
                    
                    tokenData = DecryptString(data, securityVersion).ToObject<RefreshTokenData>();
                }
                else
                {
                    if (formatter.Deserialize(dataStream) is not RefreshTokenData fileData)
                    {
                        GppLog.LogException("Saved token does not exist.");
                        return false;
                    }
                    
                    fileData.refresh_token = DecryptString(fileData.refresh_token);
                    tokenData = fileData;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                GppLog.LogWarning(ex);
                return false;
            }
        }
        
        internal static void SaveGuestUserId(string guestUserId)
        {
            if (string.IsNullOrEmpty(guestUserId))
            {
                GppLog.LogWarning("GuestDeviceId must not be null or empty.");
                return;
            }

            try
            {
                using FileStream dataStream = File.Create(GuestUserIdPath);
                BinaryFormatter formatter = new BinaryFormatter();
                GppGuestUserId fileData = new GppGuestUserId { GuestUserId = guestUserId };
                formatter.Serialize(dataStream, fileData);
            }
            catch (Exception e)
            {
                GppLog.LogWarning(e);
            }
        }

        internal static bool TryGetGuestUserId(out string guestUserId)
        {
            var gppGuestUserId = GetDataInFile<GppGuestUserId>(GuestUserIdPath);
            guestUserId = gppGuestUserId?.GuestUserId ?? string.Empty;
            return !string.IsNullOrEmpty(guestUserId);
        }

        internal static string CreateGuestUserId()
        {
            return GppDeviceProvider.GetGppDeviceId() + "#" + GppUtil.UnixTimeNow();
        }

        internal static void DeleteGuestUserId()
        {
            if (File.Exists(GuestUserIdPath))
            {
                File.Delete(GuestUserIdPath);
            }
        }

        internal static bool IsExpiredRefreshToken(int expiresDate)
        {
            return GppUtil.UnixTimeNow() >= expiresDate;
        }

        internal static void SaveUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                GppLog.LogWarning("userId must not be null or empty.");
                return;
            }

            try
            {
                using var dataStream = File.Create(UserIdPath);
                var formatter = new BinaryFormatter();
                var fileData = new GppUserId { UserId = userId };
                formatter.Serialize(dataStream, fileData);
            }
            catch (Exception e)
            {
                GppLog.LogWarning(e);
            }
        }

        internal static bool TryGetUserId(out string userId)
        {   
            var gppUserId = GetDataInFile<GppUserId>(UserIdPath);
            userId = gppUserId?.UserId ?? string.Empty;
            return !string.IsNullOrEmpty(userId);
        }
        
        internal static void DeleteUserId()
        {
            if (File.Exists(UserIdPath))
            {
                File.Delete(UserIdPath);
            }
        }

        internal static void SaveLastLoginPlatformUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                GppLog.LogWarning("userId must not be null or empty.");
                return;
            }

            try
            {
                using var dataStream = File.Create(LastLoginPlatformUserIdPath);
                var formatter = new BinaryFormatter();
                var fileData = new GppLastLoginPlatformUserId { LastLoginPlatformUserId = userId };
                formatter.Serialize(dataStream, fileData);
            }
            catch (Exception e)
            {
                GppLog.LogWarning(e);
            }
        }

        internal static bool TryGetLastLoginPlatformUserId(out string userId)
        {   
            var lastLoginPlatformUserId = GetDataInFile<GppLastLoginPlatformUserId>(LastLoginPlatformUserIdPath);
            userId = lastLoginPlatformUserId?.LastLoginPlatformUserId ?? string.Empty;
            return !string.IsNullOrEmpty(userId);
        }
        
        internal static void DeleteLastLoginPlatformUserId()
        {
            if (File.Exists(LastLoginPlatformUserIdPath))
            {
                File.Delete(LastLoginPlatformUserIdPath);
            }
        }

        internal static void SaveLastLoginType(PlatformType type)
        {
            try
            {
                using var dataStream = File.Create(LastLoginInfoPath);
                var formatter = new BinaryFormatter();
                var fileData = new LastLoginInfo { platformType = type };
                formatter.Serialize(dataStream, fileData);
            }
            catch (Exception e)
            {
                GppLog.LogWarning(e);
            }
        }

        internal static bool TryGetLastLoginType(out PlatformType type)
        {
            var lastLoginInfo = GetDataInFile<LastLoginInfo>(LastLoginInfoPath);
            type = lastLoginInfo?.platformType ?? PlatformType.None;
            return type is not PlatformType.None;
        }

        internal static void DeleteLastLoginInfo()
        {
            if (File.Exists(LastLoginInfoPath))
            {
                File.Delete(LastLoginInfoPath);
            }
        }

        internal static LoginMethodType GetLastLoginMethodType()
        {
            var gppLastLoginMethodInfo = GetDataInFile<GppLastLoginMethodInfo>(LastLoginMethodPath);
            return gppLastLoginMethodInfo?.loginMethodType ?? LoginMethodType.None;
        }

        internal static void SaveLastLoginMethodType(LoginMethodType loginMethodType)
        {
            try
            {
                using var dataStream = File.Create(LastLoginMethodPath);
                var formatter = new BinaryFormatter();
                var fileData = new GppLastLoginMethodInfo { loginMethodType = loginMethodType };
                formatter.Serialize(dataStream, fileData);
            }
            catch (Exception e)
            {
                GppLog.LogWarning(e);
            }
        }
        
        private static T GetDataInFile<T>(string filePath) where T : class
        {
            if (File.Exists(filePath) is false)
            {
                return null;
            }
            
            try
            {
                using var dataStream = new FileStream(filePath, FileMode.Open);

                if (dataStream.Length is 0)
                {
                    return null;
                }
            
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(dataStream) as T;
            }
            catch (Exception e)
            {
                GppLog.LogWarning(e);
                return null;
            }
        }
        
        [Serializable]
        public class GppGuestUserId
        {
            public string GuestUserId;
        }

        [Serializable]
        public class LastLoginInfo
        {
            public PlatformType platformType;
        }

        [Serializable]
        public class GppUserId
        {
            public string UserId;
        }

        [Serializable]
        public class GppLastLoginPlatformUserId
        {
            public string LastLoginPlatformUserId;
        }

        [Serializable]
        public class GppLastLoginMethodInfo
        {
            public LoginMethodType loginMethodType;
        }
    }
}