namespace StrangeToolkit.Request
{
    using Utils;
    using System.Collections;
    using System.Collections.Generic;

    public class ParametersCollection : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly List<KeyValuePair<string, string>> collection;

        public ParametersCollection()
        {
            this.collection = new List<KeyValuePair<string, string>>();
        }

        public void Add(string key, string value)
        {
            Guard.ArgumentNotNullOrEmptyString(key, "key");
            Guard.ArgumentNotNullOrEmptyString(value, "value");
            this.collection.Add(new KeyValuePair<string, string>(key, value));
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.collection).GetEnumerator();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "does not meet the business logic of the application")]
        public static implicit operator ParametersCollection(Dictionary<string, string> dictionary)
        {
            var result = new ParametersCollection();
            Guard.ArgumentNotNull(dictionary, "dictionary");

            foreach (var item in dictionary)
            {
                result.Add(item.Key, item.Value);
            }

            return result;
        }
    }
}
