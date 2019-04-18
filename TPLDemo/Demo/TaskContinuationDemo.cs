using System;
using System.Linq;
using System.Threading.Tasks;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    /// <summary>
    /// 任务延续演示
    /// </summary>
    public class TaskContinuationDemo : RunableDemoBase<RunModel>
    {
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
            });

            // 输出数据的任务 （延续在 处理数据的任务 后面）
            var printTask = processTask.ContinueWith(preTask =>
            {
                Helper.PrintLine("printTask...");
                var models = preTask.Result;
                Helper.PrintLine($"输出结果：\n\t{string.Join("\n\t", models.Select(model => model.Name))}");
            });

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
        }
    }
}
