using Egapi.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Egapi.Core.Controllers
{
    public class LoginController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/Authentication/";
        public ActionResult RenderLogin()
        {
            LoginViewModel vm = new LoginViewModel();
            vm.RedirectUrl = HttpContext.Request.Url.AbsolutePath;

            return PartialView(PARTIAL_VIEW_FOLDER + "Login.cshtml", vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult HandleLogin(LoginViewModel vm)
        {
            //check the model
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            //check if the member exists
            var member = Services.MemberService.GetByUsername(vm.Username);
            if (member == null)
            {
                ModelState.AddModelError("Log in", "The account does not exist");
                return CurrentUmbracoPage();
            }

            //check if the account is lockedout
            if (member.IsLockedOut)
            {
                ModelState.AddModelError("Log in", "The account is looked");
                return CurrentUmbracoPage();
            }

            //check if the email it's verified
            if (!member.GetValue<bool>("emailVerify"))
            {
                ModelState.AddModelError("Log in", "Please, verify your Email");
                return CurrentUmbracoPage();
            }

            if (!Members.Login(vm.Username, vm.Password))
            {
                ModelState.AddModelError("Log in", "Username or Password not correct");
                return CurrentUmbracoPage();
            }

            if (!string.IsNullOrEmpty(vm.RedirectUrl))
            {
                return Redirect(vm.RedirectUrl);
            }

            return RedirectToCurrentUmbracoPage();
        }
    }
}
