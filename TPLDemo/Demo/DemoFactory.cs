using System;
using System.Collections.Generic;
using System.Linq;
using TPLDemo.Demo.BlockDemos.BlockMQDemos;
using TPLDemo.Demo.BlockDemos.BlockPipelineDemos;
using TPLDemo.Demo.BlockDemos.BufferingBlockDemos;
using TPLDemo.Demo.BlockDemos.ExecutionBlockDemos;
using TPLDemo.Demo.BlockDemos.GroupingBlockDemos;
using TPLDemo.Demo.ParallelDemos;
using TPLDemo.Demo.PLINQ;
using TPLDemo.Demo.TaskDemos;
using TPLDemo.Demo.ThreadDemos;

namespace TPLDemo.Demo
{
    public static class DemoFactory
    {
        private static readonly Dictionary<string, (Lazy<IRunableDemo> RunableDemo, string Description)> runableDemos = new Dictionary<string, (Lazy<IRunableDemo> RunableDemo, string Description)>
        {
            { "parallel", (new Lazy<IRunableDemo>(() =>  new ParallelDemo()), "并行计算演示") },
            { "atomic", (new Lazy<IRunableDemo>(() =>  new AtomicDemo()), "原子操作演示") },
            { "task", (new Lazy<IRunableDemo>(() =>  new TaskDemo()), "Task-任务类演示") },
            { "continu", (new Lazy<IRunableDemo>(() =>  new TaskContinuationDemo()), "延续任务演示") },
            { "child", (new Lazy<IRunableDemo>(() =>  new ChildTaskDemo()), "子任务演示") },
            { "wait", (new Lazy<IRunableDemo>(() =>  new WaitTaskDemo()), "等待任务演示") },
            { "compose", (new Lazy<IRunableDemo>(() =>  new ComposeTaskDemo()), "组合任务演示") },
            { "factory", (new Lazy<IRunableDemo>(() =>  new TaskFactoryDemo()), "任务工厂演示") },
            { "exception", (new Lazy<IRunableDemo>(() =>  new ExceptionDemo()), "捕捉任务异常演示") },
            { "canceltask", (new Lazy<IRunableDemo>(() =>  new CancelTaskDemo()), "取消任务演示") },
            { "writeonce", (new Lazy<IRunableDemo>(() =>  new WriteOnceBlockDemo()), "WriteOnceBlock-单次写入块演示") },
            { "action", (new Lazy<IRunableDemo>(()=> new ActionBlockDemo()), "ActionBlock-行为块演示") },
            { "transform", (new Lazy<IRunableDemo>(()=> new TransformBlockDemo()), "TransformBlock-变换块演示") },
            { "transformmany", (new Lazy<IRunableDemo>(()=> new TransformManyBlockDemo()), "TransformManyBlock-多变换块演示") },
            { "batch", (new Lazy<IRunableDemo>(()=> new BatchBlockDemo()), "BatchBatch-批处理块演示") },
            { "join", (new Lazy<IRunableDemo>(()=> new JoinBlockDemo()), "JoinBlock-联接块演示") },
            { "batchedjoin", (new Lazy<IRunableDemo>(()=> new BatchedJoinBlockDemo()), "BatchJoinBlock-批处理联接块演示") },
            { "broadcast", (new Lazy<IRunableDemo>(() =>  new BroadcastBlockDemo()), "BroadcastBlock-广播块演示") },
            { "buffer", (new Lazy<IRunableDemo>(() =>  new BufferBlockDemo()), "BufferBlock-缓存块演示") },
            { "pubsub", (new Lazy<IRunableDemo>(() =>  new PubSubDemo()), "MQ发布订阅模式演示") },
            { "slb", (new Lazy<IRunableDemo>(() =>  new SLBDemo()), "MQ负载均衡模式演示") },
            { "pipeline", (new Lazy<IRunableDemo>(() =>  new BlockPipelineDemo()), "块管道演示") },
            { "cancelblock", (new Lazy<IRunableDemo>(() =>  new BlockCancelDemo()), "取消块管道演示") },
            { "plinq", (new Lazy<IRunableDemo>(() =>  new PLINQDemo()), "PLINQ-并行LINQ演示") },
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
                return runableDemos[demoId].RunableDemo.Value;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取演示
        /// </summary>
        /// <param name="demoIndex"></param>
        /// <returns></returns>
        public static IRunableDemo GetRunableDemo(int demoIndex)
        {
            try
            {
                return runableDemos[runableDemos.Keys.ElementAt(demoIndex)].RunableDemo.Value;
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
            => runableDemos.Values.LastOrDefault().RunableDemo.Value;

        /// <summary>
        /// 获取DemoID
        /// </summary>
        /// <returns></returns>
        public static string[] GetDemoIDs()
            => runableDemos.Keys.ToArray();

        /// <summary>
        /// 获取Demo描述
        /// </summary>
        /// <returns></returns>
        public static string[] GetDemoDescriptions()
            => runableDemos.Values.Select(value => value.Description).ToArray();
    }
}
