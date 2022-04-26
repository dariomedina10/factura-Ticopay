using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TicoPay.Core
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            string description = string.Empty;
            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    description = attributes[0].Description;
                }
                else
                {
                    DisplayAttribute[] displayAttributes = (DisplayAttribute[])field.GetCustomAttributes(typeof(DisplayAttribute), false);
                    if (displayAttributes.Length > 0)
                    {
                        description = displayAttributes[0].Description;
                    }
                    else
                    {
                        System.Xml.Serialization.XmlEnumAttribute[] xmlEnumAttributes = (System.Xml.Serialization.XmlEnumAttribute[])field.GetCustomAttributes(typeof(System.Xml.Serialization.XmlEnumAttribute), false);
                        if (xmlEnumAttributes.Length > 0)
                        {
                            description = xmlEnumAttributes[0].Name;
                        }
                        else
                        {
                            description = value.ToString();
                        }
                    }
                }
            }
            return description;
        }
    }
}