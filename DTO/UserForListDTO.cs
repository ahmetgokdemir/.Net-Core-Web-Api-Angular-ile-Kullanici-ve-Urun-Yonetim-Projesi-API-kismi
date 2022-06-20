using System;
using System.Collections.Generic;
using ServerApp.Models;

namespace ServerApp.DTO
{
    public class UserForListDTO
    {
      /*
      
      Kullanıcı bilgisi alındığı zaman (UsersController.cs) kullanıcı listelendiğinde Kullanıcının bazı propertyleri (introduction, hobbies, => bunlar detay sayfasında (UserForDetailsDTO) gelebilir ) bunun için dto sınıfları kullanılacak..
      
      */
      public int Id { get; set; }
      public string Name { get; set; }
      public string UserName { get; set; }
      public string Gender { get; set; }
      public int Age { get; set; } // kaynakta (User.cs) olmayan property'ler (age) DTO kısmında değersiz olarak gelir..
      public DateTime Created { get; set; }
      public DateTime LastActive { get; set; }
      public string City { get; set; }
      public string Country { get; set; }
      public ImagesForDetails Image { get; set; } // getUsers()'da image:null gelmesinin sebebi... kaynakta (User.cs) image list tipinde ama UserDTO ise Image tipinde dolayısıyla map edemez.. sonra da User bilgisini almamak için Image yerine DTO olan ImagesForDetails kullanılnır..
    }
}