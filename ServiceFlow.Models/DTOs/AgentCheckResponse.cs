using System.Collections.Generic;
namespace ServiceFlow.Models.DTOs;

public class AgentCheckResponse
{
    public int ServerID { get; set; }
    public List<VerificationTask> Tasks { get; set; } = new();
}

public class VerificationTask
{
    public int ServiceID { get; set; }
    public int VerificationID { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string VerificationType { get; set; } = string.Empty;
    public string ConfigurationJSON { get; set; } = string.Empty;
    public int ThresholdValue { get; set; }
}