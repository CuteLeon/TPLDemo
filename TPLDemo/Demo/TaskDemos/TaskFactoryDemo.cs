using System.Threading;
using System.Threading.Tasks;
using TPLDemo.Model;

namespace TPLDemo.Demo.TaskDemos
{
    /// <summary>
    /// 任务工厂演示
    /// </summary>
    public class TaskFactoryDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            var token = source.Token;
            token.Register(() => Helper.PrintLine($"取消了 Token"));
            source.CancelAfter(100);
            TaskFactory taskFactory = new TaskFactory(token);
            Task.WaitAll(
                taskFactory.StartNew(() => { Helper.PrintLine($"start new task {Task.CurrentId}"); SpinWait.SpinUntil(() => source.IsCancellationRequested); token.ThrowIfCancellationRequested(); Helper.PrintLine($"任务 {Task.CurrentId} 正常完成"); }).ContinueWith((pre) => Helper.PrintLine($"取消了任务：{pre.Id} {Task.CurrentId}"), TaskContinuationOptions.OnlyOnCanceled),
                taskFactory.StartNew(() => { Helper.PrintLine($"start new task {Task.CurrentId}"); SpinWait.SpinUntil(() => source.IsCancellationRequested); token.ThrowIfCancellationRequested(); Helper.PrintLine($"任务 {Task.CurrentId} 正常完成"); }).ContinueWith((pre) => Helper.PrintLine($"取消了任务：{pre.Id} {Task.CurrentId}"), TaskContinuationOptions.OnlyOnCanceled),
                taskFactory.StartNew(() => { Helper.PrintLine($"start new task {Task.CurrentId}"); SpinWait.SpinUntil(() => source.IsCancellationRequested); token.ThrowIfCancellationRequested(); Helper.PrintLine($"任务 {Task.CurrentId} 正常完成"); }).ContinueWith((pre) => Helper.PrintLine($"取消了任务：{pre.Id} {Task.CurrentId}"), TaskContinuationOptions.OnlyOnCanceled),
                taskFactory.StartNew(() => { Helper.PrintLine($"start new task {Task.CurrentId}"); SpinWait.SpinUntil(() => source.IsCancellationRequested); token.ThrowIfCancellationRequested(); Helper.PrintLine($"任务 {Task.CurrentId} 正常完成"); }).ContinueWith((pre) => Helper.PrintLine($"取消了任务：{pre.Id} {Task.CurrentId}"), TaskContinuationOptions.OnlyOnCanceled));
        }
    }
}
