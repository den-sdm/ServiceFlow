namespace ServiceFlow.Models.DTOs;
public class ServiceVerificationRequest
{
    public string VerificationType { get; set; } = string.Empty;
    public string ConfigurationJSON { get; set; } = string.Empty;
    public int PollingIntervalSeconds { get; set; } = 60;
    public int ThresholdValue { get; set; }
    public int AlertRepeatMinutes { get; set; } = 30;
}