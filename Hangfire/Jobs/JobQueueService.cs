using System;
using System.Threading;

namespace Hangfire.Jobs
{
    public static class JobQueueService
    {
        // 1 min delay

        [Queue("rotinas")]
        public static void RotinasJob()
        {
            Thread.Sleep(60000);
            Console.WriteLine("Rotinas Job");            
        }

        [Queue("valores")]
        public static void ValoresJob()
        {
            Thread.Sleep(60000);
            Console.WriteLine("Valores Job");
        }

        [Queue("itens")]
        public static void ItensJob()
        {
            Thread.Sleep(60000);
            Console.WriteLine("Item Job");
        }

        public static void DefaultJob()
        {
            Thread.Sleep(60000);
            Console.WriteLine("Default Job");
        }
    }
}