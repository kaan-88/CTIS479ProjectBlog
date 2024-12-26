using BLL.DAL;

namespace BLL.Models
{
    public class TagModel
    {
        public Tag Record { get; set; }

        public string Name => Record.Name;
    }
}
