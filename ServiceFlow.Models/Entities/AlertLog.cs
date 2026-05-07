using System;

namespace ServiceFlow.Models.Entities;

public class AlertLog
{
    public int AlertID { get; set; }
    public int ServiceID { get; set; }
    public int VerificationID { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public DateTime SentTime { get; set; }
    public string? Recipients { get; set; }
    public string? EmailSubject { get; set; }
    public string? EmailBody { get; set; }
}