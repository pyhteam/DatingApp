using API.Data;
using API.DTOs;
using API.Entities;
using API.Helper;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Interfaces
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PhotoService(IOptions<CloudinarySettings> config, DataContext context, IMapper mapper)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _mapper = mapper;
            _cloudinary = new Cloudinary(account);
            _context = context;
        }



        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);

            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var delete = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(delete);
            return result;
        }

        public async Task<int> Update(int id, PhotoDto photoDto)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return 0;
            }
            photo.IsMain = photoDto.IsMain;
            photo.PublicId = photoDto.PublicId;
            photo.Url = photoDto.Url;
            _context.Photos.Update(photo);
            if (await _context.SaveChangesAsync() > 0)
            {
                return 1;
            }
            return 0;
        }
        public async Task<bool> Delete(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return false;
            }
            _context.Photos.Remove(photo);
            
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<int> SavePhotoAsync(Photo photo)
        {
            _context.Photos.Add(photo);
            return await _context.SaveChangesAsync();
        }
    }
}