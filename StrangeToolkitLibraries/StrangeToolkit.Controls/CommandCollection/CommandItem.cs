namespace StrangeToolkit.Controls.CommandCollection
{
    using System;
    using System.Windows.Input;
    using Windows.UI.Xaml;
    using StrangeToolkit.Models;

    public class CommandItem : FrameworkElement, ICommand
    {
        private WeakEventHandler<object> canExecuteWeakEventHandler;

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandItem), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandItem), new PropertyMetadata(null));

        public CommandItem()
        {
            this.Command = null;
            this.CommandParameter = null;
            this.canExecuteWeakEventHandler = new WeakEventHandler<object>(OnCommandCanExecuteChanged);
        }

        private void OnCommandCanExecuteChanged(object sender, object e)
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var commadItem = (CommandItem)d;
            commadItem.OnCommandChanged(e);
        }

        private void OnCommandChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldCommand = (ICommand)e.OldValue;

            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= this.canExecuteWeakEventHandler.Handler;
            }

            var newCommand = (ICommand)e.NewValue;

            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += this.canExecuteWeakEventHandler.Handler;
            }
        }


        public bool CanExecute(object parameter)
        {
            var canExecute = false;

            var command = this.Command;
            if (command != null)
            {
                var commandParameter = this.SelectCommandParameter(parameter);
                canExecute = command.CanExecute(commandParameter);
            }

            return canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var command = this.Command;
            if (command != null)
            {
                var commandParameter = this.SelectCommandParameter(parameter);
                if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }
        }

        private object SelectCommandParameter(object inputParameter)
        {
            var parameter = inputParameter;

            if (this.CommandParameter != null)
            {
                parameter = this.CommandParameter;
            }

            return parameter;
        }
    }
}
