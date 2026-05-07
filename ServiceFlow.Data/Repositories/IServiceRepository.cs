using ServiceFlow.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceFlow.Data.Repositories;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetAllServicesAsync();
    Task<Service?> GetServiceByIdAsync(int serviceId);
    Task<Service> CreateServiceAsync(Service service);
    Task UpdateServiceAsync(Service service);
    Task DeleteServiceAsync(int serviceId);
}