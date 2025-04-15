using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace VirusChecker
{
    /// <summary>
    /// Менеджер VirusChecker
    /// </summary>
    public class VirusCheckerManager : IVirusChecker
    {
        private const string apiKey = "3937b248f38fbf1b551a1d29ef7476503df38d71e405c7af3ecca93b522acbb7";
        private const string uploadUrl = "https://www.virustotal.com/api/v3/files";

        public async Task<ScanResult> UploadAndAnalyze(string filePath, Action<string> updateStatus)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-apikey", apiKey);

            string sha256 = await GetFileSha256(filePath);
            string fileUrl = $"https://www.virustotal.com/api/v3/files/{sha256}";

            var checkResponse = await client.GetAsync(fileUrl);
            if (checkResponse.IsSuccessStatusCode)
            {
                var fileJson = JObject.Parse(await checkResponse.Content.ReadAsStringAsync());
                var stats = fileJson["data"]?["attributes"]?["last_analysis_stats"];

                if (stats != null)
                {
                    int malicious = (int?)stats["malicious"] ?? 0;
                    int harmless = (int?)stats["harmless"] ?? 0;
                    int suspicious = (int?)stats["suspicious"] ?? 0;
                    int undetected = (int?)stats["undetected"] ?? 0;

                    int total = malicious + harmless + suspicious + undetected;
                    bool isDangerous = malicious > 3;

                    updateStatus("✅ Файл уже существует в базе. Выводим результаты.");
                    return new ScanResult(isDangerous, malicious, total);
                }
            }

            updateStatus("📤 Загрузка файла...");
            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            content.Add(fileContent, "file", Path.GetFileName(filePath));

            var uploadResponse = await client.PostAsync(uploadUrl, content);
            if (!uploadResponse.IsSuccessStatusCode)
            {
                updateStatus($"❌ Ошибка при загрузке файла: {uploadResponse.StatusCode}");
                return new ScanResult(false, 0, 0);
            }

            var uploadJson = JObject.Parse(await uploadResponse.Content.ReadAsStringAsync());
            string? analysisId = uploadJson["data"]?["id"]?.ToString();

            if (string.IsNullOrEmpty(analysisId))
            {
                updateStatus("❌ Ошибка при получении ID анализа.");
                return new ScanResult(false, 0, 0);
            }

            updateStatus("⏳ Файл загружен. Ожидание завершения анализа...");
            return await PollAndAnalysisResult(client, analysisId, sha256, updateStatus);
        }

        public async Task<ScanResult> PollAndAnalysisResult(HttpClient httpClient, string analysisId, string sha256, Action<string> updateStatus)
        {
            var analysisUrl = $"https://www.virustotal.com/api/v3/analyses/{analysisId}";
            var fileUrl = $"https://www.virustotal.com/api/v3/files/{sha256}";

            int attempts = 0;
            int dots = 0;

            while (attempts++ < 100)
            {
                var response = await httpClient.GetAsync(analysisUrl);
                if (!response.IsSuccessStatusCode)
                {
                    updateStatus($"Ошибка анализа: {response.StatusCode}");
                    return new ScanResult(false, 0, 0);
                }

                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                var status = json["data"]?["attributes"]?["status"]?.ToString();

                if (status == "completed")
                {
                    var fileResponse = await httpClient.GetAsync(fileUrl);
                    if (!fileResponse.IsSuccessStatusCode)
                    {
                        updateStatus($"Ошибка при получении информации о файле: {fileResponse.StatusCode}");
                        return new ScanResult(false, 0, 0);
                    }

                    var fileJson = JObject.Parse(await fileResponse.Content.ReadAsStringAsync());
                    var stats = fileJson["data"]?["attributes"]?["last_analysis_stats"];

                    int malicious = (int?)stats?["malicious"] ?? 0;
                    int harmless = (int?)stats?["harmless"] ?? 0;
                    int suspicious = (int?)stats?["suspicious"] ?? 0;
                    int undetected = (int?)stats?["undetected"] ?? 0;

                    int total = malicious + harmless + suspicious + undetected;
                    bool isDangerous = malicious > 3;

                    string url = $"https://www.virustotal.com/gui/file/{sha256}/detection";
                    return new ScanResult(isDangerous, malicious, total);
                }
                else if (status == "queued")
                {
                    dots = (dots % 3) + 1;
                    updateStatus($"⏳ Файл в очереди на анализ{new string('.', dots)} ({attempts}/100)");
                }

                await Task.Delay(1000);
            }

            updateStatus("⏱️ Истекло время ожидания анализа.");
            return new ScanResult(false, 0, 0);
        }

        private async Task<string> GetFileSha256(string filePath)
        {
            using var sha256 = SHA256.Create();
            await using var stream = File.OpenRead(filePath);
            var hash = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}