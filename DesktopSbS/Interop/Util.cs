using DesktopSbS.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace DesktopSbS.Interop
{
    public static class Util
    {
        public static readonly CultureInfo
            CultFr = new CultureInfo("fr-FR"),
            CultEn = new CultureInfo("en-US");

        public static readonly string ExePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\";



        public static T GetVisualParent<T>(IInputElement pInput, bool strictType = true) where T : class
        {
            DependencyObject depO = pInput as DependencyObject;
            while (depO != null && !(strictType ? depO.GetType() == typeof(T) : depO is T))
            {
                depO = VisualTreeHelper.GetParent(depO);
            }
            return depO as T;

        }

        public static T GetVisualChild<T>(IInputElement pInput, int pIndex, bool strictType = true) where T : class
        {
            DependencyObject depO = pInput as DependencyObject;
            DependencyObject depParent = null;
            while (depO != null && !(strictType ? depO.GetType() == typeof(T) : depO is T))
            {
                depParent = depO;
                depO = VisualTreeHelper.GetChild(depO, 0);
            }
            if (depO != null)
            {
                depO = VisualTreeHelper.GetChild(depParent, pIndex);
            }
            return depO as T;
        }

        public static IEnumerable<T> GetVisualChildrenRecurse<T>(IInputElement pInput, bool strictType = true) where T : class
        {
            DependencyObject depO = pInput as DependencyObject;
            IInputElement child;
            int count = VisualTreeHelper.GetChildrenCount(depO);

            for (int i = 0; i < count; ++i)
            {
                child = VisualTreeHelper.GetChild(depO, i) as IInputElement;

                if (child != null)
                {
                    if ((strictType ? child.GetType() == typeof(T) : child is T))
                    {
                        yield return child as T;
                    }
                    else
                    {
                        foreach (T item in GetVisualChildrenRecurse<T>(child, strictType))
                        {
                            yield return item;
                        }
                    }
                }

            }


        }

        public static List<T> GetVisualChildren<T>(IInputElement pInput, bool strictType = true) where T : class
        {
            DependencyObject depO = pInput as DependencyObject;
            DependencyObject depParent = null;
            while (depO != null && !(strictType ? depO.GetType() == typeof(T) : depO is T))
            {
                depParent = depO;
                depO = VisualTreeHelper.GetChild(depO, 0);
            }
            if (depO != null)
            {
                if (depParent == null) return new List<T> { depO as T };

                int count = VisualTreeHelper.GetChildrenCount(depParent);
                List<T> res = new List<T>(count);
                T item;
                for (int i = 0; i < count; ++i)
                {
                    item = VisualTreeHelper.GetChild(depParent, i) as T;
                    if (item != null)
                    {
                        res.Add(item);
                    }
                }
                return res;
            }
            else
            {
                return null;
            }

        }


        public static int GetVisualChildCount<T>(IInputElement pInput, bool strictType = true) where T : class
        {
            DependencyObject depO = pInput as DependencyObject;
            DependencyObject depParent = null;
            while (depO != null && !(strictType ? depO.GetType() == typeof(T) : depO is T))
            {
                depParent = depO;
                depO = VisualTreeHelper.GetChild(depO, 0);
            }
            int count = 0;
            if (depO != null)
            {
                count = VisualTreeHelper.GetChildrenCount(depParent);
            }

            return count;
        }

        private static Typeface typeFace = new Typeface(
        (FontFamily)TextBlock.FontFamilyProperty.DefaultMetadata.DefaultValue,
        (FontStyle)TextBlock.FontStyleProperty.DefaultMetadata.DefaultValue,
        (FontWeight)TextBlock.FontWeightProperty.DefaultMetadata.DefaultValue,
        (FontStretch)TextBlock.FontStretchProperty.DefaultMetadata.DefaultValue
        );

        public static Size GetTextBlockSize(TextBlock tb)
        {
            FormattedText ft = new FormattedText(
                                tb.Text,
                                CultEn,
                                FlowDirection.LeftToRight,
                                typeFace,
                                tb.FontSize,
                                tb.Foreground);
            return new Size(ft.WidthIncludingTrailingWhitespace, ft.Height);
        }

        public static void WriteConsoleTemplate(Control ctrl)
        {
            StringBuilder stringBuilder = new StringBuilder();

            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlSettings))
            {
                XamlWriter.Save(ctrl.Template, xmlWriter);
            }

            Console.WriteLine(stringBuilder.ToString());
        }

        public static Version GetWindowsVersion()
        {
            var os = Environment.OSVersion;
            return os.Version;
        }

        public static string ReadSettings()
        {
            StringBuilder sbSettings = new StringBuilder();
            string latestVersion = Settings.Default.LatestVersion;
            foreach (SettingsPropertyValue settingsPropertyValue in Settings.Default.PropertyValues)
            {
                object value = settingsPropertyValue.PropertyValue;
                if (value is StringCollection stringCollection)
                {
                    StringBuilder sbCollec = new StringBuilder();
                    sbCollec.Append("[");
                    bool first = true;
                    foreach (string str in stringCollection)
                    {
                        if (!first)
                        {
                            sbCollec.Append("|");
                        }
                        sbCollec.Append($" {str} ");
                        first = false;
                    }
                    sbCollec.Append("]");
                    value = sbCollec.ToString();
                }

                sbSettings.AppendLine($"{settingsPropertyValue.Name}: {value}");
            }
            return sbSettings.ToString();
        }
    }
}
