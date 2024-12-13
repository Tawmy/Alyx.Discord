using Microsoft.Extensions.Hosting;

namespace Alyx.Discord.Core.Services;

internal class PostStartupService(ExternalResourceService externalResourceService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await externalResourceService.InitializeAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}