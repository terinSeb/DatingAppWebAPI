using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppCore.Interface
{
   public  interface IPhotoService
    {
        Task<ImageUploadResult> AddPhtoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string PublicId);
    }
}
