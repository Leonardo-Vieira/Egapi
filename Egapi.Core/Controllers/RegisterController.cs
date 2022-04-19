using Egapi.Core.Interfaces;
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
        private IEmailService _emailService;
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/Authentication/";

        public RegisterController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        #region Registration

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

            //if member with that email exists return to the page
            var memberExists = Services.MemberService.GetByEmail(vm.Email);
            if (memberExists != null)
            {
                ModelState.AddModelError("Register", "There is already someone with that Email");
                return CurrentUmbracoPage();
            }

            //if member with that username exists return to the page
            memberExists = Services.MemberService.GetByUsername(vm.Username);
            if (memberExists != null)
            {
                ModelState.AddModelError("Register", "There is already someone with that Usermame");
                return CurrentUmbracoPage();
            }
            //create member
            var newMember = Services.MemberService.CreateMember(vm.Username, vm.Email, vm.FirstName + " " + vm.LastName, "Member");

            newMember.PasswordQuestion = "";
            newMember.RawPasswordAnswerValue = "";

            //save member (it must be in this order)
            Services.MemberService.Save(newMember);
            Services.MemberService.SavePassword(newMember, vm.Password);

            //assign role
            Services.MemberService.AssignRole(newMember.Id, "Normal User");

            //create email verification token
            var emailToken = Guid.NewGuid().ToString();
            newMember.SetValue("emailVerifyToken", emailToken);
            Services.MemberService.Save(newMember);

            //send email verification
            _emailService.SendVerifyEmailAddressNotification(vm.Email, emailToken);

            TempData["status"] = "OK";

            return RedirectToCurrentUmbracoPage();
        }
        #endregion

        #region Verification

        public ActionResult RenderEmailVerification(string token)
        {
            var member = Services.MemberService.GetMembersByPropertyValue("emailVerifyToken", token).SingleOrDefault();

            if (member != null)
            {
                var alreadyVerify = member.GetValue<bool>("emailVerify");
                if (alreadyVerify)
                {
                    ModelState.AddModelError("Verified", "You already verified your account");
                }
                else
                {
                    member.SetValue("emailVerify", true);
                    member.SetValue("emailVerificationDate", DateTime.Now);
                    Services.MemberService.Save(member);
                }
            }
            else
            {
                ModelState.AddModelError("Error", "There was a problem");
            }
            return PartialView(PARTIAL_VIEW_FOLDER + "EmailVerification.cshtml");
        }

        #endregion
    }
}
