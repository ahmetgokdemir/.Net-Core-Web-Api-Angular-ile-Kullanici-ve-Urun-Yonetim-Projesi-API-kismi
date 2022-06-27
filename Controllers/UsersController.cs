using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Data;
using ServerApp.DTO;
using AutoMapper; // ** 
using System.Security.Claims;
using ServerApp.Helpers;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [ServiceFilter(typeof(LastActiveActionFilter))]
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
        public async Task<IActionResult> GetUsers([FromQuery]UserQueryParams userParams)
        {
            /*var users = await _repository.GetUsers();
            return Ok(users);
            */

            // query parametrelerini UserQueryParams class'a atadık.. extra parametreleri bu class sayesinde taşımak kolay..
            userParams.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var users = await _repository.GetUsers(userParams);

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

        // FollowUser methodu: takip olayı
        // int followerUserId: hesabı takip edecek olan kişi: login olan kullanıcı
        // userId: takip edilecek olan kişi..
        [HttpPost("{followerUserId}/follow/{userId}")] // api/users/1/follow/2 => 1, 2'yi takip edecek.. 
        // (*1) 1, 1'i takip edecek denince uygulama hata vermeli..
        // 1, 30'u takip edecek denince 30 no'lu id olmadığı için uygulama hata vermeli
        // (*3) id'si 1 olan kişi kalkıp id'si 2 olan kişiyi başka kullanıcıyı takip etmesini engellemeliyiz. token ile.. login olan kullanıcı işlem yapabilir..
        public async Task<IActionResult> FollowUser(int followerUserId, int userId)
        {
            // *3 kontrolü sağladık
            if (followerUserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized(); 

            // *1 kontrolü sağladık
            if(followerUserId == userId)
                return BadRequest("Kendizi takip edemezsiniz");

            // Takip etmek istenen kişi daha önceden takip listesine alınmışsa..
            var IsAlreadyFollowed = await _repository
                .IsAlreadyFollowed(followerUserId,userId); // IsAlreadyFollowed metodu, ISocialRepository.cs'de

            if(IsAlreadyFollowed) 
                return BadRequest("Zaten kullanıcıyı takip ediyorsunuz");

            // Takip etmek istenilen user id si veri tabanında yoksa (öyle bir kullanıcı yoktur..)
            if (await _repository.GetUser(userId) == null) 
                return NotFound();

            // eğer hiç bir hata yoksa entity oluşturulur..
            var follow = new UserToUser() {
                UserId = userId,
                FollowerId = followerUserId
            };

            _repository.Add<UserToUser>(follow); // Add methodunu generic olarak oluşturlmuştu.. isocialrepository.cs'de

            if(await _repository.SaveChanges())
                return Ok();

            return BadRequest("Hata Oluştu");
                    
        }


    }
}