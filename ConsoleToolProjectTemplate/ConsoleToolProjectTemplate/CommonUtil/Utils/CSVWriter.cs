using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.My.CommonUtil
{
    public class CSVWriter<T> : CSVBase, IDisposable
    {
        private StreamWriter mWriter = null;
        private int mTimes = 0;

        public CSVWriter(string path, bool needReplaceCSV)
        {
            if (needReplaceCSV && File.Exists(path))
            {
                File.Delete(path);
            }
            bool isNewCreate = !File.Exists(path);
            mWriter = new StreamWriter(path, true, Encoding.UTF8);
            if (isNewCreate || needReplaceCSV)
            {
                InitCSVFile();
            }
        }

        private void InitCSVFile()
        {
            Type type = typeof(T);
            StringBuilder builder = new StringBuilder();
            foreach (PropertyInfo property in type.GetProperties())
            {
                ColumnAttribute attr = (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();
                if (attr == null)
                {
                    continue;
                }
                builder.Append(string.Format("\"{0}\",", attr.Name));
            }
            WriteLine(builder.ToString().Trim(','));
        }

        public void WriteLine(T t)
        {
            Type type = typeof(T);
            StringBuilder builder = new StringBuilder();
            foreach (PropertyInfo property in type.GetProperties())
            {
                object value = property.GetValue(t, new object[] { });
                if (value != null)
                {
                    string tempValue = value.ToString();
                    builder.Append(string.Format("\"{0}\",", tempValue));
                }
                else
                {
                    builder.Append("\"\",");
                }
            }
            WriteLine(builder.ToString().Trim(','));
        }

        private void WriteLine(string line)
        {
            mWriter.WriteLine(line);
            mTimes++;

            if (mTimes >= 1000)
            {
                mWriter.Flush();
                mTimes = 0;
            }
        }

        public void Dispose()
        {
            if (mWriter != null)
            {
                mWriter.Dispose();
            }
        }
    }
}
