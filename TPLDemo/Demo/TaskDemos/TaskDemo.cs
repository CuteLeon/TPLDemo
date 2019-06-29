using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TPLDemo.Model;

namespace TPLDemo.Demo.TaskDemos
{
    /// <summary>
    /// 任务演示
    /// </summary>
    public class TaskDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            var models = this.CreateCollection();

            // Task 将在异步线程运行
            Task task = new Task(() => Helper.PrintLine("Greet from Task."));
            task.Start();

            Helper.PrintLine("Hello from Main.");

            // 确保 Task 完成之后再退出控制台
            if (task.Status != TaskStatus.RanToCompletion ||
                task.Status != TaskStatus.Canceled ||
                task.Status != TaskStatus.Faulted)
            {
                task.Wait();
            }
            Helper.PrintSplit();

            Task.Factory.StartNew(() => Helper.PrintLine("Greet from Task.Factory.")).Wait();
            Helper.PrintSplit();

            Task[] tasks = models.Take(10).Select(model => Task.Factory.StartNew(
                    (m) => Helper.PrintLine($"Processing {(m as RunModel).Name}"),
                    model))
                .ToArray();
            // 等待全部 Task 完成
            Task.WaitAll(tasks);
            // 回到主线程输出 Task State
            Array.ForEach(tasks, (t) => Helper.PrintLine($"Task-{t.Id} Processed {(t.AsyncState as RunModel).Name}"));
            Helper.PrintSplit();

            /* Task<TResult> 继承自 Task，当以 Task 访问时无法获取 Result
             * Result 属性会导致线程阻塞以等待异步线程完成
             */
            Task<RunModel> funcTask = new Task<RunModel>(() => new RunModel() { Name = "I'm Result-1." });
            funcTask.Start();
            Helper.PrintLine($"Task.Result = {funcTask.Result}");
            funcTask = Task.Factory.StartNew<RunModel>(() => new RunModel() { Name = "I'm Result-2." });
            Helper.PrintLine($"Task.Result = {funcTask.Result}");
            Helper.PrintSplit();

            // await
            Helper.PrintLine($"async 需要和 await 同时使用才可以达到异步效果");
            Helper.PrintLine($"请求执行一个复杂的功能...");
            task = this.ComplexFunction(models.FirstOrDefault());
            Helper.PrintLine($"不希望等待复杂的功能，而继续运行...");

            task.Wait();
        }

        /// <summary>
        /// 在新线程执行的方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task ComplexFunction(RunModel model)
        {
            Helper.PrintLine($"执行一个复杂的功能：参数={model.Name}");

            // 需要使用 await 而不是 Task.Result 以避免 Run() 阻塞
            int result;

            result = await this.ComplexCalculationTask(model);
            Helper.PrintLine($"得到结果-1：{result}");

            /* 对于非 Task<> 类型的返回值，需要使用 Task.Run 包装下，
             * await Task.Run().ConfigureAwait(false); 防止死锁；
             */
            result = await Task.Run(() => this.ComplexCalculation(model))
                .ConfigureAwait(false);
            Helper.PrintLine($"得到结果-2：{result}");
        }

        private Task<int> ComplexCalculationTask(RunModel model)
        {
            Helper.PrintLine($"开始一次复杂的计算：参数={model.Name}");
            // 需要使用 Task 而不是直接 return 以避免 Run() 阻塞
            return Task.Factory.StartNew(() =>
            {
                Helper.PrintLine($"Task.Factory.StartNew：参数={model.Name}");
                Thread.Sleep(100);
                return Environment.TickCount;
            });
        }

        private int ComplexCalculation(RunModel model)
        {
            Helper.PrintLine($"Native invoke：参数={model.Name}");
            Thread.Sleep(100);
            return Environment.TickCount;
        }
    }
}
