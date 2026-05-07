using System;
namespace ServiceFlow.Models.DTOs;
public class ServiceStatusSummaryDto
{
    public int ServiceID { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string FriendlyName { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public int CriticalityLevel { get; set; }
    public bool IsDown { get; set; }
    public int CurrentValue { get; set; }
    public int ThresholdValue { get; set; }
    public DateTime LastCheckTime { get; set; }
    public DateTime? DownSince { get; set; }
    public string? TimeSinceDown { get; set; }
    public string? ErrorMessage { get; set; }
}