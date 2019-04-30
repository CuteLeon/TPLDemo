using System;
using System.Threading;

namespace TPLDemo
{
    public static class Helper
    {
        public static void PrintLine(string message)
            => Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} threadId={Thread.CurrentThread.ManagedThreadId} : {message}");

        public static void PrintSplit()
            => Console.WriteLine("——————————————");

        private static readonly Lazy<Random> random = new Lazy<Random>(() => new Random());
        public static Random Random { get => random.Value; }
    }
}
