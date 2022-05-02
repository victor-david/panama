﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Restless.Panama.Network
{
    /// <summary>
    /// Manages network requests.
    /// </summary>
    public class NetworkManager
    {
        #region Private
        private HttpClient client;
        #endregion

        /************************************************************************/

        #region Public fields
        /// <summary>
        /// Defines the content type for a standard text/html request
        /// </summary>
        public const string TextHtmlContent = "text/html";

        /// <summary>
        /// User agent string
        /// </summary>
        public const string UserAgent = "Mozilla/5.0, (Windows NT 10.0; Win64; x64), AppleWebKit/537.36, (KHTML, like Gecko), Chrome/70.0.3538.77, Safari/537.36";
        #endregion

        /************************************************************************/

        #region Constructor / Singleton access
        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        public static NetworkManager Instance { get; } = new NetworkManager();

        private NetworkManager()
        {
            client = new HttpClient(new HttpTimeoutHandler()
            {

                DefaultTimeout = TimeSpan.FromSeconds(30),

                InnerHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = CertificateValidation,
                    SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13,
                },
            })
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

        static NetworkManager()
        {
        }

        /// <summary>
        /// Destructor for <see cref="NetworkManager"/>. Disposes of HTTP client.
        /// </summary>
        ~ NetworkManager()
        {
            if (client != null)
            {
                client.Dispose();
            }

            client = null;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Asynchronously gets a network request
        /// </summary>
        /// <param name="url">The url</param>
        /// <param name="headers">An IEnumerable that provides custom headers, or null if not needed.</param>
        /// <returns>A <see cref="NetworkResponse"/> object.</returns>
        public async Task<NetworkResponse> GetHttpAsync(string url, IEnumerable<HttpHeader> headers = null)
        {
            try
            {
                using (HttpRequestMessage request = new(HttpMethod.Get, url))
                {
                    AddStandardRequestHeaders(request, TextHtmlContent);
                    /* Add caller specified headers if any */
                    AddCallerHeaders(request, headers);

                    HttpResponseMessage response = await client.SendAsync(request);
                    string body = await response.Content.ReadAsStringAsync();
                    return new NetworkResponse(response, url, body);
                }
            }
            catch (Exception ex)
            {
                return new NetworkResponse(ex);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void AddStandardRequestHeaders(HttpRequestMessage request, string contentType)
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            request.Headers.Host = request.RequestUri.Host;
            request.Headers.Add("Connection", "Close");
            request.Headers.Add("User-Agent", UserAgent);
            request.Headers.Date = new DateTimeOffset(DateTime.Now);
            request.Headers.Add("Accept-Language", "en-us");
            request.Headers.Add("Cache-Control", "no-cache");
        }

        private void AddCallerHeaders(HttpRequestMessage request, IEnumerable<HttpHeader> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Name, header.Value);
                }
            }
        }

        private bool CertificateValidation(HttpRequestMessage sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                //AppLog.Instance.Log(LogAction.Critical, nameof(NetworkManager), certificate, chain);
            }
            return sslPolicyErrors == SslPolicyErrors.None;
        }
        #endregion
    }
}