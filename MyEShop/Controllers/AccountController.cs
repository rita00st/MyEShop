using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using MyEShop.Models.Services.Interface;
using MyEShop.Models.ViewModel;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyEShop.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _service;
        public AccountController(IUserService service)
        {
            _service = service;
        }

        #region Rejester
        public IActionResult Rejester()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Rejester(RejesterViewModel rejesterVM)
        {
            if (ModelState.IsValid)
            {
                //if (_service.IsExistUserByEmail(rejesterVM.Email.ToLower()))
                //{
                //    ModelState.AddModelError("Email", "ایمیل وارد شده تکراری است");
                //    return View(rejesterVM);
                //}

                User user = new User()
                {
                    Email = rejesterVM.Email,
                    IsAdmin = false,
                    RejesterDate = DateTime.Now,
                    Password = rejesterVM.Password,
                };
                _service.AddUser(user);
                return View("SuccsessRejester", rejesterVM);
            }
            return View(rejesterVM);
        }


        public IActionResult VerifyEmail(string email)
        {
            if (_service.IsExistUserByEmail(email.ToLower()))
            {
                //ModelState.AddModelError("Email", "ایمیل وارد شده تکراری است");
                return Json($"ایمیل {email} تکراری است");
            }
            return Json(true);
        }
        #endregion

        #region Login
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = _service.GetUserForLogin(loginVM.Email, loginVM.Password);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "اطلاعات وارد شده صحیح نیست");
                    return View(loginVM);
                }

                var claims = new List<Claim>
                {
                new Claim(type: ClaimTypes.NameIdentifier, value: user.Id.ToString()),
                new Claim(type: ClaimTypes.Name, value: user.Email),
                 new Claim("IsAdmin", user.IsAdmin.ToString()),

                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                var properties = new AuthenticationProperties
                {
                    //IsPersistent = loginVM.RememberMe
                    IsPersistent = loginVM.RememberMe
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal, properties);
                return RedirectToAction("Index","Home");
            }

            return View(loginVM);
        }

        #endregion

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
