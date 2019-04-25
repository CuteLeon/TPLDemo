using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.ExecutionBlockDemos
{
    /// <summary>
    /// 变换块演示
    /// </summary>
    public class TransformBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            // 指定块最大并行数量为 4
            TransformBlock<RunModel, string> transformBlock = new TransformBlock<RunModel, string>(
                (model) => $"I'm {model.Name}",
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4 });

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
