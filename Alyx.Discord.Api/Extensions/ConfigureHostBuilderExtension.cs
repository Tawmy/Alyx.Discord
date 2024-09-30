using Alyx.Discord.Core.Requests.InteractionData.Add;
using Alyx.Discord.Core.Requests.InteractionData.Get;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace Alyx.Discord.Api.Extensions;

public static class ConfigureHostBuilderExtension
{
    public static void AddGenericRequestHandlers(this ConfigureHostBuilder host)
    {
        host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        host.ConfigureContainer<ContainerBuilder>(b =>
        {
            // InstancePerDependency() = transient (same lifetime of MediatR services)
            b.RegisterGeneric(typeof(InteractionDataAddRequestHandler<>))
                .AsImplementedInterfaces()
                .InstancePerDependency();
            b.RegisterGeneric(typeof(InteractionDataGetRequestHandler<>))
                .AsImplementedInterfaces()
                .InstancePerDependency();
        });
    }
}