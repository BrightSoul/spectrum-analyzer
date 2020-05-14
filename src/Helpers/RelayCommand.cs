using System;
using System.Windows.Input;

namespace SpectrumAnalyzer.Helpers
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
            this.execute((TParameter)parameter);
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
