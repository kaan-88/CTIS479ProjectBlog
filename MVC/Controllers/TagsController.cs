using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using BLL.Controllers.Bases;
using BLL.Services.Bases;
using BLL.Services;
using BLL.DAL;
using BLL.Models;
using System.Linq;

// Generated from Custom Template.

namespace MVC.Controllers
{
    [Authorize]
    public class TagsController : MvcController
    {
        // Service injections:
        private readonly IService<Tag, TagModel> _tagService;

        /* 
         * Can be uncommented and used for many-to-many relationships. 
         * "ManyToManyRecord" may be replaced with the related entity name in the controller and views.
         */
        // private readonly IService<ManyToManyRecord, ManyToManyRecordModel> _manyToManyRecordService;

        public TagsController(
            IService<Tag, TagModel> tagService
        //, IService<ManyToManyRecord, ManyToManyRecordModel> manyToManyRecordService
        )
        {
            _tagService = tagService;
            // _manyToManyRecordService = manyToManyRecordService;
        }

        // GET: Tags
        [AllowAnonymous]
        public IActionResult Index()
        {
            // Get collection service logic:
            var list = _tagService.Query().ToList();
            return View(list);
        }

        // GET: Tags/Details/5
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _tagService.Query().SingleOrDefault(q => q.Record.Id == id);
            return View(item);
        }

        protected void SetViewData()
        {
            // Single-select dropdown for tags (if applicable, adjust based on your model needs)
            ViewData["TagId"] = new SelectList(_tagService.Query().ToList(), "Record.Id", "Name");

            // Multi-select dropdown for tags (if many-to-many relationship applies)
            ViewBag.TagIds = new MultiSelectList(_tagService.Query().ToList(), "Record.Id", "Name");
        }


        // GET: Tags/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            SetViewData();
            return View();
        }

        // POST: Tags/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(TagModel tag)
        {
            if (ModelState.IsValid)
            {
                // Insert item service logic:
                var result = _tagService.Create(tag.Record);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Details), new { id = tag.Record.Id });
                }
                ModelState.AddModelError("", result.Message);
            }

            SetViewData();
            return View(tag);
        }

        // GET: Tags/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _tagService.Query().SingleOrDefault(q => q.Record.Id == id);
            SetViewData();
            return View(item);
        }

        // POST: Tags/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(TagModel tag)
        {
            if (ModelState.IsValid)
            {
                // Update item service logic:
                var result = _tagService.Update(tag.Record);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Details), new { id = tag.Record.Id });
                }
                ModelState.AddModelError("", result.Message);
            }

            SetViewData();
            return View(tag);
        }

        // GET: Tags/Delete/5
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _tagService.Query().SingleOrDefault(q => q.Record.Id == id);
            return View(item);
        }

        // POST: Tags/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            var result = _tagService.Delete(id);
            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
