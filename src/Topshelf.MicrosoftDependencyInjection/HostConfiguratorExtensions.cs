using System;

using Topshelf.Logging;
using Topshelf.HostConfigurators;

namespace Topshelf.MicrosoftDependencyInjection
{
    public static class HostConfiguratorExtensions
    {
        public static HostConfigurator UseServiceProvider(this HostConfigurator configurator, IServiceProvider provider)
        {
            var logWriter = HostLogger.Get(typeof(HostConfiguratorExtensions));

            logWriter.Info("[MicrosoftDependencyInjection] Integration Started in host.");

            configurator.AddConfigurator(new MicrosoftDependencyInjectionBuilderConfigurator(provider));

            return configurator;
        }
    }
}
