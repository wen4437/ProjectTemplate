using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.My.CommonUtil
{
    public class CSVReader : CSVBase, IDisposable
    {
        private StreamReader mReader = null;
        private Dictionary<string, string> mValueMapping = new Dictionary<string, string>();
        private CSVType mCSVType = CSVType.Full;

        public CSVReader(string filePath)
            : this(filePath, CSVType.Full)
        {

        }

        public CSVReader(string filePath, CSVType csvType)
        {
            mCSVType = csvType;
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file \"{0}\" was not found.", filePath);
            }
            mReader = new StreamReader(filePath, Encoding.UTF8);

            InitValueMapping();
        }

        private void InitValueMapping()
        {
            string line = mReader.ReadLine();
            string[] columns;
            if (mCSVType == CSVType.Full)
            {
                columns = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
            }
            else
            {
                columns = line.Split(',');
            }
            foreach (string key in columns)
            {
                mValueMapping[key.Trim('\"')] = "";
            }
        }

        public T ReadLine<T>() where T : new()
        {
            string line = mReader.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                return default(T);
            }

            string[] values;
            if (mCSVType == CSVType.Full)
            {
                values = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
            }
            else
            {
                values = line.Split(',');
            }

            for (int i = 0; i < values.Length; i++)
            {
                mValueMapping[mValueMapping.Keys.ToList()[i]] = values[i].Trim('\"');
            }

            T t = new T();
            Type type = t.GetType();
            foreach (PropertyInfo property in type.GetProperties())
            {
                ColumnAttribute attr = (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();
                if (attr != null)
                {
                    string value = string.Empty;
                    if (mValueMapping.TryGetValue(attr.Name, out value))
                    {
                        if (value.Contains("&AVE#;"))
                        {
                            value = value.Replace("&AVE#;", ",");
                        }
                        property.SetValue(t, value, new object[] { });
                    }
                }
            }
            return t;
        }

        public bool EndOfCSVFile
        {
            get
            {
                return mReader.EndOfStream;
            }
        }

        public void Dispose()
        {
            if (mReader != null)
            {
                mReader.Dispose();
            }
        }
    }
}
