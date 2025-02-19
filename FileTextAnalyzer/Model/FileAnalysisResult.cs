using System.Collections.Generic;

namespace FileTextAnalyzer.Model
{
    public class FileAnalysisResult
    {
        public int UniqueWordCount { get; set; }
        public int PunctuationCount { get; set; }
        public List<WordStat> RepeatedWords { get; set; }
    }
}
