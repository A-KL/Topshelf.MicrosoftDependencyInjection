using Topshelf.Logging;

using Topshelf.ServiceConfigurators;

namespace Topshelf.MicrosoftDependencyInjection
{
    public static class MicrosoftDependencyInjectionConfiguratorExtensions
    {
        public static ServiceConfigurator<T> ConstructUsingServiceProvider<T>(this ServiceConfigurator<T> configurator) where T : class
        {
            var logger = HostLogger.Get(typeof(HostConfiguratorExtensions));

            logger.Info("[Topshelf.MicrosoftDependencyInjection] Service configured to construct using DependencyInjection provider.");

            configurator.ConstructUsing(serviceFactory => (T)MicrosoftDependencyInjectionBuilderConfigurator.ServiceProvider.GetService(typeof(T)));

            return configurator;
        }
    }
}
