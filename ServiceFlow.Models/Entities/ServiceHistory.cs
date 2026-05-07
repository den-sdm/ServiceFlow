using System;

namespace ServiceFlow.Models.Entities;

public class ServiceHistory
{
    public int HistoryID { get; set; }
    public int ServiceID { get; set; }
    public int VerificationID { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime EventTime { get; set; }
    public int? Value { get; set; }
    public string? ErrorMessage { get; set; }
}