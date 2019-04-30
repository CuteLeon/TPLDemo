using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.BlockPipelineDemos
{
    /// <summary>
    /// 块管道演示
    /// </summary>
    public class BlockPipelineDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            var producer = new TransformBlock<int, IEnumerable<RunModel>>(count => count > 0 ? this.CreateCollection(count) : Enumerable.Empty<RunModel>());
            var packager = new TransformBlock<IEnumerable<RunModel>, RunModel>(models => models.OrderByDescending(m => m.Index).Take(1).FirstOrDefault());
            var customer = new ActionBlock<RunModel>(models => Helper.PrintLine($"消费了 {models.Name}"));
            var outcast = new ActionBlock<object>(models => Helper.PrintLine($"没有合格产品"));

            // 数量小于 5 的产品不包装，直接抛弃
            producer.LinkTo(packager, models => models.Count() > 5);
            producer.LinkTo(outcast);

            // 产品批次里最大 Index 小于 8 的不销售，直接抛弃
            packager.LinkTo(customer, model => model.Index >= 8);
            packager.LinkTo(outcast);

            producer.Completion.ContinueWith((pre) => packager.Complete());
            packager.Completion.ContinueWith((pre) => { customer.Complete(); outcast.Complete(); });

            for (int index = 0; index < 5; index++)
            {
                int count = Helper.Random.Next(20);
                Helper.PrintLine($"生产第 {index} 批产品：{count} 个");
                producer.Post(count);
            }

            producer.Complete();
            Task.WaitAll(
                customer.Completion,
                outcast.Completion);
        }
    }
}
