using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    /// <summary>
    /// 任务工厂演示
    /// </summary>
    public class TaskFactoryDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskFactory taskFactory = new TaskFactory(cancellation.Token);
            taskFactory.StartNew(() => { Helper.PrintLine($"start new task {Task.CurrentId}"); }, cancellation.Token);
            cancellation.Cancel();
        }
    }
}
