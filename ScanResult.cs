namespace VirusChecker
{
    /// <summary>
    /// Модель отчета анализа
    /// </summary>
    public record ScanResult(bool IsDangerous, int MaliciousCount, int TotalEngines);
}