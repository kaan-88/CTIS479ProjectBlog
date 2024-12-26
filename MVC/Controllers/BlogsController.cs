using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL.Controllers.Bases;
using BLL.Services;
using BLL.Models;
using BLL.Services.Bases;
using BLL.DAL;
using Microsoft.AspNetCore.Authorization;

namespace MVC.Controllers
{
    public class BlogsController : MvcController
    {
        private readonly IBlogService _blogService;
        private readonly IUserService _userService;
        private readonly IService<Tag, TagModel> _tagService;

        public BlogsController(
            IBlogService blogService,
            IUserService userService,
            IService<Tag, TagModel> tagService
        )
        {
            _blogService = blogService;
            _userService = userService;
            _tagService = tagService;
        }

        // GET: Blogs
        [AllowAnonymous]
        public IActionResult Index()
        {
            var list = _blogService.Query().ToList();
            return View(list);
        }

        // GET: Blogs/Details/5
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var item = _blogService.Query().SingleOrDefault(q => q.Record.Id == id);
            return View(item);
        }

        protected void SetViewData()
        {
            ViewData["UserId"] = new SelectList(_userService.Query().ToList(), "Record.Id", "Name");
            ViewBag.TagIds = new MultiSelectList(_tagService.Query().ToList(), "Record.Id", "Name");
        }

        // GET: Blogs/Create
        [Authorize]
        public IActionResult Create()
        {
            SetViewData();
            return View();
        }

        // POST: Blogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(BlogModel blog)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User.FindFirst("Id");
                if (userIdClaim == null)
                {
                    TempData["Message"] = "You must be logged in to create a blog.";
                    return RedirectToAction("Login", "Users");
                }

                blog.Record.UserId = int.Parse(userIdClaim.Value);

                var result = _blogService.Create(blog.Record);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Details), new { id = blog.Record.Id });
                }
                ModelState.AddModelError("", result.Message);
            }

            SetViewData();
            return View(blog);
        }

        // GET: Blogs/Edit/5
        [Authorize]
        public IActionResult Edit(int id)
        {
            var item = _blogService.Query().SingleOrDefault(q => q.Record.Id == id);
            if (item == null) return NotFound();
            SetViewData();
            return View(item);
        }

        // POST: Blogs/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Edit(BlogModel blog)
        {
            if (ModelState.IsValid)
            {
                var result = _blogService.Update(blog.Record);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Details), new { id = blog.Record.Id });
                }
                ModelState.AddModelError("", result.Message);
            }
            SetViewData();
            return View(blog);
        }

        // GET: Blogs/Delete/5
        [Authorize]
        public IActionResult Delete(int id)
        {
            var item = _blogService.Query().SingleOrDefault(q => q.Record.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: Blogs/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult DeleteConfirmed(int id)
        {
            var result = _blogService.Delete(id);
            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
