using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BLL.Services
{
    public interface IBlogService
    {
        IQueryable<BlogModel> Query();
        ServiceBase Create(Blog record);
        ServiceBase Update(Blog record);
        ServiceBase Delete(int id);
    }

    public class BlogService : ServiceBase, IBlogService
    {
        public BlogService(BlogDbContext db) : base(db)
        {
        }

        public IQueryable<BlogModel> Query()
        {
            return _db.Blogs
                .Include(b => b.User) // Eager load User (Author)
                .Include(b => b.BlogTags).ThenInclude(bt => bt.Tag) // Eager load Tags
                .OrderByDescending(b => b.PublishDate) // Order by Publish Date
                .ThenBy(b => b.Title) // Secondary order by Title
                .Select(b => new BlogModel
                {
                    Record = b
                });
        }

        public ServiceBase Create(Blog record)
        {
            if (string.IsNullOrWhiteSpace(record.Title))
                return Error("Title is required.");

            if (_db.Blogs.Any(b => b.Title.ToLower() == record.Title.ToLower().Trim()))
                return Error("A blog with the same title already exists.");

            record.Title = record.Title?.Trim();
            record.PublishDate = DateTime.Now; // Automatically set the publish date
            _db.Blogs.Add(record);
            _db.SaveChanges();
            return Success("Blog created successfully.");
        }

        public ServiceBase Update(Blog record)
        {
            if (_db.Blogs.Any(b => b.Id != record.Id && b.Title.ToLower() == record.Title.ToLower().Trim()))
                return Error("A blog with the same title already exists.");

            var entity = _db.Blogs
                .Include(b => b.BlogTags) // Include relational data
                .SingleOrDefault(b => b.Id == record.Id);

            if (entity == null)
                return Error("Blog not found!");            

            // Update fields
            entity.Title = record.Title?.Trim();
            entity.Content = record.Content;
            entity.Rating = record.Rating;
            entity.UserId = record.UserId;
            // Manage relational data (Tags)
            _db.BlogTags.RemoveRange(entity.BlogTags); // Remove existing tags
            entity.BlogTags = record.BlogTags; // Add new tags

            _db.Blogs.Update(entity);
            _db.SaveChanges();
            return Success("Blog updated successfully.");
        }

        public ServiceBase Delete(int id)
        {
            var entity = _db.Blogs
                .Include(b => b.BlogTags) // Include relational data
                .SingleOrDefault(b => b.Id == id);

            if (entity == null)
                return Error("Blog not found!");

            // Remove relational data
            _db.BlogTags.RemoveRange(entity.BlogTags);
            _db.Blogs.Remove(entity);

            _db.SaveChanges();
            return Success("Blog deleted successfully.");
        }
    }
}
