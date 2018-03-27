using Commonality.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commonality.Test
{
    [TestClass]
    public class TimeSpanFormatConverterTest
    {
        private TimeSpanFormatConverter Converter;

        [TestInitialize]
        public void SetUp()
        {
            Converter = new TimeSpanFormatConverter();
        }
        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(Converter);
        }
        [TestMethod]
        public void DefaultFormat()
        {
            var ts = TimeSpan.FromMilliseconds(101023902932);

            var actual = Converter.Convert(ts, typeof(string), null);

            var expected = ts.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NullInput()
        {
            var actual = Converter.Convert(null, typeof(string), null);

            var expected = TimeSpanFormatConverter.Empty;

            Assert.AreEqual(expected, actual);
        }
        
        [TestMethod]
        public void SpecificFormat()
        {
            var ts = TimeSpan.FromHours(1.33);

            var format = "hh:mm";

            var actual = Converter.Convert(ts, typeof(string), format);

            var expected = ts.ToString("hh\\:mm");

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
            var ts = TimeSpan.FromHours(1.33);
            Converter.Convert(ts, typeof(int), null);
        }
    }
}
