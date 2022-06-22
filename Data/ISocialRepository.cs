using System.Collections.Generic;
using System.Threading.Tasks;
using ServerApp.Models;

namespace ServerApp.Data
{
    public interface ISocialRepository
    {   
        void Add<T>(T entity) where T:class;
        void Delete<T>(T entity) where T:class;
        Task<bool> SaveChanges(); // context üzerinde değişiklik yapıldığı zaman kullanılacak method..
        Task<User> GetUser(int id); // id'ye göre User getir..
        Task<IEnumerable<User>> GetUsers(); // tüm User'ları getir..
    }
}