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
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>src.DateOfBirth.CalculateAge()));
            CreateMap<Image,ImagesForDetails>();

            // dest => UserForListDTO ,  src => User, src.Images.FirstOrDefault => Liste içerisinden tek bir eleman seçilecek, i.IsProfile => true olan kısmı filtreler..

            // kaynakta (User.cs) olmayan property'ler (age) DTO kısmında değersiz olarak gelir.. bunun için .ForMember(dest => dest.Age, opt .. kısmı eklenir..

            // dest.Age'e src.DateOfBirth.CalculateAge() set olunur..

            // DateOfBirth.CalculateAge() extension method.. => Helpers => ExtensionMethods.cs kısmında
        }
    }
}