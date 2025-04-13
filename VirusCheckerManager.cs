using Newtonsoft.Json.Linq;

namespace VirusChecker
{
    public class VirusCheckerManager : IVirusChecker
    {
        private const string apiKey = "3937b248f38fbf1b551a1d29ef7476503df38d71e405c7af3ecca93b522acbb7";
        private const string uploadUrl = "https://www.virustotal.com/api/v3/files";

        public async Task<string> UploadAndAnalyze(string filePath, Action<string> updateStatus)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-apikey", apiKey);

            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            content.Add(fileContent, "file", Path.GetFileName(filePath));

            var response = await client.PostAsync(uploadUrl, content);
            var result = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(result);
            string? analysisId = json["data"]?["id"]?.ToString();

            if (string.IsNullOrEmpty(analysisId))
            {
                updateStatus("Ошибка при загрузке файла");
                return "❌ Не удалось получить идентификатор анализа.";
            }

            updateStatus("Файл загружен.\nОжидание завершения анализа...");
            return await PollAndAnalysisResult(client, analysisId, updateStatus);
        }

        public async Task<string> PollAndAnalysisResult(HttpClient httpClient, string analysisId, Action<string> updateStatus)
        {
            var analysisUrl = $"https://www.virustotal.com/api/v3/analyses/{analysisId}";
            int dots = 0;

            while (true)
            {
                var response = await httpClient.GetAsync(analysisUrl);
                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                var status = json["data"]?["attributes"]?["status"]?.ToString();

                if (status == "completed")
                {
                    var stats = json["data"]?["attributes"]?["stats"];
                    int malicious = (int?)stats?["malicious"] ?? 0;

                    string result = malicious > 3
                        ? "⚠️ Файл потенциально опасен."
                        : "✅ Файл выглядит безопасным.";

                    return result;
                }

                dots = (dots % 3) + 1;
                updateStatus($"Анализируем файл{new string('.', dots)}");
                await Task.Delay(1000);
            }
        }
    }
}