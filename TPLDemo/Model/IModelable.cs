using System.Collections.Generic;

namespace TPLDemo.Model
{
    public interface IModelable<TModel>
        where TModel : IModel
    {
        IEnumerable<TModel> CreateCollection();
    }
}
