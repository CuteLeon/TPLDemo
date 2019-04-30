using System;
using System.Threading;
using System.Threading.Tasks;
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

            ActionBlock<RunModel> server_1 = this.CreateServer("鹰眼神射");
            ActionBlock<RunModel> server_2 = this.CreateServer("暗夜萝莉");
            ActionBlock<RunModel> server_3 = this.CreateServer("不死鸟之眼");
            ActionBlock<RunModel> server_4 = this.CreateServer("王者至尊");

            gate.LinkTo(server_1);
            gate.LinkTo(server_2);
            gate.LinkTo(server_3);
            gate.LinkTo(server_4);

            gate.Completion.ContinueWith((pre) =>
            {
                server_1.Complete();
                server_2.Complete();
                server_3.Complete();
                server_4.Complete();
            });

            Array.ForEach(this.CreateCollection(), model => gate.SendAsync(model));

            gate.Complete();
            // 等待管道尾部完成
            Task.WaitAll(
                server_1.Completion,
                server_2.Completion,
                server_3.Completion,
                server_4.Completion);
        }

        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ActionBlock<RunModel> CreateServer(string name)
            => new ActionBlock<RunModel>(
                model =>
                {
                    Helper.PrintLine($"服务器 {name} 开始处理请求：{model.Name}");
                    Thread.Sleep(Helper.Random.Next(300, 1500));
                    Helper.PrintLine($"服务器 {name} 处理完成：{model.Name}");
                },
                /* 使用 ExecutionDataflowBlockOptions 间接实现 非贪婪 模式：繁忙状态下，不再接受消息；
                 * 否则服务器会运行在贪婪模式：繁忙状态下依然抢收消息
                 */
                new ExecutionDataflowBlockOptions()
                {
                    BoundedCapacity = 1,
                    MaxDegreeOfParallelism = 1
                });
    }
}
