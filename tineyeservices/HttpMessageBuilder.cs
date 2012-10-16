using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace TinEye.Services
{
    /// <summary>
    /// <para>Class used to make a message to POST.</para>
    /// <para>Copyright (C) 2011-2012 Idee Inc. All rights reserved worldwide.</para>
    /// </summary>
    /// <remarks>
    /// Different field types and/or images can be added to the POST message using the <c>Add</c>
    /// methods. Once all fields and images have been added, the full POST message to send can
    /// be generated in bytes using the <c>ToArray</c> method.
    /// <example><code>
    /// // Example how to build a POST message for a MatchEngine search call.
    /// Image img = new Image(@"C:\path\to\image.jpg", "/api/path/to/image.jpg");
    /// 
    /// HttpMessageBuilder messageBuilder = new HttpMesssageBuilder();
    /// 
    /// messageBuilder.Add("image", img);
    /// messageBuilder.Add("offset", 0);
    /// messageBuilder.Add("min_score", 1);
    /// messageBuilder.Add("limit", 100);
    /// messageBuilder.Add("check_horizontal_flip", false);
    /// 
    /// byte[] fullMessageToPost = messageBuilder.ToArray();
    /// </code></example>
    /// </remarks>
    public class HttpMessageBuilder
    {
        private Dictionary<string, string> postFields;
        private Dictionary<string, Image> postImages;

        private readonly string boundary;

        /// <summary>
        /// Gets the boundary string used to separate the POST message fields for this 
        /// HttpMessageBuilder instance.
        /// </summary>
        /// <value>The boundary string used to separate the POST message fields.</value>
        public string Boundary
        {
            get { return boundary; }
        }

        /// <summary>
        /// Initializes a new instance of HttpMessageBuilder.
        /// </summary>
        public HttpMessageBuilder()
        {
            // Store POST message parts in postFields and postImages.
            postFields = new Dictionary<string, string>();
            postImages = new Dictionary<string, Image>();

            // Boundary string used to separated different
            // fields and images in the POST message body.
            boundary = "---------------------" + DateTime.Now.Ticks.ToString("x");
        }

        /// <summary>
        /// Add a new text field to the POST message.
        /// </summary>
        /// <param name="fieldName">Name of POST message field.</param>
        /// <param name="fieldValue">Value of POST message field.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fieldName"/> is Nothing or
        /// <paramref name="fieldValue"/> is Nothing.</exception>
        /// <exception cref="ArgumentException">The POST message already has a field named 
        /// <paramref name="fieldName"/>.</exception>
        public void Add(string fieldName, string fieldValue)
        {
            if (fieldValue == null)
                throw new ArgumentNullException("fieldValue is null");

            postFields.Add(fieldName, fieldValue);
        }

        /// <summary>
        /// Add a new int field to the POST message.
        /// </summary>
        /// <param name="fieldName">Name of POST message field.</param>
        /// <param name="fieldValue">Value of POST message field.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fieldName"/> is Nothing.
        /// </exception>
        /// <exception cref="ArgumentException">The POST message already has a field named 
        /// <paramref name="fieldName"/>.</exception>
        public void Add(string fieldName, int fieldValue)
        {
            this.Add(fieldName, fieldValue.ToString());
        }

        /// <summary>
        /// Add a new boolean field to the POST message.
        /// </summary>
        /// <param name="fieldName">Name of POST message field.</param>
        /// <param name="fieldValue">Value of POST message field.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fieldName"/> is Nothing.
        /// </exception>
        /// <exception cref="ArgumentException">The POST message already has a field named 
        /// <paramref name="fieldName"/>.</exception>
        public void Add(string fieldName, bool fieldValue)
        {
            this.Add(fieldName, fieldValue.ToString().ToLower());
        }

        /// <summary>
        /// Add a new float field to the POST message.
        /// </summary>
        /// <param name="fieldName">Name of POST message field.</param>
        /// <param name="fieldValue">Value of POST message field.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fieldName"/> is Nothing.
        /// </exception>
        /// <exception cref="ArgumentException">The POST message already has a field named 
        /// <paramref name="fieldName"/>.</exception>
        public void Add(string fieldName, float fieldValue)
        {
            this.Add(fieldName, fieldValue.ToString());
        }

        /// <summary>
        /// Add an image to the POST message.
        /// </summary>
        /// <param name="fieldName">Name of POST message field.</param>
        /// <param name="image">The image to add to the POST message.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fieldName"/> is Nothing.
        /// </exception>
        /// <exception cref="ArgumentException">The POST message already has a field 
        /// named <paramref name="fieldName"/>.</exception>
        public void Add(string fieldName, Image image)
        {
            postImages.Add(fieldName, image);
        }

        /// <summary>
        /// Put all non-image fields and images for this HttpMessageBuilder instance into proper 
        /// HTTP POST message format and return the message as a byte array.
        /// </summary>
        /// <returns>The HTTP POST message contents in a byte array.</returns>
        /// <exception cref="HttpMessageBuilderException">If an error occurs converting the POST
        /// message to a byte array.</exception>
        public byte[] ToArray()
        {
            try
            {
                byte[] nonImageFields = this.GetNonImagePostFields();
                byte[] imageFields = this.GetImagePostFields();

                using (MemoryStream memStream = new MemoryStream())
                {
                    if (nonImageFields.Length > 0)
                    {
                        memStream.Write(nonImageFields, 0, nonImageFields.Length);
                    }

                    if (imageFields.Length > 0)
                    {
                        memStream.Write(imageFields, 0, imageFields.Length);
                    }

                    // Write trailing boundary to end POST message body - must end with '--'.
                    string footerBoundary = "--" + boundary + "--";
                    byte[] footerBoundaryBytes = System.Text.Encoding.UTF8.GetBytes(footerBoundary);

                    memStream.Write(footerBoundaryBytes, 0, footerBoundaryBytes.Length);

                    return memStream.ToArray();
                }
            }
            catch (HttpMessageBuilderException hmbe)
            {
                throw hmbe;
            }
            catch (Exception e)
            {
                throw new HttpMessageBuilderException("ToArray failed", e);
            }
        }

        /// <summary>
        /// Put all non-image fields for this HttpMessageBuilder instance into proper format for 
        /// an HTTP POST request and convert the message body to a array of corresponding UTF8 
        /// bytes.
        /// </summary>
        /// <returns>The non-image fields of a POST message as a byte array.</returns>
        /// <exception cref="HttpMessageBuilderException">If an error occurs generating the POST
        /// message non-image fields.</exception>
        protected byte[] GetNonImagePostFields()
        {
            try
            {
                // POST message field template for non-image data.
                string fieldTemplate =
                    "--{0}\r\nContent-Disposition: form-data; name=\"{1}\";\r\n\r\n{2}\r\n";

                StringBuilder fields = new StringBuilder();

                // Add all the non-image message fields to the buffer.
                foreach (string postField in postFields.Keys)
                {
                    fields.Append(String.Format(fieldTemplate, boundary, postField, postFields[postField]));
                }

                // Convert POST message fields to unicode bytes
                return System.Text.Encoding.UTF8.GetBytes(fields.ToString());
            }
            catch (Exception e)
            {
                throw new HttpMessageBuilderException("Failed to generate POST message from non-image fields", e);
            }
        }

        /// <summary>
        /// Put all images for this HttpMessageBuilder instance into proper format for an HTTP POST
        /// request and convert the message body to an array of bytes.
        /// </summary>
        /// <returns>The POST message body containing images to POST as a byte array.</returns>
        /// <exception cref="HttpMessageBuilderException">If an error occurs generating the POST
        /// message image fields.</exception>
        protected byte[] GetImagePostFields()
        {
            // Message field template for image data.
            string imageFieldTemplate =
                "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: image/{3}\r\n\r\n";
            
            // Trailing newline and carriage return. Have to add this after every image in POST message.
            byte[] trailingLine =  System.Text.Encoding.UTF8.GetBytes("\r\n");

            MemoryStream memStream = new MemoryStream();

            try
            {
                foreach (string fieldName in postImages.Keys)
                {
                    string imageType = Path.GetExtension(postImages[fieldName].Filepath).Replace(".", "");
                    if (imageType == "jpg")
                    {
                        imageType = "jpeg";
                    }

                    string imageFilename = Path.GetFileName(postImages[fieldName].Filepath);
                    string imageHeader =
                        String.Format(imageFieldTemplate, this.boundary, fieldName, imageFilename, imageType);

                    byte[] imageHeaderBytes = System.Text.Encoding.UTF8.GetBytes(imageHeader);

                    memStream.Write(imageHeaderBytes, 0, imageHeaderBytes.Length);
                    memStream.Write(postImages[fieldName].Data, 0, postImages[fieldName].Data.Length);
                    memStream.Write(trailingLine, 0, trailingLine.Length);
                }
                return memStream.ToArray();
            }
            catch (Exception e)
            {
                throw new HttpMessageBuilderException("Failed to generate POST message from image fields", e);
            }
            finally
            {
                if (memStream != null)
                    ((IDisposable)memStream).Dispose();
            }
        }
    }
}
