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
            using (QueryService query = QueryService.GetQueryService("[connectionString]"))
            {
                query.ClearParameters();
                query.SetCommandParameter("@Demo", "DemoValue");
                DBDemo demo = query.GetResult<DBDemo>("select * from Demo where Title=@Demo");
            }
        }

        //public void Client_Demo()
        //{
        //    var context0 = ClientContextFactory.CreateContext();
        //    var context1 = ClientContextFactory.CreateContext("[WebUrl]");
        //    var context2 = ClientContextFactory.CreateContext("[WebUrl]", "[User]", "[Password]");
        //    var context3 = ClientContextFactory.CreateContext("[WebUrl]", "[User]", "[Password]", SPMode.Local);
        //}
    }
}
