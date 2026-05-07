using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Core.Services;
using ServiceFlow.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IMonitoringService _monitoringService;

    public ServicesController(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    // GET /api/services (for React dashboard)
    [HttpGet]
    public async Task<ActionResult> GetAllServicesForWeb()
    {
        try
        {
            var dashboard = await _monitoringService.GetDashboardSummaryAsync();

            // Transform to format expected by React app
            var webFormat = dashboard.AllServices.Select(s => new
            {
                id = s.ServiceID,
                name = s.FriendlyName ?? s.ServiceName,
                serverHostname = s.Hostname,
                status = s.IsDown ? 3 : 1,
                currentValue = s.CurrentValue,
                thresholdValue = s.ThresholdValue,
                pollingSeconds = 60,
                lastChecked = s.LastCheckTime
            }).ToList();

            return Ok(webFormat);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // GET /api/services/dashboard
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardSummaryDto>> GetDashboard()
    {
        try
        {
            var summary = await _monitoringService.GetDashboardSummaryAsync();
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // GET /api/services/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDetailDto>> GetService(int id)
    {
        try
        {
            var service = await _monitoringService.GetServiceDetailsAsync(id);
            if (service == null)
                return NotFound(new { error = $"Service {id} not found" });

            return Ok(service);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // POST /api/services (for React Settings form)
    [HttpPost]
    public async Task<ActionResult> CreateServiceForWeb([FromBody] JsonElement request)
    {
        try
        {
            var serviceName = request.GetProperty("serviceName").GetString() ?? "";
            var serverHostname = request.GetProperty("serverHostname").GetString() ?? "";
            var thresholdValue = request.TryGetProperty("thresholdValue", out var tv) ? tv.GetInt32() : 100;
            var pollingSeconds = request.TryGetProperty("pollingSeconds", out var ps) ? ps.GetInt32() : 60;
            var criticalityLevel = request.TryGetProperty("criticalityLevel", out var cl) ? cl.GetInt32() : 2;
            var alertEmails = request.TryGetProperty("alertEmails", out var ae) ? ae.GetString() ?? "" : "";
            var configJson = request.TryGetProperty("configJson", out var cj) ? cj.GetString() ?? "{}" : "{}";

            var config = JsonDocument.Parse(configJson);
            var uncPath = config.RootElement.TryGetProperty("UncPath", out var up) ? up.GetString() ?? "" : "";
            var filePattern = config.RootElement.TryGetProperty("FilePattern", out var fp) ? fp.GetString() ?? "*.*" : "*.*";

            var verifications = new List<CreateVerificationRequest>();
            verifications.Add(new CreateVerificationRequest
            {
                VerificationType = "FileProcessing",
                ConfigurationJSON = $"{{\"FolderPath\":\"{uncPath}\",\"FilePattern\":\"{filePattern}\",\"CheckType\":\"FileCount\"}}",
                PollingIntervalSeconds = pollingSeconds,
                ThresholdValue = thresholdValue,
                AlertRepeatMinutes = 30
            });

            var emails = new List<string>();
            if (!string.IsNullOrEmpty(alertEmails))
            {
                foreach (var email in alertEmails.Split(','))
                {
                    var trimmed = email.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        emails.Add(trimmed);
                    }
                }
            }

            var createRequest = new CreateServiceRequest
            {
                Hostname = serverHostname,
                ServiceName = serviceName,
                FriendlyName = serviceName,
                Description = $"File monitoring for {uncPath}",
                CriticalityLevel = criticalityLevel,
                Verifications = verifications,
                DistributionEmails = emails
            };

            var service = await _monitoringService.CreateServiceAsync(createRequest);
            return Ok(new { success = true, serviceId = service.ServiceID });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // PUT /api/services/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] CreateServiceRequest request)
    {
        try
        {
            await _monitoringService.UpdateServiceAsync(id, request);
            return Ok(new { message = "Service updated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // DELETE /api/services/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        try
        {
            await _monitoringService.DeleteServiceAsync(id);
            return Ok(new { message = "Service deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}