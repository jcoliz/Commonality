﻿using System;

namespace Commonality.Converters
{
    /// <summary>
    /// General purpose "bool to {something}" converter 
    /// </summary>
    /// <remarks>
    /// Instead of just converting from bool, it converts from ANYTHING, by comparing it with the
    /// default value for that type.
    /// </remarks>
    /// <example>
    /// public class DefaultToVisibilityConverter : DefaultConverter
    /// {
    ///     public override object Convert(object value, Type targetType, object parameter)
    ///         => base.Convert<Visibility>(Visibility.Collapsed, Visibility.Visible, value, targetType, parameter);
    /// }
    /// </example>
    public abstract class DefaultConverter<T> : IBaseValueConverter
    {
        private T YesValue;
        private T NoValue;

        protected DefaultConverter(T yes, T no)
        {
            YesValue = yes;
            NoValue = no;
        }

        public object Convert(object value, Type targetType, object parameter)
        {
            if (typeof(T) != targetType)
            {
                throw new ArgumentException("Expected type: " + typeof(T).Name, nameof(targetType));
            }

            bool invert = (parameter?.GetType() == typeof(string) && ((parameter as string) == "invert" || (parameter as string) == "false"));

            bool isdefault;

            var t = value?.GetType();

            if (t == typeof(bool))
            {
                isdefault = (bool)value == false;
            }
            else if (t == typeof(int) || t == typeof(double))
            {
                isdefault = (System.Convert.ToDouble(value) == 0.0);
            }
            else if (t == typeof(DateTime))
            {
                isdefault = (DateTime)value == DateTime.MinValue;
            }
            else if (t == typeof(string))
            {
                isdefault = (value as string).Length == 0;
            }
            else
            {
                isdefault = value == null;
            }

            return isdefault ^ invert ? YesValue : NoValue;
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
