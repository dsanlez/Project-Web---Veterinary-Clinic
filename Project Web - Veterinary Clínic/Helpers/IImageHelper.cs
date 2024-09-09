using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}
