using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using StrangeToolkit.Synchonization;

namespace StrangeToolkit.MessageBox
{
    public class MessageBoxManager
    {
        private static readonly TimeSpan WaitTime = TimeSpan.FromSeconds(0.5);

        private static readonly Queue<MessageBoxItem> MessageBoxItemsQueue = new Queue<MessageBoxItem>();

        private static readonly AsyncLock MessageLocker = new AsyncLock();

        #region Implementation of Singletone

        private static readonly Lazy<MessageBoxManager> _instanceLazy = new Lazy<MessageBoxManager>(() => new MessageBoxManager(), true);

        public static MessageBoxManager Instance
        {
            get { return _instanceLazy.Value; }
        }

        private MessageBoxManager()
        {

        }

        #endregion

        public Task<IUICommand> ShowAsync(MessageDialog messageDialog)
        {
            var messageBoxItem = new MessageBoxItem(messageDialog);
            MessageBoxItemsQueue.Enqueue(messageBoxItem);
            DisplayMessageBox();
            return messageBoxItem.DialogCompletionSource.Task;
        }

        private static async void DisplayMessageBox()
        {
            while (MessageBoxItemsQueue.Any())
            {
                using (await MessageLocker.LockAsync())
                {
                    if (MessageBoxItemsQueue.Any())
                    {
                        var messageItemToDisplay = MessageBoxItemsQueue.Peek();
                        try
                        {
                            var dialogResult = await messageItemToDisplay.Dialog.ShowAsync();
                            messageItemToDisplay.DialogCompletionSource.TrySetResult(dialogResult);
                        }
                        catch (Exception ex)
                        {
                            messageItemToDisplay.DialogCompletionSource.TrySetException(ex);
                        }

                        MessageBoxItemsQueue.Dequeue();
                        await Task.Delay(WaitTime);
                        DisplayMessageBox();
                    }
                }
            }
        }
    }
}
