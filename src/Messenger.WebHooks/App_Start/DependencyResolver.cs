namespace Messenger.WebHooks.App_Start
{
    using System;
    using SimpleInjector;

    public static class DependencyResolver
    {
        public static Container Container { get; private set; }

        public static void SetupContainer(Container container)
        {
            Container = container;
        }

        public static T GetInstance<T>() where T : class
        {
            if (Container == null)
            {
                throw new InvalidOperationException("Cannot resolve dependencies before the container has been initialized.");
            }

            return Container.GetInstance<T>();
        }
    }
}