namespace StrangeToolkit.Navigation
{
    using System;

    public class NavigationMapItem
    {
        public NavigationMapItem(Type type, object source)
        {
            this.Type = type;
            this.Source = source;
        }

        public Type Type { get; set; }

        public object Source { get; set; }
    }
}
