﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Microsoft.SharePoint.Client;
using System.Security;

namespace System.My.CommonUtil
{
    public class ClientContextFactory : IDisposable
    {
        private ClientContext mContext = null;
        private object lockObj = new object();
        private string cacheWebUrl = string.Empty;

        public ClientContext CreateContext()
        {
            return CreateContext(cacheWebUrl);
        }

        public ClientContext CreateContext(string webUrl)
        {
            if (mContext == null)
            {
                lock (lockObj)
                {
                    if (mContext == null)
                    {
                        cacheWebUrl = webUrl;
                        mContext = new ClientContext(webUrl);
                    }
                }
            }
            return mContext;
        }

        public ClientContext CreateContext(string webUrl, string user, string password)
        {
            return CreateContext(webUrl, user, password, SPMode.Online);
        }

        public ClientContext CreateContext(string webUrl, string user, string password, SPMode mode)
        {
            CreateContext(webUrl);
            SetClientContext(user, password, mode);
            return mContext;
        }

        public ClientContext ChangeContext(string url, string user, string password)
        {
            return ChangeContext(url, user, password, SPMode.Online);
        }

        public ClientContext ChangeContext(string url, string user, string password, SPMode mode)
        {
            if (mContext != null && !mContext.Web.Url.Trim('/').Equals(url.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                mContext.Dispose();
                mContext = null;
            }
            return CreateContext(url, user, password, mode);
        }

        public void SetClientContext(string user, string password)
        {
            SetClientContext(user, password, SPMode.Online);
        }

        public void SetClientContext(string user, string password, SPMode mode)
        {
            if (mContext == null)
            {
                return;
            }

            switch (mode)
            {
                case SPMode.Online:
                    SecureString securePassword = new SecureString();
                    foreach (char c in password)
                    {
                        securePassword.AppendChar(c);
                    }
                    mContext.Credentials = new SharePointOnlineCredentials(user, securePassword);
                    break;
                case SPMode.Local:
                    mContext.Credentials = new NetworkCredential(user, password);
                    break;
            }
        }

        public void Dispose()
        {
            if (mContext != null)
            {
                mContext.Dispose();
                mContext = null;
            }
        }
    }

    public enum SPMode
    {
        Online,
        Local
    }
}
