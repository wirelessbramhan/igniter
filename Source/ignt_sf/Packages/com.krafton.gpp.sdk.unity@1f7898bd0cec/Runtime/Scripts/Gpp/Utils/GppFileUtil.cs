using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gpp.Utils
{
    internal static class GppFileUtil
    {
        public static void Save<T>(string filePath, T data, bool useBinaryStream = false, bool useIndentedFormatting = true)
        {
            CreateDirectoryIfNotExist(filePath);
            if (!File.Exists(filePath))
            {
                Create(filePath, data, useBinaryStream, useIndentedFormatting);
            }
            else
            {
                Update(filePath, data, useBinaryStream, useIndentedFormatting);
            }
        }

        public static T Load<T>(string filePath, bool useBinaryStream = false)
        {
            if (!File.Exists(filePath))
            {
                return default(T);
            }

            return useBinaryStream ? ReadBinary<T>(filePath) : ReadFile<T>(filePath);
        }

        public static async Task SaveAsync<T>(string filePath, T data, bool useBinaryStream = false, bool useIndentedFormatting = true)
        {
            CreateDirectoryIfNotExist(filePath);
            if (!File.Exists(filePath))
            {
                await CreateAsync(filePath, data, useBinaryStream, useIndentedFormatting);
            }
            else
            {
                await UpdateAsync(filePath, data, useBinaryStream, useIndentedFormatting);
            }
        }

        public static async Task<T> LoadAsync<T>(string filePath, bool useBinaryStream = false)
        {
            if (!File.Exists(filePath))
            {
                return default;
            }

            if (useBinaryStream)
            {
                return await ReadBinaryAsync<T>(filePath);
            }

            return await ReadFileAsync<T>(filePath);
        }

        private static void Create<T>(string filePath, T data, bool useBinaryStream, bool useIndentedFormatting)
        {
            if (useBinaryStream)
            {
                WriteBinary(filePath, data);
            }
            else
            {
                WriteFile(filePath, data, useIndentedFormatting);
            }
        }

        private static async Task CreateAsync<T>(string filePath, T data, bool useBinaryStream, bool useIndentedFormatting)
        {
            if (useBinaryStream)
            {
                await WriteBinaryAsync(filePath, data);
            }
            else
            {
                await WriteFileAsync(filePath, data, useIndentedFormatting);
            }
        }

        private static void Update<T>(string filePath, T data, bool useBinaryStream, bool useIndentedFormatting)
        {
            if (useBinaryStream)
            {
                WriteBinary(filePath, data);
            }
            else
            {
                WriteFile(filePath, data, useIndentedFormatting);
            }
        }

        private static async Task UpdateAsync<T>(string filePath, T data, bool useBinaryStream, bool useIndentedFormatting)
        {
            if (useBinaryStream)
            {
                await WriteBinaryAsync(filePath, data);
            }
            else
            {
                await WriteFileAsync(filePath, data, useIndentedFormatting);
            }
        }

        private static void WriteFile<T>(string filePath, T data, bool useIndentedFormatting)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = useIndentedFormatting ? Formatting.Indented : Formatting.None
            };
            string jsonData = JsonConvert.SerializeObject(data, settings);
            File.WriteAllText(filePath, jsonData);
        }

        private static async Task WriteFileAsync<T>(string filePath, T data, bool useIndentedFormatting)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = useIndentedFormatting ? Formatting.Indented : Formatting.None
            };
            string jsonData = JsonConvert.SerializeObject(data, settings);
            await File.WriteAllTextAsync(filePath, jsonData);
        }

        private static T ReadFile<T>(string filePath)
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        private static async Task<T> ReadFileAsync<T>(string filePath)
        {
            string jsonData = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        private static void WriteBinary<T>(string filePath, T data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            formatter.Serialize(fs, data);
        }

        private static async Task WriteBinaryAsync<T>(string filePath, T data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            await using FileStream fs = new FileStream(filePath, FileMode.Create);
            await Task.Run(() => formatter.Serialize(fs, data));
        }

        private static T ReadBinary<T>(string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using FileStream fs = new FileStream(filePath, FileMode.Open);
            return (T)formatter.Deserialize(fs);
        }

        private static async Task<T> ReadBinaryAsync<T>(string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            await using FileStream fs = new FileStream(filePath, FileMode.Open);
            return await Task.Run(() => (T)formatter.Deserialize(fs));
        }

        public static void Delete(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static async Task DeleteAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        private static void CreateDirectoryIfNotExist(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}