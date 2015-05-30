namespace StrangeToolkit.Controls.ControlsExtensions
{
    using StrangeToolkit.Navigation;
    using System.Windows.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    public static class NavigationListener
    {
        public static readonly DependencyProperty NavigateToPageCommandProperty =
            DependencyProperty.RegisterAttached(
            "NavigateToPageCommand",
            typeof(ICommand),
            typeof(NavigationListener),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ExecuteModeProperty =
            DependencyProperty.RegisterAttached(
            "ExecuteMode",
            typeof(ExecuteNavigationMode),
            typeof(NavigationListener),
            new PropertyMetadata(ExecuteNavigationMode.All));

        static NavigationListener()
        {
            NavigationProvider.Instance.Navigated -= OnApplicationNavigated;
            NavigationProvider.Instance.Navigated += OnApplicationNavigated;
        }

        public static ICommand GetNavigateToPageCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(NavigateToPageCommandProperty);
        }

        public static void SetNavigateToPageCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(NavigateToPageCommandProperty, value);
        }

        public static ExecuteNavigationMode GetExecuteMode(DependencyObject obj)
        {
            return (ExecuteNavigationMode)obj.GetValue(ExecuteModeProperty);
        }

        public static void SetExecuteMode(DependencyObject obj, ExecuteNavigationMode value)
        {
            obj.SetValue(ExecuteModeProperty, value);
        }

        private static void OnApplicationNavigated(object sender, Navigation.NavigationEventArgs.NavigationProviderEventArgs e)
        {
            var page = e.ToSource.Page as Page;
            var command = GetNavigateToPageCommand(page);
            var isNeedExecute = IsNeedExecute(e.Mode, page);
            if (command != null && isNeedExecute)
            {
                if (command.CanExecute(e))
                {
                    command.Execute(e);
                }
            }
        }

        private static bool IsNeedExecute(NavigationMode navigationMode, Page page)
        {
            var isNeed = false;
            var executeMode = GetExecuteMode(page);
            if (executeMode == ExecuteNavigationMode.New && (navigationMode == NavigationMode.New || navigationMode == NavigationMode.Forward))
            {
                isNeed = true;
            }
            else if (executeMode == ExecuteNavigationMode.Back && (navigationMode == NavigationMode.Back || navigationMode == NavigationMode.Refresh))
            {
                isNeed = true;
            }
            else if (executeMode == ExecuteNavigationMode.All)
            {
                isNeed = true;
            }

            return isNeed;
        }
    }

    public enum ExecuteNavigationMode
    {
        All,

        New,

        Back
    }
}
