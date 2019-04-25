using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
