namespace StrangeToolkit.Navigation.NavigationEventArgs
{
    using System;

    public class NavigationProviderRemovedEventArgs : EventArgs
    {
        public NavigationProviderRemovedEventArgs(NavigationSource removedSource)
        {
            this.RemovedSource = removedSource;
        }

        public NavigationSource RemovedSource { get; set; }
    }
}
