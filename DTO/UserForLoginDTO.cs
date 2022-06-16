using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // =>  [Required] kullanabilmek için

namespace ServerApp.DTO
{
    public class UserForLoginDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]        
        public string Password { get; set; }  

    }
}