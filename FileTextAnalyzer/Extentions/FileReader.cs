using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using System;
using System.Text;
using TextFileAnalyzer;
using System.IO;

namespace FileTextAnalyzer.Extentions
{
    # region FileReader 
    public static class FileReader
    {
        public static string ReadFileContent(string filePath)
        {
            try
            {
                string extension = System.IO.Path.GetExtension(filePath);


                switch (extension)
                {
                    case ".txt":
                        return File.ReadAllText(filePath);
                    case ".docx":
                        return ReadDocxFile(filePath);
                    case ".pdf":
                        return ReadPdfFile(filePath);
                    default:
                        throw new NotSupportedException("Desteklenmeyen dosya türü.");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Dosya okunurken hata: {ex.Message}");
                throw; // Hata üst katmana iletilmek üzere yeniden fırlatılır.
            }
        }

        private static string ReadDocxFile(string filePath)
        {
            try
            {
                using (var wordDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(filePath, false))
                {
                    DocumentFormat.OpenXml.Wordprocessing.Body body = wordDoc.MainDocumentPart.Document.Body;
                    return body.InnerText;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($".docx dosyası okunurken hata: {ex.Message}");
                throw;
            }
        }

        private static string ReadPdfFile(string filePath)
        {
            try
            {
                // PdfReader ile PDF dosyasını aç ve her sayfadaki metni çıkar
                StringBuilder text = new StringBuilder();
                using (PdfReader reader = new PdfReader(filePath))
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    }
                }
                return text.ToString();
            }
            catch (Exception ex)
            {
                Logger.Log($"PDF dosyası okunurken hata: {ex.Message}");
                throw;
            }
        }
    }
    #endregion
}
