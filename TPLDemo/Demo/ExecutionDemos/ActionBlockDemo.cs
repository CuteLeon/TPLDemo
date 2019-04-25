using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.ExecutionDemos
{
    public class ActionBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            ActionBlock<RunModel> actionBlock = new ActionBlock<RunModel>((model) =>
            Helper.PrintLine($"ActionBlock: {model.Name}"));
            var models = this.CreateCollection();

            Parallel.ForEach(models, model => actionBlock.Post(model));

            // 将块置为完成，并等待完成
            actionBlock.Complete();
            actionBlock.Completion.Wait();
        }
    }
}
