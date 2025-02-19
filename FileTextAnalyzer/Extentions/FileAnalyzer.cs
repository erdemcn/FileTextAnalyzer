using FileTextAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileTextAnalyzer.Extentions
{

    #region FileAnalyzer

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

    #endregion

}
