using System.Collections.Generic;

namespace TPLDemo
{
    public interface IRunableDemo<TModel>
        where TModel : IModel
    {
        IEnumerable<TModel> CreateCollection();

        void Run();
    }
}
