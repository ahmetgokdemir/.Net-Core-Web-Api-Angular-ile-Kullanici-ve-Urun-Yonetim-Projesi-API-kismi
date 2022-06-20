using System.Collections.Generic;
using System.Threading.Tasks;
using ServerApp.Models;

namespace ServerApp.Data
{
    public interface ISocialRepository
    {
        Task<User> GetUser(int id); // id'ye göre User getir..
        Task<IEnumerable<User>> GetUsers(); // tüm User'ları getir..
    }
}