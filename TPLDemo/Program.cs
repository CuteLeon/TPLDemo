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
                try
                {
                    var runable = string.IsNullOrEmpty(input) ?
                        DemoFactory.GetLastRunableDemo() :
                        DemoFactory.GetRunableDemo(input);

                    runable?.Run();
                }
                catch (Exception ex)
                {
                    Helper.PrintLine($"运行演示遇到异常：{ex.Message}");
                }
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
            Helper.PrintLine($"输入 Demo ID 以执行：\n\t· {string.Join("\n\t· ", DemoFactory.GetDemoIDs())}\n\t或输出 exit 以退出。\n  请输入：");
            return Console.ReadLine();
        }
    }
}
