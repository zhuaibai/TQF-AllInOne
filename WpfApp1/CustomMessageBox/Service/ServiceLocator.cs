using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WpfApp1.CustomMessageBox.Service
{
    public static class ServiceLocator
    {
        private static IServiceProvider _serviceProvider;

        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetService<T>() where T : class
        {
            return _serviceProvider?.GetService(typeof(T)) as T;
        }

        public static void Register<T>(T service) where T : class
        {
            if (_serviceProvider == null)
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddSingleton(typeof(T), service);
                _serviceProvider = serviceCollection.BuildServiceProvider();
            }
        }
    }
}
