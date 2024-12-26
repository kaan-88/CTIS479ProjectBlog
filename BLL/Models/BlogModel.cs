using BLL.DAL;
using System.ComponentModel;

namespace BLL.Models
{
    public class BlogModel
    {
        public Blog Record { get; set; }

        [DisplayName("Title")]
        public string Title => Record.Title;

        [DisplayName("Content")]
        public string Content => Record.Content;

        [DisplayName("Rating")]
        public string Rating => Record.Rating.HasValue ? Record.Rating.Value.ToString("N2") : "No Rating";

        [DisplayName("Publish Date")]
        public string PublishDate => Record.PublishDate.ToString("MM/dd/yyyy");

        [DisplayName("Author")]
        public string AuthorName => Record.User?.UserName ?? "No Author";

        [DisplayName("Tags")]
        public string Tags => string.Join(", ", Record.BlogTags?.Select(bt => bt.Tag?.Name) ?? new List<string>());

        // Manage relational IDs (Tags)
        [DisplayName("Tags")]
        public List<int> TagIds
        {
            get => Record.BlogTags?.Select(bt => bt.TagId).ToList() ?? new List<int>();
            set => Record.BlogTags = value.Select(v => new BlogTag { BlogId = Record.Id, TagId = v }).ToList();
        }
    }
}
