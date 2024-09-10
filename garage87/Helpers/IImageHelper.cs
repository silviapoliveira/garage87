using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace garage87.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}
