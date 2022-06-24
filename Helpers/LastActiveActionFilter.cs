using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using ServerApp.Data;
using System;

namespace ServerApp.Helpers
{
    public class LastActiveActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, 
        ActionExecutionDelegate next)
        {
           var resultContext = await next(); // süreç durduruldu..

            var id = int.Parse(resultContext.HttpContext.User // user id bilgisi alındı.. User için oluşturulan contexten user bilgisi alınır..
                .FindFirst(ClaimTypes.NameIdentifier).Value);

            // userscontroller.cs de ISocialRepository inject edildi.. ve repository'i interface üzerinden çağırılır..
            // İnject işlemi yapmadan servis bilgisinin kopyasını alalım

            var repository = (ISocialRepository)resultContext.HttpContext
                        .RequestServices.GetService(typeof(ISocialRepository));

            var user = await repository.GetUser(id);
            user.LastActive = DateTime.Now;
            await repository.SaveChanges();

            // [ServiceFilter(typeof(LastActiveActionFilter))] => userscontroller.cs
            // services.AddScoped<LastActiveActionFilter>(); => startup.cs
        }
    }
}