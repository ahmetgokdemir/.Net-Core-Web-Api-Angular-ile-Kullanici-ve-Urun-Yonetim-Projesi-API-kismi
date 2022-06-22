using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Data;
using ServerApp.DTO;
using AutoMapper; // ** 
using System.Security.Claims;

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

        // api/user/id
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDTO model)
        {   
            // token içerisindeki id ile (ClaimTypes.NameIdentifier).Value) güncelllenmek istenen kişinin id'si (int id) kontrol edilmeli ki user başka user'ınkine müdahale edemesin
            // Authcontroller.cs de new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), set edilmişti..
            // ClaimTypes.NameIdentifier).Value => user'ın id bilgisini tutar.. 

            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return BadRequest("not valid request");

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _repository.GetUser(id); // güncellenecek kullanıcı çekilir..

            _mapper.Map(model,user); // güncelleme işlemi

            if (await _repository.SaveChanges())
                return Ok();

            throw new System.Exception("güncelleme sırasında hata oluştu");

        }
    }
}