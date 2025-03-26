using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TikaOnDotNet;
using TikaOnDotNet.TextExtraction;
using System.Configuration;
using iTextSharp.text.pdf.parser;
//using iTextSharp.text.pdf;
using Xceed.Words.NET;
using Path = System.IO.Path;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;
using System.Text.RegularExpressions;
using Microsoft.ML;
using Microsoft.ML.Data;
using ERPChatbot4.MLModels  ;
using opennlp.tools.ml.maxent.quasinewton;
using System.Text;
using ERPChatbot4.Service;

namespace ERPChatbot4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {

       // private static string _documentContent = string.Empty;
        private readonly QuestionAnsweringModel _qaModel;
        private readonly IDocumentService _documentService;

        public DocumentController(QuestionAnsweringModel qaModel, IDocumentService documentService)
        {
            _qaModel = qaModel;
            _documentService = documentService;
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            var result = await _documentService.UploadDocumentAsync(file);

            return Ok(result);
        }

        [HttpGet("ask")]
        public async Task<IActionResult> AskQuestion([FromQuery] string question)
        {

            var result = await _documentService.AskQuestion(question);
            return Content(result);
        }
    }
}
