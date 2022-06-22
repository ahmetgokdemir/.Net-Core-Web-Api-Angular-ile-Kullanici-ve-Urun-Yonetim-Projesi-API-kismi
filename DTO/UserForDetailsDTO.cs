using System;
using System.Collections.Generic;
using ServerApp.Models;

namespace ServerApp.DTO
{
    public class UserForDetailsDTO
    {
      public int Id { get; set; }
      public string Name { get; set; }
      public string UserName { get; set; }
      public string Gender { get; set; }
      public int Age { get; set; }
      public DateTime Created { get; set; }
      public DateTime LastActive { get; set; }
      public string Introduction { get; set; } // DetailDTO sınıfına extra eklenen propertyler...
      public string Hobbies { get; set; } // DetailDTO sınıfına extra eklenen propertyler...
      public string City { get; set; } 
      public string Country { get; set; }

      public string ProfileImageUrl { get; set; } // isProfile true olan gelecek.. MapperProfiles kısmında ayarlaması yapılacak..
      public List<ImagesForDetails> Images { get; set; } // -User property'si olmadan; List olarak tanımlanmasının nedeni Kullanıcının birden fazla resmi olabilir.


    }
}