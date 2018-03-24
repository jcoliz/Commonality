namespace Commonality
{
    /// <summary>
    /// Provides information about the application to components who might like to discover it
    /// </summary>
    public interface IApplicationInfo
    {
        /// <summary>
        /// Title of the app
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Version # of the app
        /// </summary>
        string Version { get; }
    }
}
