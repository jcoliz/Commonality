﻿using System;

namespace Commonality.Converters
{
    /// <summary>
    /// Value converter to apply a custom timespan format to convert a timespan to a string
    /// </summary>
    public class TimeSpanFormatConverter : IBaseValueConverter
    {
        public static readonly string Empty = "---";

        /// <summary>
        /// Convert value to a string using a standard format
        /// </summary>
        /// <param name="value">Value to be converted, expecting a TimeSPan</param>
        /// <param name="targetType">Type to convert into, expects string</param>
        /// <param name="parameter">Optional format parameter</param>
        /// <returns>Formatted string</returns>

        public object Convert(object value, Type targetType, object parameter)
        {
            if (targetType != typeof(string))
            {
                throw new NotSupportedException($"TimeSpanFormatConverter converts only to string, not {targetType.Name}");
            }

            String format = null;
            if (parameter != null && (String)parameter != null)
                format = ((String)parameter).Replace(":", "\\:");

            TimeSpan? ts = value as TimeSpan?;

            String result = Empty;
            if (ts.HasValue)
            {
                if (format != null)
                    result = ts.Value.ToString(format);
                else
                    result = ts.Value.ToString();
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
