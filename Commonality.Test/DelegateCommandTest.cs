using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commonality.Test
{
    [TestClass]
    public class DelegateCommandTest
    {
        private DelegateCommand SimpleCommand;
        private DelegateCommand ComplexCommand;

        private object ActionParameter;
        private bool CanExecuteResult;

        private void Action(object parameter)
        {
            ActionParameter = parameter;
        }

        private bool CanExecuteFunc(object arg)
        {
            return CanExecuteResult;
        }

        [TestInitialize]
        public void SetUp()
        {
            SimpleCommand = new DelegateCommand(Action);
            ComplexCommand = new DelegateCommand(Action,CanExecuteFunc);
            ActionParameter = null;
            CanExecuteResult = false;
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(SimpleCommand);
        }

        [TestMethod]
        public void Execute()
        {
            var expected = "Hello";
            SimpleCommand.Execute(expected);

            Assert.AreSame(expected, ActionParameter);
        }
        [TestMethod]
        public void CanExecute()
        {
            var actual = SimpleCommand.CanExecute(null);

            Assert.AreEqual(true, actual);
        }
        [TestMethod]
        public void CanExecuteComplex()
        {
            CanExecuteResult = true;
            var actual = ComplexCommand.CanExecute(null);

            Assert.AreEqual(true, actual);
        }
        [TestMethod]
        public void CanExecuteChanged()
        {
            int actual = 0;
            SimpleCommand.CanExecuteChanged += (s, e) =>
            {
                ++actual;
            };
            SimpleCommand.RaiseCanExecuteChanged();

            Assert.AreEqual(1, actual);
        }
        [TestMethod]
        public void CanExecuteChangedNoEvent()
        {
            SimpleCommand.RaiseCanExecuteChanged();

            // Assert that nothing blew up :)
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullCommand()
        {
            var NullCommand = new DelegateCommand(null);
        }
    }
}
