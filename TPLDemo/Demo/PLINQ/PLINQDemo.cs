using System;
using System.Linq;
using TPLDemo.Model;

namespace TPLDemo.Demo.PLINQ
{
    /// <summary>
    /// 并行LINQ演示
    /// </summary>
    public class PLINQDemo : RunableDemoBase<RunModel>
    {
        public override void Run()
        {
            // PLINQ 是保守的，当不安全或并行不会提高查询效率时将默认使用顺序方法
            // ParallelEnumerable 编译在 System.Core.dll
            var models = this.CreateCollection(10);

            /* 串行转并行
             * 设置：
             * AsUnordered：抛弃顺序
             * WithMergeOptions：不缓存查询结果，查询出任一结果后立即返回
             * WithDegreeOfParallelism：并行度=处理器数量
             * WithExecutionMode：执行模式=并行优先
             */
            var pmodels = models.AsParallel()
                .AsUnordered()
                .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism);
            // AsOrdered 并行时保留排序结果
            var pmodelOrdered = ParallelEnumerable.AsParallel(models).AsOrdered();
            _ = from model in models.AsParallel() select model;

            // 并行转串行
            _ = pmodels.AsSequential();
            _ = ParallelEnumerable.AsSequential(pmodels);
            _ = from model in pmodels.AsSequential() select model;

            // 并行输出
            Helper.PrintLine($"并行输出：{string.Join("、", pmodels.Select(m => m.Name))}");

            // ForAll
            pmodels.ForAll((m) => Helper.PrintLine($"并行执行：{m.Name}"));

            // 保留顺序 (可能会比直接顺序执行更慢)
            Helper.PrintLine($"保留顺序并行输出：{string.Join("、", pmodelOrdered.Select(m => m.Name))}");

            // 捕捉异常
            try
            {
                pmodels.ForAll(m => { if (m.Index % 2 == 1) throw new ArgumentException(); });
            }
            catch (AggregateException ex)
            {
                Helper.PrintLine($"PLINQ 遇到 {ex.InnerExceptions.Count} 个异常：{string.Join("\n", ex.InnerExceptions.Select(e => e.Message))}");
            }

            // 自定义聚合函数
            _ = pmodels.Aggregate(
                0,
                (sum, model) => { sum += model.Index; return sum; },
                (sum, result) => { result += sum; return result; },
                (result) => new RunModel(result));

            _ = pmodels.Aggregate(
                new RunModel(),
                (result, model) => { result.Index += model.Index; return result; });
        }
    }
}
