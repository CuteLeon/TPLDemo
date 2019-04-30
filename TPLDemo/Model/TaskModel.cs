using System.Threading.Tasks;

namespace TPLDemo.Model
{
    public class TaskModel : ModelBase
    {
        public Task Task { get; set; }

        public TaskModel()
            : base()
        {
        }

        public TaskModel(int index)
            : base(index)
        {
        }
    }
}
