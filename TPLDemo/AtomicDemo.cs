using System.Linq;
using System.Threading;

namespace TPLDemo
{
    public class AtomicDemo : RunableDemoBase<RunModel>
    {
        public static long Count = 0;

        public override void Run()
        {
            Enumerable.Range(1, 100).AsParallel().ForAll((index) =>
            {
                Helper.Print($"{Interlocked.Read(ref Count)} + {index} = {Interlocked.Add(ref Count, index)}");
            });

            Helper.Print(Count.ToString());
        }
    }
}
