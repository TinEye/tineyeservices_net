using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Newtonsoft.Json.Linq;

namespace TinEye.Services
{
    /// <summary>
    /// <para>Provides methods to call the WineEngine API methods.</para>
    /// <note>For a list of available WineEngine API methods, refer to the documentation for your
    /// WineEngine API installation.</note>
    /// <para>Copyright (C) 2016 Idée Inc. All rights reserved worldwide.</para>
    /// </summary>
    public class WineEngineRequest : MobileEngineRequest
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the WineEngineRequest class to issue HTTP requests to
        /// the WineEngine API.
        /// </summary>
        /// <param name="apiURL">The WineEngine API URL.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        public WineEngineRequest(String apiURL) :
            base(apiURL, null, null) { }

        /// <summary>
        /// Initializes a new instance of the WineEngineRequest class to issue HTTP requests 
        /// using HTTP basic authentication to the WineEngine API.
        /// </summary>
        /// <param name="apiURL">The WineEngine API URL.</param>
        /// <param name="username">The username for HTTP basic authentication when connecting to
        /// the WineEngine API.</param>
        /// <param name="password">The password for HTTP basic authentication when connecting to
        /// the WineEngine API.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        public WineEngineRequest(String apiURL, String username, String password) :
            base(apiURL, username, password) { }
    }
}