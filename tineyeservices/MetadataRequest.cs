using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;
using Newtonsoft.Json.Linq;

namespace TinEye.Services
{
    /// <summary>
    /// <para>Provides methods to call the TinEye Services API methods that deal with searching
    /// and tagging images with metadata.</para>
    /// <para>Copyright (C) 2011-2016 Idée Inc. All rights reserved worldwide.</para>
    /// </summary>
    public class MetadataRequest : TinEyeServiceRequest
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the MetadataRequest class to issue HTTP requests to
        /// a TinEye Services API that supports image metadata.
        /// </summary>
        /// <param name="apiURL">The URL to a specific TinEye Services API URL.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        /// <exception cref="TinEyeServiceException">If the API URL does not end with /rest/.</exception>
        public MetadataRequest(String apiURL) : 
            base(apiURL, null, null) { }

        /// <summary>
        /// Initializes a new instance of the MetadataRequest class to issue HTTP requests 
        /// using HTTP basic authentication to a TinEye Services API that supports image
        /// metadata.
        /// </summary>
        /// <param name="apiURL">The URL to a specific TinEye Services API.</param>
        /// <param name="username">The username for HTTP basic authentication when connecting to
        /// the TinEye Services API.</param>
        /// <param name="password">The password for HTTP basic authentication when connecting to
        /// the TinEye Services API.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        /// <exception cref="TinEyeServiceException">If the API URL does not end with /rest/.</exception>
        public MetadataRequest(String apiURL, String username, String password) : 
            base(apiURL, username, password) { }

        /// <summary>
        /// Add the images in <c>images</c> to the hosted image collection.
        /// </summary>
        /// <remarks>
        /// <para>If the images have JSON metadata, add the images to the collection with metadata.
        /// </para>
        /// <note>If the metadata is included, each image must have metadata or the API will return
        /// an error.</note>
        /// <para>If the ignoreBackground option is set to true, an image region containing
        /// 75% or more of the image's edge pixels is considered a background region and its
        /// associated color is not included when indexing each image. This is mainly useful
        /// for product shots of objects on solid background colors. This option is not recommended 
        /// for collections containing images that don’t have solid color backgrounds like natural 
        /// images for example. 
        /// </para>
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
        /// <param name="images">List of images to add to collection.</param>
        /// <param name="ignoreBackground">If true, ignore the background color of the images. 
        /// If false, include the background color of the images.</param>
        /// <param name="ignoreInteriorBackground">If true, ignore regions that have the 
        /// same color as the background region but that are surrounded by non-background regions.
        /// </param>
        /// <returns>The API JSON response with the status of the image addition.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the API 
        /// <c>add</c> request or parsing the response.</exception>
        public JObject AddImage(Image[] images, bool ignoreBackground, bool ignoreInteriorBackground)
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
                    if (images[i].Metadata != null)
                    {
                        messageBuilder.Add("metadata[" + i + "]", images[i].Metadata.ToString());
                    }
                }
                messageBuilder.Add("ignore_background",          ignoreBackground);
                messageBuilder.Add("ignore_interior_background", ignoreInteriorBackground);

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
        /// <para>If the images have JSON metadata, add the images to the collection with metadata.
        /// </para>
        /// <note>If the metadata is included, each image must have metadata or the API will return 
        /// an error.</note>
        /// <para>If the ignoreBackground option is set to true, an image region containing
        /// 75% or more of the image's edge pixels is considered a background region and its
        /// associated color is not included when indexing each image. This is mainly useful
        /// for product shots of objects on solid background colors. This option is not recommended 
        /// for collections containing images that don’t have solid color backgrounds like natural 
        /// images for example. 
        /// </para>
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
        /// <param name="ignoreBackground">If true, ignore the background color of the images. 
        /// If false, include the background color of the images.</param>
        /// <param name="ignoreInteriorBackground">If true, ignore regions that have the 
        /// same color as the background region but that are surrounded by non-background regions.
        /// </param>
        /// <returns>The API JSON response with the status of the image addition.
        /// </returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing an API 
        /// <c>add</c> request or parsing the response.</exception>
        public JObject AddUrl(Image[] images, bool ignoreBackground, bool ignoreInteriorBackground)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < images.Length; i++)
                {
                    messageBuilder.Add("urls[" + i + "]",      images[i].ImageURL);
                    messageBuilder.Add("filepaths[" + i + "]", images[i].CollectionFilepath);

                    if (images[i].Metadata != null)
                    {
                        messageBuilder.Add("metadata[" + i + "]", images[i].Metadata.ToString());
                    }
                }
                messageBuilder.Add("ignore_background",          ignoreBackground);
                messageBuilder.Add("ignore_interior_background", ignoreInteriorBackground);

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
        /// Get the keywords from the index associated with the images with the given collection
        /// image filepaths.
        /// </summary>
        /// <remarks>
        /// <para>Returns the API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>get_metadata</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects for each match with the following fields:
        ///         <list type="table">
        ///            <item><term>filepath</term>
        ///                  <description>The collection match filepath</description></item>
        ///            <item><term>metadata</term>
        ///                  <description>JSON object with the keywords associated with the 
        ///                  image</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="filepaths">List of collection image filepaths to retrieve keywords for.
        /// </param>
        /// <returns>The API JSON response with the image keywords.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the API 
        /// <c>get_metadata</c> request or parsing the response.</exception>
        public JObject GetMetadata(string[] filepaths)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < filepaths.Length; i++)
                {
                    messageBuilder.Add("filepaths[" + i + "]", filepaths[i]);
                }

                return this.PostApiRequest("get_metadata", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetMetadata failed", e);
                throw new TinEyeServiceException("GetMetadata failed", e);
            }
        }

        /// <summary>
        /// Get the metadata tree structure that can be searched.
        /// </summary>
        /// <remarks>
        /// <para>Returns the API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>get_search_metadata</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array with a single JSON object containing the metadata tree
        ///         structure that can be searched also including the keyword type and number
        ///         of images in the index associated with that keyword</description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <returns>The API JSON response with the searchable metadata tree structure.
        /// </returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the API 
        /// <c>get_search_metadata</c> request or parsing the response.</exception>
        public JObject GetSearchMetadata()
        {
            try
            {
                return this.GetApiRequest("get_search_metadata");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetSearchMetadata failed", e);
                throw new TinEyeServiceException("GetSearchMetadata failed", e);
            }
        }

        /// <summary>
        /// Get the metadata that can be returned by a search method along with each match.
        /// </summary>
        /// <remarks>
        /// <para>Returns the API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>get_return_metadata</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array with a single JSON object containing the keywords that can 
        ///         be returned along with the data type and count of images in the index that 
        ///         have that keyword for each keyword</description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <returns>The API JSON response with the metadata available to return with 
        /// search results.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the API 
        /// <c>get_return_metadata</c> request or parsing the response.</exception>
        public JObject GetReturnMetadata()
        {
            try
            {
                return this.GetApiRequest("get_return_metadata");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetReturnMetadata failed", e);
                throw new TinEyeServiceException("GetReturnMetadata failed", e);
            }
        }

        /// <summary>
        /// Update the metadata for a list of images already present in the hosted image collection.
        /// </summary>
        /// <para>Note there must be one metadata entry for each image filepath passed in.</para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>update_metadata</c></description></item>
        ///   <item><term>result</term>
        ///         <description>empty array</description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// <param name="filepaths">Array of hosted image filepaths to update metadata for.</param>
        /// <param name="metadata">The metadata entries to associate with each image filepath.</param>
        /// <returns>The MulticolorEngine API JSON response</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>update_metadata</c> request or parsing the response.
        /// </exception>
        public JObject UpdateMetadata(string[] filepaths, JObject[] metadata)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                if (filepaths.Length != metadata.Length)
                    throw new TinEyeServiceException("filepaths and metadata list must have the same number of entries.");

                for (int i = 0; i < filepaths.Length; i++)
                {
                    messageBuilder.Add("filepaths[" + i + "]", filepaths[i]);
                    messageBuilder.Add("metadata[" + i + "]", metadata[i].ToString());
                }

                return this.PostApiRequest("update_metadata", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("UpdateMetadata failed", e);
                throw new TinEyeServiceException("UpdateMetadata failed", e);
            }
        }

        /// <summary>
        /// Add common search options to API POST request.
        /// </summary>
        /// <param name="messageBuilder">The messageBuilder being used to make the POST request.
        /// </param>
        /// <param name="metadata">Metadata to perform additional filtering on search results.
        /// </param>
        /// <param name="returnMetadata">Metadata fields to return with each match, which can
        /// include sorting options.</param>
        /// <param name="sortMetadata">If true, sort results by metadata score instead of by
        /// match score.</param>
        /// <param name="minScore">Minimum score of search results to return.</param>
        /// <param name="offset">Offset from start of search results to return.</param>
        /// <param name="limit">The maximum number of results to return.</param>
        /// <returns>The messageBuilder with all the passed in specified POST fields added.
        /// </returns>
        protected HttpMessageBuilder AddExtraSearchOptions(HttpMessageBuilder messageBuilder,
                                                           JObject metadata, JArray returnMetadata,
                                                           bool sortMetadata, int minScore,
                                                           int offset, int limit)
        {
            if (metadata != null)
            {
                messageBuilder.Add("metadata", metadata.ToString());
                messageBuilder.Add("return_metadata", returnMetadata.ToString());
                messageBuilder.Add("sort_metadata", sortMetadata);
            }

            messageBuilder.Add("min_score", minScore);
            messageBuilder.Add("offset", offset);
            messageBuilder.Add("limit", limit);

            return messageBuilder;
        }
    }
}
