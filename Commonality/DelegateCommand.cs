using System;
using System.Windows.Input;

namespace Commonality
{
    /// <summary>
    /// Quick and reusable ICommand
    /// </summary>
    /// <seealso>
    /// http://www.wpftutorial.net/delegatecommand.html
    /// </seealso>
    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// How to determine whether we can execute
        /// </summary>
        private Func<object, bool> canExecute;

        /// <summary>
        /// What is the action to take
        /// </summary>
        private Action<object> executeAction;

        /// <summary>
        /// Raised when the 'canExecute' value changes
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="executeAction">Action to be taken when executed</param>
        public DelegateCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="executeAction">Action to be taken when executed</param>
        /// <param name="canExecute">Function to be called when deciding whether we can execute</param>
        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            if (executeAction == null)
                throw new ArgumentNullException(nameof(executeAction));

            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Determines whether we can execute
        /// </summary>
        /// <param name="parameter">Untyped parameter passed through to canexecute function sent in constructor</param>
        /// <returns>True if we can execute</returns>
        public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

        /// <summary>
        /// Declare to the framework that our ability to execute has changed. 
        /// </summary>
        /// <remarks>
        /// Only caller knows when this is happening
        /// </remarks>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <param name="parameter">Untyped parameter passed straight through to the execute action sent in constructor</param>
        public void Execute(object parameter) => executeAction?.Invoke(parameter);
    }
}
