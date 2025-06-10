using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IPhotoService
    {

        Task<string> UploadPhotoAsync(IFormFile file);
        Task<bool> DeletePhotoAsync(string publicId);

    }
}
