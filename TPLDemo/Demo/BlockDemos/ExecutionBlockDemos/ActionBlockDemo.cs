using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.ExecutionBlockDemos
{
    /// <summary>
    /// 功能块演示
    /// </summary>
    public class ActionBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            // 指定块最大并行数量为 4
            ActionBlock<RunModel> actionBlock = new ActionBlock<RunModel>(
                (model) => Helper.PrintLine($"ActionBlock: {model.Name}"),
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4 });
            var models = this.CreateCollection();

            Parallel.ForEach(models, model => actionBlock.Post(model));

            // 将块置为完成，并等待完成
            actionBlock.Complete();
            actionBlock.Completion.Wait();
        }
    }
}
