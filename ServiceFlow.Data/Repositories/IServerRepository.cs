using ServiceFlow.Models.Entities;
using System.Threading.Tasks;

namespace ServiceFlow.Data.Repositories;

public interface IServerRepository
{
    Task<Server?> GetServerByHostnameAsync(string hostname);
    Task<Server> CreateOrUpdateServerAsync(string hostname, string? ipAddress);
}