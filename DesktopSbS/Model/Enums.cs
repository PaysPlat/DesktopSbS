using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSbS.Model
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ShortcutCommands
    {

        [Description("Show about DesktopSbS")]
        About = 1,

        [Description("Decrease parallax efect")]
        DecreaseParallax = 2,

        [Description("Increase parallax efect")]
        IncreaseParallax = 3,

        [Description("Shutdown DesktopSbS")]
        ShutDown = 4,

        [Description("Side-by-Side / Top-Bottom")]
        SwitchSbSMode = 5,

        [Description("Pause 3D mode")]
        Pause3D = 6,

        [Description("Hide cursor in 3D mode")]
        HideDestCursor = 7,

        [Description("Keep windows ratio")]
        KeepRatio = 8
    }

    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    FieldInfo fi = value.GetType().GetField(value.ToString());
                    if (fi != null)
                    {
                        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        return ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
                    }
                }

                return string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
