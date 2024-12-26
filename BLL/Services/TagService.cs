using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using System;
using System.Linq;

namespace BLL.Services
{
    public class TagService : ServiceBase, IService<Tag, TagModel>
    {
        public TagService(BlogDbContext db) : base(db)
        {
        }

        public ServiceBase Create(Tag record)
        {
            if (string.IsNullOrWhiteSpace(record.Name))
                return Error("Tag name is required.");

            if (_db.Tags.Any(t => t.Name.ToLower() == record.Name.ToLower().Trim()))
                return Error("A tag with the same name already exists.");

            record.Name = record.Name?.Trim();
            _db.Tags.Add(record);
            _db.SaveChanges();
            return Success("Tag created successfully.");
        }

        public ServiceBase Delete(int id)
        {
            var entity = _db.Tags.SingleOrDefault(t => t.Id == id);
            if (entity == null)
                return Error("Tag not found!");

            if (_db.BlogTags.Any(bt => bt.TagId == id))
                return Error("Tag is associated with one or more blogs and cannot be deleted.");

            _db.Tags.Remove(entity);
            _db.SaveChanges();
            return Success("Tag deleted successfully.");
        }

        public IQueryable<TagModel> Query()
        {
            return _db.Tags
                .OrderBy(t => t.Name)
                .Select(t => new TagModel { Record = t });
        }

        public ServiceBase Update(Tag record)
        {
            var entity = _db.Tags.SingleOrDefault(t => t.Id == record.Id);
            if (entity == null)
                return Error("Tag not found!");

            if (_db.Tags.Any(t => t.Id != record.Id && t.Name.ToLower() == record.Name.ToLower().Trim()))
                return Error("A tag with the same name already exists.");

            entity.Name = record.Name?.Trim();
            _db.Tags.Update(entity);
            _db.SaveChanges();
            return Success("Tag updated successfully.");
        }
    }
}
