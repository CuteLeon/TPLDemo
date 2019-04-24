using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    /// <summary>
    /// 取消任务演示
    /// </summary>
    public class CancelTaskDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            token.Register(() => { Helper.PrintLine($"取消了 Token"); });
            source.CancelAfter(250);

            Task task = Task.Factory.StartNew(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(100);
                        Helper.PrintLine("检查取消状态...");
                        /* 如果 Token 已经请求取消了，则抛出异常
                         * 这种方式会使 Task 状态置为 Canceled；
                         */
                        // token.ThrowIfCancellationRequested();

                        /* 或者温和的方式，悄悄return；
                         * 这种方式会使 Task 状态置为 RanToCompletion；
                         */
                        if (token.IsCancellationRequested)
                        {
                            Helper.PrintLine($"悄悄 return");
                            // 这里还可以悄悄处理些事情
                            return;
                        }
                    }
                },
                token);

            try
            {
                task.Wait();
            }
            catch (AggregateException e1) when (!(e1.InnerException is TaskCanceledException))
            {
                // 线程內异常将以 AggregateException 传出，同 task.Exception
                Helper.PrintLine($"AggregateException：{string.Join("；", e1.InnerExceptions.Select(ex => ex.Message))}");
            }
            finally
            {
                source.Dispose();
                Helper.PrintLine($"任务已经取消");
            }
            Helper.PrintSplit();

            // 取消延续任务：延续任务使用 TaskContinuationOptions.NotOnCanceled 或 前后两个任务使用同一个 CancellationToken。
            var cancellation = new CancellationTokenSource();
            var task_1 = Task.Factory.StartNew(() => { Helper.PrintLine("task_1"); cancellation.Cancel(); }, cancellation.Token);
            var task_2 = task_1.ContinueWith((pre) => { Helper.PrintLine("task_2"); }, cancellation.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current);
            task_1.Wait();
            Helper.PrintSplit();

            cancellation = new CancellationTokenSource();
            cancellation.CancelAfter(150);
            // 取消子任务：父子任务使用同一个 CancellationToken
            task_1 = Task.Factory.StartNew(
                () =>
                {
                    Helper.PrintLine("父任务启动");
                    Thread.Sleep(100);
                    Helper.PrintLine("准备启动子任务...");

                    task_2 = Task.Factory.StartNew(
                        () =>
                        {
                            Helper.PrintLine("子任务启动");
                            Thread.Sleep(100);

                            if (cancellation.Token.IsCancellationRequested)
                            {
                                Helper.PrintLine("取消子任务");
                                return;
                            }
                            Helper.PrintLine("子任务完成");
                        },
                        cancellation.Token,
                        TaskCreationOptions.AttachedToParent,
                        TaskScheduler.Default);

                    task_2.Wait();
                    if (cancellation.Token.IsCancellationRequested)
                    {
                        Helper.PrintLine("取消父任务");
                        return;
                    }
                    Helper.PrintLine("父任务完成");
                },
                cancellation.Token);

            task_1.Wait();
            cancellation.Dispose();
        }
    }
}
