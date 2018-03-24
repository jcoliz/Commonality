using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commonality.Test
{
    [TestClass]
    public class ViewModelBaseTest
    {
        private ViewModelBase ModelBase;

        [TestInitialize]
        public void SetUp()
        {
            ModelBase = new ViewModelBase();
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(ModelBase);
        }
    }
}
