using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    /// <summary>
    /// 缓冲块演示
    /// </summary>
    public class BufferBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            BufferBlock<RunModel> bufferBlock = new BufferBlock<RunModel>();

            var models = this.CreateCollection().Take(20).ToArray();

            // BufferBlock 为先进先出队列，每个数据块读取一次后自动销毁
            Helper.PrintLine("BufferBlock<> 支持并发操作");
            Parallel.ForEach(models, model => bufferBlock.Post(model));

            Parallel.For(0, models.Length, (index) =>
            {
                var model = bufferBlock.Receive();
                Helper.PrintLine($"No.{index} {model.Name}");
            });
            Helper.PrintSplit();

            Helper.PrintLine("BufferBlock<> 先进先出队列");
            Array.ForEach(models, model => bufferBlock.Post(model));
            for (int index = 0; index < models.Length; index++)
            {
                // Receive() 会导致线程阻塞，直到下次 Post
                var model = bufferBlock.Receive();
                Helper.PrintLine($"No.{index} {model.Name}");
            }
        }
    }
}
