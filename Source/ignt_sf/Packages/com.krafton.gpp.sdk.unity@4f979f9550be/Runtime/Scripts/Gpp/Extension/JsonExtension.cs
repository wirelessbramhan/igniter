using System.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Gpp.Extension
{
    internal static class JsonExtension
    {
        public static byte[] ToUtf8Json<T>(this T obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public static string ToJsonString<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static string ToPrettyJsonString<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static T ToObject<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static string ToDataString(this byte[] data)
        {
            if (data == null || data.Length == 0)
                return string.Empty;

            return Encoding.UTF8.GetString(data);
        }

        public static T ToObject<T>(this byte[] data)
        {
            if (data == null || data.Length == 0)
                return default;

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
        }

        // 객체의 [DataMember] Name을 Key, 값을 문자열 Value로 변환한 Dictionary 반환
        public static Dictionary<string, string> ToDictionary<T>(this T obj)
        {
            var result = new Dictionary<string, string>();
            if (obj == null) return result;

            var type = obj.GetType();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            foreach (var prop in type.GetProperties(flags))
            {
                var dataMember = prop.GetCustomAttribute<DataMemberAttribute>();
                if (dataMember == null) continue;

                var key = string.IsNullOrEmpty(dataMember.Name) ? prop.Name : dataMember.Name;

                var valueObj = prop.GetValue(obj);

                string valueStr;

                if (valueObj == null)
                {
                    valueStr = string.Empty;
                }
                // 문자열 컬렉션 처리
                else if (valueObj is IEnumerable<string> stringEnumerable)
                {
                    valueStr = string.Join(",", stringEnumerable);
                }
                // 비-문자열 컬렉션 처리
                else if (valueObj is System.Collections.IEnumerable enumerable and not string)
                {
                    var list = new List<string>();
                    foreach (var item in enumerable)
                    {
                        if (item == null) continue;
                        list.Add(Convert.ToString(item, CultureInfo.InvariantCulture));
                    }
                    valueStr = string.Join(",", list);
                }
                else
                {
                    valueStr = valueObj == null 
                        ? string.Empty 
                        : (valueObj as string ?? Convert.ToString(valueObj, CultureInfo.InvariantCulture));
                }

                result[key] = valueStr ?? string.Empty;
            }

            return result;
        }
    }
}