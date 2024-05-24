using Microsoft.Extensions.DependencyInjection;

namespace HomeCollection.Utils
{
    public static class ServiceLocator
    {
        private static IServiceProvider _serviceProvider;

        public static void SetLocatorProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetService<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
