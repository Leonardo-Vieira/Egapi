using Egapi.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Egapi.Core.Controllers
{
    public class RegisterController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/";
        public ActionResult RenderRegister()
        {
            var vm = new RegisterViewModel();

            return PartialView(PARTIAL_VIEW_FOLDER + "Register.cshtml", vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult HandleRegister(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return RedirectToCurrentUmbracoPage();
        }
    }
}
