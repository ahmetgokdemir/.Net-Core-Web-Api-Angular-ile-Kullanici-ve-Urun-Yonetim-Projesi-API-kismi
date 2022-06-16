using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; //*** dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 3.1.4 indirilmese idi hata verirdi bu sayfa..
using ServerApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; //*** IdentityDbContext<User,Role,int> kullanabilmek için

namespace ServerApp.Data
{
    public class SocialContext: IdentityDbContext<User,Role,int>
    {
        public SocialContext(DbContextOptions<SocialContext> options):base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; } // veritabanında tablo olacak..
    }
}