using Microsoft.AspNetCore.Mvc;

namespace ERPChatbot4.Service
{
    public interface IDocumentService
    {
        Task<string> UploadDocumentAsync(IFormFile file);

        Task<string> AskQuestion([FromQuery] string question);

       /* Task<(bool isSuccess, string message, string? content)> UploadDocumentAsync(IFormFile file);
        string AskQuestion(string question);*/
    }
}
