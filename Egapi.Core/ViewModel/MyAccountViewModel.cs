using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egapi.Core.ViewModel
{
    public class MyAccountViewModel
    {
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Por favor insira o seu Nome")]
        [MinLength(3)]
        public string FirstName { get; set; }

        [Display(Name = "Apelido")]
        [Required(ErrorMessage = "Por favor insira o seu Apelido")]
        [MinLength(3)]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Por favor insira o seu Email")]
        [EmailAddress(ErrorMessage = "Por favor insira um Email válido")]
        public string Email { get; set; }

        [UIHint("Data de Nascimento")]
        [Display(Name = "Data de Nascimento")]
        public DateTime BirthDate { get; set; }

        [UIHint("Password")]
        [Display(Name = "Password")]
        [MinLength(10)]
        public string Password { get; set; }

        [UIHint("Confirmar Password")]
        [Display(Name = "Confirmar Password")]
        [EqualTo("Password", ErrorMessage = "As Passwords precisam de coincidir")]
        [MinLength(10)]
        public string ConfirmPassword { get; set; }
    }
}
