using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using TicoPay.Core;

namespace TicoPay.Application.Helpers
{

    public class EnumHelper
    {
        public static string GetXmlEnumAttributeValueFromEnum(Type enumType,Enum value) 
        {           
            if (!enumType.IsEnum) return null;//or string.Empty, or throw exception

            var member = enumType.GetMember(value.ToString()).FirstOrDefault();
            if (member == null) return null;//or string.Empty, or throw exception

            var attribute = member.GetCustomAttributes(false).OfType<XmlEnumAttribute>().FirstOrDefault();
            if (attribute == null) return null;//or string.Empty, or throw exception
            return attribute.Name;
        }

        public static string GetDescription(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.GetDescription();
        }

        public static IList GetDescriptions(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(string.Format("El tipo {0}, debe ser enumerativo.", enumType.Name));
            }
            ArrayList list = new ArrayList();
            Array enumValues = Enum.GetValues(enumType);
            foreach (Enum value in enumValues)
            {
                list.Add(GetDescription(value));
            }
            return list;
        }

        public static int Parse(Type enumType, string value)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("type");
            }
            IList list = EnumHelper.ToList(enumType);
            Enum key = null;
            foreach (KeyValuePair<Enum, string> item in list)
            {
                string descripcions = EnumHelper.GetDescription(item.Key);
                if (string.Equals(descripcions, value, StringComparison.InvariantCultureIgnoreCase) || 
                    string.Equals(value, item.Key.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    key = item.Key;
                    break;
                }
            }
            int tempValue = 0;
            if (enumType.IsEnum && int.TryParse(value, out tempValue))
            {
                return (int)Enum.Parse(enumType, value);
            }
            if (key == null)
            {
                return -1;
            }
            if (enumType.IsEnum)
            {
                return (int)Enum.Parse(enumType, key.ToString());
            }
            throw new ArgumentException(string.Format("El tipo {0}, debe ser enumerativo.", enumType.Name));
        }

        public static IList<SelectListItem> GetSelectList(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(string.Format("El tipo {0}, debe ser enumerativo.", enumType.Name));
            }
            List<SelectListItem> list = new List<SelectListItem>();
            Array enumValues = Enum.GetValues(enumType);
            foreach (Enum value in enumValues)
            {
                list.Add(new SelectListItem { Value = value.ToString(), Text = value.GetDescription() });
            }
            return list;
        }

        public static IList<SelectListItem> GetSelectListValues(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(string.Format("El tipo {0}, debe ser enumerativo.", enumType.Name));
            }
            List<SelectListItem> list = new List<SelectListItem>();
            Array enumValues = Enum.GetValues(enumType);
            int x = 0;
            foreach (Enum value in enumValues)
            {
                list.Add(new SelectListItem { Value = x.ToString(), Text = value.GetDescription() });
                x++;
            }
            return list;
        }


        public static IList<SelectListItem> GetSelectList(params Enum[] enums)
        {
            if (enums == null)
            {
                throw new ArgumentNullException("enums");
            }
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Enum value in enums)
            {
                list.Add(new SelectListItem { Value = value.ToString(), Text = value.GetDescription() });
            }
            return list;
        }

        private static IList ToList(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(string.Format("El tipo {0}, debe ser enumerativo.", enumType.Name));
            }
            ArrayList list = new ArrayList();
            Array enumValues = Enum.GetValues(enumType);
            foreach (Enum value in enumValues)
            {
                list.Add(new KeyValuePair<Enum, string>(value, GetDescription(value)));
            }
            return list;
        }

        
    }
}