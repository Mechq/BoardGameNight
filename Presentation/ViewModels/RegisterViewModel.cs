using System.ComponentModel.DataAnnotations;
using Domain;

namespace Presentation.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Gender Gender { get; set; } 

        [Required]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        public string? Diet { get; set; }

        public int? AddressId { get; set; }
    }
}