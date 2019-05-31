using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.My.CommonUtil;

namespace ConsoleToolProjectTemplate
{
    class Program
    {
        private static Logger log = Logger.CreateLogger(typeof(Program));

        static void Main(string[] args)
        {
            string commandPath = string.Empty;
            if (args.Length == 1)
            {
                commandPath = args[0];
            }
            else
            {
                commandPath = "Config\\config.xml";
            }
            log.Info("Start to execute Main, Command Path: {0}", commandPath);

            try
            {
                Config config = ConfigurationUtil.GetConfiguration<Config>(commandPath);
                //Your Code.
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while executing Main. Exception: {0}", ex);
            }
            log.Info("Finished.");
            log.Debug("********************The End********************");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
