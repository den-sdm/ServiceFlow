using System;
using System.Collections.Generic;

namespace ServiceFlow.Models.Entities;

public class Service
{
    public int ServiceID { get; set; }
    public int ServerID { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string FriendlyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CriticalityLevel { get; set; } = 2;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public Server Server { get; set; } = null!;
    public ICollection<ServiceVerification> Verifications { get; set; } = new List<ServiceVerification>();
    public ICollection<DistributionList> DistributionLists { get; set; } = new List<DistributionList>();
}