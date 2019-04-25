using System;
using System.Collections.Generic;

using Topshelf.Builders;
using Topshelf.Configurators;
using Topshelf.HostConfigurators;

namespace Topshelf.MicrosoftDependencyInjection
{
    internal class MicrosoftDependencyInjectionBuilderConfigurator : HostBuilderConfigurator
    {
        public MicrosoftDependencyInjectionBuilderConfigurator(IServiceProvider provider)
        {
            ServiceProvider = provider;
        }

        public static IServiceProvider ServiceProvider { get; private set; }

        public IEnumerable<ValidateResult> Validate()
        {
            yield break;
        }

        public HostBuilder Configure(HostBuilder builder)
        {
            return builder;
        }
    }
}
