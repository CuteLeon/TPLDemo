using System.Linq;
using System.Threading;
using TPLDemo.Model;

namespace TPLDemo.Demo.ThreadDemos
{
    /// <summary>
    /// 原子操作演示
    /// </summary>
    public class AtomicDemo : RunableDemoBase<RunModel>
    {
        public static long Count = 0;

        public override void Run()
        {
            /* 类似操作：
            Thread.VolatileRead();
            Thread.VolatileWrite();
             */
            Enumerable.Range(1, 100).AsParallel().ForAll((index) =>
            {
                Helper.PrintLine($"{Interlocked.Read(ref Count)} + {index} = {Interlocked.Add(ref Count, index)}");
            });

            Helper.PrintLine(Count.ToString());
        }
    }
}
