using Egapi.Core.Interfaces;
using Egapi.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Egapi.Core.Controllers
{
    public class ContactUsController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/";
        IEmailService _emailService;
        public ContactUsController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public ActionResult RenderContactUs()
        {
            ContactUsViewModel vm = new ContactUsViewModel();

            return PartialView(PARTIAL_VIEW_FOLDER + "AboutUs.cshtml", vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult HandleContactUs(ContactUsViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Contact Us", "Algo correu mal");
                return CurrentUmbracoPage();
            }

            if (!string.IsNullOrEmpty(vm.Name) && !string.IsNullOrEmpty(vm.Email) && !string.IsNullOrEmpty(vm.Message))
            {
                _emailService.SendEmailToAdminNotification(vm.Name, vm.Email, vm.Message);
            }
            else
            {
                ModelState.AddModelError("Contact Us", "Por favor, preencha todos os campos!");
                return CurrentUmbracoPage();
            }

            TempData["status"] = "OK";

            return RedirectToCurrentUmbracoPage();
        }
    }
}
