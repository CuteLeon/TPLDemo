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
                Helper.PrintLine($"任务已经取消");
            }
        }
    }
}
