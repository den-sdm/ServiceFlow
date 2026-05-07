using System;
using System.Collections.Generic;

namespace ServiceFlow.Models.Entities;

public class Server
{
    public int ServerID { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public string? IPAddress { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastHeartbeat { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public ICollection<Service> Services { get; set; } = new List<Service>();
    public AgentHeartbeat? AgentHeartbeat { get; set; }
}