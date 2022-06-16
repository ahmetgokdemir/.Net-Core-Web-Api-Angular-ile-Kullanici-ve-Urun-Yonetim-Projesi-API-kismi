using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // =>  [Required] kullanabilmek için

namespace ServerApp.DTO
{
    public class UserForRegisterDTO
    {
        [Required(ErrorMessage ="name gerekli bir alan.")]
        [StringLength(50,MinimumLength =10)]
        public string Name { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Gender { get; set; }  
        [Required]
        public string Password { get; set; }  

    }
}