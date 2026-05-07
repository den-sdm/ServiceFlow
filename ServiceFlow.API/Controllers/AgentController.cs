using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Core.Services;
using ServiceFlow.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace ServiceFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;

    public AgentController(IAgentService agentService)
    {
        _agentService = agentService;
    }

    [HttpPost("check")]
    public async Task<ActionResult<AgentCheckResponse>> GetTasks([FromBody] AgentCheckRequest request)
    {
        try
        {
            var response = await _agentService.GetTasksForAgentAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] StatusUpdateRequest request)
    {
        try
        {
            await _agentService.UpdateServiceStatusAsync(request);
            return Ok(new { message = "Status updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("heartbeat")]
    public async Task<IActionResult> Heartbeat([FromBody] AgentCheckRequest request)
    {
        try
        {
            await _agentService.UpdateHeartbeatAsync(request.Hostname, request.AgentVersion);
            return Ok(new { message = "Heartbeat received" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}