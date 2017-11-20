using System;
using Autofac;

namespace GenericAccess.Data.Common
{
    public static class ServiceLocator
    {
        private static Func<ILifetimeScope> _container;

        public static void SetResolver(Func<ILifetimeScope> container)
        {
            _container = container;
        }

        public static TService GetService<TService>()
        {
            return _container().Resolve<TService>();
        }

        public static object GetService(Type type)
        {
            return _container().Resolve(type);
        }

        public static TService GetByKey<TService>(object key)
        {
            return _container().ResolveKeyed<TService>(key);
        }
    }
}
