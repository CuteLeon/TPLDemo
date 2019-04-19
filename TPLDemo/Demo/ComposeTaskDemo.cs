using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TPLDemo.Demo
{
    /// <summary>
    /// 组合任务演示
    /// </summary>
    public class ComposeTaskDemo : WaitTaskDemo
    {
        public override void Run()
        {
            // When*() 方法不会阻塞调用线程，且之后的代码在新的线程执行
            var tasks = this.CreateCollection().Select(model => model.Task).ToArray();
            Array.ForEach(tasks, task => task.Start());

            // 等待所有任务完成
            Task.WhenAll(tasks).ContinueWith((preTask) =>
            {
                Helper.PrintLine($"WhenAll 所有任务完成。");
                Helper.PrintSplit();
            }).Wait();

            tasks = this.CreateCollection().Select(model => model.Task).ToArray();
            Array.ForEach(tasks, task => task.Start());
            // 等待任一任务完成
            // 虽然任一任务完成后，主线程跳过等待所有任务而继续执行，但是剩余的任务仍在继续执行
            Task.WhenAny(tasks).ContinueWith((preTask) =>
            {
                Thread.VolatileWrite(ref this.skiped, 1);
                Helper.PrintLine($"WhenAny 任一任务完成。");
            }).Wait();

            Helper.PrintSplit();
            Helper.PrintLine($"计算结果：{this.Sum(1, 1).Result} {this.Add(1, 2).Result}");
        }

        protected Task<int> Sum(int x, int y)
        {
            Task.Delay(10).Wait();
            return Task.FromResult(x + y);
        }

        protected async Task<int> Add(int x, int y)
        {
            await Task.Delay(10);
            return x + y;
        }
    }
}
