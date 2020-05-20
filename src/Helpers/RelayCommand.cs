using System;
using System.Windows.Input;

namespace Spettro.Helpers
{
    public class RelayCommand<TParameter> : ICommand
    {
        private Predicate<TParameter> canExecute;
        private Action<TParameter> execute;

        public RelayCommand(Action<TParameter> execute, Predicate<TParameter> canExecute = null)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null) return true;

            return this.canExecute((TParameter)parameter);
        }

        public void Execute(object parameter)
        {
            if (parameter is TParameter)
            {
                this.execute((TParameter)parameter);
            } else
            {
                this.execute((TParameter) Convert.ChangeType(parameter, typeof(TParameter)));
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, null);
        }
    }

    public class RelayCommand : RelayCommand<object> {
        public RelayCommand(Action execute, Func<bool> canExecute = null) : base(arg => execute(), arg => canExecute != null ? canExecute() : true)
        {

        }
    }
}
