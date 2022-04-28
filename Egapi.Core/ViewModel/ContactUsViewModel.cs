using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egapi.Core.ViewModel
{
    public class ContactUsViewModel
    {
        [UIHint("Nome")]
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Por favor insira o seu Nome")]
        public string Name { get; set; }

        [UIHint("Email")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Por favor insira o seu Email")]
        [EmailAddress(ErrorMessage = "Por favor insira um Email válido")]
        public string Email { get; set; }

        [UIHint("Menssagem")]
        [Display(Name = "Menssagem")]
        [Required(ErrorMessage = "Por favor insira a Menssagem")]
        public string Message { get; set; }
    }
}
