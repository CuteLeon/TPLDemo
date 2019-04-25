using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.GroupingBlockDemos
{
    /// <summary>
    /// 批处理块演示
    /// </summary>
    public class BatchBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            // 每 3 个数据组成一次批处理
            BatchBlock<RunModel> batchBlock = new BatchBlock<RunModel>(3);
            Array.ForEach(this.CreateCollection(10), (model) => batchBlock.Post(model));

            // 最后一波数据数量可能不足以组成一次批处理，使用 Complete 直接处理
            batchBlock.Complete();

            for (int index = 1; index <= 4; index++)
            {
                var models = batchBlock.Receive();
                Helper.PrintLine($"第 {index} 批 {models.Length} 个数据：{string.Join("、", models.Select(model => model.Name))}");
            }
        }
    }
}
