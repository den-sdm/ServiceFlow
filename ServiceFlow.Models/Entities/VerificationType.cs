using System;
using System.Collections.Generic;
namespace ServiceFlow.Models.Entities;

public class VerificationType
{
    public int VerificationTypeID { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }

    public ICollection<ServiceVerification> ServiceVerifications { get; set; } = new List<ServiceVerification>();
}