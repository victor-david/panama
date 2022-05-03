using System;
using System.Net.Http;

namespace Restless.Panama.Network
{
    /// <summary>
    /// Represents a response from <see cref="NetworkManager"/>.
    /// This is a convenience wrapper class used to hold the extracted
    /// response body before passing back to the caller.
    /// </summary>
    public class NetworkResponse
    {
        #region Public properties
        /// <summary>
        /// Gets the url that was used in this operation.
        /// </summary>
        public string Url
        {
            get;
        }

        /// <summary>
        /// The http response message.
        /// </summary>
        public HttpResponseMessage HttpResponse
        {
            get;
        }

        /// <summary>
        /// The response body.
        /// </summary>
        public string ResponseBody
        {
            get;
        }

        /// <summary>
        /// Gets the exception that occured, or null if none.
        /// </summary>
        public Exception Exception
        {
            get;
        }

        /// <summary>
        /// Gets a boolean value that indicates if this response is faulted.
        /// If this property is true, you cannot use <see cref="HttpResponse"/> or <see cref="ResponseBody"/>,
        /// only the <see cref="Exception"/> property.
        /// </summary>
        public bool IsFaulted => Exception != null;

        /// <summary>
        /// Gets a boolean value that indicates if this response is successful.
        /// </summary>
        public bool IsSuccess => !IsFaulted && HttpResponse != null && HttpResponse.IsSuccessStatusCode;

        /// <summary>
        /// Gets a string that describes the Http response.
        /// Concatenates status code and reason.
        /// </summary>
        public string HttpResponseString => $"{(int)HttpResponse.StatusCode} {HttpResponse.ReasonPhrase}";
        #endregion

        /************************************************************************/

        #region Constructors (internal)
        /// <summary>
        /// Initializes a new instance of the<see cref="NetworkResponse"/> class.
        /// </summary>
        /// <param name="httpResponse">The http response.</param>
        /// <param name="url">The final destination url.</param>
        /// <param name="responseBody">The response body.</param>
        internal NetworkResponse(HttpResponseMessage httpResponse, string url, string responseBody)
        {
            HttpResponse = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));
            Url = url;
            ResponseBody = responseBody ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the<see cref="NetworkResponse"/> class.
        /// </summary>
        /// <param name="exception">The exception that occurred, must not be null.</param>
        /// <param name="url">The final destination url.</param>
        internal NetworkResponse(Exception exception, string url)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            Url = url;
        }

        /// <summary>
        /// Initializes a new instance of the<see cref="NetworkResponse"/> class.
        /// </summary>
        /// <param name="exception">The exception that occurred, must not be null.</param>
        internal NetworkResponse(Exception exception) : this(exception, null)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets the object as string. An alias for <see cref="HttpResponseString"/>.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return HttpResponseString;
        }
        #endregion
    }
}