using System.Threading.Channels;
using NGO.Business;
using NGO.Model;

namespace NGO.Web.Infrastructure
{
    public class EmailBackgroundWorker : BackgroundService
    {
        private readonly Channel<EmailDataModel> _emailChannel;
        private readonly IServiceProvider _serviceProvider;
        // Uncomment if you want to add logging
        // private readonly ILogger<EmailBackgroundWorker> _logger;

        public EmailBackgroundWorker(Channel<EmailDataModel> emailChannel, IServiceProvider serviceProvider /*, ILogger<EmailBackgroundWorker> logger*/)
        {
            _emailChannel = emailChannel;
            _serviceProvider = serviceProvider;
            // _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // _logger?.LogInformation("EmailBackgroundWorker started.");

            await foreach (var emailData in _emailChannel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    // Create a scope for each email operation
                    using var scope = _serviceProvider.CreateScope();
                    var smtpEmailProvider = scope.ServiceProvider.GetRequiredService<SMTPEmailProvider>();

                    // _logger?.LogInformation($"Processing email for: {string.Join(", ", emailData.To)}");
                    await smtpEmailProvider.SendAsync(emailData);
                    // _logger?.LogInformation("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    // _logger?.LogError($"Failed to send email: {ex.Message}");
                }
            }
        }
    }
}