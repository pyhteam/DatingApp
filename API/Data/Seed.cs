using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static int SeedUsers(DataContext dataContext)
        {
            if (dataContext.Users.Any())
            {
                return 0;
            }
            var userData = System.IO.File.ReadAllText(@"Data\UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("123456"));
                user.PasswordSalt = hmac.Key;

                dataContext.Users.Add(user);
            }

            return dataContext.SaveChanges();
        }
    }
}