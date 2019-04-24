using System.Threading.Tasks;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    /// <summary>
    /// 子任务演示
    /// </summary>
    public class ChildTaskDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            // 默认情况下，子任务和父任务没有在同一线程，父进程不会等待子进程而直接结束
            // 父任务可以通过 TaskCreationOptions.DenyChildAttach 声明禁止附加子任务。
            Task childTask = null;
            var parentTask_1 = Task.Factory.StartNew(() =>
            {
                Helper.PrintLine("开始执行父任务-1...");

                childTask = Task.Factory.StartNew(() =>
                      {
                          Helper.PrintLine("开始执行子任务-1...");
                          Task.Delay(10).Wait();
                          Helper.PrintLine("子任务-1完成...");
                      });
                // 如果子任务返回 Result，则在父任务内获取子任务的 Result 可以间接地实现父任务等待子任务
                Helper.PrintLine("父任务-1完成...");
            });

            // 需要同时手动等待父任务和子任务结束
            parentTask_1.Wait();
            childTask.Wait();

            Helper.PrintSplit();
            var parentTask_2 = Task.Factory.StartNew(() =>
            {
                Helper.PrintLine("开始执行父任务-2...");

                Task.Factory.StartNew(() =>
                    {
                        Helper.PrintLine("开始执行子任务-2...");
                        Task.Delay(10).Wait();
                        Helper.PrintLine("子任务-2完成...");
                    },
                    TaskCreationOptions.AttachedToParent);
            });

            // 这里只需要手动等待父任务结束，而不用手动等待子任务结束，因为父任务会隐式等待子任务
            parentTask_2.Wait();
            Helper.PrintLine("父任务-2完成...");
            Helper.PrintSplit();

            Task.FromResult(
                Task.Factory.StartNew(() =>
                {
                    Helper.PrintLine("任务开始");
                    Helper.PrintLine("任务完成");
                }))
                .Unwrap() // 拆开重复包装的 Task
                .Wait();
        }
    }
}
