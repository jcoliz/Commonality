using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Commonality.Test
{
    [TestClass]
    public class ViewModelBaseTest
    {
        private ViewModel Model;

        [TestInitialize]
        public void SetUp()
        {
            Model = new ViewModel();
            Service.Clear();
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(Model);
        }

        [TestMethod]
        public void PropertyChanged()
        {
            string actual = string.Empty;

            Model.PropertyChanged += (s, e) =>
            {
                actual = e.PropertyName;
            };

            Model.TestString = "NewValue";

            Assert.AreEqual(nameof(ViewModel.TestString), actual);
        }
        [TestMethod]
        public void PropertyChangedViaContext()
        {
            Model = new ViewModel(new FakeContext());
            PropertyChanged();
        }
        [TestMethod]
        public void PropertyChangedThrowsWhoCares()
        {
            Model.PropertyChanged += (s, e) =>
            {
                throw new PlatformNotSupportedException();
            };

            Model.TestString = "NewValue";
        }
        [TestMethod]
        public void PropertyChangedWhoCares()
        {
            Model.TestString = "NewValue";
        }
        [TestMethod]
        public void ExceptionRaised()
        {
            Exception actual_exception = null;

            Model.ExceptionRaised += (s, e) =>
            {
                actual_exception = e;
            };

            Exception expected_exception = new NotImplementedException("Booo!") { Source = "code" };

            Model.Throw(expected_exception);

            Assert.AreSame(expected_exception, actual_exception);
        }
        [TestMethod]
        public void ExceptionRaisedViaContext()
        {
            Model = new ViewModel(new FakeContext());
            ExceptionRaised();
        }
        [TestMethod]
        public void ExceptionRaisedIsLogged()
        {
            var logger = new Logger();
            Service.Set<ILogger>(logger);

            Exception expected_exception = new NotImplementedException("Booo!") { Source = "code" };

            Model.Throw(expected_exception);

            Assert.AreSame(expected_exception, logger.actual_exception);
        }
        [TestMethod]
        public void ExceptionRaisedThrowsWhoCares()
        {
            Model.ExceptionRaised += (s, e) =>
            {
                throw new PlatformNotSupportedException();
            };

            Exception expected_exception = new NotImplementedException("Booo!") { Source = "code" };

            Model.Throw(expected_exception);
        }
        [TestMethod]
        public void ExceptionRaisedWhoCares()
        {
            Exception expected_exception = new NotImplementedException("Booo!") { Source = "code" };

            Model.Throw(expected_exception);
        }
        [TestMethod]
        public void ExceptionRaisedWhoCaresViaContext()
        {
            Model = new ViewModel(new FakeContext());
            ExceptionRaisedWhoCares();
        }
        [TestMethod]
        public void MessageReady()
        {
            string actual = string.Empty;

            Model.MessageReady += (s, e) =>
            {
                actual = e;
            };
            var expected = "Hello!";

            Model.Message = expected;

            Assert.AreSame(expected, actual);
        }
        [TestMethod]
        public void MessageReadyViaContext()
        {
            Model = new ViewModel(new FakeContext());
            MessageReady();
        }
        [TestMethod]
        public void MessageReadyThrowsWhoCares()
        {
            Model.MessageReady += (s, e) =>
            {
                throw new PlatformNotSupportedException();
            };
            var expected = "Hello!";

            Model.Message = expected;
        }
        [TestMethod]
        public void MessageReadyWhoCares()
        {
            var expected = "Hello!";
            Model.Message = expected;
        }
        [TestMethod]
        public void MessageReadyWhoCaresViaContext()
        {
            Model = new ViewModel(new FakeContext());
            MessageReadyWhoCares();            
        }
    }

    class ViewModel: ViewModelBase
    {
        public ViewModel()
        {

        }

        public ViewModel(SynchronizationContext context)
        {
            Context = context;
        }

        public string TestString
        {
            set
            {
                SetProperty(nameof(TestString));
            }
        }

        public string Message
        {
            set
            {
                SetMessage(value);
            }
        }

        public void Throw(Exception ex)
        {
            SetError(ex);
        }
    }

    class Logger : ILogger
    {
        public string actual_code = null;
        public Exception actual_exception = null;

        public void LogError(Exception ex)
        {
            actual_exception = ex;
        }

        public Task LogErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task LogInfoAsync(string message)
        {
            throw new NotImplementedException();
        }

        void ILogger.Error(string key, Exception ex)
        {
            actual_code = key;
            actual_exception = ex;
        }

        void ILogger.LogEvent(string message, params string[] parameters)
        {
            throw new NotImplementedException();
        }

        Task ILogger.LogEventAsync(string message, params string[] parameters)
        {
            throw new NotImplementedException();
        }

        void ILogger.LogInfo(string message)
        {
            throw new NotImplementedException();
        }

        Task ILogger.StartSession()
        {
            throw new NotImplementedException();
        }
    }

    class FakeContext: SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state)
        {
            d.Invoke(state);
        }
    }
}
