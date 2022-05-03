using System;

namespace Restless.Panama.Network
{
    /// <summary>
    /// Represents an http header
    /// </summary>
    public class HttpHeader
    {
        /// <summary>
        /// Gets the header name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the header value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHeader"/> class.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The header value.</param>
        public HttpHeader(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Name = name;
            Value = value;
        }
    }
}