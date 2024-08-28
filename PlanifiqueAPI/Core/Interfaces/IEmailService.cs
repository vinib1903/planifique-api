namespace PlanifiqueAPI.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string name);
    }
}