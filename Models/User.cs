using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ServerApp.Models
{
    public class User: IdentityUser<int> // int: id alanı
    {
        //  sıfırdan oluşturmaya gerek yok.. gereksiz property'ler (UserName,Email,Id) silinsin onlar IdentityUser sınıfından gelecektir..
        //  public int Id { get; set; }
        //  public string UserName { get; set; }
        //  public string Email { get; set; }
        public string Name { get; set; }

        public string Gender { get; set; }  

        public DateTime DateOfBirth { get; set; }

        public DateTime Created { get; set; }    

        public DateTime LastActive { get; set; }  

        public string City { get; set; }       

        public string Country { get; set; }

        public string Introduction { get; set; } 

        public string Hobbies { get; set; }   
        public ICollection<Image> Images { get; set; }


        public ICollection<UserToUser> Followings { get; set; } // login olanın takip edilen olduğu liste (takip edilen)
        public ICollection<UserToUser> Followers { get; set; } // login olanın takipçi olduğu liste (takip eden)



    }
}