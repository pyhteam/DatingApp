using API.Data;
using API.DTOs;
using API.Entities;
using API.Helper;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Interfaces
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<MemberDto> GetUserByIdAsync(int id)
        {
            return await _context.Users
            .Include(x => x.Photos)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<MemberDto> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users
            .Include(x => x.Photos)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(x => x.Username == userName);
        }

        public async Task<PagedList<MemberDto>> GetUsersAsync(UserParams userParams)
        {
            var query = _context.Users
            .AsQueryable();
            query = query.Where(x => x.UserName != userParams.CurrentUsername);
            if (!string.IsNullOrEmpty(userParams.Gender))
            {
                query = query.Where(x => x.Gender == userParams.Gender);
            }
            // sort 
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };
            // filter by age
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
            query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .AsNoTracking(), userParams.PageNumber, userParams.PageSize);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(MemberDto user)
        {
            var userFromDb = _context.Users.Find(user.Id);
            userFromDb.City = user.City;
            userFromDb.Country = user.Country;
            userFromDb.Introduction = user.Introduction;
            userFromDb.Interests = user.Interests;
            userFromDb.LookingFor = user.LookingFor;
            _context.Entry(userFromDb).State = EntityState.Modified;


        }

        public void UpdateLastActive(string username)
        {
            var user = _context.Users.SingleOrDefault(x => x.UserName == username);
            user.LastActive = DateTime.Now;
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}