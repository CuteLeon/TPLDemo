using System;
using System.Threading;

namespace TPLDemo
{
    public static class Helper
    {
        public static void PrintLine(string message)
            => Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} tid={Thread.CurrentThread.ManagedThreadId} : {message}");

        public static void PrintSplit()
            => Console.WriteLine("——————————————");
    }
}
