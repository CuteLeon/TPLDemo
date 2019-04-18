namespace TPLDemo.Model
{
    public abstract class ModelBase : IModel
    {
        public ModelBase()
        {
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public override string ToString()
            => this.Name;
    }
}
