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

        private SqlConnection mConnection = null;
        private SqlCommand mCommand = null;
        private string mConnectionStr;

        private static object obj = new object();

        private SqlCommand Command
        {
            get
            {
                if (mConnection == null || mCommand == null || mConnection.State == ConnectionState.Closed || mConnection.State == ConnectionState.Broken)
                {
                    InitConnection();
                }
                return mCommand;
            }
        }

        private QueryService(string connection)
        {
            this.mConnectionStr = connection;
            InitConnection();
        }

        private void InitConnection()
        {
            try
            {
                if (mConnection != null)
                {
                    mConnection.Dispose();
                }
                if (mCommand != null)
                {
                    mCommand.Dispose();
                }

                mConnection = new SqlConnection(this.mConnectionStr);
                mConnection.Open();
                mCommand = new SqlCommand();
                mCommand.Connection = mConnection;
            }
            catch
            {
                throw;
            }
        }

        public static void InitQueryService(string connection)
        {
            GetQueryService(connection);
        }

        public static QueryService GetQueryService(string connection)
        {
            if (mQuery == null)
            {
                lock (obj)
                {
                    if (mQuery == null)
                    {
                        mQuery = new QueryService(connection);
                    }
                }
            }
            return mQuery;
        }

        public static QueryService GetQueryService()
        {
            return mQuery;
        }

        public List<T> GetResults<T>(string command) where T : new()
        {
            List<T> list = new List<T>();
            lock (obj)
            {
                SqlDataReader reader = null;
                try
                {
                    Command.CommandText = command;
                    reader = Command.ExecuteReader();
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

        public T GetResult<T>(string command) where T : new()
        {
            T temp = new T();
            lock (obj)
            {
                SqlDataReader reader = null;
                try
                {
                    Command.CommandText = command;
                    reader = Command.ExecuteReader();
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
            lock (obj)
            {
                SqlDataReader reader = null;
                try
                {
                    Command.CommandText = command;
                    reader = Command.ExecuteReader();
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
            Command.Parameters.AddWithValue(key, value);
        }

        public void ClearParameters()
        {
            Command.Parameters.Clear();
        }

        public void Dispose()
        {
            if (mCommand != null)
            {
                mCommand.Dispose();
            }
            if (mConnection != null)
            {
                mConnection.Dispose();
            }
        }
    }
}
