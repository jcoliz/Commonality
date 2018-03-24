using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commonality.Test
{
    [TestClass]
    class FileSystemLoggerTest
    {
        private FileSystemLogger Logger;

        [TestInitialize]
        public void SetUp()
        {
            Logger = new FileSystemLogger();
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(Logger);
        }

    }
}