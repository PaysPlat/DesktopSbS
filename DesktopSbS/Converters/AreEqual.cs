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

namespace DesktopSbS.Converters
{

    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    public class AreEqual : MarkupExtension, IMultiValueConverter
    {
        #region IValueConverter Members


        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (values == null) return Binding.DoNothing;

            bool isEqual = true;

            for (int i = 1; i < values.Length; ++i)
            {
                isEqual &= values[i - 1]?.Equals(values[i]) == true;
                if (!isEqual) break;
            }
            return isEqual;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion

        private static IMultiValueConverter converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new AreEqual();
            }
            return converter;


        }
    }
}
