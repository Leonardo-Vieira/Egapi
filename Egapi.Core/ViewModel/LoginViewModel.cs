using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egapi.Core.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Por favor insira o seu Username")]
        public string Username { get; set; }

        [UIHint("Password")]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Por favor insira a sua Password")]
        public string Password { get; set; }

        public string RedirectUrl { get; set; }
    }
}
