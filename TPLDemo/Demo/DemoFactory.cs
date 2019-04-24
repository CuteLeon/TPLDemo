﻿using System;
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
            { "wait", new Lazy<IRunableDemo>(() =>  new WaitTaskDemo()) },
            { "compose", new Lazy<IRunableDemo>(() =>  new ComposeTaskDemo()) },
            { "exception", new Lazy<IRunableDemo>(() =>  new ExceptionDemo()) },
            { "factory", new Lazy<IRunableDemo>(() =>  new TaskFactoryDemo()) },
            { "cancel", new Lazy<IRunableDemo>(() =>  new CancelTaskDemo()) },
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
