using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IUserService
    {
        public IQueryable<UserModel> Query();
        public ServiceBase Create(User record);
        public ServiceBase Update(User record);
        public ServiceBase Delete(int id);
    }
    public class UserService : ServiceBase, IUserService
    {
        public UserService(BlogDbContext db) : base(db)
        {
        }
        public IQueryable<UserModel> Query()
        {
            return _db.Users
                    .Include(u => u.Role)
                    .OrderBy(u => u.UserName)
                    .Select(u => new UserModel
                    {
                        Record = u
                    });
            //return _db.Users.OrderBy(u => u.UserName).Select(u => new UserModel() { Record = u });
        }
        public ServiceBase Create(User record)
        {
            if (_db.Users.Any(u => u.UserName.ToUpper() == record.UserName.ToUpper().Trim()))
                return Error("User with the same name exists!");
            record.UserName = record.UserName?.Trim();
            _db.Users.Add(record);
            _db.SaveChanges();
            return Success("User created successfully!");
        }

        public ServiceBase Update(User record)
        {
            if (_db.Users.Any(u => u.Id != record.Id && u.UserName.ToUpper() == record.UserName.ToUpper().Trim()))
                return Error("User with the same name exists!");
            var entity = _db.Users.SingleOrDefault(u => u.Id == record.Id);
            if (entity is null)
                return Error("User can't be found!");
            entity.UserName = record.UserName?.Trim();
            entity.Password = record.Password;
            entity.IsActive = record.IsActive;
            entity.RoleId = record.RoleId;
            _db.Users.Update(entity);
            _db.SaveChanges();
            return Success("User updated successfully!");
        }

        public ServiceBase Delete(int id)
        {
            var entity = _db.Users.Include(u => u.Blogs).SingleOrDefault(u => u.Id == id);
            if (entity is null)
                return Error("User can't be found!");
            if (entity.Blogs.Any())
                return Error("User has relational Blogs!");
            _db.Users.Remove(entity);
            _db.SaveChanges();
            return Success("User deleted successfully!");
        }

    }
}
