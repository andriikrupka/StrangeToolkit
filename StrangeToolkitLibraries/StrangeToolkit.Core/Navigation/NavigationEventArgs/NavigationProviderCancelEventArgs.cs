namespace StrangeToolkit.Navigation.NavigationEventArgs
{
    using System;
    using Windows.UI.Xaml.Navigation;

    public class NavigationProviderCancelEventArgs : EventArgs
    {
        public NavigationProviderCancelEventArgs(NavigationMode mode, NavigationSource fromSource, NavigationSource toSource)
        {
            this.FromSource = fromSource;
            this.ToSource = toSource;
            this.NavigationMode = mode;
        }

        public NavigationSource FromSource { get; private set; }

        public NavigationSource ToSource { get; private set; }
       
        public bool IsCancel { get; set; }

        public NavigationMode NavigationMode { get; private set; }
    }
}
