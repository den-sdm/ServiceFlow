using Microsoft.EntityFrameworkCore;
using ServiceFlow.Models.Entities;
using System;
using System.Threading.Tasks;

namespace ServiceFlow.Data.Repositories;

public class ServerRepository : IServerRepository
{
    private readonly ServiceFlowDbContext _context;

    public ServerRepository(ServiceFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Server?> GetServerByHostnameAsync(string hostname)
    {
        return await _context.Servers
            .FirstOrDefaultAsync(s => s.Hostname == hostname);
    }

    public async Task<Server> CreateOrUpdateServerAsync(string hostname, string? ipAddress)
    {
        var server = await GetServerByHostnameAsync(hostname);

        if (server == null)
        {
            server = new Server
            {
                Hostname = hostname,
                IPAddress = ipAddress,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            _context.Servers.Add(server);
        }
        else
        {
            if (!string.IsNullOrEmpty(ipAddress))
                server.IPAddress = ipAddress;
            server.ModifiedDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return server;
    }
}