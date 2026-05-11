using Microsoft.EntityFrameworkCore;
using ServiceFlow.Data;
using ServiceFlow.Data.Repositories;
using ServiceFlow.Models.DTOs;
using ServiceFlow.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceFlow.Core.Services;

public class MonitoringService : IMonitoringService
{
    private readonly ServiceFlowDbContext _context;
    private readonly IServiceRepository _serviceRepository;
    private readonly IServerRepository _serverRepository;

    public MonitoringService(
        ServiceFlowDbContext context,
        IServiceRepository serviceRepository,
        IServerRepository serverRepository)
    {
        _context = context;
        _serviceRepository = serviceRepository;
        _serverRepository = serverRepository;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var services = await _serviceRepository.GetAllServicesAsync();

        var statusSummaries = new List<ServiceStatusSummaryDto>();

        foreach (var service in services)
        {
            foreach (var verification in service.Verifications)
            {
                var status = verification.ServiceStatus;

                statusSummaries.Add(new ServiceStatusSummaryDto
                {
                    ServiceID = service.ServiceID,
                    ServiceName = service.ServiceName,
                    FriendlyName = service.FriendlyName,
                    Hostname = service.Server.Hostname,
                    CriticalityLevel = service.CriticalityLevel,
                    IsDown = status?.IsDown ?? false,  // Default to false if no status
                    CurrentValue = status?.CurrentValue ?? 0,  // Default to 0
                    ThresholdValue = verification.ThresholdValue,
                    LastCheckTime = status?.LastCheckTime ?? DateTime.UtcNow,  // Default to now
                    DownSince = status?.DownSince,
                    TimeSinceDown = status?.DownSince.HasValue == true
                        ? GetTimeSinceDown(status.DownSince.Value)
                        : null,
                    ErrorMessage = status?.ErrorMessage
                });
            }
        }

        var downServices = statusSummaries.Where(s => s.IsDown).OrderBy(s => s.CriticalityLevel).ToList();

        return new DashboardSummaryDto
        {
            TotalServices = services.Count(),
            ServicesDown = downServices.Count,
            ServicesUp = statusSummaries.Count - downServices.Count,
            CriticalServicesDown = downServices.Count(s => s.CriticalityLevel == 1),
            DownServices = downServices,
            AllServices = statusSummaries.OrderBy(s => s.IsDown ? 0 : 1).ThenBy(s => s.CriticalityLevel).ToList()
        };
    }

    public async Task<ServiceDetailDto?> GetServiceDetailsAsync(int serviceId)
    {
        var service = await _serviceRepository.GetServiceByIdAsync(serviceId);
        if (service == null) return null;

        return MapToServiceDetailDto(service);
    }

    public async Task<ServiceDetailDto> CreateServiceAsync(CreateServiceRequest request)
    {
        var server = await _serverRepository.CreateOrUpdateServerAsync(request.Hostname, null);

        var service = new Service
        {
            ServerID = server.ServerID,
            ServiceName = request.ServiceName,
            FriendlyName = request.FriendlyName,
            Description = request.Description,
            CriticalityLevel = request.CriticalityLevel,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        foreach (var verificationRequest in request.Verifications)
        {
            var verificationType = await _context.VerificationTypes
                .FirstOrDefaultAsync(vt => vt.TypeName == verificationRequest.VerificationType);

            if (verificationType == null)
                throw new InvalidOperationException($"Invalid verification type: {verificationRequest.VerificationType}");

            service.Verifications.Add(new ServiceVerification
            {
                VerificationTypeID = verificationType.VerificationTypeID,
                ConfigurationJSON = verificationRequest.ConfigurationJSON,
                PollingIntervalSeconds = verificationRequest.PollingIntervalSeconds,
                ThresholdValue = verificationRequest.ThresholdValue,
                AlertRepeatMinutes = verificationRequest.AlertRepeatMinutes,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            });
        }

        foreach (var email in request.DistributionEmails)
        {
            service.DistributionLists.Add(new DistributionList
            {
                EmailAddress = email,
                CreatedDate = DateTime.UtcNow
            });
        }

        var createdService = await _serviceRepository.CreateServiceAsync(service);
        var fullService = await _serviceRepository.GetServiceByIdAsync(createdService.ServiceID);
        return MapToServiceDetailDto(fullService!);
    }

    public async Task UpdateServiceAsync(int serviceId, CreateServiceRequest request)
    {
        var service = await _context.Services
            .Include(s => s.DistributionLists)
            .FirstOrDefaultAsync(s => s.ServiceID == serviceId);

        if (service == null)
            throw new InvalidOperationException($"Service {serviceId} not found");

        // Update basic properties
        service.ServiceName = request.ServiceName;
        service.FriendlyName = request.FriendlyName;
        service.Description = request.Description;
        service.CriticalityLevel = request.CriticalityLevel;
        service.ModifiedDate = DateTime.UtcNow;

        // Remove old distribution lists
        var existingLists = service.DistributionLists.ToList();
        foreach (var existing in existingLists)
        {
            _context.DistributionLists.Remove(existing);
        }

        // Add new distribution lists
        foreach (var email in request.DistributionEmails)
        {
            service.DistributionLists.Add(new DistributionList
            {
                ServiceID = serviceId,
                EmailAddress = email,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteServiceAsync(int serviceId)
    {
        await _serviceRepository.DeleteServiceAsync(serviceId);
    }

    private ServiceDetailDto MapToServiceDetailDto(Service service)
    {
        var currentStatus = service.Verifications
            .Where(v => v.ServiceStatus != null && v.ServiceStatus.IsDown)
            .Select(v => new ServiceStatusSummaryDto
            {
                ServiceID = service.ServiceID,
                ServiceName = service.ServiceName,
                FriendlyName = service.FriendlyName,
                Hostname = service.Server.Hostname,
                CriticalityLevel = service.CriticalityLevel,
                IsDown = v.ServiceStatus!.IsDown,
                CurrentValue = v.ServiceStatus.CurrentValue,
                ThresholdValue = v.ThresholdValue,
                LastCheckTime = v.ServiceStatus.LastCheckTime,
                DownSince = v.ServiceStatus.DownSince,
                TimeSinceDown = v.ServiceStatus.DownSince.HasValue
                    ? GetTimeSinceDown(v.ServiceStatus.DownSince.Value)
                    : null,
                ErrorMessage = v.ServiceStatus.ErrorMessage
            })
            .FirstOrDefault();

        return new ServiceDetailDto
        {
            ServiceID = service.ServiceID,
            ServiceName = service.ServiceName,
            FriendlyName = service.FriendlyName,
            Description = service.Description,
            Hostname = service.Server.Hostname,
            CriticalityLevel = service.CriticalityLevel,
            CriticalityLevelName = GetCriticalityLevelName(service.CriticalityLevel),
            IsActive = service.IsActive,
            Verifications = service.Verifications.Select(v => new ServiceVerificationDto
            {
                VerificationID = v.VerificationID,
                VerificationType = v.VerificationType.TypeName,
                ConfigurationJSON = v.ConfigurationJSON,
                PollingIntervalSeconds = v.PollingIntervalSeconds,
                ThresholdValue = v.ThresholdValue,
                AlertRepeatMinutes = v.AlertRepeatMinutes,
                IsActive = v.IsActive
            }).ToList(),
            DistributionList = service.DistributionLists
                .Where(dl => dl.IsActive)
                .Select(dl => dl.EmailAddress)
                .ToList(),
            CurrentStatus = currentStatus
        };
    }

    private string GetCriticalityLevelName(int level)
    {
        return level switch
        {
            1 => "Critical",
            2 => "High",
            3 => "Medium",
            4 => "Low",
            _ => "Unknown"
        };
    }

    private string GetTimeSinceDown(DateTime downSince)
    {
        var duration = DateTime.UtcNow - downSince;

        if (duration.TotalMinutes < 60)
            return $"{(int)duration.TotalMinutes}m";
        if (duration.TotalHours < 24)
            return $"{(int)duration.TotalHours}h {duration.Minutes}m";
        return $"{(int)duration.TotalDays}d {duration.Hours}h";
    }
}