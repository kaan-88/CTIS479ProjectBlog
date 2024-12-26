using BLL.Controllers.Bases;
using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace MVC.Controllers
{
    [Authorize]
    public class FavoritesController : MvcController
    {
        const string SESSIONKEY = "Favorites";

        private readonly HttpServiceBase _httpService;
        private readonly IService<Blog, BlogModel> _blogService;

        public FavoritesController(HttpServiceBase httpService, IService<Blog, BlogModel> blogService)
        {
            _httpService = httpService;
            _blogService = blogService;
        }

        private int GetUserId() => Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == "Id").Value);

        private List<FavoritesModel> GetSession(int userId)
        {
            var favorites = _httpService.GetSession<List<FavoritesModel>>(SESSIONKEY);
            return favorites?.Where(f => f.UserId == userId).ToList();
        }
        public IActionResult Get()
        {
            return View("List", GetSession(GetUserId()));
        }

        public IActionResult Remove(int blogId)
        {
            var favorites = GetSession(GetUserId());
            var favoritesItem = favorites.FirstOrDefault(c => c.BlogId == blogId);
            favorites.Remove(favoritesItem);
            _httpService.SetSession(SESSIONKEY, favorites);
            return RedirectToAction(nameof(Get));
        }

        // GET: /Favorites/Add?blogId=17
        public IActionResult Add(int blogId)
        {
            int userId = GetUserId();
            var favorites = GetSession(userId);
            favorites = favorites ?? new List<FavoritesModel>();
            if (!favorites.Any(f => f.BlogId == blogId))
            {
                var blog = _blogService.Query().SingleOrDefault(p => p.Record.Id == blogId);
                var favoritesItem = new FavoritesModel()
                {
                    BlogId = blogId,
                    UserId = userId,
                    BlogTitle = blog.Title
                };
                favorites.Add(favoritesItem);
                _httpService.SetSession(SESSIONKEY, favorites);
                TempData["Message"] = $"\"{blog.Title}\" added to favorites.";
            }
            return RedirectToAction("Index", "Blogs");
        }
    }
}
