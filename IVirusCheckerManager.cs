namespace VirusChecker
{
    public interface IVirusChecker
    {
        public Task<string> UploadAndAnalyze(string filePath, Action<string> updateStatus);

        public Task<string> PollAndAnalysisResult(HttpClient httpClient, string analysisId, Action<string> updateStatus);
    }
}