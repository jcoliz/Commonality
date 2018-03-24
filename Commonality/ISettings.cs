using System.Collections.Generic;

namespace Commonality
{
    /// <summary>
    /// Defines a platform-dependent service to retrieve settings
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Get the value for a certain key
        /// </summary>
        /// <param name="key">Name of the key</param>
        /// <returns>Stored value for that key, or null if not found</returns>
        string GetKey(string key); 

        /// <summary>
        /// Set the value for a certain key
        /// </summary>
        /// <param name="key">Name of the key</param>
        /// <param name="value">Value for that key</param>
        void SetKey(string key, string value);

        /// <summary>
        /// Get a collection of values for a certain key
        /// </summary>
        /// <param name="key">Name of the key</param>
        /// <returns>Collection of values found, or empty if not found</returns>
        IEnumerable<string> GetCompositeKey(string key);

        /// <summary>
        /// Set a collection of values for a certain key
        /// </summary>
        /// <param name="key">Name of the key</param>
        /// <param name="values">Collection of values</param>
        void SetCompositeKey(string key, IEnumerable<string> values);
    }
}
