using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinEye.Services
{
    /// <summary>
    /// <para>An exception that is thrown when processing an HttpMessageBuilder instance fields or
    /// images into a complete POST message in byte format.</para>
    /// <para>Copyright (C) 2011-2012 Idee Inc. All rights reserved worldwide.</para>
    /// </summary>
    [Serializable()]
    public class HttpMessageBuilderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the HttpMessageBuilderException class.
        /// </summary>
        public HttpMessageBuilderException() : base() { }

        /// <summary>
        /// Initializes a new instance of the HttpMessageBuilderException class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HttpMessageBuilderException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the HttpMessageBuilderException class with a specified 
        /// error message and a reference to the inner exception that is the cause of this 
        /// exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public HttpMessageBuilderException(string message, System.Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the HttpMessageBuilderException class with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about 
        /// the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about 
        /// the source or destination.</param>
        protected HttpMessageBuilderException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
