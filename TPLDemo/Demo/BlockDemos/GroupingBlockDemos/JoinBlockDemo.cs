using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.GroupingBlockDemos
{
    /// <summary>
    /// 联接块演示
    /// </summary>
    public class JoinBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            // 只可以传入两个或三个泛型类型
            JoinBlock<RunModel, string, RunModel> joinBlock = new JoinBlock<RunModel, string, RunModel>();
            var models = this.CreateCollection(2);

            // 注意：并发操作下，必须 lock，否则组内的数据可能因为顺序而匹配为错误的组
            joinBlock.Target1.Post(models[0]);
            joinBlock.Target2.Post("next is");
            joinBlock.Target3.Post(models[1]);

            joinBlock.Target1.Post(models[1]);
            joinBlock.Target2.Post("last is");
            joinBlock.Target3.Post(models[0]);

            // JoinBlock 每个 Target 的数据必须匹配为完整组时才可以触发处理逻辑
            for (int index = 0; index < 2; index++)
            {
                var result = joinBlock.Receive();
                Helper.PrintLine($"{result.Item1.Name} {result.Item2} {result.Item3}");
            }
        }
    }
}
