using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.My.CommonUtil
{
    public class QueryService : IDisposable
    {
        private static QueryService mQuery = null;
        private static object lockObj = new object();
        private static string mConnString = string.Empty;
        private static SqlConnection mConnection = null;
        private static SqlCommand mCommand = null;

        public static QueryService CreateQueryService(string connection)
        {
            if (mQuery == null)
            {
                lock (lockObj)
                {
                    if (mQuery == null)
                    {
                        mQuery = new QueryService(connection);
                    }
                }
            }
            else
            {
                if (mConnection.State == ConnectionState.Broken || mConnection.State == ConnectionState.Closed)
                {
                    mConnection.Dispose();
                    mConnection = new SqlConnection(mConnString);
                    mConnection.Open();
                    if (mCommand != null)
                    {
                        mCommand.Dispose();
                        mCommand = mConnection.CreateCommand();
                    }
                }
            }
            return mQuery;
        }

        public static QueryService CreateQueryService()
        {
            return CreateQueryService(mConnString);
        }

        public void SetConnectionString(string connection)
        {
            mConnString = connection;
        }

        private QueryService() { }

        private QueryService(string connection)
        {
            mConnString = connection;
            mConnection = new SqlConnection(mConnString);
            mConnection.Open();
        }

        public List<T> GetResults<T>(string command) where T : new()
        {
            List<T> list = new List<T>();
            lock (lockObj)
            {
                SqlDataReader reader = null;
                try
                {
                    mCommand.CommandText = command;
                    reader = mCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        T temp = new T();
                        foreach (PropertyInfo property in typeof(T).GetProperties())
                        {
                            try
                            {
                                ColumnAttribute attribute = (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();
                                if (attribute != null)
                                {
                                    property.SetValue(temp, reader[attribute.Name], null);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                        list.Add(temp);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
            return list;
        }

        public T GetSingleResult<T>(string command) where T : new()
        {
            T temp = new T();
            lock (lockObj)
            {
                SqlDataReader reader = null;
                try
                {
                    mCommand.CommandText = command;
                    reader = mCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        foreach (PropertyInfo property in typeof(T).GetProperties())
                        {
                            try
                            {
                                ColumnAttribute attribute = (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();
                                if (attribute != null)
                                {
                                    property.SetValue(temp, reader[attribute.Name], null);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
            return temp;
        }

        public int GetCount(string command)
        {
            int temp = -1;
            lock (lockObj)
            {
                SqlDataReader reader = null;
                try
                {
                    mCommand.CommandText = command;
                    reader = mCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        temp = reader.IsDBNull(0) ? -1 : reader.GetInt32(0);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
            return temp;
        }

        public void SetCommandParameter(string key, object value)
        {
            mCommand.Parameters.AddWithValue(key, value);
        }

        public void ClearParameters()
        {
            mCommand.Parameters.Clear();
        }

        public void Dispose()
        {
            if (mCommand != null)
            {
                mCommand.Dispose();
                mCommand = null;
            }
            if (mConnection != null)
            {
                mConnection = null;
                mConnection.Dispose();
            }
            mQuery = null;
        }
    }
}
