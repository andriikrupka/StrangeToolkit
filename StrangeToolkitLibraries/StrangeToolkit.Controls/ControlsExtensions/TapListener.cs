namespace StrangeToolkit.Controls.ControlsExtensions
{
    using StrangeToolkit.Utils;
    using System.Windows.Input;
    using Windows.UI.Xaml;

    public static class TapListener
    {
        public static readonly DependencyProperty TapCommandProperty =
             DependencyProperty.RegisterAttached(
             "TapCommand",
             typeof(ICommand),
             typeof(TapListener),
             new PropertyMetadata(null, OnPropertyChanged));

        public static readonly DependencyProperty TapParameterProperty =
            DependencyProperty.RegisterAttached(
            "TapParameter",
            typeof(object),
            typeof(TapListener),
            new PropertyMetadata(null));

        public static readonly DependencyProperty IsHandleTapProperty =
            DependencyProperty.RegisterAttached(
            "IsHandleTap",
            typeof(bool),
            typeof(TapListener),
            new PropertyMetadata(false, OnPropertyChanged));

        public static ICommand GetTapCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(TapCommandProperty);
        }

        public static void SetTapCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(TapCommandProperty, value);
        }

        public static bool GetIsHandleTap(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHandleTapProperty);
        }

        public static void SetIsHandleTap(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHandleTapProperty, value);
        }

        public static object GetTapParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(TapParameterProperty);
        }

        public static void SetTapParameter(DependencyObject obj, object value)
        {
            obj.SetValue(TapParameterProperty, value);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if (e.OldValue.IsNotNull())
            {
                uiElement.Tapped -= OnTapped;
            }

            if (e.NewValue.IsNotNull())
            {
                uiElement.Tapped += OnTapped;
            }
        }



        private static void OnTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var dependencyObjec = (DependencyObject)sender;
            var needHandleTap = GetIsHandleTap(dependencyObjec);

            if (needHandleTap)
            {
                e.Handled = true;
            }

            var tapCommand = GetTapCommand(dependencyObjec);

            if (tapCommand.IsNotNull())
            {
                var tapParameter = GetTapParameter(dependencyObjec);

                if (tapCommand.CanExecute(tapParameter))
                {
                    tapCommand.Execute(tapParameter);
                }
            }
        }
    }
}
