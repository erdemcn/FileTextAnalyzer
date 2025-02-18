using FileTextAnalyzer.Extentions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;



namespace TextFileAnalyzer
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Logger.Log("Uygulama başlatıldı.");

            try
            {
                // Kullanıcıdan dosya seçimi
                string filePath = FileSelector.SelectFile();
                if (string.IsNullOrEmpty(filePath))
                {
                    Logger.Log("Dosya seçilmedi.");
                    Console.WriteLine("Dosya seçilmedi. Uygulama kapatılıyor.");
                    return;
                }

                Logger.Log($"Seçilen dosya: {filePath}");

                // Dosya içeriğini oku
                string content = FileReader.ReadFileContent(filePath);
                if (string.IsNullOrEmpty(content))
                {
                    Logger.Log("Dosya içeriği boş veya okunamadı.");
                    Console.WriteLine("Dosya içeriği boş veya okunamadı.");
                    return;
                }

                // Analiz işlemleri
                FileAnalysisResult result = FileAnalyzer.AnalyzeContent(content);

                // Sonuçların yazdırılması
                Console.WriteLine("=== Analiz Sonuçları ===");
                Console.WriteLine($"Bağlaçlar ve sayılar hariç toplam farklı kelime sayısı: {result.UniqueWordCount}");
                Console.WriteLine("\nEn çok tekrar eden kelimeler:");
                foreach (var wordStat in result.RepeatedWords)
                {
                    Console.WriteLine($"{wordStat.Word} : {wordStat.Count}");
                }
                Console.WriteLine($"\nNoktalama işaretleri sayısı: {result.PunctuationCount}");

                Logger.Log("Analiz başarıyla tamamlandı.");
            }
            catch (Exception ex)
            {
                Logger.Log($"Hata: {ex.Message}");
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }

            Console.WriteLine("\nÇıkmak için herhangi bir tuşa basın...");
            Console.ReadKey();
        }
    }
   

    /// <summary>
    /// Dosya içeriğini okuma işlemlerini yapan sınıf.
    /// </summary>
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

    /// <summary>
    /// Dosya içeriğini analiz eden sınıf.
    /// </summary>
    public static class FileAnalyzer
    {
        private static readonly HashSet<string> ExcludedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "ve", "ile", "de", "da", "ama", "fakat", "lakin", "ki"
    };

        public static FileAnalysisResult AnalyzeContent(string content)
        {
            FileAnalysisResult result = new FileAnalysisResult();

            result.PunctuationCount = content.Count(c => char.IsPunctuation(c));

            var words = Regex.Matches(content, @"\b[\w']+\b")
                             .Cast<Match>()
                             .Select(m => m.Value)
                             .ToList();

            Dictionary<string, int> wordFrequencies = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (string word in words)
            {
                if (double.TryParse(word, out _))
                    continue;

                if (ExcludedWords.Contains(word))
                    continue;

                string lowerWord = word.ToLower();

                if (wordFrequencies.ContainsKey(lowerWord))
                    wordFrequencies[lowerWord]++;
                else
                    wordFrequencies[lowerWord] = 1;
            }

            result.UniqueWordCount = wordFrequencies.Count;

            result.RepeatedWords = wordFrequencies
                .Where(kvp => kvp.Value > 1) // Sadece 1'den fazla geçen kelimeleri dahil et
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => new WordStat { Word = kvp.Key, Count = kvp.Value })
                .ToList();

            return result;
        }
    }


    /// <summary>
    /// Analiz sonuçlarını tutan veri modeli.
    /// </summary>
    public class FileAnalysisResult
    {
        public int UniqueWordCount { get; set; }
        public int PunctuationCount { get; set; }
        public List<WordStat> RepeatedWords { get; set; }
    }

    /// <summary>
    /// Tekrar eden kelime ve tekrar sayısını tutan model.
    /// </summary>
    public class WordStat
    {
        public string Word { get; set; }
        public int Count { get; set; }

    }

    /// <summary>
    /// Uygulama loglama işlemlerini yapan sınıf.
    /// </summary>
    public static class Logger
    {
        private static readonly string logFilePath = "log.txt";

        public static void Log(string message)
        {
            try
            {
                string logEntry = $"{DateTime.Now:G} - {message}";
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch
            {

            }
        }
    }
}
