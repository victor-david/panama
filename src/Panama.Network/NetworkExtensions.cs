using System;
using System.Net.Http;

namespace Restless.Panama.Network
{
    /// <summary>
    /// Provides extension methods for network operations.
    /// </summary>
    public static class NetworkExtensions
    {
        #region Private
        private const string TimeoutOptionsKey = "TimeoutOptionKey";
        private static HttpRequestOptionsKey<TimeSpan> TimeoutKey = new(TimeoutOptionsKey);
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets the timeout for the request
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="timeout">The timeout to set for the request.</param>
        public static void SetTimeout(this HttpRequestMessage request, TimeSpan timeout)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            request.Options.Set(TimeoutKey, timeout);
        }

        /// <summary>
        /// Gets the timeout for the request, or null if not set.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A TimeSpan, or null if no timeout set.</returns>
        public static TimeSpan? GetTimeout(this HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Options.TryGetValue(TimeoutKey, out TimeSpan timeout))
            {
                return timeout;
            }
            return null;
        }
        #endregion
    }
}