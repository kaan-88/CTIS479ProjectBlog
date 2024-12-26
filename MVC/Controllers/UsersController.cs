using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BLL.Controllers.Bases;
using BLL.Services.Bases;
using BLL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using BLL.DAL;

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : MvcController
    {
        private readonly IService<User, UserModel> _userService;

        public UsersController(IService<User, UserModel> userService)
        {
            _userService = userService;
        }

        // Allow anyone to access the Login page
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // Allow anyone to post login credentials
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserModel user)
        {
            if (ModelState.IsValid)
            {
                // Authenticate user
                var userModel = _userService.Query().SingleOrDefault(u =>
                    u.Record.UserName == user.Record.UserName &&
                    u.Record.Password == user.Record.Password &&
                    u.Record.IsActive);

                if (userModel != null)
                {
                    // Create claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userModel.Name),
                        new Claim(ClaimTypes.Role, userModel.Role ?? "Viewer"),
                        new Claim("Id", userModel.Record.Id.ToString())
                    };

                    // Create identity and principal
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Sign in
                    await HttpContext.SignInAsync(principal, new AuthenticationProperties
                    {
                        IsPersistent = true // Remember the login session
                    });

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View();
        }

        // Allow anyone to log out
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Users");
        }

        public IActionResult Index()
        {
            var users = _userService.Query().ToList();
            return View(users);
        }

        public IActionResult Details(int id)
        {
            var user = _userService.Query().SingleOrDefault(u => u.Record.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        public IActionResult Create()
        {
            ViewBag.RoleId = new SelectList(_userService.Query().Select(u => u.Record.Role).Distinct(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var result = _userService.Create(user.Record);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message);
            }

            return View(user);
        }

        public IActionResult Edit(int id)
        {
            // Retrieve the user to edit
            var user = _userService.Query().SingleOrDefault(u => u.Record.Id == id);
            if (user == null)
                return NotFound();

            // Populate roles for the dropdown
            ViewBag.RoleId = new SelectList(_userService.Query().Select(u => u.Record.Role).Distinct(), "Id", "Name");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var result = _userService.Update(user.Record);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message);
            }

            // Repopulate roles in case of validation failure
            ViewBag.RoleId = new SelectList(_userService.Query().Select(u => u.Record.Role).Distinct(), "Id", "Name");

            return View(user);
        }

        public IActionResult Delete(int id)
        {
            var user = _userService.Query().SingleOrDefault(u => u.Record.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var result = _userService.Delete(id);
            if (result.IsSuccessful)
            {
                TempData["Message"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
