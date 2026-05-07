using Microsoft.EntityFrameworkCore;
using ServiceFlow.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceFlow.Data.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly ServiceFlowDbContext _context;

    public ServiceRepository(ServiceFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetAllServicesAsync()
    {
        return await _context.Services
            .Include(s => s.Server)
            .Include(s => s.Verifications)
                .ThenInclude(v => v.VerificationType)
            .Include(s => s.Verifications)
                .ThenInclude(v => v.ServiceStatus)
            .Include(s => s.DistributionLists)
            .Where(s => s.IsActive)
            .ToListAsync();
    }

    public async Task<Service?> GetServiceByIdAsync(int serviceId)
    {
        return await _context.Services
            .Include(s => s.Server)
            .Include(s => s.Verifications)
                .ThenInclude(v => v.VerificationType)
            .Include(s => s.Verifications)
                .ThenInclude(v => v.ServiceStatus)
            .Include(s => s.DistributionLists)
            .FirstOrDefaultAsync(s => s.ServiceID == serviceId);
    }

    public async Task<Service> CreateServiceAsync(Service service)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task UpdateServiceAsync(Service service)
    {
        service.ModifiedDate = DateTime.UtcNow;
        _context.Services.Update(service);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteServiceAsync(int serviceId)
    {
        var service = await _context.Services.FindAsync(serviceId);
        if (service != null)
        {
            service.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}