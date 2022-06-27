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
                        .AsQueryable(); // tüm kullanıcıları aldıktan sonra filtrelemek doğru değil.. ToListAsync yerine AsQueryable
            
            if(userParams.Followers)
            {
                // takip edenler
                var result = await GetFollows(userParams.UserId, false);
                users = users.Where(u=>result.Contains(u.Id));
            }

            if(userParams.Followings)
            {
                // takip edilenler
                 var result = await GetFollows(userParams.UserId, true);
                users = users.Where(u=>result.Contains(u.Id));
            }

            return await users.ToListAsync(); // bu sorguyu filtrelemeler bittikten sonra en sonunda çalıştırdık..

        }
        
        public async Task<bool> IsAlreadyFollowed(int followerUserId, int userId)
        {
            // UserToUser, SocialContext.cs'de tanımlandı..
            return await _context.UserToUser
                .AnyAsync(i=>i.FollowerId == followerUserId && i.UserId == userId);
        }

        private async Task<IEnumerable<int>> GetFollows(int userId, bool IsFollowings)
        {
            // <int> --> .Select(i=>i.UserId); || .Select(i=>i.FollowerId); kullanıcıların id'lerini geriye döndüreceğiz o yüzden int
            var user = await _context.Users
                                .Include(i=>i.Followers)
                                .Include(i=>i.Followings)
                                .FirstOrDefaultAsync(i=>i.Id==userId);

            if (IsFollowings)
            {
                return user.Followers
                            .Where(i=>i.FollowerId==userId)
                            .Select(i=>i.UserId);
            }
            else
            {
                return user.Followings
                            .Where(i=>i.UserId==userId)
                            .Select(i=>i.FollowerId);
            }
        }


    }
}