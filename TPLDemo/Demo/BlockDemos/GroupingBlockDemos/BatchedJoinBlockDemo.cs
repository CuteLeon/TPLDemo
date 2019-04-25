using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.GroupingBlockDemos
{
    /// <summary>
    /// 批处理联结块演示
    /// </summary>
    public class BatchedJoinBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            // 注意：每 2 个数据为一组，每 3 组数据组成一批，所以 batchSize = 2 * 3
            BatchedJoinBlock<RunModel, string> batchedJoinBlock = new BatchedJoinBlock<RunModel, string>(2 * 3);
            var models = this.CreateCollection();

            Parallel.For(0, 10, (index) =>
            {
                // 注意：并发操作下，必须 lock，否则组内的数据可能因为顺序而匹配为错误的组
                lock (batchedJoinBlock)
                {
                    batchedJoinBlock.Target1.Post(models[index]);
                    batchedJoinBlock.Target2.Post($"posted as {index}");
                }
            });
            batchedJoinBlock.Complete();

            // BatchedJoinBlock 每个 Target 的数据必须匹配为完整组且组的数量足够组成一次批处理时才可以触发处理逻辑
            for (int index = 1; index <= 4; index++)
            {
                var result = batchedJoinBlock.Receive();
                Helper.PrintLine($"第 {index} 批：元素数量={result.Item1.Count}, {result.Item2.Count}");

                for (int i = 0; i < result.Item1.Count; i++)
                {
                    Helper.PrintLine($"{result.Item1[i].Name} {result.Item2[i]}");
                }
            }
        }
    }
}
