using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

namespace TinEye.Services
{
    /// <summary>
    /// <para>Provides methods to call the TinEye Services API methods that are common across all
    /// of the TinEye Services APIs (excluding the TinEye Commercial API).</para>
    /// <para>Copyright (C) 2011-2012 Idee Inc. All rights reserved worldwide.</para>
    /// </summary>
    public class TinEyeServiceRequest
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly String apiURL;
        private readonly String username;
        private readonly String password;

        /// <summary>
        /// Initializes a new instance of the TinEyeServiceRequest class to issue HTTP requests to
        /// a specific TinEye Services API.
        /// </summary>
        /// <param name="apiURL">The URL for a specific TinEye Services API.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        public TinEyeServiceRequest(String apiURL) : 
            this(apiURL, null, null) { }

        /// <summary>
        /// Initializes a new instance of the TinEyeServiceRequest class to issue HTTP requests to 
        /// a specific TinEye Services API using HTTP basic authentication.
        /// </summary>
        /// <param name="apiURL">The TinEye Services API URL.</param>
        /// <param name="username">The username for HTTP basic authentication when connecting to
        /// the TinEye Services API.</param>
        /// <param name="password">The password for HTTP basic authentication when connecting to
        /// the TinEye Services API.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        public TinEyeServiceRequest(String apiURL, String username, String password)
        {
            if (apiURL == null)
            {
                if (log.IsErrorEnabled)
                    log.Error("Failed to construct TinEyeServiceRequest: apiURL is null");
                throw new ArgumentNullException("apiURL is null");
            }

            // All API URLs have to end with /rest/ or else the URL is incorrect.
            if (apiURL.EndsWith("/"))
            {
                this.apiURL = apiURL;
            }
            else
            {
                this.apiURL = apiURL + "/";
            }
            this.username = username;
            this.password = password;
        }

        /// <summary>
        /// Delete images from the collection.
        /// </summary>
        /// <remarks>
        /// <para>Returns the API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>delete</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Empty array</description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="filepaths">Filepaths of images to delete as returned by a search or list 
        /// call.</param>
        /// <returns>The API JSON response with image deletion status.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the API 
        /// <c>delete</c> request or parsing the response.</exception>
        public JObject Delete(string[] filepaths)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < filepaths.Length; i++)
                {
                    messageBuilder.Add("filepaths[" + i + "]", filepaths[i]);
                }

                return this.PostApiRequest("delete", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Delete failed", e);
                throw new TinEyeServiceException("Delete failed", e);
            }
        }

        /// <summary>
        /// Get the count of all the images in the hosted image collection.
        /// </summary>
        /// <remarks>
        /// <para>Returns the API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>count</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array with one entry which is the image count</description>
        ///   </item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <returns>The API JSON response with the collection image count.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the API 
        /// <c>count</c> request or parsing the response.</exception>
        public JObject Count()
        {
            try
            {
                return this.GetApiRequest("count");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Count failed", e);
                throw new TinEyeServiceException("Count failed", e);
            }
        }

        /// <summary>
        /// Get a list of images from the collection.
        /// </summary>
        /// <remarks>
        /// <para>Returns the API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>list</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array with list of collection image filepaths</description>
        ///   </item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="offset">Offset from start of search results to return.</param>
        /// <param name="limit">Maximum number of images to list.</param>
        /// <returns>The API JSON response with list of collection images.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the API 
        /// <c>list</c> request or parsing the response.</exception>
        public JObject List(int offset, int limit)
        {
            string queryParams = "offset=" + offset + "&limit=" + limit;

            try
            {
                return this.GetApiRequest("list", queryParams);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("List failed", e);
                throw new TinEyeServiceException("List failed", e);
            }
        }

        /// <summary>
        /// Check if the API server is running.
        /// </summary>
        /// <remarks>
        /// <para>Returns the API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>ping</c></description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <returns>The API JSON response with the server status.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the API 
        /// <c>ping</c> request or parsing the response.</exception>
        public JObject Ping()
        {
            try
            {
                return this.GetApiRequest("ping");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Ping failed", e);
                throw new TinEyeServiceException("Ping failed", e);
            }
        }

        /// <summary>
        /// Issue an HTTP GET request to the specified API <c>method</c>.
        /// </summary>
        /// <param name="method">The API method to issue the HTTP GET request to.</param>
        /// <returns>The JSON response returned by the API.</returns>
        /// <exception cref="ArgumentNullException">If method is Nothing.</exception>
        /// <exception cref="HttpUtilsException">If an error occurs making or issuing the HTTP GET
        /// request to the API.</exception>
        /// <exception cref="Newtonsoft.Json.JsonReaderException">If an error occurs parsing the 
        /// API response to JSON.</exception>
        protected JObject GetApiRequest(string method)
        {
            return this.GetApiRequest(method, null);
        }

        /// <summary>
        /// Issue an HTTP GET request to the specified API <c>method</c> with the 
        /// given API method <c>queryParams</c>.
        /// </summary>
        /// <param name="method">The API method to issue the HTTP GET request to.</param>
        /// <param name="queryParams">List of name=value query parameters to include in the
        /// API method call. Pairs must be separated by ampersands.</param>
        /// <returns>The JSON response returned by the API.</returns>
        /// <exception cref="ArgumentNullException">If method is Nothing.</exception>
        /// <exception cref="HttpUtilsException">If an error occurs making or issuing the HTTP GET
        /// request to the API.</exception>
        /// <exception cref="Newtonsoft.Json.JsonReaderException">If an error occurs parsing the 
        /// API response to JSON.</exception>
        protected JObject GetApiRequest(string method, string queryParams)
        {
            if (method == null)
            {
                if (log.IsErrorEnabled)
                    log.Error("GET request to API failed: 'method' is null");
                throw new ArgumentNullException("method is null");
            }
            
            HttpUtils httpHelper = new HttpUtils(this.username, this.password);
            string requestURL = apiURL + method + "/";

            if (queryParams != null)
            {
                requestURL += "?" + queryParams;
            }

            try
            {
                string jsonResponse = httpHelper.Get(requestURL);
                return JObject.Parse(jsonResponse);
            }
            catch (HttpUtilsException hue)
            {
                if (log.IsErrorEnabled)
                    log.Error("GET request to API failed", hue);
                throw hue;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("failed to parse GET API response", e);
                throw new Newtonsoft.Json.JsonReaderException("failed to parse GET API response", e);
            }
        }

        /// <summary>
        /// Issue an HTTP POST request with the given <c>postRequest</c> to the specified 
        /// API <c>method</c>.
        /// </summary>
        /// <param name="method">The API method to issue the POST request to.</param>
        /// <param name="postRequest">The POST request body to send to the API.</param>
        /// <param name="boundary">Field string separating different fields in the POST message.
        /// This is required to send the correct content-type header in the request.</param>
        /// <returns>The JSON response returned by the API.</returns>
        /// <exception cref="ArgumentNullException">If method is Nothing.</exception>
        /// <exception cref="HttpUtilsException">If an error occurs making or issuing the HTTP POST
        /// request to the API.</exception>
        /// <exception cref="Newtonsoft.Json.JsonReaderException">If an error occurs parsing the 
        /// API response to JSON.</exception>
        protected JObject PostApiRequest(string method, byte[] postRequest, string boundary)
        {
            HttpUtils httpHelper = new HttpUtils(this.username, this.password);

            if (method == null)
            {
                if (log.IsErrorEnabled)
                    log.Error("POST request to API failed: 'method' is null");
                throw new ArgumentNullException("method is null");
            }

            string requestURL = apiURL + method + "/";

            try
            {
                string response = httpHelper.Post(requestURL, postRequest, boundary);
                return JObject.Parse(response);
            }
            catch (HttpUtilsException hue)
            {
                if (log.IsErrorEnabled)
                    log.Error("GET request to API failed", hue);
                throw hue;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("failed to parse POST API response", e);
                throw new Newtonsoft.Json.JsonReaderException("failed to parse POST API response", e);
            }
        }
    }
}
