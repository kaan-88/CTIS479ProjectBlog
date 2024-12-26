using System.ComponentModel;

namespace BLL.Models
{
    public class FavoritesModel
    {
        public int BlogId { get; set; }
        public int UserId { get; set; }

        [DisplayName("Blog Title")]
        public string BlogTitle { get; set; }

    }
}
