using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace System.My.CommonUtil
{
    public class ConfigurationUtil
    {
        private string mPath;
        private ConfigurationUtil(string path)
        {
            this.mPath = path;
        }

        public static T GetConfiguration<T>(string path) where T : new()
        {
            T t = new T();
            XmlDocument document = new XmlDocument();
            document.Load(path);

            XmlNodeList nodes = document.DocumentElement.SelectNodes("//param");
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (XmlNode node in nodes)
            {
                string value = node.Attributes["key"].Value;
                PropertyInfo property = properties.FirstOrDefault(s => s.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
                property.SetValue(t, node.Attributes["value"].Value, null);
            }
            return t;
        }

        public T GetConfiguration<T>() where T : new()
        {
            T t = new T();
            XmlDocument document = new XmlDocument();
            document.Load(mPath);

            XmlNodeList nodes = document.DocumentElement.SelectNodes("//param");
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (XmlNode node in nodes)
            {
                string value = node.Attributes["key"].Value;
                PropertyInfo property = properties.FirstOrDefault(s => s.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
                property.SetValue(t, node.Attributes["value"].Value, null);
            }

            return t;
        }
    }

    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public ColumnAttribute()
        { }

        public ColumnAttribute(string name)
        {
            this.Name = name;
        }
    }
}
