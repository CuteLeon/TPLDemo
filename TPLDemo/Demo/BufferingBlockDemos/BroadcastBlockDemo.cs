using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BufferingBlockDemos
{
    /// <summary>
    /// 广播块演示
    /// </summary>
    public class BroadcastBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            BroadcastBlock<RunModel> broadcastBlock = new BroadcastBlock<RunModel>((model) => model);

            var models = this.CreateCollection();
            Parallel.ForEach(models, model => broadcastBlock.Post(model));

            // BroadcastBlock 只会保留最后一个结果，且不限读取次数
            Parallel.For(0, 10, (index) =>
            {
                var model = broadcastBlock.Receive();
                Helper.PrintLine($"{index} {model.Name}");
            });
        }
    }
}
