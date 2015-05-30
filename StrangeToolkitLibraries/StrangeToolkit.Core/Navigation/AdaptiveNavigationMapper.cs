namespace StrangeToolkit.Navigation
{
    using System;
    using System.Collections.Generic;

    public class AdaptiveNavigationMapper
    {
        private Dictionary<Type, object> typeToAssociateDictionary = new Dictionary<Type, object>();

        private Dictionary<object, Type> associateToType = new Dictionary<object, Type>();

        public void AddMapping(Type type, object associatedSource)
        {
            this.typeToAssociateDictionary.Add(type, associatedSource);
            this.associateToType.Add(associatedSource, type);
        }

        public Type GetTypeSource(object associatedSource)
        {
            var typeSource = this.associateToType[associatedSource];
            return typeSource;
        }

        public object GetAssociatedSource (Type typeSource)
        {
            var associatedSource = this.typeToAssociateDictionary[typeSource];
            return associatedSource;
        }
    }
}
