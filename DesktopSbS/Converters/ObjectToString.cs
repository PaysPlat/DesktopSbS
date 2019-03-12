using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data;
using System.Windows.Media;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using System.Globalization;

namespace DesktopSbS.Converters
{

    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(object), typeof(string))]
    public class ObjectToString : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        private static IValueConverter converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new ObjectToString();
            }
            return converter;


        }

    }
}
