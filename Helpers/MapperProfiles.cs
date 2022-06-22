using AutoMapper; //** 
using ServerApp.DTO;
using ServerApp.Models;
using System.Linq;

namespace ServerApp.Helpers
{
    public class MapperProfiles: Profile
    {
        public MapperProfiles()
        {
            //CreateMap<User,UserForListDTO>();
            CreateMap<User,UserForListDTO>()
                .ForMember(dest => dest.Image, opt => 
                    opt.MapFrom(src => src.Images.FirstOrDefault(i=>i.IsProfile)))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>src.DateOfBirth.CalculateAge()));
            CreateMap<User,UserForDetailsDTO>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>src.DateOfBirth.CalculateAge()))
                 .ForMember(dest => dest.ProfileImageUrl, opt => 
                    opt.MapFrom(src => src.Images.FirstOrDefault(i=>i.IsProfile).Name))
                .ForMember(dest => dest.Images, opt => 
                    opt.MapFrom(src => src.Images.Where(i=>!i.IsProfile).ToList()));
            CreateMap<Image,ImagesForDetails>(); // Image den ImagesForDetails'e set olunur

            CreateMap<User,UserForUpdateDTO>().ReverseMap(); // CreateMap<UserForUpdateDTO,User>(); ikisi de aynı.. // UserForUpdateDTO dan User'a set olunur


            // dest => UserForListDTO ,  src => User, src.Images.FirstOrDefault => Liste içerisinden tek bir eleman seçilecek, i.IsProfile => true olan kısmı filtreler..

            // kaynakta (User.cs) olmayan property'ler (age) DTO kısmında değersiz olarak gelir.. bunun için .ForMember(dest => dest.Age, opt ..OfBirth.CalculateAge(), kısmı eklendi..

            // dest.Age'e src.DateOfBirth.CalculateAge() set olunur..

            // DateOfBirth.CalculateAge() extension method.. => Helpers => ExtensionMethods.cs kısmında

            /*
             .ForMember(dest => dest.ProfileImageUrl, opt => 
                    opt.MapFrom(src => src.Images.FirstOrDefault(i=>i.IsProfile).Name))

                    IsProfile true olan resim ProfileImageUrl kısmına set olunur.. bunu yapmamızdaki sebep alttaki kodda (.Where(i=>!i.IsProfile).ToList) IsProfile true olan resim çıkarılmasıdır. ekranda kullanabilmek için map işlemi yapıldı..
            
            */
        }
    }
}