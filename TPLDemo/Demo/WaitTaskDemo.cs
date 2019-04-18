using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    /// <summary>
    /// 等待任务演示
    /// </summary>
    public class WaitTaskDemo : RunableDemoBase<TaskModel>
    {
        protected byte skiped = 0;

        public override void Run()
        {
            // Wait*() 后的代码继续在主线程执行
            var tasks = this.CreateCollection().Select(model => model.Task).ToArray();
            Array.ForEach(tasks, task => task.Start());

            // 等待所有任务完成
            Task.WaitAll(tasks);
            Helper.PrintLine($"WaitAll 所有任务完成。");
            Helper.PrintSplit();

            tasks = this.CreateCollection().Select(model => model.Task).ToArray();
            Array.ForEach(tasks, task => task.Start());
            // 等待任一任务完成
            // 虽然任一任务完成后，主线程跳过等待所有任务而继续执行，但是剩余的任务仍在继续执行
            Task.WaitAny(tasks);
            Thread.VolatileWrite(ref this.skiped, 1);

            Helper.PrintLine($"WaitAny 任一任务完成。");
        }

        protected override TaskModel CreateModel(int index)
        {
            var model = base.CreateModel(index);
            model.Task = new Task(() =>
            {
                if (Thread.VolatileRead(ref this.skiped) == 1)
                {
                    return;
                }
                Helper.PrintLine($"任务 {index} 开始...");

                if (Thread.VolatileRead(ref this.skiped) == 1)
                {
                    return;
                }
                Helper.PrintLine($"任务 {index} 完成。");
            });
            return model;
        }
    }
}
