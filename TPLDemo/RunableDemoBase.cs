using System;
using System.Collections.Generic;
using System.Linq;

namespace TPLDemo
{
    public abstract class RunableDemoBase<TModel> : IRunableDemo<TModel>
        where TModel : ModelBase, new()
    {
        public virtual IEnumerable<TModel> CreateCollection()
            => Enumerable.Range(1, 100).Select(index => this.CreateModel(index)).ToList();

        public abstract void Run();

        protected virtual TModel CreateModel(int index)
            => new TModel() { Index = index, Name = $"Model-{index}" };
    }
}
