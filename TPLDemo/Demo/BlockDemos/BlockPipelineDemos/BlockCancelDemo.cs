using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.BlockPipelineDemos
{
    public class BlockCancelDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            CancellationTokenSource cancellationSource = new CancellationTokenSource();
            // 获取当前任务调度器
            TaskScheduler taskScheduler;
            try
            {
                // 可以获取 UI 线程所在的主线程，以访问UI
                SynchronizationContext.SetSynchronizationContext(SynchronizationContext.Current);
                taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }
            catch
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }

            TransformBlock<int, RunModel> producer = new TransformBlock<int, RunModel>(
                (index) => new RunModel(index),
                new ExecutionDataflowBlockOptions()
                {
                    CancellationToken = cancellationSource.Token
                });
            BufferBlock<RunModel> processer = new BufferBlock<RunModel>(
                new DataflowBlockOptions()
                {
                    CancellationToken = cancellationSource.Token
                });
            ActionBlock<RunModel> display = new ActionBlock<RunModel>(
                (model) => Helper.PrintLine($"显示: {model.Name}"),
                new ExecutionDataflowBlockOptions()
                {
                    CancellationToken = cancellationSource.Token,
                    TaskScheduler = taskScheduler
                });

            producer.LinkTo(processer, new DataflowLinkOptions { PropagateCompletion = true });
            processer.LinkTo(display, new DataflowLinkOptions { PropagateCompletion = true });

            cancellationSource.CancelAfter(100);
            Array.ForEach(Enumerable.Range(1, 10).ToArray(), (index) =>
            {
                Thread.Sleep(15);
                producer.Post(index);
            });
            producer.Complete();
            display.Completion.Wait();
        }
    }
}
