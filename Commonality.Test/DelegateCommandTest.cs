using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commonality.Test
{
    [TestClass]
    public class DelegateCommandTest
    {
        private DelegateCommand Command;

        [TestInitialize]
        public void SetUp()
        {
            Command = new DelegateCommand(Action);
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(Command);
        }

        private void Action(object parameter)
        {

        }
    }
}
