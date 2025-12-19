using System;
using System.Runtime.InteropServices;
using System.Text;
using Gpp.Extensions.Ps5;
using Gpp.Log;
using UnityEngine;
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
using System.Globalization;
#endif

namespace Gpp.Utils
{
    internal static class GeoUtility
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_GAMECORE_SCARLETT
        // Windows API 호출을 위한 PInvoke 선언
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetUserGeoID(int GeoClass);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetGeoInfo(int Location, int GeoType, StringBuilder lpGeoData, int cchData, int LangId);

        // GeoID 클래스와 GeoType 값 정의
        private const int GEOCLASS_NATION = 16;
        private const int GEO_ISO2 = 0x0004; // ISO 3166-1 두 자리 국가 코드
#endif

        /// <summary>
        /// 시스템에서 설정된 국가 코드를 가져오는 함수
        /// Windows에서는 GeoID를, Mac에서는 Locale을 사용
        /// </summary>
        /// <returns>ISO 3166-1 형식의 국가 코드</returns>
        public static string GetCountryCode()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_GAMECORE_SCARLETT
            return GetCountryCodeForWindows();
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            return GetCountryCodeForMac();
#elif UNITY_PS5
            return Ps5Ext.Impl().GetCountryCode();
#else
            Debug.LogWarning("현재 플랫폼에서는 국가 코드를 가져올 수 없습니다.");
            return "Unknown";
#endif
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_GAMECORE_SCARLETT
        private static string GetCountryCodeForWindows()
        {
            try
            {
                // 현재 사용자의 GeoID를 가져옴
                int geoID = GetUserGeoID(GEOCLASS_NATION);

                if (geoID != -1)
                {
                    // 국가 코드 (ISO 3166-1 두 자리 코드)를 담을 StringBuilder 생성
                    StringBuilder geoInfo = new StringBuilder(3); // ISO 2-letter code

                    // GetGeoInfo를 사용하여 국가 코드 가져오기
                    int result = GetGeoInfo(geoID, GEO_ISO2, geoInfo, geoInfo.Capacity, 0);

                    // 결과 값이 0보다 크면 성공적으로 국가 코드를 가져온 것
                    if (result > 0)
                    {
                        return geoInfo.ToString();
                    }
                }

                GppLog.LogWarning("국가 코드를 가져오지 못했습니다.");
                return "Unknown"; // 오류 시 반환되는 기본 값
            }
            catch (Exception ex)
            {
                GppLog.LogWarning($"GeoID를 사용한 국가 코드 가져오기 실패: {ex.Message}");
                return "Unknown";
            }
        }
#endif

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    private static string GetCountryCodeForMac()
    {
        try
        {
            // Mac에서는 CultureInfo를 사용하여 국가 코드 가져옴
            RegionInfo region = new RegionInfo(CultureInfo.CurrentCulture.Name);
            return region.TwoLetterISORegionName; // ISO 3166-1 두 자리 국가 코드
        }
        catch (Exception ex)
        {
            GppLog.LogWarning($"Mac에서 국가 코드 가져오기 실패: {ex.Message}");
            return "Unknown";
        }
    }
#endif
    }
}