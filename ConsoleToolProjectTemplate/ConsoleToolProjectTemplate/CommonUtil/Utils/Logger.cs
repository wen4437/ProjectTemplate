using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using System.IO;

namespace System.My.CommonUtil
{
    public class Logger
    {
        private ILog log = null;
        private static object obj = new object();
        private static Logger mLogger = null;

        public static Logger CreateLogger(Type type)
        {
            if (mLogger == null)
            {
                lock(obj)
                {
                    if (mLogger == null)
                    {
                        mLogger = new Logger(type);
                    }
                }
            }
            return mLogger;
        }

        public static Logger CreateLogger(string name)
        {
            if (mLogger == null)
            {
                lock (obj)
                {
                    if (mLogger == null)
                    {
                        mLogger = new Logger(name);
                    }
                }
            }
            return mLogger;
        }

        private Logger(Type type)
        {
            if (log == null)
            {
                InitLog4Net();
                log = LogManager.GetLogger(type);
            }
        }

        private Logger(string name)
        {
            if (log == null)
            {
                InitLog4Net();
                log = LogManager.GetLogger(name);
            }
        }

        private void InitLog4Net()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Config\\log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }

        /// <summary>
        /// 输出到Console和Log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="format"></param>
        public void Info(string message, params object[] format)
        {
            lock(obj)
            {
                log.Info(string.Format(message, format));
            }
        }

        /// <summary>
        /// 只输出到Log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="format"></param>
        public void Debug(string message, params object[] format)
        {
            lock (obj)
            {
                log.Debug(string.Format(message, format));
            }
        }

        /// <summary>
        /// 输出到Log file和Console黄色输出
        /// </summary>
        /// <param name="message"></param>
        /// <param name="format"></param>
        public void Warn(string message, params object[] format)
        {
            lock (obj)
            {
                log.Warn(string.Format(message, format));
            }
        }

        /// <summary>
        /// 输出到Log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="format"></param>
        public void Error(string message, params object[] format)
        {
            lock (obj)
            {
                log.Error(string.Format(message, format));
            }
        }

        /// <summary>
        /// 输出到Log file和Console红色异常信息输出
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="format"></param>
        public void Error(string message, Exception ex, params object[] format)
        {
            lock (obj)
            {
                log.Error(string.Format(message, format), ex);
            }
        }

        private string GetMessage(string message, params object[] format)
        {
            return format.Length == 0 ? message : string.Format(message, format);
        }
    }
}