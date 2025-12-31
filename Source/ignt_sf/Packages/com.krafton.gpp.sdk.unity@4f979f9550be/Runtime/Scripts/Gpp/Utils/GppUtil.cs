using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Gpp.Constants;
using UnityEditor;
using UnityEngine;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using Object = UnityEngine.Object;
using System.Linq;
using Random = System.Random;

namespace Gpp.Utils
{
    internal static class GppUtil
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern string gppCommon_getOSDetail();
#endif
        public static string UnescapeJsonString(string jsonString)
        {
            jsonString = Regex.Unescape(jsonString);
            jsonString = jsonString.Replace("\\\"", "\"");
            jsonString = jsonString.Replace("\"{", "{").Replace("}\"", "}");

            return jsonString;
        }

        public static int UnixTimeNow()
        {
            DateTime dt = new DateTime(1970, 1, 1);
            return (int)DateTime.UtcNow.Subtract(dt).TotalSeconds;
        }

        public static long UnixTimeMillisecondsNow()
        {
            DateTime dt = new DateTime(1970, 1, 1);
            return Convert.ToInt64(DateTime.UtcNow.Subtract(dt).TotalMilliseconds);
        }

        public static string GetCurrentTimeInISO8601()
        {
            DateTime now = DateTime.Now;
            DateTime utcNow = now.ToUniversalTime();
            return utcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        public static string GenerateNonce()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
            var random = new Random();
            var nonce = new char[128];
            for (int i = 0; i < nonce.Length; i++)
            {
                nonce[i] = chars[random.Next(chars.Length)];
            }

            return new string(nonce);
        }

        public static string GenerateCodeChallenge(string codeVerifier)
        {
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            var b64Hash = Convert.ToBase64String(hash);
            var code = Regex.Replace(b64Hash, "\\+", "-");
            code = Regex.Replace(code, "\\/", "_");
            code = Regex.Replace(code, "=+$", "");
            return code;
        }

        public static string ConvertUrlToWebSocketUrl(string url)
        {
            const string urlPattern = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
            return Regex.IsMatch(url, urlPattern, RegexOptions.IgnoreCase) ? Regex.Replace(url, @"^.*://", "wss://") : "";
        }

        public static string EnsureTrailingSlash(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL must not be null or empty.", nameof(url));

            if (!url.EndsWith("/"))
                url += "/";

            return url;
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static string XorString(string key, string input)
        {
            var resultBuilder = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                resultBuilder.Append((char)(input[i] ^ key[(i % key.Length)]));
            }

            return resultBuilder.ToString();
        }

        public static GameObject FindChildWithName(GameObject parent, string name)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }

                GameObject foundObject = FindChildWithName(child.gameObject, name);
                if (foundObject != null)
                {
                    return foundObject;
                }
            }

            return null;
        }

        public static T FindChildWithName<T>(GameObject parent, string name) where T : Component
        {
            foreach (Transform child in parent.transform)
            {
                if (child.name == name)
                {
                    return child.gameObject.GetComponent<T>();
                }

                GameObject foundObject = FindChildWithName(child.gameObject, name);
                if (foundObject != null)
                {
                    return foundObject.GetComponent<T>();
                }
            }

            return null;
        }

        public static T FindParent<T>(GameObject gameObject) where T : Component
        {
            Transform parent = gameObject.transform.parent;

            while (parent != null)
            {
                T parentComponent = parent.GetComponent<T>();
                if (parentComponent != null)
                {
                    return parentComponent;
                }

                parent = parent.parent;
            }

            return null;
        }

        public static string ConvertUnixTimeToLocalTime(long unixTime, string format = "yyyy/MM/dd HH:mm:ss")
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime utcDateTime = epoch.AddSeconds(unixTime);
            DateTime localDateTime = utcDateTime.ToLocalTime();
            return localDateTime.ToString(format);
        }

        public static string GetMarketUrl(StoreType marketType = StoreType.GooglePlayStore)
        {
            string packageName = Application.identifier;
            string url = string.Empty;

            switch (marketType)
            {
                case StoreType.None:
                case StoreType.GooglePlayStore:
                    url = $"market://details?id={packageName}";
                    break;
                case StoreType.AppStore:
                    url = $"itms-apps://itunes.apple.com/app/id{packageName}";
                    break;
                case StoreType.GalaxyStore:
                    url = $"samsungapps://ProductDetail/{packageName}";
                    break;
                default:
                    throw new ArgumentException("Invalid market type.");
            }

            return url;
        }

        public static bool TryFindGameObjectWithName(string name, out GameObject gameObject)
        {
            GameObject foundObj = GameObject.Find(name);
            if (foundObj == null)
            {
                gameObject = null;
                return false;
            }

            gameObject = foundObj;
            return true;
        }

        public static bool TryFindFirstComponent<T>(out GameObject foundObject) where T : Component
        {
            T[] components = Resources.FindObjectsOfTypeAll<T>();

            if (components.Length > 0)
            {
                foundObject = components[0].gameObject;
                return true;
            }

            foundObject = null;
            return false;
        }

        public static Texture2D CreateQrCodeImage(string code, int width = 256, int height = 256)
        {
            var qrWriter = new QRCodeWriter();
            var qrOptions = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 0
            };
            var bitMatrix = qrWriter.encode(code, BarcodeFormat.QR_CODE, width, height, qrOptions.Hints);
            Texture2D texture = new Texture2D(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = bitMatrix[x, y] ? Color.black : Color.white;
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            texture.ignoreMipmapLimit = true;
            return texture;
        }

        public static string GetOperationSystem()
        {
            var operatingSystem = SystemInfo.operatingSystem;
#if UNITY_ANDROID
            operatingSystem = "Android";
#elif UNITY_IOS
            operatingSystem = "IOS";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            operatingSystem = "Windows";
#elif UNITY_STANDALONE_OSX
            operatingSystem = "Mac";
#elif UNITY_GAMECORE_SCARLETT
            operatingSystem = "XBoxScarlett";
#elif UNITY_PS5
            operatingSystem = "PS5";
#endif
            return operatingSystem;
        }

        public static string GetDevice()
        {
            string device = SystemInfo.deviceType.ToString();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_OSX
            device = "PC";
#elif UNITY_IOS || UNITY_ANDROID
            device = "MOBILE";
#elif UNITY_PS5 || UNITY_XBOXONE || UNITY_GAMECORE
            device = "CONSOLE";
#else
            device = "NONE";
#endif
            return device;
        }

        public static void DeleteChildByName(GameObject parent, string childNameToDelete)
        {
            Transform childTransform = parent.transform.Find(childNameToDelete);

            if (childTransform != null)
            {
                Object.Destroy(childTransform.gameObject);
                Debug.Log($"Deleted child: {childNameToDelete}");
            }
            else
            {
                Debug.Log($"Child with name '{childNameToDelete}' not found under parent '{parent.name}'.");
            }
        }

        public static void DeleteChildren(Transform transform, bool exceptFirstElement = false)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (exceptFirstElement && i == 0)
                {
                    continue;
                }
                Object.Destroy(transform.GetChild(i));
            }
        }

        public static bool IsRtlLang()
        {
            if (!GppSDK.IsInitialized)
            {
                return false;
            }

            var langCode = GppSDK.GetSession().LanguageCode;
            return langCode.Equals("ar", StringComparison.OrdinalIgnoreCase) || langCode.Equals("ur", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetOSDetail()
        {
            string osDetail = SystemInfo.operatingSystem;
#if UNITY_EDITOR
#elif UNITY_ANDROID
            using (AndroidJavaClass androidJavaObject = new AndroidJavaClass("com.krafton.commonlibrary.GppCommon"))
            {
                osDetail = androidJavaObject.CallStatic<string>("getAndroidOSInfo");
            }
#elif UNITY_IOS
            osDetail = gppCommon_getOSDetail();
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            osDetail = osDetail.Split(' ').Where(x => !x.Contains("bit")).Aggregate((cur, next) => $"{cur} {next}");
#elif UNITY_GAMECORE_SCARLETT
            osDetail = Gpp.Extensions.Xbox.XboxExt.Impl().OSVersion();
#endif
            return osDetail;
        }
    }
}