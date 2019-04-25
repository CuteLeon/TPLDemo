using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.ExecutionDemos
{
    /// <summary>
    /// 变换块演示
    /// </summary>
    public class TransformBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            TransformBlock<RunModel, string> transformBlock = new TransformBlock<RunModel, string>((model) => $"I'm {model.Name}");

            var models = this.CreateCollection();

            // TransformBlock 可以同时作为输入和输出
            Parallel.ForEach(models, model => transformBlock.Post(model));
            Parallel.For(0, models.Length, (index) =>
            {
                Helper.PrintLine(transformBlock.Receive());
            });
        }
    }
}
