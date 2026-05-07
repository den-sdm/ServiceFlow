namespace ServiceFlow.Models.DTOs;
public class StatusUpdateRequest
{
    public int ServiceID { get; set; }
    public int VerificationID { get; set; }
    public int CurrentValue { get; set; }
    public bool IsDown { get; set; }
    public string? ErrorMessage { get; set; }
}