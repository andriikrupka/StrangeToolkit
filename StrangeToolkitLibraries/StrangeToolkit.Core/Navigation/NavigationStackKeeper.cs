namespace StrangeToolkit.Navigation
{
    using StrangeToolkit.Serializer;
    using StrangeToolkit.Storage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class NavigationStackKeeper
    {
        private const string NavigationStackKey = "NavigationStack";

        private const string NavigationStackKnowTypesKey = "NavigationStackKnowTypesKey";

        private static readonly Lazy<NavigationStackKeeper> instance = new Lazy<NavigationStackKeeper>(() => new NavigationStackKeeper(), true);

        public static NavigationStackKeeper Instance
        {
            get
            {
                return instance.Value;
            }
        }

        public async Task SaveHistory(List<NavigationSource> backStack)
        {
            var knownTypes = this.GenerationKnownTypes(backStack);
            var serializer = new XmlSerializer { KnownTypes = knownTypes };

            await this.SaveKnownTypes(knownTypes);
            await StorageProvider.Instance.WriteToFileAsync(NavigationStackKey, backStack, serializer);
        }

        private async Task SaveKnownTypes(List<Type> knownTypes)
        {
            var knowsTypeString = knownTypes.Select(k => k.AssemblyQualifiedName);
            await StorageProvider.Instance.WriteToFileAsync(NavigationStackKnowTypesKey, knowsTypeString);
        }

        private List<Type> GenerationKnownTypes(IEnumerable<NavigationSource> backStack)
        {
            var knownTypes = new List<Type>();
            foreach (var item in backStack)
            {
                this.AddType(knownTypes, item);
                this.AddType(knownTypes, item.AssociatedSource);
                this.AddType(knownTypes, item.Parameters);
            }

            return knownTypes;
        }

        private void AddType(ICollection<Type> knowTypes, object value)
        {
            if (value != null)
            {
                var valueType = value.GetType();
                if (!knowTypes.Contains(valueType))
                {
                    knowTypes.Add(valueType);
                }
            }
        }

        public async Task<List<NavigationSource>> RestoreBackStack()
        {
            var knownTypesString = await StorageProvider.Instance.ReadFromFileAsync<List<string>>(NavigationStackKnowTypesKey);
            var knownTypes = knownTypesString.Select(Type.GetType).ToList();
            var serializer = new XmlSerializer
            {
                KnownTypes = knownTypes
            };

            var backStack = await StorageProvider.Instance.ReadFromFileAsync<List<NavigationSource>>(NavigationStackKey, serializer) ?? new List<NavigationSource>();
            return backStack;
        }

    }
}
