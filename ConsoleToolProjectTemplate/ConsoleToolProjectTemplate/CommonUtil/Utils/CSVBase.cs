using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.My.CommonUtil
{
    public class CSVBase
    {
        public CSVType CSVType { get; set; }
    }

    public enum CSVType
    {
        Simple,
        Full
    }
}
