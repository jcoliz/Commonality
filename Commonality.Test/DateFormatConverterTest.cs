using Commonality.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commonality.Test
{
    [TestClass]
    public class DateFormatConverterTest
    {
        private DateFormatConverter Converter;

        [TestInitialize]
        public void SetUp()
        {
            Converter = new DateFormatConverter();
        }
        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(Converter);
        }
    }
}
