using FileTextAnalyzer.Extentions;
using System;
using System.Collections.Generic;
using System.IO;



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
   

   

    
    public class FileAnalysisResult
    {
        public int UniqueWordCount { get; set; }
        public int PunctuationCount { get; set; }
        public List<WordStat> RepeatedWords { get; set; }
    }

    
    public class WordStat
    {
        public string Word { get; set; }
        public int Count { get; set; }

    }

    
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
            catch(Exception ex)
            {
                Console.WriteLine($"Loglama hatası: {ex.Message}");
            }
        }
    }
}
