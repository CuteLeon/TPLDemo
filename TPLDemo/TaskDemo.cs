using System.Reflection.Emit;
using System.Threading.Tasks;

namespace TPLDemo
{
    public class TaskDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            var models = this.CreateCollection();

            // Task 将在异步线程运行
            Task task = new Task(() => Helper.Print("Greet from Task."));
            task.Start();
            
            Helper.Print("Hello from Main.");

            // 确保 Task 完成之后再退出控制台
            if (task.Status != TaskStatus.RanToCompletion ||
                task.Status != TaskStatus.Canceled ||
                task.Status != TaskStatus.Faulted)
            {
                task.Wait();
            }
            Helper.PrintSplit();

            Task.Factory.StartNew(() => Helper.Print("Greet from Task.Factory."));
        }
    }
}
