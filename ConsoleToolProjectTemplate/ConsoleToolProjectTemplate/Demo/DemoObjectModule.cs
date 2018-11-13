using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.My.CommonUtil;
using System.Xml.Serialization;

namespace ConsoleToolProjectTemplate
{
    public class ConfigDemo
    {
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
    }

    public class CsvDemo
    {
        [Column("Title")]
        public string Title { get; set; }

        [Column("Test Column")]
        public string TestColumn { get; set; }
    }

    public class DBDemo
    {
        [Column("Title")]
        public string Title { get; set; }

        [Column("Test Column")]
        public string TestColumn { get; set; }
    }

    [XmlRoot("XmlDemoRoot")]
    public class XmlDemo
    {
        [XmlElement("ID")]
        public string ID { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("XmlValueDemo")]
        public List<XmlValueDemo> Values { get; set; }
    }

    public class XmlValueDemo
    {
        [XmlAttribute("Value")]
        public string Value;
    }
}
