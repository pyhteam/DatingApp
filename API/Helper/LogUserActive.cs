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
            var username = resultContext.HttpContext.User.GetUsername();
            var userRepo = resultContext.HttpContext.RequestServices.GetService(typeof(IUserRepository)) as IUserRepository;
            var user = await userRepo.GetUserByUserNameAsync(username);
            userRepo.UpdateLastActive(user.Username);
            await userRepo.SaveAllAsync();

        }
    }
}