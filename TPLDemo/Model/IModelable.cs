namespace TPLDemo.Model
{
    public interface IModelable<TModel>
        where TModel : IModel
    {
        TModel[] CreateCollection(int count);
    }
}
