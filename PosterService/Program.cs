using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PosterService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new Service1() 
            };
            ServiceBase.Run(ServicesToRun);


//            var service = new Service1();
//#if DEBUG
//            service.Start(args);

//            Console.WriteLine("Service started, press any key to kill");
//            Console.ReadKey();

//            service.Stop();
//#else
//                        ServiceBase.Run(new ServiceBase[] { service });
//#endif



        }
    }
}
