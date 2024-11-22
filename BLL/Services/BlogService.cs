using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BLL.Services
{
    public interface IBlogService
    {
        public IQueryable<BlogModel> Query();
        public ServiceBase Create(Blog record);
        public ServiceBase Update(Blog record);
        public ServiceBase Delete(int id);
    }

    public class BlogService : ServiceBase, IBlogService
    {
        public BlogService(BlogDbContext db) : base(db)
        {
        }

        public IQueryable<BlogModel> Query()
        {
            return _db.Blogs
                .Include(b => b.User) // Include the User for eager loading
                .OrderByDescending(b => b.PublishDate)
                .Select(b => new BlogModel
                {
                    Record = b
                });
        }

        public ServiceBase Create(Blog record)
        {
            if (string.IsNullOrWhiteSpace(record.Title))
                return Error("Title is required.");

            record.Title = record.Title.Trim();
            record.PublishDate = DateTime.Now; // Automatically set the publish date
            _db.Blogs.Add(record);
            _db.SaveChanges();
            return Success("Blog created successfully!");
        }

        public ServiceBase Update(Blog record)
        {
            var entity = _db.Blogs.SingleOrDefault(b => b.Id == record.Id);
            if (entity == null)
                return Error("Blog not found.");

            entity.Title = record.Title?.Trim();
            entity.Content = record.Content;
            entity.Rating = record.Rating;
            entity.UserId = record.UserId;
            _db.Blogs.Update(entity);
            _db.SaveChanges();
            return Success("Blog updated successfully!");
        }

        public ServiceBase Delete(int id)
        {
            var entity = _db.Blogs.SingleOrDefault(b => b.Id == id);
            if (entity == null)
                return Error("Blog not found.");

            _db.Blogs.Remove(entity);
            _db.SaveChanges();
            return Success("Blog deleted successfully!");
        }
    }
}
