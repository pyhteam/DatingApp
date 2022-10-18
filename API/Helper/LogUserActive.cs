using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helper
{
    public class LogUserActive : IAsyncActionFilter
    {
        // kiem tra user login thi cap nhap lastActive
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            var userId = resultContext.HttpContext.User.GetUserId();
            var ouw = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user = await ouw.UserRepository.GetUserByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await ouw.Complete();

        }
    }
}