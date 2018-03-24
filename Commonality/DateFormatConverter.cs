using System;

namespace Commonality.Converters
{
    /// <summary>
    /// Value converter to apply a custom timespan format to
    /// convert a timespan to a string
    /// </summary>
    public class DateFormatConverter : IBaseValueConverter
    {
        /// <summary>
        /// Convert value to a string using a standard format
        /// </summary>
        /// <param name="value">Value to be converted, expecting a DateTime</param>
        /// <param name="targetType">Type to convert into, expects string</param>
        /// <param name="parameter">Optional format parameter</param>
        /// <returns>Formatted string</returns>
        public object Convert(object value, Type targetType, object parameter)
        {
            if (targetType != typeof(string))
            {
                throw new NotSupportedException($"DateFormatConverter converts only to string, not {targetType.Name}");
            }

            String format = null;
            if (parameter != null && (String)parameter != null)
                format = (String)parameter;

            DateTime? input = value as DateTime?;

            String result = "---";
            if (input.HasValue)
            {
                if (format != null)
                    result = input.Value.ToString(format);
                else
                    result = input.Value.ToString();
            }

            return result;
        }

        /// <summary>
        /// Convert back not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter)
        {
            throw new NotImplementedException();
        }
    }

}
