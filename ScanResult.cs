namespace VirusChecker
{
    public record ScanResult(bool IsDangerous, int MaliciousCount, int TotalEngines);
}