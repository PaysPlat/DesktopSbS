using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Linq;

namespace DesktopSbS.Converters
{
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    [ValueConversion(typeof(double), typeof(double))]
    public class DoubleOperation : MarkupExtension, IMultiValueConverter
    {
        #region IMultiValueConverter Members


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return Binding.DoNothing;

            List<double> operands = values.Select(v => v is double ? v : (v is int ? (double?)(int)v : null) ).OfType<double>().ToList();

            if (operands.Count == 0) return Binding.DoNothing;

            if (operands.Count == 1) return operands[0];

            string op = (parameter as string)?.ToLower();
            switch (op)
            {
                case "add":
                default:
                    return operands.Aggregate((a, b) => a + b);
                case "substract":
                    return operands.Aggregate((a, b) => a - b);
                case "multiply":
                    return operands.Aggregate((a, b) => a * b);
                case "divide":
                    return operands.Aggregate((a, b) => a / b);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        private static IMultiValueConverter converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new DoubleOperation();
            }
            return converter;


        }

    }
}
