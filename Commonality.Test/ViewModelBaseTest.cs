using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

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

            Model.MessageReady += (s, e) =>
            {

            };
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
        public void ExceptionRaised()
        {
            string actual_code = null;
            Exception actual_exception = null;

            Model.ExceptionRaised += (s, e) =>
            {
                actual_code = e.code;
                actual_exception = e.ex;
            };

            string expected_code = "code";
            Exception expected_exception = new NotImplementedException("Booo!");

            Model.Throw(expected_code, expected_exception);

            Assert.AreEqual(expected_code,actual_code);
            Assert.AreSame(expected_exception, actual_exception);
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
    }

    class ViewModel: ViewModelBase
    {
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

        public void Throw(string code, Exception ex)
        {
            SetError(code, ex);
        }
    }
}
