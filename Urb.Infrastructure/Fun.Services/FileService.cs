using Fun.Application.Fun.IServices;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Infrastructure.Fun.Services
{
    public class FileService: IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
            => _env = env;
        public Task<Stream> OpenReadAsync(string relativePath)
        {

            var fullPath = Path.Combine(_env.WebRootPath, relativePath);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"File not found: {relativePath}");

            return Task.FromResult<Stream>(File.OpenRead(fullPath));
        }
    }
}
