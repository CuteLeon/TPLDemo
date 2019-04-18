using System;
using System.Collections.Generic;
using System.Linq;

namespace TPLDemo.Demo
{
    public static class DemoFactory
    {
        private static readonly Dictionary<string, Lazy<IRunableDemo>> runableDemos = new Dictionary<string, Lazy<IRunableDemo>>()
        {
            { "parallel", new Lazy<IRunableDemo>(() =>  new ParallelDemo()) },
            { "atomic", new Lazy<IRunableDemo>(() =>  new AtomicDemo()) },
            { "task", new Lazy<IRunableDemo>(() =>  new TaskDemo()) },
            { "continu", new Lazy<IRunableDemo>(() =>  new TaskContinuationDemo()) },
            { "child", new Lazy<IRunableDemo>(() =>  new ChildTaskDemo()) },
        };

        /// <summary>
        /// 获取演示
        /// </summary>
        /// <param name="demoId"></param>
        /// <returns></returns>
        public static IRunableDemo GetRunableDemo(string demoId)
        {
            try
            {
                return runableDemos[demoId]?.Value;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取DemoID
        /// </summary>
        /// <returns></returns>
        public static string[] GetDemoIDs()
            => runableDemos.Keys.ToArray();
    }
}
