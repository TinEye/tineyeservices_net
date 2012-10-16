using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Newtonsoft.Json.Linq;

namespace TinEye.Services
{
    /// <summary>
    /// <para>Provides methods to call the MatchEngine API methods.</para>
    /// <note>For a list of available MatchEngine API methods, refer to the documentation for your
    /// MatchEngine API installation.</note>
    /// <para>Copyright (C) 2011-2012 Idee Inc. All rights reserved worldwide.</para>
    /// </summary>
    public class MatchEngineRequest : TinEyeServiceRequest
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the MatchEngineRequest class to issue HTTP requests to
        /// the MatchEngine API.
        /// </summary>
        /// <param name="apiURL">The MatchEngine API URL.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        public MatchEngineRequest(String apiURL) : 
            base(apiURL, null, null) { }

        /// <summary>
        /// Initializes a new instance of the MatchEngineRequest class to issue HTTP requests 
        /// using HTTP basic authentication to the MatchEngine API .
        /// </summary>
        /// <param name="apiURL">The MatchEngine API URL.</param>
        /// <param name="username">The username for HTTP basic authentication when connecting to
        /// the MatchEngine API.</param>
        /// <param name="password">The password for HTTP basic authentication when connecting to
        /// the MatchEngine API.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        public MatchEngineRequest(String apiURL, String username, String password) : 
            base(apiURL, username, password) { }

        /// <summary>
        /// Add the images in <c>images</c> to the hosted image collection.
        /// </summary>
        /// <remarks>
        /// <para>Returns the API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>add</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Empty array</description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="images">List of images with image data to add to collection.</param>
        /// <returns>The API JSON response with the image addition status.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the API 
        /// <c>add</c> request or parsing the response.</exception>
        public JObject AddImage(Image[] images)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < images.Length; i++)
                {
                    messageBuilder.Add("images[" + i + "]", images[i]);

                    if (images[i].CollectionFilepath != null)
                    {
                        messageBuilder.Add("filepaths[" + i + "]", images[i].CollectionFilepath);
                    }
                }

                return this.PostApiRequest("add", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("AddImage failed", e);
                throw new TinEyeServiceException("AddImage failed", e);
            }
        }

        /// <summary>
        /// Add the images in <c>images</c> to the hosted image collection using their URLs.
        /// </summary>
        /// <remarks>
        /// <note>Each Image in <c>images</c> must have a URL and collection filepath set.</note>
        /// <para>Returns the API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>add</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Empty array</description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="images">List of images with URLs and collection filepaths to add to 
        /// collection.</param>
        /// <returns>The API JSON response with the image addition status.
        /// </returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the API 
        /// <c>add</c> request or parsing the response.</exception>
        public JObject AddUrl(Image[] images)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < images.Length; i++)
                {
                    messageBuilder.Add("urls[" + i + "]", images[i].ImageURL);
                    messageBuilder.Add("filepaths[" + i + "]", images[i].CollectionFilepath);
                }

                return this.PostApiRequest("add", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("AddUrl failed", e);
                throw new TinEyeServiceException("AddUrl failed", e);
            }
        }

        /// <summary>
        /// Search the hosted image collection using an image and return any matches with 
        /// corresponding scores.
        /// </summary>
        /// <remarks>
        /// <para>Returns the MatchEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>search</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects for each match with the following fields:
        ///         <list type="table">
        ///            <item><term>score</term>
        ///                  <description>Relevance score of match</description></item>
        ///            <item><term>overlay</term>
        ///                  <description>URL pointing to the match overlay image on the API server
        ///                  </description></item>
        ///            <item><term>filepath</term>
        ///                  <description>The collection match filepath</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="image">The image to search for using its data.</param>
        /// <param name="minScore">Minimum score to return for results.</param>
        /// <param name="offset">Offset to start returning results from.</param>
        /// <param name="limit">Maximum number of results to return.</param>
        /// <param name="checkHorizontalFlip">If true, also search for horizontally flipped image 
        /// in collection.</param>
        /// <returns>The MatchEngine API JSON response with search results.</returns>
        /// <exception cref="TinEyeServiceException">If exception occurs issuing the MatchEngine API 
        /// <c>search</c> request or parsing the response.</exception>
        public JObject SearchImage(Image image, int minScore, int offset, int limit, bool checkHorizontalFlip)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                messageBuilder.Add("image", image);
                messageBuilder.Add("min_score", minScore);
                messageBuilder.Add("offset", offset);
                messageBuilder.Add("limit", limit);
                messageBuilder.Add("check_horizontal_flip", checkHorizontalFlip);

                return this.PostApiRequest("search", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("SearchData failed", e);
                throw new TinEyeServiceException("SearchImage failed", e);
            }
        }

        /// <summary>
        /// Search collection using the filepath of an image in the hosted image collection 
        /// and return any matches with corresponding scores.
        /// </summary>
        /// <remarks>
        /// <para>Returns the MatchEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>search</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects for each match with the following fields:
        ///         <list type="table">
        ///            <item><term>score</term>
        ///                  <description>Relevance score of match</description></item>
        ///            <item><term>overlay</term>
        ///                  <description>URL pointing to the match overlay image on the API server
        ///                  </description></item>
        ///            <item><term>filepath</term>
        ///                  <description>The collection match filepath</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="filepath">A filepath of an image already in the collection as returned by 
        /// a search or list operation.</param>
        /// <param name="minScore">Minimum score to return for results.</param>
        /// <param name="offset">Offset to start returning results from.</param>
        /// <param name="limit">Maximum number of results to return.</param>
        /// <param name="checkHorizontalFlip">If true, also search for horizontally flipped image 
        /// in collection.</param>
        /// <returns>The MatchEngine API JSON response with search results.</returns>
        /// <exception cref="TinEyeServiceException">If exception occurs issuing the 
        /// MatchEngine API <c>search</c> request or parsing the response.</exception>
        public JObject SearchFilepath(string filepath, int minScore, int offset, int limit, bool checkHorizontalFlip)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                messageBuilder.Add("filepath", filepath);
                messageBuilder.Add("min_score", minScore);
                messageBuilder.Add("offset", offset);
                messageBuilder.Add("limit", limit);
                messageBuilder.Add("check_horizontal_flip", checkHorizontalFlip);

                return this.PostApiRequest("search", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("SearchFilepath failed", e);
                throw new TinEyeServiceException("SearchFilepath failed", e);
            }
        }

        /// <summary>
        /// Search collection using an image URL and return any matches with corresponding scores.
        /// </summary>
        /// <remarks>
        /// <para>Returns the MatchEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>search</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects for each match with the following fields:
        ///         <list type="table">
        ///            <item><term>score</term>
        ///                  <description>Relevance score of match</description></item>
        ///            <item><term>overlay</term>
        ///                  <description>URL pointing to the match overlay image on the API server
        ///                  </description></item>
        ///            <item><term>filepath</term>
        ///                  <description>The collection match filepath</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="url">The URL to the image to search for against the collection.</param>
        /// <param name="minScore">Minimum score to return for results.</param>
        /// <param name="offset">Offset to start returning results from.</param>
        /// <param name="limit">Maximum number of results to return.</param>
        /// <param name="checkHorizontalFlip">If true, also search for horizontally flipped image 
        /// in collection.</param>
        /// <returns>The MatchEngine API JSON response with search results.</returns>
        /// <exception cref="TinEyeServiceException">If exception occurs issuing the 
        /// MatchEngine API <c>search</c> request or parsing the response.</exception>
        public JObject SearchUrl(string url, int minScore, int offset, int limit, bool checkHorizontalFlip)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                messageBuilder.Add("url", url);
                messageBuilder.Add("min_score", minScore);
                messageBuilder.Add("offset", offset);
                messageBuilder.Add("limit", limit);
                messageBuilder.Add("check_horizontal_flip", checkHorizontalFlip);

                return this.PostApiRequest("search", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("SearchUrl failed", e);
                throw new TinEyeServiceException("SearchUrl failed", e);
            }
        }

        /// <summary>
        /// Compare <c>image1</c> to <c>image2</c> and return the match score.
        /// </summary>
        /// <remarks>
        /// <para>Returns the MatchEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>compare</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects for each match with the following fields:
        ///         <list type="table">
        ///            <item><term>score</term>
        ///                  <description>Relevance score of match</description></item>
        ///            <item><term>match_percent</term>
        ///                  <description>Percentage of images that matched</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="image1">The first image to compare using its data.</param>
        /// <param name="image2">The second image to compare using its data.</param>
        /// <param name="minScore">The minimum score of the result to return.</param>
        /// <param name="checkHorizontalFlip">If true, also check if <c>image2</c> is the 
        /// horizontally flipped version of <c>image1</c></param>
        /// <returns>The MatchEngine API JSON response with compare results.</returns>
        /// <exception cref="TinEyeServiceException">If exception occurs issuing the 
        /// MatchEngine API <c>compare</c> request or parsing the response.</exception>
        public JObject CompareImage(Image image1, Image image2, int minScore, bool checkHorizontalFlip)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                messageBuilder.Add("image1", image1);
                messageBuilder.Add("image2", image2);
                messageBuilder.Add("min_score", minScore);
                messageBuilder.Add("check_horizontal_flip", checkHorizontalFlip);

                return this.PostApiRequest("compare", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CompareImage failed", e);
                throw new TinEyeServiceException("CompareImage failed", e);
            }
        }

        /// <summary>
        /// Compare the image at <c>url1</c> to the image at <c>url2</c> and return the match 
        /// score.
        /// </summary>
        /// <remarks>
        /// <para>Returns the MatchEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>compare</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects for each match with the following fields:
        ///         <list type="table">
        ///            <item><term>score</term>
        ///                  <description>Relevance score of match</description></item>
        ///            <item><term>match_percent</term>
        ///                  <description>Percentage of images that matched</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="url1">URL to the first image to compare.</param>
        /// <param name="url2">URL to the second image to compare.</param>
        /// <param name="minScore">The minimum score of the result to return.</param>
        /// <param name="checkHorizontalFlip">If true, also check if <c>image2</c> is the 
        /// horizontally flipped version of <c>image1</c></param>
        /// <returns>The MatchEngine API JSON response with compare results.</returns>
        /// <exception cref="TinEyeServiceException">If exception occurs issuing the 
        /// MatchEngine API <c>compare</c> request or parsing the response.</exception>
        public JObject CompareUrl(string url1, string url2, int minScore, bool checkHorizontalFlip)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                messageBuilder.Add("url1", url1);
                messageBuilder.Add("url2", url2);
                messageBuilder.Add("min_score", minScore);
                messageBuilder.Add("check_horizontal_flip", checkHorizontalFlip);

                return this.PostApiRequest("compare", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CompareUrl failed", e);
                throw new TinEyeServiceException("CompareUrl failed", e);
            }
        }
    }
}
