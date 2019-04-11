using System;

namespace TPLDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IRunableDemo<RunModel> runable = new ParallelDemo();
            runable.Run();

            Console.Read();
        }
    }
}
