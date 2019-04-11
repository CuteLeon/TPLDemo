﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TPLDemo
{
    public class ParallelDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            var models = this.CreateCollection() as List<RunModel>;

            Parallel.For(10, 30, (index) =>
            {
                Helper.Print(models[index].Name);
            });
            Helper.PrintSplit();

            Parallel.ForEach(models, (model) =>
            {
                Helper.Print(model.Name);
            });
            Helper.PrintSplit();

            // 延时后自动取消并行操作，注意产生 OperationCanceledException
            CancellationTokenSource cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(99999));

            /* 或者手动取消并行操作
             * 注意产生 OperationCanceledException
            Task.Factory.StartNew(() =>
            {
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
                                Helper.Print($"Return at {model.Name}");
                                return;
                            }
                            Helper.Print($"Broken but go on {model.Name}");
                        }

                        // 是否发生异常或请求停止
                        if (state.IsExceptional || state.IsStopped)
                        {
                            Helper.Print($"Return at {model.Name}");
                            return;
                        }

                        // 在某次迭代请求Break：请求退出，但仍允许执行某些迭代
                        if (model.Index > 53)
                        {
                            state.Break();
                            Helper.Print($"Break at {model.Name}");
                            return;
                        }

                        // 在某次迭代请求Stop：请求退出，其他迭代应立即退出
                        if (model.Index > 80)
                        {
                            Helper.Print($"Stop at {model.Name}");
                            state.Stop();
                        }

                        // 处理迭代
                        Helper.Print($"Process {model.Name}");
                    });

                Helper.Print($"result.IsCompleted = {result.IsCompleted}");
            }
            catch (OperationCanceledException ex) when (ex != null)
            {
            }
            catch (AggregateException ea) when (ea.InnerExceptions.Count > 0)
            {
                Helper.Print(string.Join("\n", ea.Flatten().InnerExceptions.Select(inex => inex.Message)));
            }
            finally
            {
                cancellation.Dispose();
            }
            Helper.PrintSplit();

            // 并行调用 Action
            Parallel.Invoke(new Action[]
            {
                new Action(()=>{ Helper.Print("0"); }),
                new Action(()=>{ Helper.Print("1"); }),
                new Action(()=>{ Helper.Print("2"); }),
                new Action(()=>{ Helper.Print("3"); }),
                new Action(()=>{ Helper.Print("4"); }),
                new Action(()=>{ Helper.Print("5"); }),
                new Action(()=>{ Helper.Print("6"); }),
                new Action(()=>{ Helper.Print("7"); }),
                new Action(()=>{ Helper.Print("8"); }),
                new Action(()=>{ Helper.Print("9"); }),
                new Action(()=>{ Helper.Print("10"); }),
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
            Helper.Print(count.ToString());
        }
    }
}
