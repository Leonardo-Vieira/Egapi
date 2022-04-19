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

            return PartialView(PARTIAL_VIEW_FOLDER + "Login.cshtml", vm);
        }

        public ActionResult HandleLogin()
        {
            return CurrentUmbracoPage();
        }
    }
}
