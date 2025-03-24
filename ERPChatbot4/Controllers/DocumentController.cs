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

            /* if (file == null || file.Length == 0)
                 return BadRequest("Please upload a valid document.");

             try
             {
                 using (var memoryStream = new MemoryStream())
                 {
                    await file.CopyToAsync(memoryStream);

                     if (memoryStream.Length == 0)
                         return BadRequest("Uploaded document is empty   .");

                     memoryStream.Position = 0; // Reset stream position for reading.

                     var fileExtension = Path.GetExtension(file.FileName).ToLower();

                     if (fileExtension.ToString() == ".pdf")
                     {
                         _documentContent = ExtractTextFromPdf(memoryStream);
                     }
                     else if (fileExtension.ToString() == ".docx")
                     {
                         _documentContent = ExtractTextFromDocx(memoryStream);
                     }
                     else
                     {
                         return BadRequest("Unsupported file format. Please upload a .pdf or .docx file.");
                     }
                 }
             }
             catch (Exception ex)
             {

                 return StatusCode(500, $"Error extracting document content: {ex.Message}");
             }*/

            /* return Ok("Document uploaded successfully.");*/
        }

        [HttpGet("ask")]
        public async Task<IActionResult> AskQuestion([FromQuery] string question)
        {

            var result= await _documentService.AskQuestion(question);
            return Content(result);

           /* if (string.IsNullOrEmpty(_documentContent))
                return BadRequest("Please upload a document first.");

            string pattern = _qaModel.PredictPattern(question);

            if (!string.IsNullOrEmpty(pattern))
            {
                var match = Regex.Match(_documentContent, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return Ok(*//*{question}: *//*$"{match.Groups[1].Value}");
                }
            }

            return Ok("Sorry, I couldn't find the answer in the document.");*/
        }

      /*  // PDF Extraction Using PdfPig
        private string ExtractTextFromPdf(Stream stream)
        {
            using (var reader = PdfDocument.Open(stream))
            {
                var text = new StringBuilder();
                foreach (var page in reader.GetPages())
                {
                    text.AppendLine(page.Text);
                }
                return text.ToString();
            }
        }

        // DOCX Extraction Using Open XML SDK
        private string ExtractTextFromDocx(Stream stream)
        {

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                stream.Position = 0;

                // Check if the stream is non-empty
                if (stream.Length == 0)
                {
                    throw new FileFormatException("The uploaded document is empty.");
                }

                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(memoryStream, false))
                {
                    var body = wordDocument.MainDocumentPart.Document.Body;
                    return body?.InnerText ?? string.Empty;
                }
            }
        }*/
    }
}
