using System.Linq;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    public abstract class RunableDemoBase<TModel> : IRunableDemo, IModelable<TModel>
        where TModel : ModelBase, new()
    {
        public virtual TModel[] CreateCollection(int count = 10)
            => Enumerable.Range(1, count).Select(index => this.CreateModel(index)).ToArray();

        public abstract void Run();

        protected virtual TModel CreateModel(int index)
            => new TModel() { Index = index, Name = $"Model-{index}" };
    }
}
