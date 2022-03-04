namespace Web_App.Services.Contracts
{
    public interface IEmailService
    {
      public Task SendEmailAsync(string from, string to, string subject, string body);
    }
}
