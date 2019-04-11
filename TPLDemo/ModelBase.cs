namespace TPLDemo
{
    public abstract class ModelBase : IModel
    {
        public ModelBase()
        {
        }

        public int Index { get; set; }

        public string Name { get; set; }
    }
}
