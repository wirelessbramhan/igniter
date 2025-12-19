using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Gpp.Extension;
using Gpp.Log;
using Gpp.Models;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Gpp.Utils
{
    public class GppDeviceUtil
    {
#if UNITY_STANDALONE_WIN
        private const string KEY_WIN32_PROCESSOR = "Win32_Processor";
        private const string KEY_WIN32_VIDEO_CONTROLLER = "Win32_VideoController";
        private const string KEY_WIN32_PHYSICAL_MEMORY = "Win32_PhysicalMemory";
        private const string KEY_WIN32_BASEBOARD = "Win32_BaseBoard";

        private static readonly Dictionary<string, string[]> hardwareKeyValuePairs = new()
        {
            [KEY_WIN32_PROCESSOR] = new[] { "Name", "Manufacturer", "SocketDesignation", "ThreadCount", "SystemName", "MaxClockSpeed", "ExtClock", "Architecture" },
            [KEY_WIN32_VIDEO_CONTROLLER] = new[] { "Name", "Description", "AdapterRAM", "DriverVersion" },
            [KEY_WIN32_PHYSICAL_MEMORY] = new[] { "Name", "Model", "Capacity", "Manufacturer" },
            [KEY_WIN32_BASEBOARD] = new[] { "Name", "Model", "Product", "Manufacturer" }
        };

        public static PCHardware GetHardwareInfo()
        {
            var hardware = new PCHardware();
            var jobj = new JObject();
            
            foreach (var pair in hardwareKeyValuePairs)
            {
                var jsonString = GetHardwareJsonString(pair);
                
                if (string.IsNullOrEmpty(jsonString))
                {
                    jobj.Add(pair.Key, null);
                    continue;
                }
                
                var jArray = JArray.Parse(jsonString);
                jobj.Add(pair.Key, jArray);
            }

            if (jobj[KEY_WIN32_PROCESSOR] is JArray cpuArray)
            {
                hardware.DeviceHardwareCPUs = GetHardwareCPUS(cpuArray);
            }
            
            if (jobj[KEY_WIN32_VIDEO_CONTROLLER] is JArray gpuArray)
            {
                hardware.DeviceHardwareGPUs = GetHardwareGPUs(gpuArray);
            }
            
            if (jobj[KEY_WIN32_PHYSICAL_MEMORY] is JArray ramArray)
            {
                hardware.DeviceHardwareRAMs = GetHardwareRAMs(ramArray);
                hardware.RamTotalCapacity = hardware.DeviceHardwareRAMs.Sum(x => Convert.ToInt64(x.RamCapacity));
            }

            if (jobj[KEY_WIN32_BASEBOARD] is JArray mbArray)
            {
                if (mbArray.Count is 0)
                {
                    return hardware;
                }
                
                var jToken = mbArray[0];
                hardware.MainboardName = jToken["Name"]?.ToString();
                hardware.MainboardModel = jToken["Model"]?.ToString();
                hardware.MainboardProduct = jToken["Product"]?.ToString();
                hardware.MainboardManufacturer = jToken["Manufacturer"]?.ToString();
            }
            else
            {
                hardware.RamTotalCapacity = SystemInfo.systemMemorySize * 1024L * 1024L;
                hardware.MainboardProduct = SystemInfo.deviceModel;
            }
            
            return hardware;
        }

        private static string GetHardwareJsonString(KeyValuePair<string, string[]> pair)
        {
            switch (pair.Key)
            {   
                case KEY_WIN32_PROCESSOR:
                {
                    var jsonObject = new JObject
                    {
                        { "Name", SystemInfo.processorType },
                        { "ThreadCount", SystemInfo.processorCount },
                        { "SystemName", SystemInfo.deviceName }
                    };
                        
                    return new List<JObject>() { jsonObject }.ToJsonString();
                }
                case KEY_WIN32_VIDEO_CONTROLLER:
                {
                    var jsonObject = new JObject
                    {
                        { "Name", SystemInfo.graphicsDeviceName },
                        { "Description", SystemInfo.graphicsDeviceName }
                    };
                        
                    return new List<JObject>() { jsonObject }.ToJsonString();
                }
                case KEY_WIN32_PHYSICAL_MEMORY:
                {
                    var jsonObject = new JObject();
                    return new List<JObject>() { jsonObject }.ToJsonString();
                }
                case KEY_WIN32_BASEBOARD:
                {
                    break;
                }
            }
                
            return string.Empty;
        }

        private static DeviceHardwareCPU[] GetHardwareCPUS(JArray jArray)
        {
            DeviceHardwareCPU[] cpus = new DeviceHardwareCPU[jArray.Count];

            for (int i = 0; i < jArray.Count; i++)
            {
                JToken jToken = jArray[i];
                cpus[i] = new DeviceHardwareCPU()
                {
                    CpuName = !IsValidJToken(jToken["Name"]) ? "NULL" : jToken["Name"].ToString(),
                    CpuManufacturer = !IsValidJToken(jToken["Manufacturer"]) ? "NULL" : jToken["Manufacturer"].ToString(),
                    CpuSocketDesignation = !IsValidJToken(jToken["SocketDesignation"]) ? "NULL" : jToken["SocketDesignation"].ToString(),
                    CpuThreadCount = !IsValidJToken(jToken["ThreadCount"]) ? -1 : (int)jToken["ThreadCount"],
                    CpuSystemName = !IsValidJToken(jToken["SystemName"]) ? "NULL" : jToken["SystemName"].ToString(),
                    CpuMaxClockSpeed = !IsValidJToken(jToken["MaxClockSpeed"]) ? -1 : (int)jToken["MaxClockSpeed"],
                    CpuExtClock = !IsValidJToken(jToken["ExtClock"]) ? -1 : (int)jToken["ExtClock"],
                    CpuArchitecture = !IsValidJToken(jToken["Architecture"]) ? "NULL" : jToken["Architecture"].ToString(),
                };
            }

            return cpus;
        }

        private static DeviceHardwareRAM[] GetHardwareRAMs(JArray jArray)
        {
            DeviceHardwareRAM[] rams = new DeviceHardwareRAM[jArray.Count];
            for (int i = 0; i < jArray.Count; i++)
            {
                JToken jToken = jArray[i];
                rams[i] = new DeviceHardwareRAM()
                {
                    RamName = !IsValidJToken(jToken["Name"]) ? "NULL" : jToken["Name"].ToString(),
                    RamModel = !IsValidJToken(jToken["Model"]) ? "NULL" : jToken["Model"].ToString(),
                    RamCapacity = !IsValidJToken(jToken["Capacity"]) ? -1 : Convert.ToInt64(jToken["Capacity"].ToString()),
                    RamManufacturer = !IsValidJToken(jToken["Manufacturer"]) ? "NULL" : jToken["Manufacturer"].ToString(),
                };
            }

            return rams;
        }

        private static DeviceHardwareGPU[] GetHardwareGPUs(JArray jArray)
        {
            DeviceHardwareGPU[] gpus = new DeviceHardwareGPU[jArray.Count];

            for (int i = 0; i < jArray.Count; i++)
            {
                JToken jToken = jArray[i];

                gpus[i] = new DeviceHardwareGPU()
                {
                    GpuName = !IsValidJToken(jToken["Name"]) ? "NULL" : jToken["Name"].ToString(),
                    GpuDescription = !IsValidJToken(jToken["Description"]) ? "NULL" : jToken["Description"].ToString(),
                    GpuAdapterRam = (jToken["AdapterRAM"] == null || string.IsNullOrEmpty(jToken["AdapterRAM"].ToString())) ? -1 : Convert.ToInt64(jToken["AdapterRAM"].ToString()),
                    GpuDriverVersion = !IsValidJToken(jToken["DriverVersion"]) ? "NULL" : jToken["DriverVersion"].ToString()
                };
            }

            return gpus;
        }
#elif UNITY_ANDROID
        public static AndroidHardware GetHardwareInfo()
        {
            using AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var unityPlayerActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

            string deviceInfo = "";

            using (AndroidJavaClass javaClass = new AndroidJavaClass("com.krafton.commonlibrary.GppCommon"))
            {
                deviceInfo = javaClass.CallStatic<string>("getDeviceHardwareInfo", unityPlayerActivity);
            }

            if (string.IsNullOrEmpty(deviceInfo))
                return new AndroidHardware();

            JObject jobj = JObject.Parse(deviceInfo);
            AndroidHardware hardware = jobj.ToObject<AndroidHardware>();
            hardware.AvailMem = Convert.ToInt64(jobj["AvailMem"]);
            hardware.TotlaMem = Convert.ToInt64(jobj["TotalMem"]);

            return hardware;
        }
#elif UNITY_IOS
        [DllImport("__Internal")]
        private static extern string gppCommon_getDeviceInfo();
        public static IOSHardware GetHardwareInfo()
        {
            string deviceInfo = gppCommon_getDeviceInfo();
            JObject jobj = JObject.Parse(deviceInfo);

            IOSHardware hardware = jobj.ToObject<IOSHardware>();
            hardware.TotalMemory = Convert.ToInt64(jobj["totalMemory"]);
            hardware.DeviceName = jobj["deviceName"].ToString();
            return hardware;
        }
#elif UNITY_STANDALONE_OSX
        [DllImport("__Internal")]
        private static extern string gppCommon_getDeviceInfo();
        public static MacHardware GetHardwareInfo()
        {
            //     var manufacturer: String = ""
            // var name: String = ""
            // var architecture: String = ""
            // var systemName: String = ""
            // var threadCount: Int = 0
            // var maxClockSpeed: Int = 0
            // var totalMemCapacity: Int64 = 0
            var macDeviceInfo = gppCommon_getDeviceInfo();
            Debug.Log($"gppCommon_getDeviceInfo :: {macDeviceInfo}");

            MacHardware macHardware = new MacHardware();
            macHardware.DeviceHardwareCPUs = new DeviceHardwareCPU[1];

            JObject macHardWareInfo = JObject.Parse(macDeviceInfo);
            DeviceHardwareCPU deviceHardwareCPU = new DeviceHardwareCPU();
            deviceHardwareCPU.CpuArchitecture = macHardWareInfo["architecture"].ToString();
            deviceHardwareCPU.CpuName = macHardWareInfo["name"].ToString();
            deviceHardwareCPU.CpuManufacturer = macHardWareInfo["manufacturer"].ToString();
            deviceHardwareCPU.CpuSocketDesignation = string.Empty;
            deviceHardwareCPU.CpuThreadCount = int.Parse(macHardWareInfo["threadCount"].ToString());
            deviceHardwareCPU.CpuSystemName = macHardWareInfo["systemName"].ToString();
            deviceHardwareCPU.CpuMaxClockSpeed = 0;
            deviceHardwareCPU.CpuExtClock = 0;
            macHardware.DeviceHardwareCPUs[0] = deviceHardwareCPU;
            macHardware.MainboardManufacturer = macHardWareInfo["manufacturer"].ToString();
            macHardware.MainboardModel = SystemInfo.deviceModel;
            macHardware.MainboardName = SystemInfo.deviceName;
            macHardware.MainboardProduct = SystemInfo.deviceModel;
            macHardware.RamTotalCapacity = long.Parse(macHardWareInfo["totalMemCapacity"].ToString());
            return macHardware;
        }
#elif UNITY_GAMECORE_SCARLETT
        public static XboxHardware GetHardwareInfo()
        {
            XboxHardware hardware = new XboxHardware();
            hardware.Model = "XBOX SCARLETT";
            return hardware;
        }
#elif UNITY_PS5
        public static Ps5Hardware GetHardwareInfo()
        {
            Ps5Hardware hardware = new Ps5Hardware();
            hardware.Model = "PS5";
            return hardware;
        }
#endif

        private static bool IsValidJToken(JToken jToken)
        {
            return jToken != null && !string.IsNullOrEmpty(jToken.ToString());
        }
    }
}
