using System;
using System.Linq;
using System.Threading.Tasks;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    /// <summary>
    /// 异常演示
    /// </summary>
    public class ExceptionDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            // AggregateException.InnerExceptions 可以访问异步任务的多个异常
            try
            {
                Parallel.For(0, 10, (index) =>
                {
                    Task.Delay(10 * index).Wait();
                    throw new Exception($"exception in {index}");
                });
            }
            catch (AggregateException ex)
            {
                Helper.PrintLine($"遇到 {ex.InnerExceptions.Count} 个异常：\n\t{string.Join("\n\t", ex.InnerExceptions.Select(e => e.Message))}");
            }

            var task = Task.Factory.StartNew(() => { throw new Exception("exception in task."); });
            var continuation = task.ContinueWith((pre) => { Helper.PrintLine($"异步处理任务的异常：{pre.Exception.Message}"); }, TaskContinuationOptions.OnlyOnFaulted);
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                Helper.PrintLine($"try 捕捉到 {ex.InnerExceptions.Count} 个异常：\n\t{string.Join("\n\t", ex.InnerExceptions.Select(e => e.Message))}");
            }

            // 附加子任务的异常将在 AggregateException 内层层嵌套，可使用 Flatten() 方法平展
            task = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(
                    () =>
                    {
                        Task.Factory.StartNew(
                            () =>
                            {
                                throw new Exception("孙任务发生异常");
                            },
                            TaskCreationOptions.AttachedToParent);

                        throw new Exception("子任务发生异常");
                    },
                    TaskCreationOptions.AttachedToParent);
            });

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                // 使用 Flatten() 方法转换 InnerExceptions 树状结构为一维列表
                Helper.PrintLine($"try 捕捉到 {ex.InnerExceptions.Count} 个异常：\n\t{string.Join("\n\t", ex.InnerExceptions.Select(e => e.Message))}");
                var fex = ex.Flatten();
                Helper.PrintLine($"try 捕捉到 {fex.InnerExceptions.Count} 个异常：\n\t{string.Join("\n\t", fex.InnerExceptions.Select(e => e.Message))}");

                // 抛出平展后的异常
                throw fex;
            }
        }
    }
}
