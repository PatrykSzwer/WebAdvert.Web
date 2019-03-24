using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAdvert.Web.Models.Accounts;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;
        private readonly SignInManager<CognitoUser> _signInManager;
        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }

        public async Task<IActionResult> Signup()
        {
            var model = new SignupViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            // This logic shouldn't be in construtor. It's just for presentation/training purposes.
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists");
                    return View(model);
                }
                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
                var createdUser = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                if (createdUser.Succeeded)
                {
                    RedirectToAction("Confirm");
                }
            }

            return View();
        }
    }
}
