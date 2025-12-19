using System;
using System.Text;
using System.Security.Cryptography;
using Gpp.Log;
using UnityEngine;

namespace Gpp.Utils
{
    public static class GPPSecurityHelper
    {
        internal const string VERSION = "1_0";
        
        private const string FALLBACK_ENCRYPTION_KEY = "GPP_FALLBACK_KEY";
        private const int AES_BLOCK_SIZE = 16;
        
        public static string EncryptString(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return string.Empty;
            }
            
            var plainData = Encoding.UTF8.GetBytes(plainText);
            var originalSize = plainData.Length;
            var paddingSize = AES_BLOCK_SIZE - (originalSize % AES_BLOCK_SIZE);
            var paddedSize = originalSize + paddingSize;
            var paddedData = new byte[paddedSize];
            
            Array.Copy(plainData, paddedData, originalSize);
            
            for (var i = originalSize; i < paddedSize; i++)
            {
                paddedData[i] = (byte)paddingSize;
            }
            
            var aesKey = GetEncryptionKey(key);
            var encryptedData = EncryptAesEcb(paddedData, aesKey);
            
            return Convert.ToBase64String(encryptedData);
        }
        
        public static string DecryptString(string encryptedText, string key)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return string.Empty;
            }
            
            try
            {
                var encryptedData = Convert.FromBase64String(encryptedText);
                
                if (encryptedData.Length is 0 || encryptedData.Length % AES_BLOCK_SIZE != 0)
                {
                    return string.Empty;
                }
                
                var aesKey = GetEncryptionKey(key);
                var decryptedData = DecryptAesEcb(encryptedData, aesKey);
                
                if (!RemovePkcs7Padding(ref decryptedData))
                {
                    return string.Empty;
                }
                
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception e)
            {
                GppLog.LogWarning(e);
                return string.Empty;
            }
        } 
        
        private static byte[] GetEncryptionKey(string key)
        {
            var effectiveKey = key;
            
            if (string.IsNullOrEmpty(effectiveKey))
            {
                effectiveKey = FALLBACK_ENCRYPTION_KEY;
                GppLog.Log("Using fallback key (WEAK SECURITY)");
            }
            
            var data = Encoding.UTF8.GetBytes(effectiveKey);
            var keyBytes = new byte[32];
            Array.Clear(keyBytes, 0, 32);
            
            for (var i = 0; i < data.Length; i++)
            {
                keyBytes[i % 32] ^= data[i];
            }
            
            return keyBytes;
        }

        private static byte[] EncryptAesEcb(byte[] data, byte[] key)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            
            using var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }
        
        private static byte[] DecryptAesEcb(byte[] data, byte[] key)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            
            using var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(data, 0, data.Length);
        }

        private static bool RemovePkcs7Padding(ref byte[] data)
        {
            if (data.Length is 0)
            {
                return false;
            }
            
            var paddingValue = data[data.Length - 1];
            
            if (paddingValue is 0 or > AES_BLOCK_SIZE)
            {
                return false;
            }
            
            if (paddingValue > data.Length)
            {
                return false;
            }
            
            var startIndex = data.Length - paddingValue;
            for (var i = startIndex; i < data.Length; i++)
            {
                if (data[i] != paddingValue)
                {
                    return false;
                }
            }
            
            var unpaddedData = new byte[startIndex];
            Array.Copy(data, unpaddedData, startIndex);
            data = unpaddedData;
            
            return true;
        }
    }
}