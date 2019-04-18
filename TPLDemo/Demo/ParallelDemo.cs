using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    /// <summary>
    /// 并行演示
    /// </summary>
    public class ParallelDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            var models = this.CreateCollection() as List<RunModel>;

            Parallel.For(10, 30, (index) =>
            {
                Helper.PrintLine(models[index].Name);
            });
            Helper.PrintSplit();

            Parallel.ForEach(models, (model) =>
            {
                Helper.PrintLine(model.Name);
            });
            Helper.PrintSplit();

            // 延时后自动取消并行操作，注意产生 OperationCanceledException
            CancellationTokenSource cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(99999));

            /* 或者手动取消并行操作
             * 注意产生 OperationCanceledException
            Task.Factory.StartNew(() =>
            {
                // 手动取消功能将阻塞一条线程
                Helper.Print("按下 C 键取消并行操作：");
                if (Console.ReadKey().Key == ConsoleKey.C)
                {
                    if (!cancellation.IsCancellationRequested)
                    {
                        cancellation.Cancel();
                    }
                }
            });
             */
            try
            {
                var result = Parallel.ForEach(
                    models,
                    new ParallelOptions()
                    {
                        CancellationToken = cancellation.Token,
                        // 最大并行数
                        MaxDegreeOfParallelism = Environment.ProcessorCount,
                    },
                    (model, state) =>
                    {
                        // 是否此次迭代或其他迭代请求退出
                        if (state.ShouldExitCurrentIteration)
                        {
                            // 如果当前执行的元素标识小于最小请求退出的元素标识，则继续执行此次迭代
                            if (state.LowestBreakIteration < model.Index)
                            {
                                Helper.PrintLine($"Return at {model.Name}");
                                return;
                            }
                            Helper.PrintLine($"Broken but go on {model.Name}");
                        }

                        // 是否发生异常或请求停止
                        if (state.IsExceptional || state.IsStopped)
                        {
                            Helper.PrintLine($"Return at {model.Name}");
                            return;
                        }

                        // 在某次迭代请求Break：请求退出，但仍允许执行某些迭代
                        if (model.Index > 53)
                        {
                            state.Break();
                            Helper.PrintLine($"Break at {model.Name}");
                            return;
                        }

                        // 在某次迭代请求Stop：请求退出，其他迭代应立即退出
                        if (model.Index > 80)
                        {
                            Helper.PrintLine($"Stop at {model.Name}");
                            state.Stop();
                        }

                        // 处理迭代
                        Helper.PrintLine($"Process {model.Name}");
                    });

                Helper.PrintLine($"result.IsCompleted = {result.IsCompleted}");
            }
            catch (OperationCanceledException ex) when (ex != null)
            {
            }
            catch (AggregateException ea) when (ea.InnerExceptions.Count > 0)
            {
                Helper.PrintLine(string.Join("\n", ea.Flatten().InnerExceptions.Select(inex => inex.Message)));
            }
            finally
            {
                cancellation.Dispose();
            }
            Helper.PrintSplit();

            // 并行调用 Action
            Parallel.Invoke(new Action[]
            {
                new Action(()=>{ Helper.PrintLine("0"); }),
                new Action(()=>{ Helper.PrintLine("1"); }),
                new Action(()=>{ Helper.PrintLine("2"); }),
                new Action(()=>{ Helper.PrintLine("3"); }),
                new Action(()=>{ Helper.PrintLine("4"); }),
                new Action(()=>{ Helper.PrintLine("5"); }),
                new Action(()=>{ Helper.PrintLine("6"); }),
                new Action(()=>{ Helper.PrintLine("7"); }),
                new Action(()=>{ Helper.PrintLine("8"); }),
                new Action(()=>{ Helper.PrintLine("9"); }),
                new Action(()=>{ Helper.PrintLine("10"); }),
            });
            Helper.PrintSplit();

            int count = 0;
            Parallel.ForEach(
                models,
                // 初始化线程变量
                () => 0,
                // 迭代方法仅操作本线程的变量
                (model, state, subCount) => subCount += model.Index,
                // 线程结束后合并本线程变量到总变量
                (subCount) => Interlocked.Add(ref count, subCount));
            Helper.PrintLine(count.ToString());
            Helper.PrintSplit();

            // Partitioner.Create 避免小数组并行操作因为分区和委托调度而比顺序操作更消耗性能
            var partitioners = Partitioner.Create(models);
            Parallel.ForEach(partitioners, (model) =>
            {
                Helper.PrintLine(model.Name);
            });
        }
    }
}
