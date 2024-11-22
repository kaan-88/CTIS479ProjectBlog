using BLL.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class BlogModel
    {
        public Blog Record { get; set; } 

        public string Title => Record.Title;
        public string Content => Record.Content;
        public decimal? Rating => Record.Rating;
        public DateTime PublishDate => Record.PublishDate;
        public string AuthorName => Record.User?.UserName ?? "No Author";
    }
}
