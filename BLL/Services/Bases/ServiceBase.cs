using BLL.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Bases
{
    public abstract class ServiceBase
    {
        public bool IsSuccessful {  get; set; }
        public string Message { get; set; } = string.Empty;
        
        protected readonly BlogDbContext _db;

        protected ServiceBase(BlogDbContext db)
        {
            _db = db;
        }

        public ServiceBase Success(string message = "")
        {
            IsSuccessful = true;
            Message = message;
            return this;
        }

        public ServiceBase Error(string message = "")
        {
            IsSuccessful = false;
            Message = message;
            return this;
        }

    }
}
