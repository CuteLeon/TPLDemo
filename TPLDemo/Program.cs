using System;

namespace TPLDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IRunableDemo<RunModel> runable = new AtomicDemo();
            runable.Run();

            Console.Read();
        }
    }
}
