namespace StrangeToolkit.Navigation
{
    using StrangeToolkit.Navigation.NavigationEventArgs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    public class NavigationProvider
    {
        private Frame frame;

        private bool isNavigatingNow;

        private AdaptiveNavigationMapper adaptiveMapper;

        private List<NavigationSource> backStack = new List<NavigationSource>();

        private NavigationSource currentSource;

        private NavigationSource commingSource;

        private readonly static Lazy<NavigationProvider> instance = new Lazy<NavigationProvider>(() => new NavigationProvider(), true);

        public static NavigationProvider Instance
        {
            get
            {
                return instance.Value;
            }
        }

        public INavigationParameters CurrentParameters { get; private set; }

        public List<NavigationSource> BackStack
        {
            get
            {
                return this.backStack;
            }
        }

        public int BackStackDepth
        {
            get { return frame.BackStackDepth; }
        }

        public event EventHandler<NavigationProviderCancelEventArgs> Navigating;

        public event EventHandler<NavigationProviderEventArgs> Navigated;

        public event EventHandler NavigationFailed;

        public event EventHandler NavigatingStopped;

        public void Initialize(Frame frame, AdaptiveNavigationMapper adaptiveMapper)
        {
            this.frame = frame;
            this.frame.Navigating += this.OnFrameNavigating;
            this.frame.Navigated += this.OnFrameNavigated;
            this.frame.NavigationFailed += this.OnFrameNavigationFailed;
            this.frame.NavigationStopped += this.OnFrameNavigationStopped;
            this.adaptiveMapper = adaptiveMapper;
        }

        public bool IsStackRestoringNow { get; private set; }

        public bool CanGoBack
        {
            get
            {
                return this.frame.CanGoBack;
            }
        }

        public void GoBack()
        {
            if (this.CanGoBack)
            {
                this.frame.GoBack();
            }
        }

        public void Navigate(object navigationSource)
        {
            this.Navigate(navigationSource, null);
        }

        public void Navigate(object navigationSource, INavigationParameters parameters)
        {
            if (!this.isNavigatingNow)
            {
                this.isNavigatingNow = true;

                this.CurrentParameters = parameters;
                var typeSource = adaptiveMapper.GetTypeSource(navigationSource);
                this.commingSource = new NavigationSource
                {
                    Parameters = this.CurrentParameters,
                    AssociatedSource = navigationSource,
                    Page = typeSource
                };

                this.frame.Navigate(typeSource, parameters);
            }
        }

        public void RemoveBackEntry()
        {
            var depth = this.frame.BackStackDepth - 1;
            if (depth < 0)
            {
                depth = 0;
            }

            if (this.frame.BackStack.Count > depth)
            {
                this.frame.BackStack.RemoveAt(depth);
                this.backStack.RemoveAt(this.backStack.Count - 2);
            }
        }

        public void ClearPreviousBackEntry()
        {
            while (this.BackStackDepth > 1)
            {
                this.RemoveBackEntry();
            }
        }

        private void OnFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            var associatedSource = this.adaptiveMapper.GetAssociatedSource(e.SourcePageType);
            if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Forward)
            {
                if (this.commingSource == null)
                {
                    this.commingSource = new NavigationSource { AssociatedSource = associatedSource };
                }

                this.backStack.Add(this.commingSource);
            }
            else
            {
                if (this.backStack.Count > 0)
                {
                    var count = this.backStack.Count;
                    this.backStack.RemoveAt(count - 1);
                    this.commingSource = this.backStack[count - 2];
                }

            }

            var handler = this.Navigating;

            if (handler != null)
            {
                var eventArgs = new NavigationProviderCancelEventArgs(e.NavigationMode, currentSource, commingSource);
                handler.Invoke(this, eventArgs);

                var isCancel = eventArgs.IsCancel;
                e.Cancel = isCancel;
            }
        }

        private void OnFrameNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var previousSource = this.currentSource;
            this.currentSource = this.commingSource;
            this.currentSource.Page = e.Content;
            this.commingSource = null;

            this.CurrentParameters = this.currentSource.Parameters;

            var handler = this.Navigated;
            if (handler != null)
            {
                var navigationProviderEventArgs = new NavigationProviderEventArgs(previousSource, currentSource, e.NavigationMode);
                handler.Invoke(this, navigationProviderEventArgs);
            }

            this.isNavigatingNow = false;
        }

        private void OnFrameNavigationStopped(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
#warning //TODO: check this to rollback navigation Parameters!!!!!
            this.isNavigatingNow = false;
        }

        private void OnFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
#warning //TODO: check this to rollback navigation Parameters!!!!!
            this.isNavigatingNow = false;
        }

        public async Task SaveHistory()
        {
            await NavigationStackKeeper.Instance.SaveHistory(this.backStack);
        }

        public async Task RestoreHistory()
        {
            if (!this.IsStackRestoringNow)
            {
                this.IsStackRestoringNow = true;
                try
                {
                    var restoredBackStack = await NavigationStackKeeper.Instance.RestoreBackStack();
                    if (restoredBackStack.Any())
                    {
                        foreach (var navigationSource in restoredBackStack)
                        {
                            this.Navigate(navigationSource.AssociatedSource, navigationSource.Parameters);
                        }
                    }
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    this.IsStackRestoringNow = false;
                }
            }
        }
    }
}
