using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using ServiceFlow.Data;
using ServiceFlow.Data.Repositories;
using ServiceFlow.Models.DTOs;

public class AgentService : IAgentService
{
    private readonly ServiceFlowDbContext _context;
    private readonly IServerRepository _serverRepository;
    private readonly string _connectionString;

    public AgentService(
        ServiceFlowDbContext context,
        IServerRepository serverRepository,
        IConfiguration configuration)
    {
        _context = context;
        _serverRepository = serverRepository;
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<AgentCheckResponse> GetTasksForAgentAsync(AgentCheckRequest request)
    {
        // Register/update server and heartbeat
        var server = await _serverRepository.CreateOrUpdateServerAsync(request.Hostname, null);
        await UpdateHeartbeatAsync(request.Hostname, request.AgentVersion);

        // Get tasks using stored procedure
        using var connection = new SqlConnection(_connectionString);
        var tasks = await connection.QueryAsync<VerificationTask>(
            "ServiceFlow.usp_GetServicesForCheck",
            new { ServerID = server.ServerID },
            commandType: System.Data.CommandType.StoredProcedure
        );

        return new AgentCheckResponse
        {
            ServerID = server.ServerID,
            Tasks = tasks.ToList()
        };
    }

    public async Task UpdateServiceStatusAsync(StatusUpdateRequest request)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "ServiceFlow.usp_UpdateServiceStatus",
            new
            {
                ServiceID = request.ServiceID,
                VerificationID = request.VerificationID,
                CurrentValue = request.CurrentValue,
                IsDown = request.IsDown,
                ErrorMessage = request.ErrorMessage
            },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task UpdateHeartbeatAsync(string hostname, string agentVersion)
    {
        var server = await _serverRepository.GetServerByHostnameAsync(hostname);
        if (server == null) return;

        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "ServiceFlow.usp_UpdateAgentHeartbeat",
            new
            {
                ServerID = server.ServerID,
                AgentVersion = agentVersion,
                Status = "Healthy"
            },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }
}