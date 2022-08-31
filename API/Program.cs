
using Microsoft.OpenApi.Models;
using API.Extensions;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApplicationServices(builder.Configuration);
// route to lowercase
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
// Add Cors 
builder.Services.AddCors();
// Add Authentication
builder.Services.AddIdentityService(builder.Configuration);
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
    options => options.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()

);
app.UseAuthentication(); // Enable authentication
app.UseAuthorization();

app.MapControllers();

app.Run();
