namespace StrangeToolkit.Serializer
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public abstract class Serializer
    {
        private List<Type> knownTypes = new List<Type>();

        public List<Type> KnownTypes
        {
            get
            {
                return this.knownTypes;
            }
            set
            {
                this.knownTypes = value;
            }
        }

        public abstract void Serialize<T>(T value, Stream stream);

        public abstract T Deserialize<T>(Stream stream);
    }
}
