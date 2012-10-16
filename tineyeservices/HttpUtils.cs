using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using log4net;

namespace TinEye.Services
{
    /// <summary>
    /// <para>Provides methods to issue HTTP GET or POST requests.</para>
    /// <para>Copyright (C) 2011-2012 Idee Inc. All rights reserved worldwide.</para>
    /// </summary>
    /// <remarks>
    /// An HttpUtils instance can issue HTTP GET or POST requests and can issue requests requiring 
    /// HTTP basic authentication if the instance is initialized with the username and password.
    /// </remarks>
    public class HttpUtils
    {
        private static readonly log4net.ILog log = 
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string username;
        private readonly string password;

        /// <summary>
        /// Initialize a new instance of the HttpUtils class to issue HTTP requests.
        /// </summary>
        public HttpUtils() : this(null, null) { }

        /// <summary>
        /// Initialize a new instance of the HttpUtils class to issue HTTP requests with 
        /// HTTP basic authentication.
        /// </summary>
        /// <param name="username">The username for HTTP basic authentication.</param>
        /// <param name="password">The password for HTTP basic authentication.</param>
        public HttpUtils(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        /// <summary>
        /// Issue an HTTP POST with the <paramref name="postRequestBody"/> to the 
        /// <paramref name="requestUrl"/>.
        /// </summary>
        /// <param name="requestUrl">The URL to issue the POST request to.</param>
        /// <param name="postRequestBody">Body of POST request (including any files to upload).
        /// </param>
        /// <param name="boundary">The boundary string used to separate different fields in the 
        /// POST message body. This is required to set the correct HTTP ContentType header for
        /// the POST request.</param>
        /// <returns>The server response.</returns>
        /// <exception cref="HttpUtilsException">If an error issuing the POST request or parsing 
        /// the server response occurs.</exception>
        public string Post(String requestUrl, byte[] postRequestBody, string boundary)
        {
            try
            {
                Uri requestUri = new Uri(requestUrl);

                HttpWebRequest apiRequest = WebRequest.Create(requestUri) as HttpWebRequest;

                apiRequest.Method = "POST";
                apiRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                apiRequest.ContentLength = postRequestBody.Length;
                apiRequest.KeepAlive = false;

                if (username != null && password != null)
                {
                    CredentialCache authCredentials = new CredentialCache();
                    authCredentials.Add(requestUri, "Basic", new NetworkCredential(username, password));
                    apiRequest.Credentials = authCredentials;
                }

                // Set POST request body to send API.
                using (Stream postDataStream = apiRequest.GetRequestStream())
                {
                    postDataStream.Write(postRequestBody, 0, postRequestBody.Length);
                }

                // Get response  
                string apiResponse = "";
                using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    apiResponse = reader.ReadToEnd();
                }
                return apiResponse;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Post failed with exception: ", e);
                throw new HttpUtilsException("Post failed", e);
            }
        }

        /// <summary>
        /// <para>Issue an HTTP GET request to the given <paramref name="requestUrl"/>.</para>
        /// <para>The <paramref name="requestUrl"/> can contain query parameters 
        /// (name=value pairs).</para>
        /// </summary>
        /// <param name="requestUrl">The URL to issue the GET request to (with optional query 
        /// parameters)</param>
        /// <returns>The server response.</returns>
        /// <exception cref="HttpUtilsException">If an error issuing the GET request or parsing 
        /// the server response occurs.</exception>
        public string Get(String requestUrl)
        {
            try
            {
                Uri requestUri = new Uri(requestUrl);

                HttpWebRequest apiRequest = WebRequest.Create(requestUri) as HttpWebRequest;

                apiRequest.Method = "GET";
                apiRequest.KeepAlive = false;

                if (username != null && password != null)
                {
                    CredentialCache authCredentials = new CredentialCache();
                    authCredentials.Add(requestUri, "Basic", new NetworkCredential(username, password));
                    apiRequest.Credentials = authCredentials;
                }

                // Get response  
                string apiResponse = "";
                using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    apiResponse = reader.ReadToEnd();
                }
                return apiResponse;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Get failed", e);
                throw new HttpUtilsException("Get failed", e);
            }
        }
    }
}
