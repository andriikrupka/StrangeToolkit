namespace StrangeToolkit.Controls.ControlsExtensions
{
    using StrangeToolkit.Utils;
    using System.Windows.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public static class ItemClickListener
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(ItemClickListener),
            new PropertyMetadata(null, OnCommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(ItemClickListener),
            new PropertyMetadata(null));


        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as ListViewBase;
            if (listView.IsNotNull())
            {
                if (e.OldValue.IsNotNull())
                {
                    listView.ItemClick -= OnItemClick;
                }

                if (e.NewValue.IsNotNull())
                {
                    listView.ItemClick += OnItemClick;
                }
            }
        }

        private static void OnItemClick(object sender, ItemClickEventArgs e)
        {
            var dependencyObject = (DependencyObject)sender;

            var command = GetCommand(dependencyObject);
            if (command.IsNotNull())
            {
                var commandParameter = GetCommandParameter(dependencyObject);

                if (commandParameter.IsNull())
                {
                    commandParameter = e.ClickedItem;
                }

                if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }
        }
    }
}
