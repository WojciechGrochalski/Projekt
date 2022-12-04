using System.ComponentModel.DataAnnotations;

namespace angularapi.Models
{
    public class AuthModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Pass { get; set; }
    }
}

