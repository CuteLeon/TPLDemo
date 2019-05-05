using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPLDemo.Model;

namespace TPLDemo.Demo.InvokeDemos
{
    /// <summary>
    /// Invoke 演示
    /// </summary>
    public class InvokeDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            // Func 委托
            Func<int, RunModel[]> createDelegate = this.CreateCollection;
            RunModel[] models = null;

            /* .Net Core 3.0 pre 暂不支持此操作
             * BeginInvoke：
             * 在新线程异步执行委托，不会阻塞当前线程；
             * 可以传入 AsyncCallback 在调用结束后执行回调；
             * 可以使用 IAsyncResult.AsyncWaitHandle.WaitOne() 和 EndInvoke() 阻塞当前线程并等待异步委托调用结束；
             * 需要使用 EndInvoke() 获取 Func<> 委托的返回结果；
             */
            Helper.PrintLine("BeginInvoke");
            /*
            IAsyncResult result = createDelegate.BeginInvoke(3, new AsyncCallback(r => this.PrintIAsyncResult(r)), "BeginInvoke");
            this.PrintIAsyncResult(result);
            Helper.PrintLine("EndInvoke");
            models = createDelegate.EndInvoke(result);
            Helper.PrintLine($"结果：{string.Join("、", models.Select(m => m.Name))}");
             */

            /* Invoke：
             * 使用 Invoke 使委托在 UI 主线程调用
             */
            Helper.PrintLine("Invoke");
            models = createDelegate.Invoke(6);
            Helper.PrintLine($"结果：{string.Join("、", models.Select(m => m.Name))}");

            Helper.PrintLine("DynamicInvoke");
            models = createDelegate.DynamicInvoke(6) as RunModel[];
            Helper.PrintLine($"结果：{string.Join("、", models.Select(m => m.Name))}");
        }

        public void PrintIAsyncResult(IAsyncResult asyncResult)
        {
            Helper.PrintLine(@$"输出异步执行结果：\n
\tIsCompleted = {asyncResult.IsCompleted}\n
\tCompletedSynchronously = {asyncResult.CompletedSynchronously}\n
\tAsyncState.Type = {asyncResult.AsyncState?.GetType()?.FullName ?? "null"}\n
\tModels = {((asyncResult.AsyncState is RunModel[] models) ? string.Join("、", models.Select(m => m.Name)) : "null")}");
        }

        public override RunModel[] CreateCollection(int count = 10)
        {
            Helper.PrintLine($"调用 CreateCollection");
            return base.CreateCollection(count);
        }
    }
}
