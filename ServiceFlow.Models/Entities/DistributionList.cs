using System;
namespace ServiceFlow.Models.Entities;


public class DistributionList
{
    public int ListID { get; set; }
    public int ServiceID { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }

    public Service Service { get; set; } = null!;
}