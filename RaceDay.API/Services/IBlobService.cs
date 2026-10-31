using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RaceDay.API.Services
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}