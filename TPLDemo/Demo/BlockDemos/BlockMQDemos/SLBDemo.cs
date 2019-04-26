using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.BlockMQDemos
{
    /// <summary>
    /// 负载均衡模式
    /// </summary>
    public class SLBDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            BufferBlock<RunModel> gate = new BufferBlock<RunModel>();

            TaskActionBlock<RunModel> server_1 = new TaskActionBlock<RunModel>(model => { Helper.PrintLine($"server_1 开始处理请求：{model.Name}"); Thread.Sleep(1000); Helper.PrintLine($"server_1 处理完成。"); });
            TaskActionBlock<RunModel> server_2 = new TaskActionBlock<RunModel>(model => { Helper.PrintLine($"server_2 开始处理请求：{model.Name}"); Thread.Sleep(1000); Helper.PrintLine($"server_2 处理完成。"); });
            TaskActionBlock<RunModel> server_3 = new TaskActionBlock<RunModel>(model => { Helper.PrintLine($"server_3 开始处理请求：{model.Name}"); Thread.Sleep(1000); Helper.PrintLine($"server_3 处理完成。"); });
            TaskActionBlock<RunModel> server_4 = new TaskActionBlock<RunModel>(model => { Helper.PrintLine($"server_4 开始处理请求：{model.Name}"); Thread.Sleep(1000); Helper.PrintLine($"server_4 处理完成。"); });

            gate.LinkTo(server_1.ActionBlock, (model) => !server_1.Processing);
            gate.LinkTo(server_2.ActionBlock, (model) => !server_2.Processing);
            gate.LinkTo(server_3.ActionBlock, (model) => !server_3.Processing);
            gate.LinkTo(server_4.ActionBlock, (model) => !server_4.Processing);

            Array.ForEach(this.CreateCollection(), model => gate.SendAsync(model));
        }

        protected class TaskActionBlock<TModel>
        {
            private volatile bool processing = false;
            public bool Processing { get => this.processing; protected set => this.processing = value; }

            public readonly ActionBlock<TModel> ActionBlock;

            public TaskActionBlock(Action<TModel> action)
                => this.ActionBlock = new ActionBlock<TModel>(
                    (model) =>
                    {
                        this.processing = true;
                        action.Invoke(model);
                        this.processing = false;
                    },
                    new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1 });
        }
    }
}
