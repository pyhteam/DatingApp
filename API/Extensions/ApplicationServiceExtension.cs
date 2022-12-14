
using API.Data;
using API.Helper;
using API.Interfaces;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {   // add cloudinary settings
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            // Add DI
            services.AddSingleton<PresenceTracker>(); // add singleton for presence tracker
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<LogUserActive>();
            services.AddScoped<IPhotoService, PhotoService>();
          
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);


            // Add ConnectionString
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")
            ));
            return services;
        }
    }
}