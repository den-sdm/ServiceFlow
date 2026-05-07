using System.Collections.Generic;
namespace ServiceFlow.Models.DTOs;
public class DashboardSummaryDto
{
    public int TotalServices { get; set; }
    public int ServicesDown { get; set; }
    public int ServicesUp { get; set; }
    public int CriticalServicesDown { get; set; }
    public List<ServiceStatusSummaryDto> DownServices { get; set; } = new();
    public List<ServiceStatusSummaryDto> AllServices { get; set; } = new();
}