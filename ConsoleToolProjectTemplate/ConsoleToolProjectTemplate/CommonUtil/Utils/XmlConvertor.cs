using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace System.My.CommonUtil
{
    public class XmlUtil
    {
        private static Dictionary<Type, XmlSerializer> mSerializers = new Dictionary<Type, XmlSerializer>();
        private static Queue<MemoryStream> mStreams = new Queue<MemoryStream>();
        private static XmlSerializerNamespaces NAMESPACE = new XmlSerializerNamespaces();
        private static XmlWriterSettings SETTINGS = new XmlWriterSettings();
        private const int DEFAULT_CACHE_SIZE = 10;
        private static bool mIsCache = false;

        static XmlUtil()
        {
            NAMESPACE.Add("", "");
            SETTINGS.Indent = false;
            SETTINGS.OmitXmlDeclaration = true;
            SETTINGS.Encoding = new UTF8Encoding(false);
            SETTINGS.CheckCharacters = false;
        }

        public static bool IsCache
        {
            get { return mIsCache; }
            set { mIsCache = value; }
        }

        public static string ConvertToXml(object obj)
        {
            string result = "";
            MemoryStream stream = null;
            try
            {
                XmlSerializer serializer;
                Type type = obj.GetType();
                if (mIsCache)
                {
                    lock (mSerializers)
                    {
                        if (!mSerializers.TryGetValue(type, out serializer))
                        {
                            serializer = new XmlSerializer(type);
                            mSerializers[type] = serializer;
                        }
                    }
                    lock (mStreams)
                    {
                        if (mStreams.Count == 0)
                        {
                            stream = new MemoryStream();
                        }
                        else
                        {
                            stream = mStreams.Dequeue();
                        }
                    }
                }
                else
                {
                    serializer = new XmlSerializer(type);
                    stream = new MemoryStream();
                }
                stream.Position = 0;
                stream.SetLength(0);
                using (XmlWriter writer = XmlWriter.Create(stream, SETTINGS))
                {
                    serializer.Serialize(writer, obj, NAMESPACE);
                }
                result = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (stream != null)
                {
                    if (mIsCache
                        && stream.Capacity < 2048
                        && mStreams.Count < DEFAULT_CACHE_SIZE)
                    {
                        lock (mStreams)
                        {
                            if (mStreams.Count < DEFAULT_CACHE_SIZE)
                                mStreams.Enqueue(stream);
                            else stream.Close();
                        }
                    }
                    else stream.Close();
                }
            }
            return result;
        }

        public static object ConvertToObject(Stream stream, Type type)
        {
            if (stream != null)
            {
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(stream);
            }
            return new object();
        }

        public static T ConvertToObject<T>(Stream stream)
        {
            return (T)ConvertToObject(stream, typeof(T));
        }

        public static object ConvertToObject(string xml, Type type)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }
            var result = default(object);
            MemoryStream stream = null;
            try
            {
                XmlSerializer serializer;
                if (mIsCache)
                {
                    lock (mSerializers)
                    {
                        if (!mSerializers.TryGetValue(type, out serializer))
                        {
                            serializer = new XmlSerializer(type);
                            mSerializers[type] = serializer;
                        }
                    }
                    lock (mStreams)
                    {
                        if (mStreams.Count == 0)
                        {
                            stream = new MemoryStream();
                        }
                        else
                        {
                            stream = mStreams.Dequeue();
                        }
                    }
                }
                else
                {
                    serializer = new XmlSerializer(type);
                    stream = new MemoryStream();
                }
                byte[] buf = Encoding.UTF8.GetBytes(xml);
                stream.Position = 0;
                stream.SetLength(0);
                stream.Write(buf, 0, buf.Length);
                stream.Position = 0;
                result = serializer.Deserialize(stream);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (stream != null)
                {
                    if (mIsCache
                        && stream.Capacity < 2048
                        && mStreams.Count < DEFAULT_CACHE_SIZE)
                    {
                        lock (mStreams)
                        {
                            if (mStreams.Count < DEFAULT_CACHE_SIZE)
                                mStreams.Enqueue(stream);
                            else stream.Close();
                        }
                    }
                    else stream.Close();
                }
            }
            return result;
        }

        public static T ConvertToObject<T>(string xml)
        {
            return (T)ConvertToObject(xml, typeof(T));
        }
    }
}
