using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Topshelf;

namespace TopShelfTest
{
    public class TownCrier
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(TownCrier));
        readonly Timer _timer;
        public TownCrier()
        {
            _timer = new Timer(1000*5) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) =>
            {
                Console.WriteLine("It is {0} and all is well", DateTime.Now);
                logger.DebugFormat("log time {0:HHmmss}", DateTime.Now);
            };
        }
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }
    }

    public class Program
    {
        public static void Main()
        {
            var log4netConfig = new FileInfo(Path.Combine( AppDomain.CurrentDomain.BaseDirectory , "log4net.config"));
            log4net.Config.XmlConfigurator.ConfigureAndWatch(log4netConfig);
            var rc = HostFactory.Run(x =>                                   //1
            {
                x.Service<TownCrier>(s =>                                   //2
                {
                    s.ConstructUsing(name => new TownCrier());                //3
                    s.WhenStarted(tc => tc.Start());                         //4
                    s.WhenStopped(tc => tc.Stop());                          //5
                });
                x.RunAsLocalSystem();                                       //6

                x.SetDescription("Sample Topshelf Host");                   //7
                x.SetDisplayName("Stuff");                                  //8
                x.SetServiceName("Stuff");                                  //9
            });                                                             //10

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());  //11
            Environment.ExitCode = exitCode;
        }
    }
}
