using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(MemberDto user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<MemberDto>> GetUsersAsync();
        Task<MemberDto> GetUserByIdAsync(int id);
        Task<MemberDto> GetUserByUserNameAsync(string userName);
    }
}