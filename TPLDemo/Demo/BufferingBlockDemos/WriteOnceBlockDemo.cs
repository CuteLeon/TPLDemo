using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BufferingBlockDemos
{
    /// <summary>
    /// 一次性写入块演示
    /// </summary>
    public class WriteOnceBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            WriteOnceBlock<RunModel> writeOnceBlock = new WriteOnceBlock<RunModel>((model) => model);

            var models = this.CreateCollection().Take(10).ToArray();

            Parallel.ForEach(models, model => writeOnceBlock.Post(model));

            // WriteOnceBlock 只会保留第一个结果，且不限读取次数
            Parallel.For(0, 10, (index) =>
            {
                var model = writeOnceBlock.Receive();
                Helper.PrintLine($"{index} {model.Name}");
            });
        }
    }
}
