using System;
using System.Linq;
using System.Threading.Tasks;

namespace TPLDemo
{
    public class TaskDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            var models = this.CreateCollection();

            // Task 将在异步线程运行
            Task task = new Task(() => Helper.Print("Greet from Task."));
            task.Start();

            Helper.Print("Hello from Main.");

            // 确保 Task 完成之后再退出控制台
            if (task.Status != TaskStatus.RanToCompletion ||
                task.Status != TaskStatus.Canceled ||
                task.Status != TaskStatus.Faulted)
            {
                task.Wait();
            }
            Helper.PrintSplit();

            Task.Factory.StartNew(() => Helper.Print("Greet from Task.Factory.")).Wait();
            Helper.PrintSplit();

            Task[] tasks = models.Take(10).Select(model => Task.Factory.StartNew(
                    (m) => Helper.Print($"Processing {(m as RunModel).Name}"),
                    model))
                .ToArray();
            // 等待全部 Task 完成
            Task.WaitAll(tasks);
            // 回到主线程输出 Task State
            Array.ForEach(tasks, (t) => Helper.Print($"Task-{t.Id} Processed {(t.AsyncState as RunModel).Name}"));
            Helper.PrintSplit();

            /* Task<TResult> 继承自 Task，当以 Task 访问时无法获取 Result
             * Result 属性会导致线程阻塞以等待异步线程完成
             */
            Task<RunModel> funcTask = new Task<RunModel>(() => new RunModel() { Name = "I'm Result-1." });
            funcTask.Start();
            Helper.Print($"Task.Result = {funcTask.Result}");
            funcTask = Task.Factory.StartNew<RunModel>(() => new RunModel() { Name = "I'm Result-2." });
            Helper.Print($"Task.Result = {funcTask.Result}");
        }
    }
}
