using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using log4net;
using Newtonsoft.Json.Linq;

namespace TinEye.Services
{
    /// <summary>
    /// <para>Provides methods to call the MulticolorEngine API methods.</para>
    /// <para>Copyright (C) 2011-2012 Idee Inc. All rights reserved worldwide.</para>
    /// </summary>
    public class MulticolorEngineRequest : MetadataRequest
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the MulticolorEngineRequest class to issue HTTP requests 
        /// to the MulticolorEngine API.
        /// </summary>
        /// <param name="apiURL">The MulticolorEngine API URL.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        public MulticolorEngineRequest(String apiURL) : 
            base(apiURL, null, null) { }

        /// <summary>
        /// Initializes a new instance of the MulticolorEngineRequest class to issue HTTP requests 
        /// using HTTP basic authentication to the MulticolorEngine API.
        /// </summary>
        /// <param name="apiURL">The MulticolorEngine API URL.</param>
        /// <param name="username">The username for HTTP basic authentication when connecting to
        /// the MulticolorEngine API.</param>
        /// <param name="password">The password for HTTP basic authentication when connecting to
        /// the MulticolorEngine API.</param>
        /// <exception cref="ArgumentNullException">If the API URL is Nothing.</exception>
        public MulticolorEngineRequest(String apiURL, String username, String password) : 
            base(apiURL, username, password) { }

        /// <summary>
        /// Do a color search against the collection using an image and return matches with
        /// corresponding scores.
        /// </summary>
        /// <remarks>
        /// <para>If the ignoreBackground option is set to true, an image region containing
        /// 75% or more of the image's edge pixels is considered a background region and its
        /// associated color is not included in the search.
        /// </para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>count</term>
        ///         <description>The number of search results</description></item>
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>color_search</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects for each match with the following fields:
        ///         <list type="table">
        ///            <item><term>score</term>
        ///                  <description>Relevance score of match</description></item>
        ///            <item><term>filepath</term>
        ///                  <description>The collection match filepath</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="image">Image to search collection using colors from that image's data.
        /// </param>
        /// <param name="metadata">Metadata to perform additional filtering on search results.
        /// </param>
        /// <param name="returnMetadata">Metadata fields to return with each match, which can
        /// include sorting options.</param>
        /// <param name="sortMetadata">If true, sort results by metadata score instead of by
        /// match score.</param>
        /// <param name="ignoreBackground">If true, ignore the background color of the images. 
        /// If false, include the background color of the images.</param>
        /// <param name="ignoreInteriorBackground">If true, ignore regions that have the same 
        /// color as the background region but that are surrounded by non-background regions.</param>
        /// <param name="minScore">Minimum score of search results to return.</param>
        /// <param name="offset">Offset from start of search results to return.</param>
        /// <param name="limit">The maximum number of results to return.</param>
        /// <returns>The MulticolorEngine API JSON response with the color search results.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>color_search</c> request or parsing the response.</exception>
        public JObject SearchImage(Image image, JObject metadata, JArray returnMetadata,
                                   bool sortMetadata, bool ignoreBackground, bool ignoreInteriorBackground, 
                                   int minScore, int offset, int limit)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                messageBuilder.Add("image",                      image);
                messageBuilder.Add("ignore_background",          ignoreBackground);
                messageBuilder.Add("ignore_interior_background", ignoreInteriorBackground);

                messageBuilder = this.AddExtraSearchOptions(messageBuilder, metadata, returnMetadata,
                                                            sortMetadata, minScore, offset, limit);

                return this.PostApiRequest("color_search", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("SearchImage failed", e);
                throw new TinEyeServiceException("SearchImage failed", e);
            }
        }

        /// <summary>
        /// Do a color search against the collection using the filepath of an image in the 
        /// collection and return matches with corresponding scores.
        /// </summary>
        /// <remarks>
        /// <para>If the ignoreBackground option is set to true, an image region containing
        /// 75% or more of the image's edge pixels is considered a background region and its
        /// associated color is not included in the search.
        /// </para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>count</term>
        ///         <description>The number of search results</description></item>
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>color_search</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects for each match with the following fields:
        ///         <list type="table">
        ///            <item><term>score</term>
        ///                  <description>Relevance score of match</description></item>
        ///            <item><term>filepath</term>
        ///                  <description>The collection match filepath</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="filepath">The collection filepath to the image whose colors to use for
        /// searching.</param>
        /// <param name="metadata">Metadata to perform additional filtering on search results.
        /// </param>
        /// <param name="returnMetadata">Metadata fields to return with each match, which can
        /// include sorting options.</param>
        /// <param name="sortMetadata">If true, sort results by metadata score instead of by
        /// match score.</param>
        /// <param name="ignoreBackground">If true, ignore the background color of the images. 
        /// If false, include the background color of the images.</param>
        /// <param name="ignoreInteriorBackground">If true, ignore regions that have the same 
        /// color as the background region but that are surrounded by non-background regions.</param>
        /// <param name="minScore">Minimum score of search results to return.</param>
        /// <param name="offset">Offset from start of search results to return.</param>
        /// <param name="limit">The maximum number of results to return.</param>
        /// <returns>The MulticolorEngine API JSON response with the color search results.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the 
        /// MulticolorEngine API <c>color_search</c> request or parsing the response.</exception>
        public JObject SearchFilepath(string filepath, JObject metadata, JArray returnMetadata,
                                      bool sortMetadata, bool ignoreBackground, bool ignoreInteriorBackground,
                                      int minScore, int offset, int limit)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                messageBuilder.Add("filepath",                   filepath);
                messageBuilder.Add("ignore_background",          ignoreBackground);
                messageBuilder.Add("ignore_interior_background", ignoreInteriorBackground);

                messageBuilder = this.AddExtraSearchOptions(messageBuilder, metadata, returnMetadata,
                                                            sortMetadata, minScore, offset, limit);

                return this.PostApiRequest("color_search", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("SearchFilepath failed", e);
                throw new TinEyeServiceException("SearchFilepath failed", e);
            }
        }

        /// <summary>
        /// Do a color search against the collection using an image URL and return matches with 
        /// corresponding scores.
        /// </summary>
        /// <remarks>
        /// <para>If the ignoreBackground option is set to true, an image region containing
        /// 75% or more of the image's edge pixels is considered a background region and its
        /// associated color is not included in the search.
        /// </para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>count</term>
        ///         <description>The number of search results</description></item>
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>color_search</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects for each match with the following fields:
        ///         <list type="table">
        ///            <item><term>score</term>
        ///                  <description>Relevance score of match</description></item>
        ///            <item><term>filepath</term>
        ///                  <description>The collection match filepath</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="imageUrl">URL to image whose colors to use for searching.</param>
        /// <param name="metadata">Metadata to perform additional filtering on search results.
        /// </param>
        /// <param name="returnMetadata">Metadata fields to return with each match, which can
        /// include sorting options.</param>
        /// <param name="sortMetadata">If true, sort results by metadata score instead of by
        /// match score.</param>
        /// <param name="ignoreBackground">If true, ignore the background color of the images. 
        /// If false, include the background color of the images.</param>
        /// <param name="ignoreInteriorBackground">If true, ignore regions that have the same 
        /// color as the background region but that are surrounded by non-background regions.</param>
        /// <param name="minScore">Minimum score of search results to return.</param>
        /// <param name="offset">Offset from start of search results to return.</param>
        /// <param name="limit">The maximum number of results to return.</param>
        /// <returns>The MulticolorEngine API JSON response with the color search results.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the 
        /// MulticolorEngine API <c>color_search</c> request or parsing the response.</exception>
        public JObject SearchUrl(string imageUrl, JObject metadata, JArray returnMetadata,
                                 bool sortMetadata, bool ignoreBackground, bool ignoreInteriorBackground,
                                 int minScore, int offset, int limit)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                messageBuilder.Add("url",                        imageUrl);
                messageBuilder.Add("ignore_background",          ignoreBackground);
                messageBuilder.Add("ignore_interior_background", ignoreInteriorBackground);

                messageBuilder = this.AddExtraSearchOptions(messageBuilder, metadata, returnMetadata,
                                                            sortMetadata, minScore, offset, limit);

                return this.PostApiRequest("color_search", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("SearchUrl failed", e);
                throw new TinEyeServiceException("SearchUrl failed", e);
            }
        }

        /// <summary>
        /// Do a color search against the collection using specified colors and return matches with
        /// corresponding scores.
        /// </summary>
        /// <remarks>
        /// <para>Each color may have an associated weight, indicating how much of that color 
        /// should appear in a search result.</para>
        /// <note>If weights are used, there must be a weight for each color.</note>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>count</term>
        ///         <description>The number of search results</description></item>
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>color_search</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects for each match with the following fields:
        ///         <list type="table">
        ///            <item><term>score</term>
        ///                  <description>Relevance score of match</description></item>
        ///            <item><term>filepath</term>
        ///                  <description>The collection match filepath</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="colors">List of colors for searching the collection.</param>
        /// <param name="weights">List of weights corresponding to the colors, or empty list.
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
        /// <returns>The MulticolorEngine API JSON response with the color search results.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the 
        /// MulticolorEngine API <c>color_search</c> request or parsing the response.</exception>
        public JObject SearchColor(Color[] colors, float[] weights, JObject metadata, JArray returnMetadata,
                                   bool sortMetadata, int minScore, int offset, int limit)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    // Use RGB format for colors
                    string color = String.Format("{0},{1},{2}", new object[] { colors[i].R, colors[i].G, colors[i].B });
                    messageBuilder.Add("colors[" + i + "]", color);
                }

                for (int j = 0; j < weights.Length; j++)
                {
                    messageBuilder.Add("weights[" + j + "]", weights[j]);
                }

                messageBuilder = this.AddExtraSearchOptions(messageBuilder, metadata, returnMetadata,
                                                            sortMetadata, minScore, offset, limit);

                return this.PostApiRequest("color_search", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("SearchColor failed", e);
                throw new TinEyeServiceException("SearchColor failed", e);
            }
        }

        /// <summary>
        /// Search against the collection using metadata and return matches with
        /// corresponding scores.
        /// </summary>
        /// <remarks>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>count</term>
        ///         <description>The number of search results</description></item>
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>color_search</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects for each match with the following fields:
        ///         <list type="table">
        ///            <item><term>metadata_score</term>
        ///                  <description>Relevance score of match based on metadata</description></item>
        ///            <item><term>filepath</term>
        ///                  <description>The collection match filepath</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="metadata">Metadata to perform filtering on search results.
        /// </param>
        /// <param name="returnMetadata">Metadata fields to return with each match, which can
        /// include sorting options.</param>
        /// <param name="sortMetadata">If true, sort results by metadata score instead of by
        /// match score.</param>
        /// <param name="minScore">Minimum score of search results to return.</param>
        /// <param name="offset">Offset from start of search results to return.</param>
        /// <param name="limit">The maximum number of results to return.</param>
        /// <returns>The MulticolorEngine API JSON response with the search results.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the 
        /// MulticolorEngine API <c>color_search</c> request or parsing the response.</exception>
        public JObject SearchMetadata(JObject metadata, JArray returnMetadata, bool sortMetadata, 
                                      int minScore, int offset, int limit)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                messageBuilder = this.AddExtraSearchOptions(messageBuilder, metadata, returnMetadata,
                                                            sortMetadata, minScore, offset, limit);

                return this.PostApiRequest("color_search", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("SearchMetadata failed", e);
                throw new TinEyeServiceException("SearchMetadata failed", e);
            }
        }

        /// <summary>
        /// Extract the dominant colors from the images passed in.
        /// </summary>
        /// <remarks>
        /// <para>Color dominance is returned as a weight between 1 and 100 showing how much of the 
        /// color associated with the weight appears in the images.</para>
        /// <para>If the ignoreBackground option is set to true, an image region containing
        /// 75% or more of the image's edge pixels is considered a background region and its
        /// associated color is disgarded. This is useful if you have an image of objects
        /// on a solid background color that you don't want to have extracted.</para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>extract_image_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>The extracted color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>rank</term>
        ///                  <description>Integer value used to group images with similar color 
        ///                  palettes together. Results are sorted by rank</description></item>
        ///            <item><term>weight</term>
        ///                  <description>Float value between 1 and 100 indicating how much of that 
        ///                  color is in the images</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="images">The images to extract colors from.</param>
        /// <param name="limit">The maximum number of colors to be extracted.</param>
        /// <param name="ignoreBackground">If true, ignore the background color of the images. 
        /// If false, include the background color of the images.</param>
        /// <param name="ignoreInteriorBackground">If true, ignore regions that have the 
        /// same color as the background region but that are surrounded by non-background regions.</param>
        /// <param name="colorFormat">To be returned, must be either rgb or hex.</param>
        /// <returns>The MulticolorEngine API JSON response with the extracted colors.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the 
        /// MulticolorEngine API <c>extract_image_colors</c> request or parsing the response.</exception>
        public JObject ExtractImageColorsImage(Image[] images, int limit, bool ignoreBackground, bool ignoreInteriorBackground, string colorFormat)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < images.Length; i++)
                {
                    messageBuilder.Add("images[" + i + "]", images[i]);
                }
                messageBuilder.Add("limit",                      limit);
                messageBuilder.Add("ignore_background",          ignoreBackground);
                messageBuilder.Add("ignore_interior_background", ignoreInteriorBackground);
                messageBuilder.Add("color_format",               colorFormat);

                return this.PostApiRequest("extract_image_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ExtractImageColorsImage failed", e);
                throw new TinEyeServiceException("ExtractImageColorsImage failed", e);
            }
        }

        /// <summary>
        /// Extract the dominant colors from images at the given URLs.
        /// </summary>
        /// <remarks>
        /// <para>Color dominance is returned as a weight between 1 and 100 showing 
        /// how much of the color associated with the weight appears in the images.</para>
        /// <para>If the ignoreBackground option is set to true, an image region containing
        /// 75% or more of the image's edge pixels is considered a background region and its
        /// associated color is disgarded. This is useful if you have an image of objects
        /// on solid background color that you don't want to have extracted.</para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>extract_image_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>The extracted color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>rank</term>
        ///                  <description>Integer value used to group images with similar color 
        ///                  palettes together. Results are sorted by rank</description></item>
        ///            <item><term>weight</term>
        ///                  <description>Float value between 1 and 100 indicating how much of that 
        ///                  color is in the images</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="imageUrls">URLs to images to extract colors from.</param>
        /// <param name="limit">The maximum number of colors to be extracted.</param>
        /// <param name="ignoreBackground">If true, ignore the background color of the images. 
        /// If false, include the background color of the images.</param>
        /// <param name="ignoreInteriorBackground">If true, ignore regions that have the 
        /// same color as the background region but that are surrounded by non-background regions.</param>
        /// <param name="colorFormat">To return, must be either rgb or hex.</param>
        /// <returns>The MulticolorEngine API JSON response with the extracted colors.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the 
        /// MulticolorEngine API <c>extract_image_colors</c> request or parsing the response.</exception>
        public JObject ExtractImageColorsUrl(string[] imageUrls, int limit, bool ignoreBackground, bool ignoreInteriorBackground, string colorFormat)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < imageUrls.Length; i++)
                {
                    messageBuilder.Add("urls[" + i + "]", imageUrls[i]);
                }
                messageBuilder.Add("limit",                      limit);
                messageBuilder.Add("ignore_background",          ignoreBackground);
                messageBuilder.Add("ignore_interior_background", ignoreInteriorBackground);
                messageBuilder.Add("color_format",               colorFormat);

                return this.PostApiRequest("extract_image_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ExtractImageColorsUrl failed", e);
                throw new TinEyeServiceException("ExtractImageColorsUrl failed", e);
            }
        }

        /// <summary>
        /// Extract the dominant colors in the hosted image collection.
        /// </summary>
        /// <remarks>
        /// <para>Color dominance is returned as a weight between 1 and 100 showing
        /// how much of the color associated with the weight appears in the collection.</para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>extract_collection_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>The extracted color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>rank</term>
        ///                  <description>Integer value used to group images with similar color 
        ///                  palettes together. Results are sorted by rank</description></item>
        ///            <item><term>weight</term>
        ///                  <description>Float value between 1 and 100 indicating how much of that 
        ///                  color is in the collection</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="limit">The maximum number of colors to be extracted.</param>
        /// <param name="colorFormat">To return, must be either rgb or hex.</param>
        /// <returns>The MulticolorEngine API JSON response with the extracted colors.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>extract_collection_colors</c> request or parsing the response.
        /// </exception>
        public JObject ExtractCollectionColors(int limit, string colorFormat)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                messageBuilder.Add("limit", limit);
                messageBuilder.Add("color_format", colorFormat);

                return this.PostApiRequest("extract_collection_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ExtractCollectionColors failed", e);
                throw new TinEyeServiceException("ExtractCollectionColors failed", e);
            }
        }

        /// <summary>
        /// Extract the dominant colors in the hosted image collection filtered by colors.
        /// The colors passed in are used to get a set of images that have those colors, and
        /// then colors are extracted from that set of images and returned.
        /// </summary>
        /// <remarks>
        /// <para>Color dominance is returned as a weight between 1 and 100 showing
        /// how much of the color associated with the weight appears in the collection.</para>
        /// <para>Each passed in color may have an associated weight to indicate how much of a color
        /// should be in the set of images to extract colors from. If weights are included then 
        /// there must be one weight for each passed in color, each weight must be between
        /// 1 and 100 and all weights must add to 100.</para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>extract_collection_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>The extracted color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>rank</term>
        ///                  <description>Integer value used to group images with similar color 
        ///                  palettes together. Results are sorted by rank</description></item>
        ///            <item><term>weight</term>
        ///                  <description>Float value between 1 and 100 indicating how much of that 
        ///                  color is in the collection</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="colors">Array of colors used to filter the results.</param>
        /// <param name="weights">Array of color weights used to filter the results.</param>
        /// <param name="limit">The maximum number of colors to be extracted.</param>
        /// <param name="colorFormat">To return, must be either rgb or hex.</param>
        /// <returns>The MulticolorEngine API JSON response with the extracted colors.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>extract_collection_colors</c> request or parsing the response.
        /// </exception>
        public JObject ExtractCollectionColorsColors(Color[] colors, float[] weights, int limit, string colorFormat)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    // Use RGB format for colors
                    string color = String.Format("{0},{1},{2}", new object[] { colors[i].R, colors[i].G, colors[i].B });
                    messageBuilder.Add("colors[" + i + "]", color);

                    if (weights.Length > 0)
                    {
                        messageBuilder.Add("weights[" + i + "]", weights[i]);
                    }
                }
                messageBuilder.Add("limit",        limit);
                messageBuilder.Add("color_format", colorFormat);

                return this.PostApiRequest("extract_collection_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ExtractCollectionColorsColors failed", e);
                throw new TinEyeServiceException("ExtractCollectionColorsColors failed", e);
            }
        }

        /// <summary>
        /// Extract the dominant colors given the filepaths of images in the hosted 
        /// image collection.
        /// </summary>
        /// <remarks>
        /// <para>Color dominance is returned as a weight between 1 and 100 showing how much of the 
        /// color associated with the weight appears in the image.</para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>extract_collection_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>The extracted color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>rank</term>
        ///                  <description>Integer value used to group images with similar color 
        ///                  palettes together. Results are sorted by rank</description></item>
        ///            <item><term>weight</term>
        ///                  <description>Float value between 1 and 100 indicating how much of that 
        ///                  color is in the collection</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="filepaths">Filepaths to images in the hosted image collection to 
        /// extract colors from.</param>
        /// <param name="limit">The maximum number of colors to be extracted.</param>
        /// <param name="colorFormat">To return, must be either rgb or hex.</param>
        /// <returns>The MulticolorEngine API JSON response with the extracted colors.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>extract_collection_colors</c> request or parsing the response.
        /// </exception>
        public JObject ExtractCollectionColorsFilepath(string[] filepaths, int limit, string colorFormat)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < filepaths.Length; i++)
                {
                    messageBuilder.Add("filepaths[" + i + "]", filepaths[i]);
                }
                messageBuilder.Add("limit",        limit);
                messageBuilder.Add("color_format", colorFormat);

                return this.PostApiRequest("extract_collection_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ExtractCollectionColorsFilepath failed", e);
                throw new TinEyeServiceException("ExtractCollectionColorsFilepath failed", e);
            }
        }

        /// <summary>
        /// Extract the dominant colors in the hosted image collection filtered by
        /// image metadata. Metadata can be set to null to extract colors from all
        /// images in the collection.
        /// </summary>
        /// <remarks>
        /// <para>Color dominance is returned as a weight between 1 and 100 showing how much of the 
        /// color associated with the weight appears in the collection.</para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>extract_collection_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>The extracted color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>rank</term>
        ///                  <description>Integer value used to group images with similar color 
        ///                  palettes together. Results are sorted by rank</description></item>
        ///            <item><term>weight</term>
        ///                  <description>Float value between 1 and 100 indicating how much of that 
        ///                  color is in the collection</description></item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="metadata">Metadata to use to filter the results, can be null.</param>
        /// <param name="limit">The maximum number of colors to be extracted.</param>
        /// <param name="colorFormat">To return, must be either rgb or hex.</param>
        /// <returns>The MulticolorEngine API JSON response with the extracted colors.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>extract_collection_colors</c> request or parsing the response.
        /// </exception>
        public JObject ExtractCollectionColorsMetadata(JObject metadata, int limit, string colorFormat)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                if (metadata != null)
                {
                    messageBuilder.Add("metadata", metadata.ToString());
                }
                messageBuilder.Add("limit",        limit);
                messageBuilder.Add("color_format", colorFormat);

                return this.PostApiRequest("extract_collection_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ExtractCollectionColorsMetadata failed", e);
                throw new TinEyeServiceException("ExtractCollectionColorsMetadata failed", e);
            }
        }

        /// <summary>
        /// Upload a list of images to the API and a color palette to get a count for
        /// each color specifying how many of the input images contain that color.
        /// </summary>
        /// <remarks>
        /// <para>If the ignoreBackground option is set to true, an image region containing
        /// 75% or more of the image's edge pixels is considered a background region and its
        /// associated color is not counted. This is useful if you have an image of objects
        /// on a solid background color that you don't want to have counted.</para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>count_image_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>A color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>num_images_partial_area</term>
        ///                  <description>The number of input images that contain the palette color
        ///                  in any percentage</description></item>
        ///            <item><term>num_images_full_area</term>
        ///                  <description>The number of input images that contain the palette color 
        ///                  in a very large percentage</description>
        ///            </item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="images">The images to count colors from.</param>
        /// <param name="countColors">The colors to be counted in the images.</param>
        /// <param name="ignoreBackground">If true, ignore the background color of the images. 
        /// If false, include the background color of the images.</param>
        /// <param name="ignoreInteriorBackground">If true, ignore regions that have the 
        /// same color as the background region but that are surrounded by non-background regions.</param>
        /// <returns>The MulticolorEngine API JSON response with the count of palette colors
        /// in the passed in images.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the 
        /// MulticolorEngine API <c>count_image_colors</c> request or parsing the response.</exception>
        public JObject CountImageColorsImage(Image[] images, Color[] countColors, bool ignoreBackground, bool ignoreInteriorBackground)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < images.Length; i++)
                {
                    messageBuilder.Add("images[" + i + "]", images[i]);
                }

                for (int i = 0; i < countColors.Length; i++)
                {
                    // Use RGB format for colors
                    string color = String.Format("{0},{1},{2}", new object[] { countColors[i].R, countColors[i].G, countColors[i].B });
                    messageBuilder.Add("count_colors[" + i + "]", color);
                }

                messageBuilder.Add("ignore_background",          ignoreBackground);
                messageBuilder.Add("ignore_interior_background", ignoreInteriorBackground);
                
                return this.PostApiRequest("count_image_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CountImageColorsImage failed", e);
                throw new TinEyeServiceException("CountImageColorsImage failed", e);
            }
        }

        /// <summary>
        /// Upload a list of image URLs to the API and a color palette to get a count for
        /// each color specifying how many of the images at the given URLs contain that color.
        /// </summary>
        /// <remarks>
        /// <para>If the ignoreBackground option is set to true, an image region containing
        /// 75% or more of the image's edge pixels is considered a background region and its
        /// associated color is not counted. This is useful if you have an image of objects
        /// on a solid background color that you don't want to have counted.</para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>count_image_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>A color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>num_images_partial_area</term>
        ///                  <description>The number of input images that contain the palette color
        ///                  in any percentage</description></item>
        ///            <item><term>num_images_full_area</term>
        ///                  <description>The number of input images that contain the palette color 
        ///                  in a very large percentage</description>
        ///            </item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="imageUrls">The URLs to images to count colors from.</param>
        /// <param name="countColors">The colors to be counted in the images.</param>
        /// <param name="ignoreBackground">If true, ignore the background color of the images. 
        /// If false, include the background color of the images.</param>
        /// <param name="ignoreInteriorBackground">If true, ignore regions that have the 
        /// same color as the background region but that are surrounded by non-background regions.</param>
        /// <returns>The MulticolorEngine API JSON response with the count of palette colors
        /// in the passed in images.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the 
        /// MulticolorEngine API <c>count_image_colors</c> request or parsing the response.</exception>
        public JObject CountImageColorsUrl(string[] imageUrls, Color[] countColors, bool ignoreBackground, bool ignoreInteriorBackground)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < imageUrls.Length; i++)
                {
                    messageBuilder.Add("urls[" + i + "]", imageUrls[i]);
                }

                for (int i = 0; i < countColors.Length; i++)
                {
                    // Use RGB format for colors
                    string color = String.Format("{0},{1},{2}", new object[] { countColors[i].R, countColors[i].G, countColors[i].B });
                    messageBuilder.Add("count_colors[" + i + "]", color);
                }

                messageBuilder.Add("ignore_background",          ignoreBackground);
                messageBuilder.Add("ignore_interior_background", ignoreInteriorBackground);

                return this.PostApiRequest("count_image_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CountImageColorsUrl failed", e);
                throw new TinEyeServiceException("CountImageColorsUrl failed", e);
            }
        }

        /// <summary>
        /// Get counts for each color specified in a color palette (list of colors) from the
        /// images in the hosted image collection.
        /// </summary>
        /// <remarks>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>count_collection_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>A color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>num_images_partial_area</term>
        ///                  <description>The number of input images that contain the palette color
        ///                  in any percentage</description></item>
        ///            <item><term>num_images_full_area</term>
        ///                  <description>The number of input images that contain the palette color 
        ///                  in a very large percentage</description>
        ///            </item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="countColors">The colors to be counted in the images.</param>
        /// <returns>The MulticolorEngine API JSON response with the extracted colors.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>count_collection_colors</c> request or parsing the response.
        /// </exception>
        public JObject CountCollectionColors(Color[] countColors)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < countColors.Length; i++)
                {
                    // Use RGB format for colors
                    string color = String.Format("{0},{1},{2}", new object[] { countColors[i].R, countColors[i].G, countColors[i].B });
                    messageBuilder.Add("count_colors[" + i + "]", color);
                }

                return this.PostApiRequest("count_collection_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CountCollectionColors failed", e);
                throw new TinEyeServiceException("CountCollectionColors failed", e);
            }
        }

        /// <summary>
        /// Given a list of file paths from the hosted image collection, and a color palette
        /// (list of colors), get a count for each color specifying the number of images that
        /// contain the color.
        /// </summary>
        /// <remarks>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>count_collection_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>A color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>num_images_partial_area</term>
        ///                  <description>The number of input images that contain the palette color
        ///                  in any percentage</description></item>
        ///            <item><term>num_images_full_area</term>
        ///                  <description>The number of input images that contain the palette color 
        ///                  in a very large percentage</description>
        ///            </item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="filepaths">Filepaths to images in the hosted image collection to 
        /// count colors from.</param>
        /// <param name="countColors">The colors to be counted in the images.</param>
        /// <returns>The MulticolorEngine API JSON response with the extracted colors.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>count_collection_colors</c> request or parsing the response.
        /// </exception>
        public JObject CountCollectionColorsFilepath(string[] filepaths, Color[] countColors)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < filepaths.Length; i++)
                {
                    messageBuilder.Add("filepaths[" + i + "]", filepaths[i]);
                }

                for (int i = 0; i < countColors.Length; i++)
                {
                    // Use RGB format for colors
                    string color = String.Format("{0},{1},{2}", new object[] { countColors[i].R, countColors[i].G, countColors[i].B });
                    messageBuilder.Add("count_colors[" + i + "]", color);
                }

                return this.PostApiRequest("count_collection_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CountCollectionColorsFilepath failed", e);
                throw new TinEyeServiceException("CountCollectionColorsFilepath failed", e);
            }
        }

        /// <summary>
        /// Filter images in the hosted image collection by color and then get counts for
        /// each color specified in a color palette (list of colors) from the filtered image list.
        /// </summary>
        /// <remarks>
        /// <para>Each passed in filter color may have an associated weight to indicate how much of a
        /// color should be in the set of images to count colors from. If weights are included
        /// then there must be one weight for each passed in color, each weight must be between
        /// 1 and 100 and all weights must add to 100.</para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>count_collection_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>A color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>num_images_partial_area</term>
        ///                  <description>The number of input images that contain the palette color
        ///                  in any percentage</description></item>
        ///            <item><term>num_images_full_area</term>
        ///                  <description>The number of input images that contain the palette color 
        ///                  in a very large percentage</description>
        ///            </item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="colors">Array of colors to filter image collection.</param>
        /// <param name="weights">Array of color weights to filter image collection.</param>
        /// <param name="countColors">The colors to be counted in the images.</param>
        /// <returns>The MulticolorEngine API JSON response with the extracted colors.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>count_collection_colors</c> request or parsing the response.
        /// </exception>
        public JObject CountCollectionColorsColors(Color[] colors, float[] weights, Color[] countColors)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    // Use RGB format for colors
                    string color = String.Format("{0},{1},{2}", new object[] { colors[i].R, colors[i].G, colors[i].B });
                    messageBuilder.Add("colors[" + i + "]", color);

                    if (weights.Length > 0)
                    {
                        messageBuilder.Add("weights[" + i + "]", weights[i]);
                    }
                }

                for (int i = 0; i < countColors.Length; i++)
                {
                    // Use RGB format for colors
                    string color = String.Format("{0},{1},{2}", new object[] { countColors[i].R, countColors[i].G, countColors[i].B });
                    messageBuilder.Add("count_colors[" + i + "]", color);
                }

                return this.PostApiRequest("count_collection_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CountCollectionColorsColors failed", e);
                throw new TinEyeServiceException("CountCollectionColorsColors failed", e);
            }
        }

        /// <summary>
        /// Given a color palette (list of colors) and metadata, get a count for each color specifying
        /// the number of the hosted image collection images that contain that color, filtered by the
        /// metadata. Metadata can be set to null to count colors from all images in the collection.
        /// </summary>
        /// <remarks>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>count_collection_colors</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array of JSON objects with the following fields:
        ///         <list type="table">
        ///            <item><term>color</term>
        ///                  <description>The color in hex format, or an array with that 
        ///                  color's 3 RGB values</description></item>
        ///            <item><term>num_images_partial_area</term>
        ///                  <description>The number of input images that contain the palette color
        ///                  in any percentage</description></item>
        ///            <item><term>num_images_full_area</term>
        ///                  <description>The number of input images that contain the palette color 
        ///                  in a very large percentage</description>
        ///            </item>
        ///         </list>
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="metadata">Metadata to filter the hosted collection images returned that
        /// contain the given colors</param>
        /// <param name="countColors">The colors to be counted in the images.</param>
        /// <returns>The MulticolorEngine API JSON response with the extracted colors.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>count_collection_colors</c> request or parsing the response.
        /// </exception>
        public JObject CountCollectionColorsMetadata(JObject metadata, Color[] countColors)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                if (metadata != null)
                {
                    messageBuilder.Add("metadata", metadata.ToString());
                }

                for (int i = 0; i < countColors.Length; i++)
                {
                    // Use RGB format for colors
                    string color = String.Format("{0},{1},{2}", new object[] { countColors[i].R, countColors[i].G, countColors[i].B });
                    messageBuilder.Add("count_colors[" + i + "]", color);
                }

                return this.PostApiRequest("count_collection_colors", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CountCollectionColorsMetadata failed", e);
                throw new TinEyeServiceException("CountCollectionColorsMetadata failed", e);
            }
        }

        /// <summary>
        /// Given one of more metadata queries, get a counter for each query specifying how many
        /// of the collection images match the query. The images counted in the results may
        /// be filtered using json metadata.
        /// </summary>
        /// <remarks>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>count_metadata</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array with a single JSON object which has a metadata field
        ///         set to a list of JSON objects with the counts for each metadata query passed in,
        ///         along with the original metadata passed in
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="countMetadata">Array of metadata queries to get image counts for.</param>
        /// <param name="metadata">Metadata to filter the images counted. Set it to null if 
        /// it is not used.</param>
        /// <returns>The MulticolorEngine API JSON response with the count of images matching
        /// each metadata query passed in.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>count_metadata</c> request or parsing the response.
        /// </exception>
        public JObject CountMetadataCollection(JObject[] countMetadata, JObject metadata)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < countMetadata.Length; i++)
                {
                    messageBuilder.Add("count_metadata[" + i + "]", countMetadata[i].ToString());
                }

                if (metadata != null)
                {
                    messageBuilder.Add("metadata", metadata.ToString());
                }

                return this.PostApiRequest("count_metadata", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CountMetadataCollection failed", e);
                throw new TinEyeServiceException("CountMetadataCollection failed", e);
            }
        }

        /// <summary>
        /// Given one of more colors and metadata queries, get a counter specifying
        /// how many of the collection images with the given colors match each query.
        /// </summary>
        /// <remarks>
        /// <para>A weight may be given for each color indicating how much of a color should 
        /// appear in a matching image. Weights must be given for each color if weights are 
        /// included, and each weight must be between 1 and 100 and they all must add to 100.
        /// </para>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>count_metadata</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array with a single JSON object which has a metadata field
        ///         set to a list of JSON objects with the counts for each image with matching colors,
        ///         for each metadata query passed in.
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="countMetadata">Array of metadata queries to get image counts for.</param>
        /// <param name="colors">Array of colors in the images to count.</param>
        /// <param name="weights">Array of color weights for each color included the
        /// images to count (optional).</param>
        /// <returns>The MulticolorEngine API JSON response with the count of images matching
        /// the colors and each metadata query passed in.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>count_metadata</c> request or parsing the response.
        /// </exception>
        public JObject CountMetadataColors(JObject[] countMetadata, Color[] colors, float[] weights)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < countMetadata.Length; i++)
                {
                    messageBuilder.Add("count_metadata[" + i + "]", countMetadata[i].ToString());
                }

                for (int j = 0; j < colors.Length; j++)
                {
                    // Use RGB format for colors
                    string color = String.Format("{0},{1},{2}", new object[] { colors[j].R, colors[j].G, colors[j].B });
                    messageBuilder.Add("colors[" + j + "]", color);

                    // weights must be the same length as the colors if the weights list is not empty.
                    if (weights.Length > 0)
                    {
                        messageBuilder.Add("weights[" + j + "]", weights[j]);
                    }
                }

                return this.PostApiRequest("count_metadata", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CountMetadataColors failed", e);
                throw new TinEyeServiceException("CountMetadataColors failed", e);
            }
        }

        /// <summary>
        /// Given one or more hosted image filepaths and metadata queries, get a counter
        /// specifying how many of the images from the specified filepaths match each query.
        /// </summary>
        /// <remarks>
        /// <para>Returns the MulticolorEngine API JSON response with the following fields:</para>
        /// <list type="table">
        ///   <item><term>status</term>
        ///         <description>One of <c>ok</c>, <c>warn</c> or <c>fail</c></description></item>
        ///   <item><term>method</term>
        ///         <description><c>count_metadata</c></description></item>
        ///   <item><term>result</term>
        ///         <description>Array with a single JSON object which has a metadata field
        ///         set to a list of JSON objects with the counts for each image from the 
        ///         given filepaths that match each metadata query passed in.
        ///         </description></item>
        ///   <item><term>error</term>
        ///         <description>Array of error messages if status is not <c>ok</c></description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="countMetadata">Array of metadata queries to get image counts for.</param>
        /// <param name="filepaths">Array of hosted image filepaths to get metadata counts for.
        /// </param>
        /// <returns>The MulticolorEngine API JSON response with the count of images matching
        /// each metadata query passed in.</returns>
        /// <exception cref="TinEyeServiceException">If an exception occurs issuing the
        /// MulticolorEngine API <c>count_metadata</c> request or parsing the response.
        /// </exception>
        public JObject CountMetadataFilepaths(JObject[] countMetadata, string[] filepaths)
        {
            HttpMessageBuilder messageBuilder = new HttpMessageBuilder();

            try
            {
                for (int i = 0; i < countMetadata.Length; i++)
                {
                    messageBuilder.Add("count_metadata[" + i + "]", countMetadata[i].ToString());
                }

                for (int j = 0; j < filepaths.Length; j++)
                {
                    messageBuilder.Add("filepaths[" + j + "]", filepaths[j]);
                }

                return this.PostApiRequest("count_metadata", messageBuilder.ToArray(), messageBuilder.Boundary);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("CountMetadataFilepaths failed", e);
                throw new TinEyeServiceException("CountMetadataFilepaths failed", e);
            }
        }
    }
}
