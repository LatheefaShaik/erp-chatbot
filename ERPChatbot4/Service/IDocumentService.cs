using Microsoft.AspNetCore.Mvc;

namespace ERPChatbot4.Service
{
    public interface IDocumentService
    {
        Task<string> UploadDocumentAsync(IFormFile file);

        Task<string> AskQuestion([FromQuery] string question);
    }
}
