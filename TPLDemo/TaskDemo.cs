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
        }
    }
}
