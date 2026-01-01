using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;



namespace ShoeEcommerce.Infrastructure.Services;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RefreshTokenCleanupService> _logger;

    private readonly TimeSpan _cleanupInterval;

    private readonly TimeSpan _retentionPeriod;

    public RefreshTokenCleanupService(
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    ILogger<RefreshTokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        // Read from configuration
        _cleanupInterval = TimeSpan.FromHours(
            configuration.GetValue<int>("RefreshTokenCleanup:CleanupIntervalHours", 24)
        );

        _retentionPeriod = TimeSpan.FromDays(
            configuration.GetValue<int>("RefreshTokenCleanup:RetentionPeriodDays", 30)
        );
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Refresh Token Cleanup Service started");

        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformCleanupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during refresh token cleanup");
            }

            await Task.Delay(_cleanupInterval, stoppingToken);
        }

        _logger.LogInformation("Refresh Token Cleanup Service stopped");
    }

    private async Task PerformCleanupAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting refresh token cleanup...");

        var cutoffDate = DateTime.UtcNow - _retentionPeriod;

        _logger.LogInformation(
            "Deleting tokens revoked or expired before {CutoffDate} ({Days} days ago)",
            cutoffDate,
            _retentionPeriod.TotalDays
        );

        // Create a scope to get scoped services
        using var scope = _serviceProvider.CreateScope();

        // Get repository from scope
        var refreshTokenRepository = scope.ServiceProvider
            .GetRequiredService<IRefreshTokenRepository>();

        // Perform cleanup
        var deletedCount = await refreshTokenRepository.DeleteOldTokensAsync(cutoffDate);

        _logger.LogInformation("Refresh token cleanup completed : Deleted {Count} old tokens" , deletedCount);
    }
}

 /// LOG MESSAGES:
 /// - Service started
 /// - Cleanup started (with cutoff date)
 /// - Cleanup completed
 /// - Errors (if any)
 /// - Service stopped
 /// 
 /// TODO: Add metrics
 /// - Number of tokens deleted --done
 /// - Time taken for cleanup
 /// - Table size before/after