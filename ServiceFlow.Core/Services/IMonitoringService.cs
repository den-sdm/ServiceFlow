using ServiceFlow.Models.DTOs;
using System.Threading.Tasks;

namespace ServiceFlow.Core.Services;

public interface IMonitoringService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    Task<ServiceDetailDto?> GetServiceDetailsAsync(int serviceId);
    Task<ServiceDetailDto> CreateServiceAsync(CreateServiceRequest request);
    Task UpdateServiceAsync(int serviceId, CreateServiceRequest request);
    Task DeleteServiceAsync(int serviceId);
}