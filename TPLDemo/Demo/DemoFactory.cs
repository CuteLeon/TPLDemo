using System;
using System.Collections.Generic;
using System.Linq;
using TPLDemo.Demo.BufferingBlockDemos;
using TPLDemo.Demo.ExecutionDemos;
using TPLDemo.Demo.ParallelDemos;
using TPLDemo.Demo.TaskDemos;
using TPLDemo.Demo.ThreadDemos;

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
            { "wait", new Lazy<IRunableDemo>(() =>  new WaitTaskDemo()) },
            { "compose", new Lazy<IRunableDemo>(() =>  new ComposeTaskDemo()) },
            { "factory", new Lazy<IRunableDemo>(() =>  new TaskFactoryDemo()) },
            { "exception", new Lazy<IRunableDemo>(() =>  new ExceptionDemo()) },
            { "cancel", new Lazy<IRunableDemo>(() =>  new CancelTaskDemo()) },
            { "buffer", new Lazy<IRunableDemo>(() =>  new BufferBlockDemo()) },
            { "broadcast", new Lazy<IRunableDemo>(() =>  new BroadcastBlockDemo()) },
            { "writeonce", new Lazy<IRunableDemo>(() =>  new WriteOnceBlockDemo()) },
            { "action", new Lazy<IRunableDemo>(()=> new ActionBlockDemo()) },
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
        /// 获取最后一个演示
        /// </summary>
        /// <returns></returns>
        public static IRunableDemo GetLastRunableDemo()
            => runableDemos.Values.LastOrDefault()?.Value;

        /// <summary>
        /// 获取DemoID
        /// </summary>
        /// <returns></returns>
        public static string[] GetDemoIDs()
            => runableDemos.Keys.ToArray();
    }
}
