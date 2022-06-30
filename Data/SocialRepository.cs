using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Data
{
    public class SocialRepository : ISocialRepository
    {
        private readonly SocialContext _context; // .net deki gibi context (ef) repository kısmında kullanılır..
        public SocialRepository(SocialContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0; // SaveChangesAsync int türünde değer döner.. sıfırdan büyük değer dönerse güncelleme başarılıdır true döner..
        }

         public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(i=>i.Images)
                                .FirstOrDefaultAsync(i=>i.Id == id);
            return user;

            // Include(i=>i.Images) ile user bilgilerinin karşılık geldiği image bilgileri image tablosundan alınır.. Join işlemi sanırım..
        }

         // api/users?followers=true&gender=male     --> followers=true&gender=male : userParams oluyor
         // parametreleri tek tek göndermek yerine bu parametreleri bir class içerisine almak doğru olur.. UserQueryParams.cs
        public async Task<IEnumerable<User>> GetUsers(UserQueryParams userParams)
        {
            /*
            var users = await _context.Users.Include(i=>i.Images).ToListAsync();
            */
            
            var users = _context.Users
                        .Where(i=>i.Id != userParams.UserId) // login olan kişi, members list'de gözükmeyecek!!
                        .Include(i=>i.Images)
                        .OrderByDescending(i=>i.LastActive) //
                        .AsQueryable(); // tüm kullanıcıları aldıktan sonra filtrelemek doğru değil.. ToListAsync yerine AsQueryable
            
            // api/users?followers=true
            if(userParams.Followers) // kullanıcının takipçileri
            {
                // takip edenler
                var result = await GetFollows(userParams.UserId, false); // .Select(i=>i.FollowerId) takipçilerin id'si
                users = users.Where(u=>result.Contains(u.Id)); // user listesi içerisinden takipçi id'lerine göre filteleme ..
            }

            // api/users?followings=true
            if(userParams.Followings) // kullanıcının takip ettiği kişiler
            {
                // takip edilenler
                 var result = await GetFollows(userParams.UserId, true); // .Select(i=>i.UserId); takip ettiği kişilerin id'si
                users = users.Where(u=>result.Contains(u.Id)); // user listesi içerisinden takip ettiği kişilerin id'lerine göre filteleme ..
            }

            // http://localhost:5000/api/users?Followings=true&gender=female
            if(!string.IsNullOrEmpty(userParams.Gender)) // başında ! var
            {
                users = users.Where(i=>i.Gender == userParams.Gender);
            }

            // http://localhost:5000/api/users?minage=20&maxage=40&gender=male
            
            if(userParams.minAge != 18 || userParams.maxAge != 100) // minAge & maxAge default değerlerinde değilse
            {
                var today = DateTime.Now;
                var min = today.AddYears(-(userParams.maxAge+1)); // min'mum date time, maxAge. yaş için doğum tarihi
                var max = today.AddYears(-userParams.minAge); // max'mum date time, minAge. yaş için doğum tarihi

                users = users.Where(i=>i.DateOfBirth>=min && i.DateOfBirth<=max);
            }

            if(!string.IsNullOrEmpty(userParams.City)) 
            {
                users = users.Where(i=>i.City.ToLower()==userParams.City.ToLower());
            }

            if(!string.IsNullOrEmpty(userParams.Country)) 
            {
                users = users.Where(i=>i.Country.ToLower()==userParams.Country.ToLower());
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                if(userParams.OrderBy == "age")
                {
                    users = users.OrderBy(i=>i.DateOfBirth);
                }
                else if (userParams.OrderBy == "created")
                {
                    users = users.OrderByDescending(i=>i.Created); // hesabı en son oluşturan(büyük tarih) en başta gelecek..
                }
            }

            return await users.ToListAsync(); // bu sorguyu filtrelemeler bittikten sonra en sonunda çalıştırdık..

        }
        
        public async Task<bool> IsAlreadyFollowed(int followerUserId, int userId)
        {
            // UserToUser, SocialContext.cs'de tanımlandı..
            return await _context.UserToUser
                .AnyAsync(i=>i.FollowerId == followerUserId && i.UserId == userId);
        }

        // IsFollowings true ise => takip ettiği/ takip edilenler || false ise => takipçileri
        private async Task<IEnumerable<int>> GetFollows(int userId, bool IsFollowings)
        {
            // <int> --> .Select(i=>i.UserId); || .Select(i=>i.FollowerId); kullanıcıların id'lerini geriye döndüreceğiz o yüzden int
            // Select, IEnumerable<int> döner!!! metotda öyle oldu (Task<IEnumerable<int>>).. 
            var user = await _context.Users
                                .Include(i=>i.Followers)
                                .Include(i=>i.Followings)
                                .FirstOrDefaultAsync(i=>i.Id==userId);

            // takip ettiği
            if (IsFollowings)
            {
                return user.Followers // login olanın takipçi olduğu liste (takip eden)
                            .Where(i=>i.FollowerId==userId) // login olan kişi takipçi durumda
                            .Select(i=>i.UserId); // takip edilenler çekildi..
            }
            else // login olan kişinin takipçileri
            {
                return user.Followings // login olanın takip edilen olduğu liste (takip edilen)
                            .Where(i=>i.UserId==userId) // login olan kişi takip edilen durumda
                            .Select(i=>i.FollowerId); // takipçiler çekildi..
            }
        }


    }
}