using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Newtonsoft.Json.Linq;

namespace TinEye.Services
{
    /// <summary>
    /// <para>Provides methods to call the MobileEngine API methods.</para>
    /// <note>For a list of available MobileEngine API methods, refer to the documentation for your
    /// MobileEngine API installation.</note>
    /// <para>Copyright (C) 2016 Idee Inc. All rights reserved worldwide.</para>
    /// </summary>
    public class MobileEngineRequest : MatchEngineRequest
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the MobileEngineRequest class to issue HTTP requests to
        /// the MobileEngine API.
        /// </summary>
        /// <param name="apiURL">The MobileEngine API URL.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        public MobileEngineRequest(String apiURL) :
            base(apiURL, null, null) { }

        /// <summary>
        /// Initializes a new instance of the MobileEngineRequest class to issue HTTP requests 
        /// using HTTP basic authentication to the MobileEngine API .
        /// </summary>
        /// <param name="apiURL">The MobileEngine API URL.</param>
        /// <param name="username">The username for HTTP basic authentication when connecting to
        /// the MobileEngine API.</param>
        /// <param name="password">The password for HTTP basic authentication when connecting to
        /// the MobileEngine API.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        public MobileEngineRequest(String apiURL, String username, String password) :
            base(apiURL, username, password) { }
    }
}