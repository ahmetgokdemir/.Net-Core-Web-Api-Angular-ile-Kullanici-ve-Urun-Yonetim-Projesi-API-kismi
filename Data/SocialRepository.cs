using System.Collections.Generic;
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

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.Include(i=>i.Images).ToListAsync();
            return users;
        }
    }
}