using System;
using System.Text;
using Newtonsoft.Json;

namespace Gpp.Utils
{
    internal static class JwtDecoder
    {
        public static T DecodeJwtToken<T>(string jwtToken)
        {
            var parts = jwtToken.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid JWT token format.");
            }

            var payload = parts[1];
            var jsonPayload = Base64UrlDecode(payload);

            var result = JsonConvert.DeserializeObject<T>(jsonPayload);

            return result;
        }

        private static string Base64UrlDecode(string input)
        {
            string output = input;
            output = output.Replace('-', '+').Replace('_', '/');
            switch (output.Length % 4)
            {
                case 0:
                    break;
                case 2:
                    output += "==";
                    break;
                case 3:
                    output += "=";
                    break;
                default:
                    throw new ArgumentException("Illegal base64url string!");
            }

            var converted = Convert.FromBase64String(output);
            return Encoding.UTF8.GetString(converted);
        }
    }
}