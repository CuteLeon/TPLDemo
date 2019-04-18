using System.Collections.Generic;
using System.Linq;
using TPLDemo.Model;

namespace TPLDemo.Demo
{
    public abstract class RunableDemoBase<TModel> : IRunableDemo, IModelable<TModel>
        where TModel : ModelBase, new()
    {
        public virtual IEnumerable<TModel> CreateCollection()
            => Enumerable.Range(1, 100).Select(index => this.CreateModel(index)).ToList();

        public abstract void Run();

        protected virtual TModel CreateModel(int index)
            => new TModel() { Index = index, Name = $"Model-{index}" };
    }
}
