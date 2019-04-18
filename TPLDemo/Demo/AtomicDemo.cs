using System.Linq;
using System.Threading;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    public class AtomicDemo : RunableDemoBase<RunModel>
    {
        public static long Count = 0;

        public override void Run()
        {
            Enumerable.Range(1, 100).AsParallel().ForAll((index) =>
            {
                Helper.PrintLine($"{Interlocked.Read(ref Count)} + {index} = {Interlocked.Add(ref Count, index)}");
            });

            Helper.PrintLine(Count.ToString());
        }
    }
}
