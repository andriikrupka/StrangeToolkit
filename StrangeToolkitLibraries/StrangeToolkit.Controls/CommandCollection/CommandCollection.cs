namespace StrangeToolkit.Controls.CommandCollection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Windows.Input;

    public class CommandCollection : ObservableCollection<CommandItem>, ICommand
    {
        private List<bool> canExecuteValues = new List<bool>();

        public CommandCollection()
        {
            this.CollectionChanged += this.OnCollectionChanged;
        }

        public event EventHandler CanExecuteChanged;

        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (e.NewItems != null)
                {
                    foreach (CommandItem item in e.NewItems)
                    {
                        item.CanExecuteChanged += this.OnItemCanExecuteChanged;
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    foreach (CommandItem item in e.OldItems)
                    {
                        item.CanExecuteChanged -= this.OnItemCanExecuteChanged;
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (CommandItem item in this)
                {
                    item.CanExecuteChanged += this.OnItemCanExecuteChanged;
                }
            }

            this.RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnItemCanExecuteChanged(object sender, EventArgs e)
        {
            this.RaiseCanExecuteChanged();
        }

        public bool CanExecute(object parameter)
        {
            this.canExecuteValues.Clear();

            var canExecuteCount = 0;
            for (int i = 0; i < this.Count; i++)
            {
                var canExecute = this[i].CanExecute(parameter);

                if (canExecute)
                {
                    canExecuteCount++;
                }

                canExecuteValues.Add(canExecute);
            }

            return canExecuteCount == this.Count;
        }

        public void Execute(object parameter)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this.canExecuteValues[i])
                {
                    this[i].Execute(parameter);
                }
            }
        }
    }
}
