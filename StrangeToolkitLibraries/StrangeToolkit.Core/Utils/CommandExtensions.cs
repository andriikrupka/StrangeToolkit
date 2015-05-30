using System.Windows.Input;

namespace StrangeToolkit.Utils
{
    public static class CommandExtensions
    {
        public static bool SafeExecute(this ICommand command, object parameter)
        {
            var result = false;
            var commandToExecute = command;
            if (commandToExecute != null)
            {
                if (commandToExecute.CanExecute(parameter))
                {
                    result = true;
                    commandToExecute.Execute(parameter);
                }
            }

            return result;
        }
    }
}
