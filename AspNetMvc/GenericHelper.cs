using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Demo
{
    public class GenericHelper<T>
    {
        public static void BuildGenericObject<T>(ref T objectItem, IDataReader reader) where T : new()
        {
            T temp = default(T);

            var item = new T();
            Type objectType = item.GetType();
            PropertyInfo[] properties = objectType.GetProperties();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                foreach (var property in properties)
                {
                    if (reader[reader.GetName(i)] != DBNull.Value && property.Name == reader.GetName(i))
                    {
                        item.GetType().GetProperty(reader.GetName(i)).SetValue(item, reader[reader.GetName(i)], null);
                    }
                }
            }

            objectItem = item;
        }

        public static void BuildGenericList<T>(ref List<T> list, IDataReader reader) where T : new()
        {
            var item = new T();
            Type objectType = item.GetType();
            PropertyInfo[] properties = objectType.GetProperties();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                foreach (var property in properties)
                {
                    if (reader[reader.GetName(i)] != DBNull.Value && property.Name == reader.GetName(i))
                    {
                        item.GetType().GetProperty(reader.GetName(i)).SetValue(item, reader[reader.GetName(i)], null);
                    }
                }
            }

            list.Add(item);
        }
    }
}