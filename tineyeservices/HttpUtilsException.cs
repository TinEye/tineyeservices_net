using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinEye.Services
{
    /// <summary>
    /// <para>An exception that is thrown when making HTTP GET or POST requests or 
    /// processing the server response using an HttpUtils instance.</para>
    /// <para>Copyright (C) 2011-2012 Idee Inc. All rights reserved worldwide.</para>
    /// </summary>
    [Serializable()]
    public class HttpUtilsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the HttpUtilsException class.
        /// </summary>
        public HttpUtilsException() : base() { }

        /// <summary>
        /// Initializes a new instance of the HttpUtilsExceptionn class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HttpUtilsException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the HttpUtilsExceptionn class with a specified 
        /// error message and a reference to the inner exception that is the cause of this 
        /// exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public HttpUtilsException(string message, System.Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the HttpUtilsException class with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about 
        /// the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about 
        /// the source or destination.</param>
        protected HttpUtilsException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
