using System.Runtime.Serialization;

namespace Gpp.Models
{

    [DataContract(Name = "pc")]
    public class PCHardware
    {
        [DataMember(Name = "cpu")]
        public DeviceHardwareCPU[] DeviceHardwareCPUs;
        [DataMember(Name = "gpu")]
        public DeviceHardwareGPU[] DeviceHardwareGPUs;
        [DataMember(Name = "ram")]
        public DeviceHardwareRAM[] DeviceHardwareRAMs;

        [DataMember(Name = "ram_total_capacity")]
        public long RamTotalCapacity;
        [DataMember(Name = "mainboard_name")]
        public string MainboardName { get; set; }

        [DataMember(Name = "mainboard_model")]
        public string MainboardModel { get; set; }

        [DataMember(Name = "mainboard_product")]
        public string MainboardProduct { get; set; }

        [DataMember(Name = "mainboard_manufacturer")]
        public string MainboardManufacturer { get; set; }
    }

    [DataContract]
    public class DeviceHardwareCPU
    {
        [DataMember(Name = "cpu_name")]
        public string CpuName { get; set; }

        [DataMember(Name = "cpu_manufacturer")]
        public string CpuManufacturer { get; set; }

        [DataMember(Name = "cpu_socket_designation")]
        public string CpuSocketDesignation { get; set; }

        [DataMember(Name = "cpu_thread_count")]
        public int CpuThreadCount { get; set; }

        [DataMember(Name = "cpu_system_name")]
        public string CpuSystemName { get; set; }

        [DataMember(Name = "cpu_max_clock_speed")]
        public int CpuMaxClockSpeed { get; set; }

        [DataMember(Name = "cpu_ext_clock")]
        public int CpuExtClock { get; set; }

        [DataMember(Name = "cpu_architecture")]
        public string CpuArchitecture { get; set; }
    }

    [DataContract]
    public class DeviceHardwareGPU
    {
        [DataMember(Name = "gpu_name")]
        public string GpuName { get; set; }

        [DataMember(Name = "gpu_description")]
        public string GpuDescription { get; set; }

        [DataMember(Name = "gpu_adapter_ram")]
        public long GpuAdapterRam { get; set; }

        [DataMember(Name = "gpu_driver_version")]
        public string GpuDriverVersion { get; set; }
    }

    [DataContract]
    public class DeviceHardwareRAM
    {
        [DataMember(Name = "ram_name")]
        public string RamName { get; set; }

        [DataMember(Name = "ram_model")]
        public string RamModel { get; set; }

        [DataMember(Name = "ram_capacity")]
        public long RamCapacity { get; set; }

        [DataMember(Name = "ram_manufacturer")]
        public string RamManufacturer { get; set; }
    }

    [DataContract(Name = "android")]
    public class AndroidHardware
    {
        [DataMember(Name = "model")]
        public string Model;
        [DataMember(Name = "manufacture")]
        public string Manufacture;
        [DataMember(Name = "brand")]
        public string Brand;
        [DataMember(Name = "board")]
        public string Board;
        [DataMember(Name = "hardware")]
        public string Hardware;
        [DataMember(Name = "available_memory")]
        public long AvailMem;
        [DataMember(Name = "total_memory")]
        public long TotlaMem;

    }

    [DataContract]
    public class IOSHardware
    {
        [DataMember(Name = "total_memory")]
        public long TotalMemory;
        [DataMember(Name = "model")]
        public string Model;
        [DataMember(Name = "name")]
        public string Name;
        [DataMember(Name = "device_name")]
        public string DeviceName;
    }

    [DataContract]
    public class MacHardware
    {
        [DataMember(Name = "cpu")]
        public DeviceHardwareCPU[] DeviceHardwareCPUs;
        [DataMember(Name = "ram_total_capacity")]
        public long RamTotalCapacity;
        [DataMember(Name = "mainboard_name")]
        public string MainboardName { get; set; }

        [DataMember(Name = "mainboard_model")]
        public string MainboardModel { get; set; }

        [DataMember(Name = "mainboard_product")]
        public string MainboardProduct { get; set; }

        [DataMember(Name = "mainboard_manufacturer")]
        public string MainboardManufacturer { get; set; }
    }

    [DataContract]
    public class XboxHardware

    {
        [DataMember(Name = "model")]
        public string Model;
    }

    [DataContract]
    public class Ps5Hardware
    {
        [DataMember(Name = "model")]
        public string Model;
    }
}

