using System;

namespace ServiceFlow.Models.Entities;

public class AgentHeartbeat
{
    public int HeartbeatID { get; set; }
    public int ServerID { get; set; }
    public DateTime LastHeartbeat { get; set; }
    public string? AgentVersion { get; set; }
    public string? Status { get; set; }

    public Server Server { get; set; } = null!;
}