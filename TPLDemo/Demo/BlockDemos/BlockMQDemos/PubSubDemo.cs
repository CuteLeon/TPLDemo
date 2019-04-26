using System.Linq;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.BlockMQDemos
{
    /// <summary>
    /// 发布订阅模式演示
    /// </summary>
    public class PubSubDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            TransformBlock<int, RunModel> producer = new TransformBlock<int, RunModel>((index) => new RunModel(index));
            BroadcastBlock<RunModel> publisher = new BroadcastBlock<RunModel>(null);
            BufferBlock<RunModel> subscriber_1 = new BufferBlock<RunModel>();
            BufferBlock<RunModel> subscriber_2 = new BufferBlock<RunModel>();
            ActionBlock<RunModel> processer_1 = new ActionBlock<RunModel>(model => Helper.PrintLine($"subscriber_1 processing {model.Name}"));
            ActionBlock<RunModel> processer_2 = new ActionBlock<RunModel>(model => Helper.PrintLine($"subscriber_2 processing {model.Name}"));

            producer.LinkTo(publisher);

            // 订阅者_1 仅订阅部分消息
            publisher.LinkTo(subscriber_1, model => model.Index < 5);
            publisher.LinkTo(subscriber_2);

            subscriber_1.LinkTo(processer_1);
            subscriber_2.LinkTo(processer_2);

            // 传播完成消息
            producer.Completion.ContinueWith((pre) => publisher.Complete());

            publisher.Completion.ContinueWith((pre) => subscriber_1.Complete());
            publisher.Completion.ContinueWith((pre) => subscriber_2.Complete());

            subscriber_1.Completion.ContinueWith((pre) => processer_1.Complete());
            subscriber_2.Completion.ContinueWith((pre) => processer_2.Complete());

            Enumerable.Range(0, 10).ToList().ForEach(index => producer.SendAsync(index));
            producer.Complete();
            // 等待管道尾部完成
            processer_1.Completion.Wait();
            processer_2.Completion.Wait();
        }
    }
}
