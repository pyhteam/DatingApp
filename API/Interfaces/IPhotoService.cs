
using API.DTOs;
using API.Entities;
using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
        Task<int> SavePhotoAsync(Photo photo);
        Task<int> Update(int id, PhotoDto photoDto);
        Task<bool> Delete(int id);
    }
}