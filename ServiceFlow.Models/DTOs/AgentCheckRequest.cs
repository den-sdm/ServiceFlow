namespace ServiceFlow.Models.DTOs;

public class AgentCheckRequest

{
    public string Hostname { get; set; } = string.Empty;
    public string AgentVersion { get; set; } = string.Empty;
}