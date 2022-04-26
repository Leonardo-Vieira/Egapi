using Egapi.Core.Interfaces;
using Egapi.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Egapi.Core.Controllers
{
    public class UserController : SurfaceController
    {
        private IEmailService _emailService;
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/Authentication/";

        public UserController(IEmailService emailService)
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

        #region My Account
        public ActionResult RenderMyAccount()
        {
            var vm = new MyAccountViewModel();

            //if is logged on
            if (Umbraco.MemberIsLoggedOn())
            {
                var member = Services.MemberService.GetByUsername(Membership.GetUser().UserName);
                if (member == null)
                {
                    ModelState.AddModelError("My Account", "Não o conseguimos encontrar no sistema");
                    return CurrentUmbracoPage();
                }

                int firstNamePosition = member.Name.LastIndexOf(" ") + 1;
                string firstName = member.Name.Substring(0, firstNamePosition);
                vm.FirstName = firstName;

                int lastNamePosition = member.Name.IndexOf(" ") + 1;
                string lastName = member.Name.Substring(lastNamePosition);
                vm.LastName = lastName;

                vm.Email = member.Email;
                vm.BirthDate = member.GetValue<DateTime>("BirthDate");

                return PartialView(PARTIAL_VIEW_FOLDER + "Members Only/MyAccount.cshtml", vm);
            }
            return CurrentUmbracoPage();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleUpdateMyAccount(MyAccountViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            //is the member logged in?
            if (!Umbraco.MemberIsLoggedOn())
            {
                ModelState.AddModelError("My Account", "Você não tem a sessão iniciada!");
                return CurrentUmbracoPage();
            }

            if (vm == null)
            {
                return CurrentUmbracoPage();
            }

            var member = Services.MemberService.GetByUsername(Membership.GetUser().UserName);
            if (member == null)
            {
                ModelState.AddModelError("My Account", "Não o conseguimos encontrar no sistema");
                return CurrentUmbracoPage();
            }

            string password = member.GetValue<string>("Password");
            if (!string.IsNullOrEmpty(vm.Password) || !string.IsNullOrEmpty(vm.ConfirmPassword))
            {
                password = vm.Password;
            }

            member.Name = vm.FirstName.Trim() + " " + vm.LastName.Trim();
            member.Email = vm.Email;

            Services.MemberService.Save(member);
            if (!string.IsNullOrEmpty(vm.Password) || !string.IsNullOrEmpty(vm.ConfirmPassword))
            {
                Services.MemberService.SavePassword(member, password);
            }

            return RedirectToCurrentUmbracoPage();
        }

        public ActionResult HandleUpdatePassword(MyAccountViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            //is the member logged in?
            if (!Umbraco.MemberIsLoggedOn())
            {
                ModelState.AddModelError("My Account", "Você não tem a sessão iniciada!");
                return CurrentUmbracoPage();
            }

            if (vm == null)
            {
                return CurrentUmbracoPage();
            }

            var member = Services.MemberService.GetByUsername(Membership.GetUser().UserName);
            if (member == null)
            {
                ModelState.AddModelError("My Account", "Não o conseguimos encontrar no sistema");
                return CurrentUmbracoPage();
            }

            try
            {
                Services.MemberService.SavePassword(member, vm.Password);
            }
            catch (Exception e)
            {

                ModelState.AddModelError("My Account", "Não deu para atualizar a password " + e.Message);
                return CurrentUmbracoPage();
            }

            return RedirectToCurrentUmbracoPage();
        }
        #endregion
    }
}
