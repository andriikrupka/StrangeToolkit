namespace StrangeToolkit.Serializer
{
    using System.IO;
    using System.Runtime.Serialization;

    public class XmlSerializer : Serializer
    {
        public override void Serialize<T>(T value, Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(T), this.KnownTypes);
            serializer.WriteObject(stream, value);
        }

        public override T Deserialize<T>(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(T), this.KnownTypes);
            return (T)serializer.ReadObject(stream);
        }
    }
}
