namespace StrangeToolkit.Serializer
{
    using System.IO;
    using System.Runtime.Serialization.Json;

    public class JsonSerializer : Serializer
    {

        public override void Serialize<T>(T value, Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(T), this.KnownTypes);
            serializer.WriteObject(stream, value);
        }

        public override T Deserialize<T>(Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(T), this.KnownTypes);
            return (T)serializer.ReadObject(stream);
        }
    }
}
