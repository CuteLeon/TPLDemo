using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    /// <summary>
    /// 任务延续演示
    /// </summary>
    public class TaskContinuationDemo : RunableDemoBase<RunModel>
    {
        protected byte skiped = 0;

        public override void Run()
        {
            /* ContinuWith：
             * 延续任务共用同一个线程（所以延续任务相互是同步的）；
             * 后驱任务可以获得前驱任务的引用；
             * 后驱任务可以使用 Wait 或 Result 等待前驱任务（其实不需要，因为延续任务相互是同步的）；
             * 最后获取的 Task 或 Task<> 对象不是完整的任务链，而是任务链的最后一个任务；
             * 后驱任务可以在前驱任务执行时或执行后随时追加；
             */

            // 获取数据的任务
            var seedTask = Task.Factory.StartNew(() => base.CreateCollection());

            // 处理数据的任务 （延续在 获取数据的任务 后面）
            var processTask = seedTask.ContinueWith(preTask =>
            {
                Helper.PrintLine("processTask...");
                var models = preTask.Result;
                Task.Delay(1000).Wait();
                Array.ForEach(models, model => model.Name += " [Processed]");
                Helper.PrintLine("处理完成。");
                return models;
            },
            TaskContinuationOptions.OnlyOnRanToCompletion);

            // 输出数据的任务 （延续在 处理数据的任务 后面）
            var printTask = processTask.ContinueWith(preTask =>
            {
                Helper.PrintLine("printTask...");
                var models = preTask.Result;
                Helper.PrintLine($"输出结果：\n\t{string.Join("\n\t", models.Select(model => model.Name))}");
            },
            TaskContinuationOptions.OnlyOnRanToCompletion);

            // 空闲
            var idleTask = printTask.ContinueWith(preTask =>
            {
                Helper.PrintLine("idleTask...");
                preTask.Wait();
                Helper.PrintLine("任务管道空闲...");
            });

            idleTask.Wait();

            // 或者这样的任务链
            var lastTask = Task.Factory.StartNew(() => Helper.PrintLine("Task_0"))
                .ContinueWith((preTask) => Helper.PrintLine($"Task_1 ID = {Task.CurrentId}"))
                .ContinueWith((preTask) => Helper.PrintLine($"Task_2 ID = {Task.CurrentId}"))
                .ContinueWith((preTask) => Helper.PrintLine($"Task_3 ID = {Task.CurrentId}"))
                .ContinueWith((preTask) => Helper.PrintLine($"Task_4 ID = {Task.CurrentId}"))
                .ContinueWith((preTask) => Helper.PrintLine($"Task_5 ID = {Task.CurrentId}"));
            lastTask.Wait();

            Helper.PrintLine($"返回的最后一个任务：{lastTask.Id}");
            Helper.PrintSplit();

            // 或者等待全部of多个任务
            Task.Factory.ContinueWhenAll(
                Enumerable.Range(1, 10).Select(index => Task.Factory.StartNew<int>(() => { Task.Delay(10 * index).Wait(); Helper.PrintLine($"task {index}"); return index; })).ToArray(),
                (tasks) =>
                {
                    // 所有任务完成后，计算结果总和
                    int sum = tasks.Sum(t => t.Result);
                    Helper.PrintLine($"结算结果总和 = {sum}");
                }).Wait();
            Helper.PrintSplit();

            // 或者等待任一of多个任务
            Task.Factory.ContinueWhenAny(
                Enumerable.Range(1, 10).Select(index => Task.Factory.StartNew<int>(() =>
                {
                    if (Thread.VolatileRead(ref this.skiped) == 1)
                    {
                        return int.MinValue;
                    }
                    Task.Delay(10 * index).Wait();
                    Helper.PrintLine($"task {index}");
                    return index;
                })).ToArray(),
                (task) =>
                {
                    // 任一任务完成后，返回结果，此时其他任务仍在执行
                    Thread.VolatileWrite(ref this.skiped, 1);
                    Helper.PrintLine($"最先完成的任务 = {task.Result}");
                }).Wait();
            Helper.PrintSplit();

            // 或者在某些特定条件下才执行延续任务
            /* 如果条件在前面的任务准备调用延续时未得到满足，则延续将直接转换为 TaskStatus.Canceled 状态，之后将无法启动。
             * 在任务完成之前，将阻止 Task.Result 属性。 但是，如果任务已取消或出错，则尝试访问 Result 属性将引发 AggregateException 异常。 可通过使用 OnlyOnRanToCompletion 选项避免此问题。
             */
            var scanTask = Task.Factory.StartNew(() => { Helper.PrintLine("扫描数据..."); });
            // 仅在前驱任务被取消时运行
            var cancelScanTask = scanTask.ContinueWith((pre) => { Helper.PrintLine("取消扫描数据..."); }, TaskContinuationOptions.OnlyOnCanceled);
            // 仅在前驱任务正常完成时运行
            var saveTask = scanTask.ContinueWith((pre) => { Helper.PrintLine("保存数据..."); throw new Exception("保存失败咯，略略略"); }, TaskContinuationOptions.OnlyOnRanToCompletion);
            // 仅在前驱任务失败时运行
            var faultedSaveTask = saveTask.ContinueWith((pre) => { Helper.PrintLine($"保存数据出错：{string.Join("", pre.Exception.InnerExceptions.Select(e => e.Message))}"); }, TaskContinuationOptions.OnlyOnFaulted);
            faultedSaveTask.Wait();
            Helper.PrintSplit();
        }
    }
}
