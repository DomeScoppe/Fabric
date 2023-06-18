using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Loom.Core
{
    public static class Serializer
    {
        public static void Serialize<T>(T instance, string path)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Create);
                var serializer = new DataContractJsonSerializer(typeof(T));

                using var writer = JsonReaderWriterFactory.CreateJsonWriter(fs, Encoding.UTF8, true, true, "   ");

                serializer.WriteObject(writer, instance);
                writer.Flush();
            }
            catch(Exception ex) 
            { 
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to serialize {instance} to {path}");

                throw;
            }
        }

        public static T Deserialize<T>(string path)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Open);
                var serializer = new DataContractJsonSerializer(typeof(T));

                T instance = (T)serializer.ReadObject(fs);

                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to deserialize {path}");

                throw;
            }
        }
    }
}
