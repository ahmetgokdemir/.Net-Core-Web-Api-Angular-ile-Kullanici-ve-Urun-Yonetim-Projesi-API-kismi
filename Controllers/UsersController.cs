using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Data;
using ServerApp.DTO;
using AutoMapper; // ** 

namespace ServerApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly ISocialRepository _repository;
        private readonly IMapper _mapper;

        // depency injection işlemleri..
        public UsersController(ISocialRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }

         // api/users ==> url yazılınca bu fonksiyon çalışacak
        public async Task<IActionResult> GetUsers()
        {
            /*var users = await _repository.GetUsers();
            return Ok(users);
            */

            var users = await _repository.GetUsers();

            /*
            AutoMapper Kütüphanesi sayesinde bu kodlara gerek kalmadı..

            var liste = new List<UserForListDTO>();

            // GetUsers ile çekilen veriler filtrelenecek ve UserForListDTO sınıfına set olunacak.. 
            foreach (var user in users)
            {
                liste.Add(new UserForListDTO() {
                    // istenilen alanlar filtrelenecek..
                    Id = user.Id,
                    UserName = user.UserName,
                    Gender = user.Gender
                });
            }

            return Ok(liste);

            */

            // users IEnumerable old. için List yerine  Map<IEnumerable .. olarak tanımlandı..
            var result = _mapper.Map<IEnumerable<UserForListDTO>>(users);

            return Ok(result);
            
        }


        // api/users/5 ==> url yazılınca bu fonksiyon çalışacak
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repository.GetUser(id);
            
            var result = _mapper.Map<UserForDetailsDTO>(user);

            return Ok(result);
            
            // return Ok(user);
        }
    }
}