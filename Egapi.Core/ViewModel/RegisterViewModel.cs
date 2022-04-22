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
        [Required(ErrorMessage = "Por favor insira o seu Nome")]
        [MinLength(3)]
        public string FirstName { get; set; }

        [Display(Name = "Apelido")]
        [Required(ErrorMessage = "Por favor insira o seu Apelido")]
        [MinLength(3)]
        public string LastName { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Por favor insira o seu Username")]
        [MinLength(3)]
        public string Username { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Por favor insira o seu Email")]
        [EmailAddress(ErrorMessage = "Por favor insira um Email válido")]
        public string Email { get; set; }

        [UIHint("Password")]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Por favor insira a sua Password")]
        [MinLength(10)]
        public string Password { get; set; }

        [UIHint("Confirmar Password")]
        [Display(Name = "Confirmar Password")]
        [Required(ErrorMessage = "Por favor confirma a sua Password")]
        [EqualTo("Password", ErrorMessage = "As Passwords precisam de coincidir")]
        public string ConfirmPassword { get; set; }
    }
}
