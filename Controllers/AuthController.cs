using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;
using Microsoft.AspNetCore.Identity;
using ServerApp.Models;
using ServerApp.DTO;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;  // ** var tokenHandler = new JwtSecurityTokenHandler(); kullanabilmek için tanımlandı..
using Microsoft.Extensions.Configuration; // **  IConfiguration configuration kulanabilmek için tanımlandı..
using System.Text; // ** Encoding kulanabilmek için tanımlandı..
using Microsoft.IdentityModel.Tokens; // ** SecurityTokenDescriptor kulanabilmek için  tanımlandı..
using System.Security.Claims; // ClaimsIdentity kulanabilmek için tanımlandı..

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    // => localhost:5000/api/User  
    public class AuthController: ControllerBase
    {
        private readonly UserManager<User> _userManager; // bu field
        private readonly SignInManager<User> _signInManager;
        public readonly IConfiguration _configuration;

        //public IConfiguration _configurations {get; set;} // bu property

        // constructor (ctor)
        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration; // bunun aracılığıyla appsettings içerisindeki key bilgisi alınabilinir..

            /* SignInManager<User> signInManager */
        }

        [HttpPost("register")]   // => localhost:5000/api/User/register  
        public async Task<ActionResult> Register(UserForRegisterDTO model) // public List<Product> GetProducts()
        {

            if(!ModelState.IsValid){
                return BadRequest(ModelState); //  UserForRegisterDTO.cs içerisindeki data annotationlar kontrol edilir..
            }

            // model.Password burada değil CreateAsync kısmında kullanılıyor..
            // model.UserName & model.Email bunlar User: IdentityUser<int> olduğu için IdentityUser'dan gelir.. ve unique'dirler...
            var user = new User{
                UserName = model.UserName,
                Email = model.Email,
                Name = model.Name,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Country = model.Country,
                City = model.City,
                Created = DateTime.Now,
                LastActive = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password); // Kullanıcı oluşturuldu..

            if(result.Succeeded)
            {
                return StatusCode(201);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]   // => localhost:5000/api/User/login  
        public async Task<ActionResult> Login(UserForLoginDTO model) 
        {
            //throw new Exception("interval exception"); => StartUUp.cs => app.UseExceptionHandler(appError...

            if(!ModelState.IsValid){
                return BadRequest(ModelState); // UserForLoginDTO.cs içerisindeki data annotationlar kontrol edilir..
            }


             var user = await _userManager.FindByNameAsync(model.UserName); // username (FindByNameAsync) veya email add. (FindByEmailAsync) ile böyle bir kullanıcı var mı kontrol edilir..
            
            if (user == null)
            {
                return BadRequest(new { message = "username is incorrect"} ); // 400 kodu
            }

            // Password validation
            /*
            user => hangi user için kontrol yapacak 
            model.Password => parola bilgisi
            false => kullanıcının hatalı giriş yapması durumunda hesabının kilitlenip kilitlenmemesi durumu .. StartUp.cs de ki durumları ezmiş oluruz..
            */
            // Password validation
            var result = await _signInManager.CheckPasswordSignInAsync(user,model.Password,false); 
            
            if (result.Succeeded)
            {
                //login
                return Ok(new {
                    token = GenerateJwtToken(user)
                    //, username = user.UserName

                }); // 200 kodu
            }
             return Unauthorized(); // 401 hatası
        }

        private string GenerateJwtToken(User user)
        {
             var tokenHandler = new JwtSecurityTokenHandler(); // Token oluşturacak yapı.. *1 
             // appsettings.json içerisinden alınacak key ama buna eişebilmek için önce  StartUp.cs'de ki IConfiguration configuration yapıya benzer yapı burada kullanılır ..
             var key =  Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value);

            // token bilgileri..
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    // token içerisinde olacak kısımlar .. 
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)

                }),
                Expires = DateTime.UtcNow.AddDays(1), // token geçerlilik süresi..
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // token şifreleme
            };

            var token = tokenHandler.CreateToken(tokenDescriptor); //1 tokenHandler

             return tokenHandler.WriteToken(token);
        }



    }
}