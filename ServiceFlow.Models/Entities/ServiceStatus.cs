using System;

namespace ServiceFlow.Models.Entities;

public class ServiceStatus
{
    public int StatusID { get; set; }
    public int ServiceID { get; set; }
    public int VerificationID { get; set; }
    public int CurrentValue { get; set; }
    public bool IsDown { get; set; }
    public DateTime LastCheckTime { get; set; }
    public DateTime? DownSince { get; set; }
    public string? ErrorMessage { get; set; }

    public Service Service { get; set; } = null!;
    public ServiceVerification Verification { get; set; } = null!;
}