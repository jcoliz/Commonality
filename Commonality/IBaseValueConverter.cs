using System;

namespace Commonality.Converters
{
    /// <summary>
    /// This interface is implemented by value converters which are intended to be used
    /// across all platforms. On a given platform, we can programatically convert them
    /// to a value converter of the correct platform-specific interface.
    /// </summary>
    /// <example>
    /// public class XamlValueConverter<T> : IValueConverter where T : IBaseValueConverter, new()
    /// {
    ///     T converter = new T();
    /// 
    ///     object IValueConverter.Convert(object value, Type targetType, object parameter, string culture)
    ///         => converter.Convert(value, targetType, parameter);
    /// 
    ///     object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string culture)
    ///         => converter.ConvertBack(value, targetType, parameter);
    /// }
    /// 
    /// public class DateFormatConverterXaml : XamlValueConverter<DateFormatConverter> {}
    /// </example>
public interface IBaseValueConverter
    {
        object Convert(object value, Type targetType, object parameter);

        object ConvertBack(object value, Type targetType, object parameter);
    }
}
