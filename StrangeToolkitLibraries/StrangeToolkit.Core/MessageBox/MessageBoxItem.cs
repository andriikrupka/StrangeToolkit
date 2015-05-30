using System.Threading.Tasks;
using Windows.UI.Popups;

namespace StrangeToolkit.MessageBox
{
    internal class MessageBoxItem
    {
        public MessageBoxItem(MessageDialog dialog)
        {
            Dialog = dialog;
            DialogCompletionSource = new TaskCompletionSource<IUICommand>();
        }

        public MessageDialog Dialog { get; private set; }

        public TaskCompletionSource<IUICommand> DialogCompletionSource { get; private set; }
    }
}
