using System;
using System.Linq;
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
                        int.TryParse(input, out int index) ? DemoFactory.GetRunableDemo(index) : DemoFactory.GetRunableDemo(input);

                    if (runable == null)
                    {
                        Helper.PrintLine($"id= {input} 返回的 Demo 为空。");
                    }
                    else
                    {
                        runable.Run();
                    }
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
            var demoKeys = DemoFactory.GetDemoIDs();
            var demoDescriptions = DemoFactory.GetDemoDescriptions();
            int maxKeyLength = demoKeys.Max(key => key.Length);
            Helper.PrintLine($"输入 Demo 的 数字序号或英文编码 以执行：\n\t{string.Join("\n\t", Enumerable.Range(0, demoKeys.Length).Select(index => $"{index.ToString().PadLeft(2)} · {demoKeys[index].PadRight(maxKeyLength)} —> {demoDescriptions[index]}"))}\n\t或输出 exit 以退出。\n  请输入：");
            return Console.ReadLine();
        }
    }
}
