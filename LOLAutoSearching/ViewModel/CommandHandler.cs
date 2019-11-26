using System;
using System.Windows.Input;

namespace LOLAutoSearching
{ 
    #region DelegateCommand Class
    public class CommandHandler : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Action execute;

        public CommandHandler(Action execute) : this(execute, null)
        {
        }

        public CommandHandler(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object o)
        {
            if (this.canExecute == null)
            {
                return true;
            }
            return this.canExecute();
        }

        public void Execute(object o)
        {
            this.execute();
        }

        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    #endregion
}