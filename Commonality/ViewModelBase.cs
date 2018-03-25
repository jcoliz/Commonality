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
        /// Event raised when we have an exception.
        /// </summary>
        public event EventHandler<Exception> ExceptionRaised;

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
        /// <remarks>
        /// DEPRECATED. Use SetError(Exception), and include the location code in ex.Source
        /// </remarks>
        /// <param name="code"></param>
        /// <param name="ex"></param>
        protected void SetError(string code, Exception ex)
        {
            try
            {
                ex.Source = code;
                SetError(ex);
            }
            catch (Exception)
            {
                // This can fail, and at this point we're in a really hard state to effectively report
                // what just happened, so we are going to (reluctantly) swallow this.
            }
        }

        /// <summary>
        /// Raise the ExceptionRaised event
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ex"></param>
        protected void SetError(Exception ex)
        {
            try
            {
                if (Context != null)
                    Context.Post(o => ExceptionRaised?.Invoke(this, ex), null);
                else
                    ExceptionRaised?.Invoke(this, ex);

                Service.TryGet<ILogger>()?.LogError(ex);
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
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property as string));
            }
            catch
            {
                // This can fail, and at this point we're in a really hard state to effectively report
                // what just happened, so we are going to (reluctantly) swallow this.
            }
        }
    }
}
