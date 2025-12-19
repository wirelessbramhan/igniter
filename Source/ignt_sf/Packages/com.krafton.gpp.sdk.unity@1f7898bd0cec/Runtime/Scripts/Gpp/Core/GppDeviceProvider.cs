using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using Gpp.Datadog;
using Gpp.Log;

namespace Gpp.Core
{
    internal static class GppDeviceProvider
    {
        public static string GetGppDeviceId(bool useStoredId = false)
        {
#if UNITY_GAMECORE_SCARLETT
            var deviceId = Gpp.Extensions.Xbox.XboxExt.Impl().DeviceId();
#else
            var deviceId = SystemInfo.deviceUniqueIdentifier.Replace("-", "");
            if (string.IsNullOrEmpty(deviceId) || deviceId.All(c => c is '0'))
            {
                var remoteLog = new GppClientRemoteLog()
                {
                    ErrorCode = ((int)ErrorCode.GPPSDKInitFailed).ToString(),
                    Log = $"SystemInfo.deviceUniqueIdentifier is empty or 0. deviceId:{deviceId}"
                };
                DatadogManager.ReportLog(remoteLog);
                
                deviceId = "unknownDeviceId";
            }
#endif
#if !UNITY_EDITOR && UNITY_IOS
            if (useStoredId)
            {
                var keychainValue = GetUserKeyChain()?.deviceId;

                if (string.IsNullOrEmpty(keychainValue))
                {
                    SaveKeychain(deviceId);
                }
                else
                {
                    deviceId = keychainValue;
                }
            }
#endif
            return deviceId;
        }

#if !UNITY_EDITOR && UNITY_IOS
        private class User
        {
            public string deviceId;
        }


        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern string getKeyChainUser();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void setKeyChainUser(string userId);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void deleteKeyChainUser();
        
        private static User GetUserKeyChain()
        {
            try
            {
                Newtonsoft.Json.Linq.JObject userJson = Newtonsoft.Json.Linq.JObject.Parse(getKeyChainUser());
                return userJson.ToObject<User>();    
            }
            catch (Exception e)
            {
                GppLog.LogWarning("failed to read keychain : " + e.Message);
                return null;
            }
        }

        internal static void SaveKeychain(string value)
        {
            try
            {
                setKeyChainUser(value);
            }
            catch (Exception e)
            {
                GppLog.LogWarning("failed to save keychain : " + e.Message);
            }
        }
        
        internal static bool HasDeviceGuid()
        {
            var user = GetUserKeyChain();
            return user != null && !string.IsNullOrEmpty(user.deviceId);
        }

        internal static void RemoveValueInKeychain()
        {
            deleteKeyChainUser();
        }
#endif
        public static class MacAddressUtil
        {
            public static string GetMacAddress()
            {
                try
                {
                    byte[] macBytes = GetMacAddressBytes();
                    if (macBytes == null || macBytes.Length == 0)
                    {
                        GppLog.LogWarning("MAC 주소를 가져올 수 없습니다.");
                        return null;
                    }

                    string macString = string.Concat(macBytes.Select(b => b.ToString("x2")));

                    using (MD5 md5 = MD5.Create())
                    {
                        byte[] hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(macString));
                        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    }
                }
                catch (Exception ex)
                {
                    GppLog.LogWarning($"MAC 주소를 가져올 수 없습니다. {ex.Message}");
                    return null;
                }
            }

            private static byte[] GetMacAddressBytes()
            {
                byte[] macAddress = null;
                try
                {
                    var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                    if (interfaces == null || interfaces.Length == 0)
                    {
                        Debug.LogWarning("[MacAddress] 사용 가능한 네트워크 인터페이스가 없습니다.");
                    }
                    else
                    {
                        NetworkInterface selected = null;

#if UNITY_STANDALONE_WIN
                NetworkInterface bestEthernet = null;
                NetworkInterface bestWifi = null;

                foreach (var nic in interfaces)
                {
                    try
                    {
                        if (nic.OperationalStatus != OperationalStatus.Up)
                            continue;

                        string desc = (nic.Description ?? string.Empty).ToLowerInvariant();
                        if (desc.Contains("bluetooth") || desc.Contains("vpn") ||
                            desc.Contains("vmware") || desc.Contains("virtual"))
                            continue;

                        if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        {
                            if (bestEthernet == null || GetInterfaceIndex(nic) < GetInterfaceIndex(bestEthernet))
                                bestEthernet = nic;
                        }
                        else if (nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                        {
                            if (bestWifi == null || GetInterfaceIndex(nic) < GetInterfaceIndex(bestWifi))
                                bestWifi = nic;
                        }
                    }
                    catch (Exception nicEx)
                    {
                        GppLog.LogWarning($"[MacAddress] 손상된 NIC(ID: {nic?.Id})를 건너뜁니다. 오류: {nicEx.Message}");
                        continue;
                    }
                }

                selected = bestEthernet ?? bestWifi;
#else
                        foreach (var nic in interfaces)
                        {
                            try
                            {
                                if (nic.OperationalStatus != OperationalStatus.Up)
                                    continue;

                                string desc = (nic.Description ?? string.Empty).ToLowerInvariant();
                                string name = (nic.Name ?? string.Empty).ToLowerInvariant();

                                if (desc.Contains("bluetooth") || desc.Contains("vpn") ||
                                    desc.Contains("vmware") || desc.Contains("virtual") ||
                                    name.Contains("utun") || name.Contains("bridge"))
                                    continue;

                                if (selected == null || GetInterfaceIndex(nic) < GetInterfaceIndex(selected))
                                    selected = nic;
                            }
                            catch (Exception nicEx)
                            {
                                GppLog.LogWarning($"[MacAddress] 손상된 NIC(ID: {nic?.Id})를 건너뜁니다. 오류: {nicEx.Message}");
                                continue;
                            }
                        }
#endif
                        macAddress = selected?.GetPhysicalAddress()?.GetAddressBytes();
                    }

                    if (macAddress == null)
                        macAddress = GetFallbackIdentifier();
                    return macAddress;
                }
                catch (Exception ex)
                {
                    GppLog.LogWarning($"[MacAddressUtil] MAC 주소 가져오기 실패. {ex.Message}");
                    return GetFallbackIdentifier();
                }
            }

            private static int GetInterfaceIndex(NetworkInterface nic)
            {
                var index = nic.GetIPProperties()?.GetIPv4Properties()?.Index ?? int.MaxValue;
                return index;

            }
            
            private static byte[] GetFallbackIdentifier()
            {
                GppLog.LogWarning("[MacAddress] MAC 주소 획득 실패. Fallback (deviceUniqueIdentifier)을 사용합니다.");
                try
                {
                    string deviceId = SystemInfo.deviceUniqueIdentifier;
                    if (!string.IsNullOrEmpty(deviceId))
                        return HashTo6Bytes(deviceId);
                }
                catch (Exception ex)
                {
                    GppLog.LogWarning($"[MacAddress] Fallback 실패! {ex.Message}");
                }
                return new byte[] { 0, 0, 0, 0, 0, 1 };
            }
            
            private static byte[] HashTo6Bytes(string source)
            {
                using (var sha = SHA256.Create())
                {
                    var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(source));
                    var mac6 = new byte[6];
                    Buffer.BlockCopy(hash, 0, mac6, 0, 6);

                    mac6[0] = (byte)((mac6[0] | 0x02) & 0xFE);
                    return mac6;
                }
            }
        }
    }
}