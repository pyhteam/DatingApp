using API.DTOs;
using API.Entities;
using API.Helper;

namespace API.Interfaces
{
    public interface IUserRepository
    {

        void Update(MemberDto user);
        void UpdateLastActive(string Username);
        Task<PagedList<MemberDto>> GetUsersAsync(UserParams userParams);
        Task<MemberDto> GetUserByIdAsync(int id);
        Task<MemberDto> GetUserByUserNameAsync(string userName);
        Task<string> GetUserGender(string userName);
    }
}