using System.ComponentModel.DataAnnotations;

namespace angularapi.Models
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
