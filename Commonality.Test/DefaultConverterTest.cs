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
            Converter = new DefaultToBoolConverter();
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(Converter);
        }
    }

    class DefaultToBoolConverter : DefaultConverter
    {
        public override object Convert(object value, Type targetType, object parameter)
        {
            return base.Convert<bool>(true,false,value,targetType,parameter);
        }
    }
}
