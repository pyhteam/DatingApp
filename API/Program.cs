
using Microsoft.OpenApi.Models;
using API.Extensions;
using API.Middleware;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using API.Entities;
using API.SignalR;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApplicationServices(builder.Configuration);



// route to lowercase
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
// Add Cors 
builder.Services.AddCors();
// Add Authentication
builder.Services.AddIdentityService(builder.Configuration);
// Add SignalR
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Dating App API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});



var app = builder.Build();

#region  Seed Data And Migrate
using var scope = builder.Services.BuildServiceProvider().CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    context.Database.Migrate();
    if (await Seed.SeedUsers(userManager, roleManager) <= 0)
    {
        Console.WriteLine("No users seeded");
    }
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>(); // Custom middleware

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(
    options => options.AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true) // allow any origin
        .AllowCredentials() // for signalR
        

);
app.UseAuthentication(); // Enable authentication
app.UseAuthorization();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

app.Run();
