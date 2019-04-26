using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.BufferingBlockDemos
{
    /// <summary>
    /// 广播块演示
    /// </summary>
    public class BroadcastBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            BroadcastBlock<RunModel> broadcastBlock = new BroadcastBlock<RunModel>((model) => model);
            // 广播块实现 发布-订阅模式
            broadcastBlock.LinkTo(new ActionBlock<RunModel>((model) => Helper.PrintLine($"action_1 : {model.Name}")));
            broadcastBlock.LinkTo(new ActionBlock<RunModel>((model) => Helper.PrintLine($"action_2 : {model.Name}")));

            var models = this.CreateCollection();
            Parallel.ForEach(models, model => broadcastBlock.Post(model));

            // BroadcastBlock 只会保留最后一个结果，且不限读取次数
            Parallel.For(0, 10, (index) =>
            {
                var model = broadcastBlock.Receive();
                Helper.PrintLine($"receive {index} {model.Name}");
            });
        }
    }
}
