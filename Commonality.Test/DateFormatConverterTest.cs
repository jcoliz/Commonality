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
        [TestMethod]
        public void DefaultFormat()
        {
            var dt = new DateTime(2018, 03, 23, 17, 50, 35);

            var actual = Converter.Convert(dt, typeof(string), null);

            var expected = dt.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NullInput()
        {
            var actual = Converter.Convert(null, typeof(string), null);

            var expected = DateFormatConverter.Empty;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SpecificFormat()
        {
            var dt = new DateTime(2018, 03, 23, 17, 50, 35);

            var format = "t";

            var actual = Converter.Convert(dt, typeof(string), format);

            var expected = dt.ToString(format);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBack()
        {
            Converter.ConvertBack(null, typeof(string), null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void NotSupportedException()
        {
            var dt = new DateTime(2018, 03, 23, 17, 50, 35);
            Converter.Convert(dt, typeof(int), null);
        }

    }
}
