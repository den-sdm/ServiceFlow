using System.Collections.Generic;

namespace ServiceFlow.Models.DTOs;

public class CreateServiceRequest
{
    public string Hostname { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string FriendlyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CriticalityLevel { get; set; }
    public List<CreateVerificationRequest> Verifications { get; set; } = new();
    public List<string> DistributionEmails { get; set; } = new();
}