using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PostsApi.Services.Interfaces;
using System;
using System.IO;

namespace PostsApi.Services.Implementations
{
    public class UserService : IUserService
    {
        public UserService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        private readonly IWebHostEnvironment _webHostEnvironment;
        public string SaveProfileImage(IFormFile formFile)
        {
            string fileFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Assets", "Users", "Avatars");

            string imageFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;

            string folderPathAndImageFileName = Path.Combine(fileFolderPath, imageFileName);

            using (var fileStream = new FileStream(folderPathAndImageFileName, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }

            return imageFileName;
        }
    }
}
