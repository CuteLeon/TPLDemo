using System.Threading.Tasks.Dataflow;
using TPLDemo.Model;

namespace TPLDemo.Demo.BlockDemos.ExecutionBlockDemos
{
    public class TransformManyBlockDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            // TransformManyBlock 可以为每次请求返回多个结果
            TransformManyBlock<RunModel, char> transformManyBlock = new TransformManyBlock<RunModel, char>((model) => $"I'm {model.Name}".ToCharArray());

            var model = this.CreateCollection(1)[0];

            transformManyBlock.Post(model);

            char[] chars = new char[model.Name.Length + 4];
            for (int index = 0; index < model.Name.Length + 4; index++)
            {
                chars[index] = transformManyBlock.Receive();
            }
            Helper.PrintLine(new string(chars));
        }
    }
}
