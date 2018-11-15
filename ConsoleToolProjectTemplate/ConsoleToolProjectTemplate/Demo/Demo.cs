using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.My.CommonUtil;

namespace ConsoleToolProjectTemplate
{
    /// <summary>
    /// 
    /// </summary>
    class Demo
    {
        public void GetConfigInfo_Demo()
        {
            ConfigDemo demo = ConfigurationUtil.GetConfiguration<ConfigDemo>("Config\\config.xml");
        }

        public void ReadCsv_Demo()
        {
            using (CSVReader reader = new CSVReader("C:\\test.csv"))
            {
                while (!reader.EndOfCSVFile)
                {
                    CsvDemo demo = reader.ReadLine<CsvDemo>();
                }
            }
        }

        public void WriteCsv_Demo()
        {
            using (CSVWriter<CsvDemo> writer = new CSVWriter<CsvDemo>("C:\\test.csv", true))
            {
                CsvDemo demo = new CsvDemo();
                demo.Title = "Demo";
                demo.TestColumn = "Test";
                writer.WriteLine(demo);
            }
        }

        public void Query_Demo()
        {
            using (QueryService query = QueryService.CreateQueryService("Connection string"))
            {
                query.ClearParameters();
                query.SetCommandParameter("@Demo", "DemoValue");
                DBDemo demo = query.GetSingleResult<DBDemo>("select * from Demo where Title=@Demo");
            }
        }

        public void Query_Demo_2()
        {
            Test test = new Test();
            QueryService query = QueryService.CreateQueryService("Connection string");
            List<Test> tList = query.GetResults<Test>(test, test.Select<Test>(t => t.ID, t => t.Name, t => t.Age),
                test.Where<Test>(
                    t => t.WhereOr<Test>(
                        w => w.ID == 1,
                        w => w.ID == 2
                    )
                ));
        }

        public void XML_Demo()
        {
            XmlDemo demo = new XmlDemo();
            demo.ID = "ID";
            demo.Name = "ABC";

            XmlValueDemo value1 = new XmlValueDemo();
            value1.Value = "TestValue_1";
            XmlValueDemo value2 = new XmlValueDemo();
            value2.Value = "TestValue_2";

            demo.Values = new List<XmlValueDemo>();
            demo.Values.Add(value1);
            demo.Values.Add(value2);

            //Convert object to xml string.
            string xml = XmlConvertor.ConvertToXml(demo);

            //Convert xml string to object.
            XmlDemo demoObj = XmlConvertor.ConvertToObject<XmlDemo>(xml);
        }

        //public void Client_Demo()
        //{
        //    var context0 = ClientContextFactory.CreateContext();
        //    var context1 = ClientContextFactory.CreateContext("[WebUrl]");
        //    var context2 = ClientContextFactory.CreateContext("[WebUrl]", "[User]", "[Password]");
        //    var context3 = ClientContextFactory.CreateContext("[WebUrl]", "[User]", "[Password]", SPMode.Local);
        //}
    }

    [Column("Test")]
    public class Test : ModelBase
    {
        [Column("a")]
        public int ID { get; set; }
        [Column("b")]
        public string Name { get; set; }
        [Column("c")]
        public string Age { get; set; }
    }
}
