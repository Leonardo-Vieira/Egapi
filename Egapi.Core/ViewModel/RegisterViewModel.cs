using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egapi.Core.ViewModel
{
    public class RegisterViewModel
    {

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Please enter your First Name")]
        [MinLength(3)]
        public string FirstName { get; set; }
        
        [Display(Name = "Apelido")]
        [Required(ErrorMessage = "Please enter your Last Name")]
        [MinLength(3)]
        public string LastName { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Please enter your Username")]
        [MinLength(3)]
        public string Username { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please enter your Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email address")]
        public string Email { get; set; }

        [UIHint("Password")]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter your Password")]
        [MinLength(10)]
        public string Password { get; set; }

        [UIHint("Confirm Password")]
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Please enter your confirm Password")]
        [EqualTo("Password", ErrorMessage = "The passwords need to match")]
        public string ConfirmPassword { get; set; }

        public string RedirectUrl { get; set; }
    }
}
