using Commonality.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commonality.Test
{
    [TestClass]
    public class DefaultConverterTest
    {
        private DefaultConverter Converter;

        [TestInitialize]
        public void SetUp()
        {
            Converter = new DefaultToStringConverter();
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(Converter);
        }

        [TestMethod]
        public void Bool()
        {
            var expected = DefaultToStringConverter.True;
            var actual = Converter.Convert(true, typeof(string), null);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void BoolInvert()
        {
            var expected = DefaultToStringConverter.False;
            var actual = Converter.Convert(true, typeof(string), "invert");

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void BoolInvertAsFalse()
        {
            var expected = DefaultToStringConverter.False;
            var actual = Converter.Convert(true, typeof(string), "false");

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void Int()
        {
            var expected = DefaultToStringConverter.True;
            var actual = Converter.Convert(5, typeof(string), null);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void String()
        {
            var expected = DefaultToStringConverter.False;
            var actual = Converter.Convert(string.Empty, typeof(string), null);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void StringNull()
        {
            var expected = DefaultToStringConverter.False;
            var actual = Converter.Convert((string)null, typeof(string), null);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void DateTime()
        {
            var expected = DefaultToStringConverter.False;
            var actual = Converter.Convert(System.DateTime.MinValue, typeof(string), null);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void Object()
        {
            var expected = DefaultToStringConverter.True;
            var actual = Converter.Convert(this, typeof(string), null);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void Null()
        {
            var expected = DefaultToStringConverter.False;
            var actual = Converter.Convert(null, typeof(string), null);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WrongType()
        {
            Converter.Convert(string.Empty, typeof(int), null);
        }
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBack()
        {
            Converter.ConvertBack(null, typeof(string), null);
        }
    }

    class DefaultToStringConverter : DefaultConverter
    {
        public static string True = "True";
        public static string False = "False";

        public override object Convert(object value, Type targetType, object parameter)
        {

            return base.Convert<string>(False,True,value,targetType,parameter);
        }
    }
}
