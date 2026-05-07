using System;
using System.Collections.Generic;

namespace ServiceFlow.Models.DTOs;

public class ServiceDto
{
    public int ServiceID { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string FriendlyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public int CriticalityLevel { get; set; }
    public string CriticalityLevelName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}