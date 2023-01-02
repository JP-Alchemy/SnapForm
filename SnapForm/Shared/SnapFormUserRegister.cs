using System;
using System.ComponentModel.DataAnnotations;

namespace SnapForm.Shared
{
    public class SnapFormUserRegister
    {
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsConfirmed { get; set; } = true;
    }
}