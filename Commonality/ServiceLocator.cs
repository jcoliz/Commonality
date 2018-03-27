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

        /// <summary>
        /// Set the current implmentation of a given service
        /// </summary>
        /// <typeparam name="T">Which kind of service</typeparam>
        /// <param name="value">The current implementation</param>
        public static void Set<T>(T value) where T : class
        {
            if (RegisteredServices == null)
                RegisteredServices = new Dictionary<Type, object>();

            RegisteredServices[typeof(T)] = value;
        }

        /// <summary>
        /// Get the current implmentation of a given service
        /// </summary>
        /// <typeparam name="T">Which kind of service</typeparam>
        /// <returns>The current implementation</returns>
        /// <exception cref="PlatformNotSupportedException">
        /// Thrown if the service is not found
        /// </exception>
        public static T Get<T>() where T : class
        {
            if (RegisteredServices == null || !RegisteredServices.ContainsKey(typeof(T)))
            {
                throw new PlatformNotSupportedException($"Service {typeof(T).Name} not found.");
            }
            return RegisteredServices[typeof(T)] as T;
        }

        /// <summary>
        /// Try to get the current implmentation of a given service
        /// </summary>
        /// <typeparam name="T">Which kind of service</typeparam>
        /// <returns>The current implementation, or null if not found</returns>
        public static T TryGet<T>() where T : class
        {
            if (RegisteredServices == null || !RegisteredServices.ContainsKey(typeof(T)))
            {
                return null;
            }
            return RegisteredServices[typeof(T)] as T;
        }

        /// <summary>
        /// Clear all current implementations
        /// </summary>
        /// <remarks>
        /// Needed for testing
        /// </remarks>
        public static void Clear()
        {
            RegisteredServices = null;
        }
    }
}
