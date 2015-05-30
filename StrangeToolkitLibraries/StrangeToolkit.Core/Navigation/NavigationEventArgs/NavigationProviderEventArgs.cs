namespace StrangeToolkit.Navigation.NavigationEventArgs
{
    using System;
    using Windows.UI.Xaml.Navigation;

    public class NavigationProviderEventArgs : EventArgs
    {
        public NavigationProviderEventArgs(NavigationSource fromSource, NavigationSource toSource, NavigationMode mode)
        {
            this.FromSource = fromSource;
            this.ToSource = toSource;
            this.Mode = mode;
        }

        public NavigationSource FromSource { get; private set; }

        public NavigationSource ToSource { get; private set; }

        public NavigationMode Mode {get; private set;}
    }
}
