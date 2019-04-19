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
        }
    }
}
