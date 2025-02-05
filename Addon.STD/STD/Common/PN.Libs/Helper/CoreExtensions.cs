using PN.SmartLib.CustomAttribute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace PN.SmartLib.Helper
{
    public static class CoreExtensions
    {
        public static object GetEnumValue<T>(int code)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enum type.");
            }

            if (Enum.IsDefined(typeof(T), code))
            {
                return Enum.ToObject(typeof(T), code);
            }

            return null; // Return null if the code does not match any enum value
        }

        /// <summary>
        /// Get Description for enum
        /// </summary>
        /// <param name="enumdata">value of enum</param>
        /// <returns>desciption of enum value</returns>
        public static string GetDescription(this Enum enumdata)
        {
            return (enumdata.GetType().GetTypeInfo().GetMember(enumdata.ToString())
                .FirstOrDefault(x => x.MemberType == MemberTypes.Field).GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)
                .SingleOrDefault() as DescriptionAttribute)?.Description ?? enumdata.ToString();
        }

        /// <summary>
        /// Get enum from discripiton
        /// </summary>
        /// <typeparam name="T">Type of enum to convert</typeparam>
        /// <param name="description">The Description of enum</param>
        /// <returns>The enum value</returns>
        public static T GetEnumValueByDescription<T>(this string description) where T : Enum
        {
            foreach (Enum enumItem in Enum.GetValues(typeof(T)))
            {
                if (enumItem.GetDescription() == description)
                {
                    return (T)enumItem;
                }
            }
            throw new ArgumentException("Not found.", nameof(description));
        }

      
        public static TEnum GetEnumValueByAPIResponseCode<TEnum>(this string description)
        where TEnum : struct
        {
            Type enumType = typeof(TEnum);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"{nameof(TEnum)} must be an enumeration type.");
            }

            foreach (var field in enumType.GetFields())
            {
                if (field.GetCustomAttribute<APIResponseAttribute>() is APIResponseAttribute attribute)
                {
                    if (attribute.Code == description)
                    {
                        return (TEnum)field.GetValue(null);
                    }
                }
            }

            throw new ArgumentException($"No {nameof(TEnum)} value found with the specified response code.");
        }

        /// <summary>
        /// Read data from datarow to hash table
        /// </summary>
        /// <param name="raw">data row form reader</param>
        /// <returns>hash table with data</returns>
        public static Hashtable ToHashtable(this DataRowView raw)
        {
            var hashtable = new Hashtable();
            foreach (DataColumn col in raw.Row.Table.Columns)
            {
                hashtable[col.ColumnName] = ToString(raw[col.ColumnName]);
            }

            return hashtable;
        }

        /// <summary>
        /// /Read datas from data view to hash tables
        /// </summary>
        /// <param name="dataView">data view form reader</param>
        /// <returns>list hash table with data</returns>
        public static Hashtable[] ToHashtableArray(this DataView dataView)
        {
            return (from DataRowView row in dataView
                    select row.ToHashtable()).ToArray();
        }

        /// <summary>
        /// To string for object
        /// return null or empty object
        /// </summary>
        /// <param name="value">string of object</param>
        /// <returns></returns>
        public static string ToString(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            return value.ToString();
        }

       
        public static string CustomTableName(Type t, ref string message)
        {
            var attribute = (CustomTable)Attribute.GetCustomAttribute(t, typeof(CustomTable));
            if (attribute == null)
            {
                message = string.Format("This is not GT Table object");
            }
            return attribute?.Name;
        }

        public static Dictionary<string, string> CustomFields(Type t)
        {
            Dictionary<string, string> _dict = new Dictionary<string, string>();

            PropertyInfo[] props = t.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                var gtFieldProp = prop.GetCustomAttributes<CustomField>().FirstOrDefault();
                if (gtFieldProp != null)
                {
                    _dict.Add(gtFieldProp.Name, gtFieldProp.DataType);
                }
            }

            return _dict;
        }

        public static string APIResponsMessage(this Enum enumdata)
        {
            return (enumdata.GetType().GetTypeInfo().GetMember(enumdata.ToString())
                .FirstOrDefault(x => x.MemberType == MemberTypes.Field).GetCustomAttributes(typeof(APIResponseAttribute), inherit: false)
                .SingleOrDefault() as APIResponseAttribute)?.Description ?? enumdata.ToString();
        }        
    }
}
