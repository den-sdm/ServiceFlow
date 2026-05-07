using System;

namespace ServiceFlow.Models.Entities;

public class ServiceVerification
{
    public int VerificationID { get; set; }
    public int ServiceID { get; set; }
    public int VerificationTypeID { get; set; }
    public string ConfigurationJSON { get; set; } = string.Empty;
    public int PollingIntervalSeconds { get; set; } = 60;
    public int ThresholdValue { get; set; }
    public int AlertRepeatMinutes { get; set; } = 30;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public Service Service { get; set; } = null!;
    public VerificationType VerificationType { get; set; } = null!;
    public ServiceStatus? ServiceStatus { get; set; }
}