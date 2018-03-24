using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Commonality;

namespace Commonality.Test
{
    [TestClass]
    public class ServiceLocatorTest
    {
        [TestInitialize]
        public void SetUp()
        {
            Service.Clear();
        }

        [TestMethod]
        public void SetGet()
        {
            var expected = new Clock();
            Service.Set<IClock>(expected);
            var actual = Service.Get<IClock>();

            Assert.AreSame(expected, actual);
        }
        [TestMethod]
        public void SetTryGet()
        {
            var expected = new Clock();
            Service.Set<IClock>(expected);
            var actual = Service.TryGet<IClock>();

            Assert.AreSame(expected, actual);
        }
        [TestMethod]
        public void NoTryGet()
        {
            var actual = Service.TryGet<IClock>();

            Assert.IsNull(actual);
        }
        [TestMethod]
        public void NoTryGetAnother()
        {
            Service.Set<ILogger>(new FileSystemLogger());
            var actual = Service.TryGet<IClock>();

            Assert.IsNull(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(PlatformNotSupportedException))]
        public void NoGet()
        {
            Service.Get<IClock>();
        }
        [TestMethod]
        [ExpectedException(typeof(PlatformNotSupportedException))]
        public void NoGetAnother()
        {
            Service.Set<ILogger>(new FileSystemLogger());
            Service.Get<IClock>();
        }
    }
}
