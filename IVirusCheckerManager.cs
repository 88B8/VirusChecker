namespace VirusChecker
{
    /// <summary>
    /// Интерфейс менеджера
    /// </summary>
    public interface IVirusChecker
    {
        /// <summary>
        /// Загружает файл на VirusTotal
        /// </summary>
        public Task<ScanResult> UploadAndAnalyze(string filePath, Action<string> updateStatus);

        /// <summary>
        /// Анализирует результаты с VirusTotal
        /// </summary>
        public Task<ScanResult> PollAndAnalysisResult(HttpClient httpClient, string analysisId, string sha256, Action<string> updateStatus);
    }
}