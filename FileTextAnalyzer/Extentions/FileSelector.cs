using System.Windows.Forms;

namespace FileTextAnalyzer.Extentions
{
    public static class FileSelector
    {
        public static string SelectFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Analiz için bir dosya seçin";
                openFileDialog.Filter = "Metin Dosyaları (*.txt)|*.txt|Word Belgeleri (*.docx)|*.docx|PDF Dosyaları (*.pdf)|*.pdf";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
            }
            return null;
        }
    }
}
