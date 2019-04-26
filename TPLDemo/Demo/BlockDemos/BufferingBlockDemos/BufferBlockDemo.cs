using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.BufferingBlockDemos
{
    /// <summary>
    /// 缓冲块演示
    /// </summary>
    public class BufferBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            BufferBlock<RunModel> bufferBlock = new BufferBlock<RunModel>();

            var models = this.CreateCollection();

            // BufferBlock 为先进先出队列，每个数据块读取一次后自动销毁
            Helper.PrintLine("BufferBlock<> 支持并发操作");
            Parallel.ForEach(models, model => bufferBlock.Post(model));

            for (int index = 0; index < models.Length; index++)
            {
                bufferBlock.ReceiveAsync().ContinueWith((pre) =>
                {
                    // ReceiveAsync 可以异步接收，并在 ContinueWith 里处理
                    var model = pre.Result;
                    Helper.PrintLine($"异步接收 No.{index} {model.Name}");
                }).Wait();
            }
            Helper.PrintSplit();

            Helper.PrintLine("BufferBlock<> 先进先出队列");
            Array.ForEach(models, model => bufferBlock.Post(model));
            for (int index = 0; index < models.Length; index++)
            {
                // Receive() 会导致线程阻塞，直到下次 Post
                var model = bufferBlock.Receive();
                Helper.PrintLine($"同步接收 No.{index} {model.Name}");
            }
            Helper.PrintSplit();

            Helper.PrintLine("BufferBlock<> 异步发送");
            Array.ForEach(models, model => bufferBlock.SendAsync(model));
            for (int index = 0; index < models.Length; index++)
            {
                var model = bufferBlock.ReceiveAsync().Result;
                Helper.PrintLine($"异步发送 No.{index} {model.Name}");
            }
        }
    }
}
