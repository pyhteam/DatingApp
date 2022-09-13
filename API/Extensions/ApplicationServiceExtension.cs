
using API.Data;
using API.Helper;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {   // add cloudinary settings
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            // Add DI
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            services.AddScoped<IPhotoService, PhotoService>();
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