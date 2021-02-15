using System;
using System.Threading;

namespace Hangfire.Jobs
{
    // Todos os jobs da classe irão pra essa fila, a não ser que especifique uma fila diferente no próprio método
    [Queue("rotinas")]
    public static class RotinasJobQueueService
    {
        public static void RotinasJob()
        {
            Thread.Sleep(60000);
            Console.WriteLine("Rotinas Job");
        }
    }
}
