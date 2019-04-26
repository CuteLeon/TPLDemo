using System;
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
            /* Unbounded 表示块使用支持的最大并行数
             * 块并行处理时，处理的数据顺序不一定与接收的顺序一致，但是输出时与接收的顺序一致
             */
            TransformBlock<RunModel, string> transformBlock = new TransformBlock<RunModel, string>(
                (model) =>
                {
                    Helper.PrintLine($"开始处理 {model.Name}");
                    return $"I'm {model.Name}";
                },
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            var models = this.CreateCollection();

            // TransformBlock 可以同时作为输入和输出
            Array.ForEach(models, model => transformBlock.Post(model));
            for (int index = 0; index < models.Length; index++)
            {
                Helper.PrintLine(transformBlock.Receive());
            }
        }
    }
}
