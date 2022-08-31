
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "You must specify a username between 4 and 20 characters")]
        public string UserName { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "You must specify a password between 6 and 20 characters")]
        public string Password { get; set; }

    }
}