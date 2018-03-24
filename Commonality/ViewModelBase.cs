using System;
using System.ComponentModel;
using System.Threading;

namespace Commonality
{
    /// <summary>
    /// Provides some basic common functionality that all ViewModels want, including INotifyPropertyChanged handling, and message/error propagation to the UI
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Encapsulates an exception and a unique code. This can be used
        /// to log the exception and tie back to where it was raised.
        /// </summary>
        public class ExceptionArgs
        {
            public string code;
            public Exception ex;

            public ExceptionArgs(Exception _ex, string _code = null)
            {
                ex = _ex;
                code = _code;
            }
        }
        /// <summary>
        /// Event raised when we have an exception.
        /// </summary>
        public event EventHandler<ExceptionArgs> ExceptionRaised;

        /// <summary>
        /// Event raised when we have an informational message for the user.
        /// </summary>
        /// <remarks>
        /// The string value is fully localized and can be displaeyd to the
        /// user as-is.
        /// </remarks>
        public event EventHandler<string> MessageReady;

        /// <summary>
        /// Event raised when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise the ExceptionRaised event
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ex"></param>
        protected void SetError(string code, Exception ex)
        {
            try
            {
                if (Context != null)
                    Context.Post(o => ExceptionRaised?.Invoke(this, new ExceptionArgs(ex, code)), null);
                else
                    ExceptionRaised?.Invoke(this, new ExceptionArgs(ex, code));

                Service.TryGet<ILogger>()?.Error(code, ex);
            }
            catch (Exception)
            {
                // This can fail, and at this point we're in a really hard state to effectively report
                // what just happened, so we are going to (reluctantly) swallow this.
            }
        }

        /// <summary>
        /// Raise the MessageReady event
        /// </summary>
        protected void SetMessage(string message)
        {
            try
            {
                if (Context != null)
                    Context.Post(o => MessageReady?.Invoke(this, message), null);
                else
                    MessageReady?.Invoke(this, message);
            }
            catch (Exception)
            {
                // This can fail, and at this point we're in a really hard state to effectively report
                // what just happened, so we are going to (reluctantly) swallow this.
            }
        }

        /// <summary>
        /// Raise the PropertyChanged event
        /// </summary>
        /// <param name="property"></param>

        protected void SetProperty(string property)
        {
            if (Context != null)
                Context.Post(DoPropertyChanged, property);
            else
                DoPropertyChanged(property);
        }

        /// <summary>
        /// Current threading context where we should post events.
        /// </summary>
        protected SynchronizationContext Context = SynchronizationContext.Current;

        /// <summary>
        /// Internal method to actually invoke the property changed handler
        /// </summary>
        /// <param name="property"></param>
        private void DoPropertyChanged(object property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property as string));
        }
    }
}
