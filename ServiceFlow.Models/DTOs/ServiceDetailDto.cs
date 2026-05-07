using System.Collections.Generic;

namespace ServiceFlow.Models.DTOs;

public class ServiceDetailDto : ServiceDto
{
    public List<ServiceVerificationDto> Verifications { get; set; } = new();
    public List<string> DistributionList { get; set; } = new();
    public ServiceStatusSummaryDto? CurrentStatus { get; set; }

}