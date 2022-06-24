using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerApp.Data;
using Microsoft.AspNetCore.Identity;
using ServerApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer; // **JwtBearerDefaults kullanabilmek için tanımlandı..
using Microsoft.IdentityModel.Tokens; //* TokenValidationParameters kullanabilmek için tanımlandı..
using System.Text; // ** Encoding kulanabilmek için tanımlandı..
using System.Net; // ** HttpStatusCode
using Microsoft.AspNetCore.Diagnostics; // ** IExceptionHandlerFeature
using Microsoft.AspNetCore.Http; // ** WriteAsync
using AutoMapper;
using ServerApp.Helpers;

namespace ServerApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowOrigins = "_myAllowOrigins"; // ** cors name

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SocialContext>(x=>x.UseSqlite("Data Source=social.db")); //** eklendi...

            services.AddIdentity<User,Role>().AddEntityFrameworkStores<SocialContext>();
            
            services.AddScoped<ISocialRepository,SocialRepository>();

            services.Configure<IdentityOptions>(options=> {

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                //options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true; // @,*,_ gibi işaretler 
                options.Password.RequiredLength = 6; // @,*,_ gibi işaretler 

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // hesap kilitlenirse 5dk. giriş yapılanamıyacak..
                options.Lockout.MaxFailedAccessAttempts = 5; // 5 kere hatalı girerse hesap kilitlenecektir..
                options.Lockout.AllowedForNewUsers = true; // hesabı yeni oluşturmuş kişinin hesabı kitlenebilir mi kontrolü

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ "; // username içerisinde olabilecek karakterler..
                options.User.RequireUniqueEmail = true; // kullanıcıların mail adresleri benzersiz olacak..


            });

            services.AddAutoMapper(typeof(Startup)); // ** StartUp.cs'de automapper'ı uygulamaya tanıtmak..

            services.AddControllers().AddNewtonsoftJson(options => {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }); //** AddNewtonsoftJson eklendi..

            // services-add-cor.. diyip enter'a bas ..
            /*
            services-add-services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://*.example.com:5000")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            */
            
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: MyAllowOrigins, // yukarıda tanımlandı..
                    builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
            

            // dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
            // token validation işlemleri / doğruluğunu kontrol etmek..
            services
                .AddAuthentication(x => {

                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer( x =>
                {
                    x.RequireHttpsMetadata = false;      // sadece https ile yapılan isteklerde çalışır..         
                    x.SaveToken = true; // token bilgisi server tarafından kontrol edilsin mi?
                    x.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Secret").Value)),
                    ValidateIssuer = false, // token bilgisini oluşturan kişi 
                    ValidateAudience = false

                };

                });


                  services.AddScoped<LastActiveActionFilter>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,UserManager<User> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                SeedDatabase.Seed(userManager).Wait();
            }
            else{
                app.UseExceptionHandler(appError => {

                    appError.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";

                        var exception = context.Features.Get<IExceptionHandlerFeature>();

                        if(exception != null)
                        {
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = exception.Error.Message


                            }.ToString());
                        }

                    });

                });

            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowOrigins);  // ** cors middleware..

            app.UseAuthentication(); // ** services.Configure<IdentityOptions>(options=> { ... işlemleri sonrası eklendi ..

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
