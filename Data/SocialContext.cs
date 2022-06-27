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
        public DbSet<Image> Images { get; set; } 
        public DbSet<UserToUser> UserToUser { get; set; }     


        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);

            // birincil anahtar ayarlamaları (iki tane olacak)..
            builder.Entity<UserToUser>()
                .HasKey(k=> new {k.UserId,k.FollowerId});

            // ilişki kurulması
            builder.Entity<UserToUser>()
                .HasOne(l=> l.User) // bir user,takip edilen
                .WithMany(a=>a.Followers) // birden fazla takipçileri
                .HasForeignKey(l=>l.UserId);

            builder.Entity<UserToUser>()
                .HasOne(l=> l.Follower) // bir user,takipçi
                .WithMany(a=>a.Followings) // birden fazla takip edilen , takipçi takip ettiği kişiler 
                .HasForeignKey(l=>l.FollowerId);
        }


    }
}