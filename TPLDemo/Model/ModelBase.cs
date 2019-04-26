namespace TPLDemo.Model
{
    public abstract class ModelBase : IModel
    {
        public ModelBase()
        {
        }

        public ModelBase(int index)
            : this()
        {
            this.Index = index;
            this.Name = $"Model_{index}";
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public override string ToString()
            => this.Name;
    }
}
