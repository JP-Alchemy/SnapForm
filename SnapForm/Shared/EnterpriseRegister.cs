using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapForm.Shared
{
    public class EnterpriseRegister
    {
        public DateTime DateCreated { get; set; } = DateTime.Now;
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(50, MinimumLength = 4)]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "The passwords must match")]
        public string ConfirmPassword { get; set; }
        [StringLength(35, ErrorMessage = "enterprise name is too long, max 35 characters.")]
        public string EnterpriseName { get; set; }
        [StringLength(16, ErrorMessage = "Industry is too long, max 16 characters.")]
        public string EnterpriseIndustry { get; set; }
        [StringLength(16, ErrorMessage = "Firstname is too long, max 16 characters.")]
        public string FirstName { get; set; }
        [StringLength(16, ErrorMessage = "Lastname is too long, max 16 characters.")]
        public string LastName { get; set; }
        public bool IsConfirmed { get; set; } = true;
    }
}
