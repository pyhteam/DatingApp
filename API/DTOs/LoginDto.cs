using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "You must specify a username")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "You must specify a password")]
        public string Password { get; set; }
    }
}