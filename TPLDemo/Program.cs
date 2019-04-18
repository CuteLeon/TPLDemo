using System;
using TPLDemo.Demo;

namespace TPLDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string input;
            while ((input = ReadDemoId()) != "exit")
            {
                Helper.PrintLine(input);

                var runable = DemoFactory.GetRunableDemo(input); ;
                runable?.Run();
            }
        }

        /// <summary>
        /// 获取用户输入DemoID
        /// </summary>
        public static string ReadDemoId()
        {
            Helper.PrintSplit();
            Helper.PrintLine("<<< TPL >>>");
            Helper.PrintSplit();
            Helper.PrintLine($"输入 Demo ID 以执行：\n\t{string.Join("\n\t", DemoFactory.GetDemoIDs())}\n请输入：");
            return Console.ReadLine();
        }
    }
}
