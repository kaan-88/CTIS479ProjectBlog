using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL.Controllers.Bases;
using BLL.Services;
using BLL.Models;

// Generated from Custom Template.

namespace MVC.Controllers
{
    public class BlogsController : MvcController
    {
        // Service injections:
        private readonly IBlogService _blogService;
        private readonly IUserService _userService;

        /* Can be uncommented and used for many to many relationships. ManyToManyRecord may be replaced with the related entiy name in the controller and views. */
        //private readonly IManyToManyRecordService _ManyToManyRecordService;

        public BlogsController(
			IBlogService blogService
            , IUserService userService

            /* Can be uncommented and used for many to many relationships. ManyToManyRecord may be replaced with the related entiy name in the controller and views. */
            //, IManyToManyRecordService ManyToManyRecordService
        )
        {
            _blogService = blogService;
            _userService = userService;

            /* Can be uncommented and used for many to many relationships. ManyToManyRecord may be replaced with the related entiy name in the controller and views. */
            //_ManyToManyRecordService = ManyToManyRecordService;
        }

        // GET: Blogs
        public IActionResult Index()
        {
            // Get collection service logic:
            var list = _blogService.Query().ToList();
            return View(list);
        }

        // GET: Blogs/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _blogService.Query().SingleOrDefault(q => q.Record.Id == id);
            return View(item);
        }

        protected void SetViewData()
        {
            // Related items service logic to set ViewData (Record.Id and Name parameters may need to be changed in the SelectList constructor according to the model):
            ViewData["UserId"] = new SelectList(_userService.Query().ToList(), "Record.Id", "Name");
            
            /* Can be uncommented and used for many to many relationships. ManyToManyRecord may be replaced with the related entiy name in the controller and views. */
            //ViewBag.ManyToManyRecordIds = new MultiSelectList(_ManyToManyRecordService.Query().ToList(), "Record.Id", "Name");
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            SetViewData();
            return View();
        }

        // POST: Blogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BlogModel blog)
        {
            if (ModelState.IsValid)
            {
                // Insert item service logic:
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
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _blogService.Query().SingleOrDefault(q => q.Record.Id == id);
            SetViewData();
            return View(item);
        }

        // POST: Blogs/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BlogModel blog)
        {
            if (ModelState.IsValid)
            {
                // Update item service logic:
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
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _blogService.Query().SingleOrDefault(q => q.Record.Id == id);
            return View(item);
        }

        // POST: Blogs/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            var result = _blogService.Delete(id);
            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
	}
}
