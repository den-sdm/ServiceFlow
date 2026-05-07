using ServiceFlow.Models.DTOs;
using System.Threading.Tasks;

public interface IAgentService
{
    Task<AgentCheckResponse> GetTasksForAgentAsync(AgentCheckRequest request);
    Task UpdateServiceStatusAsync(StatusUpdateRequest request);
    Task UpdateHeartbeatAsync(string hostname, string agentVersion);
}