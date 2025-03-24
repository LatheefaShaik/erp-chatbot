using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;
using DocumentFormat.OpenXml.Packaging;
using System.Text;
using ERPChatbot4.Service;
using ERPChatbot4.MLModels;
using opennlp.tools.ml.maxent.quasinewton;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;

public class DocumentService : IDocumentService
{
    private static string _documentContent = string.Empty;
    private readonly QuestionAnsweringModel _qaModel;

    public DocumentService(QuestionAnsweringModel qaModel)
    {
        _qaModel = qaModel;
    }

    public async Task<string> UploadDocumentAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return ("Please upload a valid document.");

        try
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                if (memoryStream.Length == 0)
                    return ("Uploaded document is empty   .");

                memoryStream.Position = 0; // setting the memory stream position as 0

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
                    return ("Unsupported file format. Please upload a .pdf or .docx file.");
                }
            }
        }
        catch (Exception ex)
        {

            return ($"Error extracting document content: {ex.Message}");
        }

        return ("Document uploaded successfully.");
    }

    // PDF Extraction Using PdfPig
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

            /* using (var   wordDocument = WordprocessingDocument.Open(memoryStream, false))
             {
                 var body = wordDocument.MainDocumentPart.Document.Body;
                 return body?.InnerText ?? string.Empty;
             }*/

            using (var wordDocument = WordprocessingDocument.Open(memoryStream, false))
            {
                var stringBuilder = new StringBuilder();
                foreach (var text in wordDocument.MainDocumentPart.Document.Body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())  // extracting only the text content.
                {
                    stringBuilder.Append(text.Text);
                }

              //  stringBuilder.AppendLine(); // Ensure each paragraph is separated

                return stringBuilder.ToString(); //Converts the collected text from StringBuilder into a final string and returns it.


            }
        }
    }

    public async Task<string> AskQuestion([FromQuery] string question)
    {
        if (string.IsNullOrEmpty(_documentContent))
            return ("Please upload a document first.");

        string pattern = _qaModel.PredictPattern(question);

        if (!string.IsNullOrEmpty(pattern))
        {
            var match = Regex.Match(_documentContent, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
        }

        return ("Sorry, I couldn't find the answer in the document.");
    }

}
   /* public async Task<(bool isSuccess, string message, string? content)> UploadDocumentAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return (false, "Please upload a valid document.", null);

        try
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                if (memoryStream.Length == 0)
                    return (false, "Uploaded document is empty.", null);

                memoryStream.Position = 0; // Reset stream position for reading.

                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                string extractedText = fileExtension switch
                {
                    ".pdf" => ExtractTextFromPdf(memoryStream),
                    ".docx" => ExtractTextFromDocx(memoryStream),
                    _ => null
                };

                if (extractedText == null)
                    return (false, "Unsupported file format. Please upload a .pdf or .docx file.", null);

              //  _cache.Set(CacheKey, _documentContent, TimeSpan.FromMinutes(30));

                return (true, "Document uploaded successfully.", extractedText);
            }
        }
        catch (Exception ex)
        {
            // Log the error (consider using a proper logging framework)
            Console.WriteLine($"Error extracting document content: {ex.Message}");

            return (false, "An error occurred while processing the document.", null);
        }
    }

    // PDF Extraction Using PdfPig
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
            memoryStream.Position = 0;

            // Check if the stream is non-empty
            if (memoryStream.Length == 0)
            {
                throw new FileFormatException("The uploaded document is empty.");
            }

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(memoryStream, false))
            {
                var body = wordDocument.MainDocumentPart.Document.Body;
                return body?.InnerText ?? string.Empty;
            }
        }
    }

    public string AskQuestion(string question)
    {
        if (string.IsNullOrEmpty(_documentContent))
            return "Please upload a document first.";

        string pattern = _qaModel.PredictPattern(question);

        if (!string.IsNullOrEmpty(pattern))
        {
            var match = Regex.Match(_documentContent, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
                return $"{question}: {match.Groups[1].Value}";
        }

        return "Sorry, I couldn't find the answer in the document.";
    }
}
*/