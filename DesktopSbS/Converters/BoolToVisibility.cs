using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace DesktopSbS.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibility : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            Visibility notVisible = (parameter as string) == "1" ? Visibility.Hidden : Visibility.Collapsed;

            return (value as bool?) == true ? Visibility.Visible : notVisible;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return (value as Visibility?) == Visibility.Visible;
        }




        #endregion

        private static BoolToVisibility converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new BoolToVisibility();
            }
            return converter;


        }
    }
}
