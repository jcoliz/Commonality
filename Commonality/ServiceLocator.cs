using System;
using System.Collections.Generic;

namespace Commonality
{
    /// <summary>
    /// Service Locator
    /// </summary>
    public static class Service
    {
        /// <summary>
        /// All the services we know about
        /// </summary>
        private static Dictionary<Type, object> RegisteredServices;

        public static void Set<T>(T value) where T : class
        {
            if (RegisteredServices == null)
                RegisteredServices = new Dictionary<Type, object>();

            RegisteredServices[typeof(T)] = value;
        }
        public static T Get<T>() where T : class
        {
            if (RegisteredServices == null || !RegisteredServices.ContainsKey(typeof(T)))
            {
                throw new PlatformNotSupportedException($"Service {typeof(T).Name} not found.");
            }
            return RegisteredServices[typeof(T)] as T;
        }
        public static T TryGet<T>() where T : class
        {
            if (RegisteredServices == null || !RegisteredServices.ContainsKey(typeof(T)))
            {
                return null;
            }
            return RegisteredServices[typeof(T)] as T;
        }

        public static void Clear()
        {
            RegisteredServices.Clear();
        }
    }
}
