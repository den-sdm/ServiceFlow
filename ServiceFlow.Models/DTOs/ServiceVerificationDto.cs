namespace ServiceFlow.Models.DTOs;
public class ServiceVerificationDto
{
    public int VerificationID { get; set; }
    public string VerificationType { get; set; } = string.Empty;
    public string ConfigurationJSON { get; set; } = string.Empty;
    public int PollingIntervalSeconds { get; set; }
    public int ThresholdValue { get; set; }
    public int AlertRepeatMinutes { get; set; }
    public bool IsActive { get; set; }
}