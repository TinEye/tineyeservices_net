using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Newtonsoft.Json.Linq;

namespace TinEye.Services
{
    /// <summary>
    /// <para>The Image class is used to represent images corresponding 
    /// to API collection images.</para>
    /// <para>Copyright (C) 2011-2012 Idee Inc. All rights reserved worldwide.</para>
    /// </summary>
    /// <remarks>
    /// <para>There are two types of images represented: those to be uploaded
    /// using the image data and those that have an image URL but no local
    /// image data.</para>
    /// <example><code>
    /// // Make an Image based on a local image:
    /// Image localImageToUpload = new Image(@"C:\some\dir\with\myImage.jpg");
    /// 
    /// // Make an Image based on a image with a given URL:
    /// Uri imageUrl = new Uri("http://www.mywebsite.com/images/myImage.jpg", 
    ///                        "/imagepath/in/collection/image.jpg");
    /// 
    /// Image imageFromUrl = new Image(imageUrl);
    /// </code></example>
    /// <para>For images to be used by the MulticolorEngine API, images can be 
    /// created with JSON keyword metadata.</para>
    /// </remarks>
    public class Image
    {
        private readonly string filepath;
        private readonly string collectionFilepath;
        private readonly string imageUrl;
        private readonly JObject metadata;

        private byte[] data;
        
        /// <summary>
        /// Gets the local filepath for this image.
        /// </summary>
        /// <value>The local filepath for this image.
        /// <note>The filepath will not be set if the image URL is set.</note></value>
        public string Filepath
        {
            get { return filepath; }
        }

        /// <summary>
        /// Gets the filepath for this image or to set for this image in the API image collection.
        /// </summary>
        /// <value>The path to the image in the API image collection.
        /// <note>This does not have to match the image URL or local filepath. 
        /// The image may be renamed as well using the collection filepath.</note></value>
        public string CollectionFilepath
        {
            get { return collectionFilepath; }
        }

        /// <summary>
        /// Gets the URL of this image.
        /// </summary>
        /// <value>The image URL.
        /// <note>The URL is not set for images that have their filepath set.</note></value>
        public string ImageURL
        {
            get { return imageUrl; }
        }

        /// <summary>
        /// Gets keyword metadata associated or to associate with this image in the API image 
        /// collection.
        /// </summary>
        /// <remarks>
        /// <note>Currently JSON metadata keywords are only available for the MulticolorEngine API.
        /// Refer to the API documentation for more information about JSON metadata keywords.
        /// </note>
        /// </remarks>
        /// <value>The JSON metadata keywords associated with the image in the API image 
        /// collection.</value>
        public JObject Metadata
        {
            get { return metadata; }
        }

        /// <summary>
        /// Gets the image data for this image. 
        /// </summary>
        /// <value>The image byte data.
        /// <note>This is only set if the image local filepath is set, it is not set if only 
        /// the image URL is set.</note></value>
        public byte[] Data
        {
            get { return data; }
        }

        /// <summary>
        /// Initializes a new instance of the Image class using the local filepath to the image.
        /// The data for the image is read for the Image when initialized.
        /// </summary>
        /// <param name="filepath">The local filepath to the image.</param>
        /// <exception cref="ArgumentException"><paramref name="filepath"/> is is a zero-length 
        /// string, contains only white space, or contains one or more invalid characters as 
        /// defined by <see cref="System.IO.Path.InvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="filepath"/> is Nothing.
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or 
        /// both exceed the system-defined maximum length. For example, on Windows-based platforms,
        /// paths must be less than 248 characters, and file names must be less than 260 
        /// characters.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid 
        /// (for example, it is on an unmapped drive).</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred while opening the file.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">The file read operation is not supported
        /// on the current platform, <paramref name="filepath"/> specified a directory, or
        /// the caller does not have the required permission.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file specified in 
        /// <paramref name="filepath"/> was not found.</exception>
        /// <exception cref="NotSupportedException"><paramref name="filepath"/> is in an invalid
        /// format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        public Image(string filepath) 
            : this(filepath, null, null) { }

        /// <summary>
        /// Initializes a new instance of the Image class using the local filepath to the image,
        /// to be associated with the <paramref name="collectionFilepath"/> in the API image 
        /// collection. The data for the image is read for the Image when initialized.
        /// </summary>
        /// <param name="filepath">The local filepath to the image.</param>
        /// <param name="collectionFilepath">The filepath to the image in the API image collection.
        /// </param>
        /// <exception cref="ArgumentException"><paramref name="filepath"/> is is a zero-length 
        /// string, contains only white space, or contains one or more invalid characters as 
        /// defined by <see cref="System.IO.Path.InvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="filepath"/> is Nothing.
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or 
        /// both exceed the system-defined maximum length. For example, on Windows-based platforms,
        /// paths must be less than 248 characters, and file names must be less than 260 
        /// characters.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid 
        /// (for example, it is on an unmapped drive).</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred while opening the file.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">The file read operation is not supported
        /// on the current platform, <paramref name="filepath"/> specified a directory, or
        /// the caller does not have the required permission.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file specified in 
        /// <paramref name="filepath"/> was not found.</exception>
        /// <exception cref="NotSupportedException"><paramref name="filepath"/> is in an invalid
        /// format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        public Image(string filepath, string collectionFilepath)
            : this(filepath, collectionFilepath, null) { }

        /// <summary>
        /// Initializes a new instance of the Image class using the local filepath to the image,
        /// to be associated with the <paramref name="metadata"/> keywords in the API image 
        /// collection. The data for the image is read for the Image when initialized.
        /// </summary>
        /// <remarks>
        /// <note>Currently JSON metadata keywords are only available for the MulticolorEngine API.
        /// Refer to the API documentation for more information about JSON metadata keywords.
        /// </note>
        /// </remarks>
        /// <param name="filepath">The local filepath to the image.</param>
        /// <param name="metadata">Keywords to associate with the image in the API image 
        /// collection.</param>
        /// <exception cref="ArgumentException"><paramref name="filepath"/> is is a zero-length 
        /// string, contains only white space, or contains one or more invalid characters as 
        /// defined by <see cref="System.IO.Path.InvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="filepath"/> is Nothing.
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or 
        /// both exceed the system-defined maximum length. For example, on Windows-based platforms,
        /// paths must be less than 248 characters, and file names must be less than 260 
        /// characters.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid 
        /// (for example, it is on an unmapped drive).</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred while opening the file.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">The file read operation is not supported
        /// on the current platform, <paramref name="filepath"/> specified a directory, or
        /// the caller does not have the required permission.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file specified in 
        /// <paramref name="filepath"/> was not found.</exception>
        /// <exception cref="NotSupportedException"><paramref name="filepath"/> is in an invalid
        /// format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        public Image(string filepath, JObject metadata)
            : this(filepath, null, metadata) { }

        /// <summary>
        /// Initializes a new instance of the Image class using the local filepath to the image,
        /// to be associated with the <paramref name="collectionFilepath"/> and 
        /// <paramref name="metadata"/> keywords in the API image collection. The data for the 
        /// image is read for the Image when initialized.
        /// </summary>
        /// <remarks>
        /// <note>Currently JSON metadata keywords are only available for the MulticolorEngine API.
        /// Refer to the API documentation for more information about JSON metadata keywords.
        /// </note>
        /// </remarks>
        /// <param name="filepath">The local filepath to the image.</param>
        /// <param name="collectionFilepath">The filepath to the image in the API image collection.
        /// </param>
        /// <param name="metadata">Keywords to associate with the image in the API image 
        /// collection.</param>
        /// <exception cref="ArgumentException"><paramref name="filepath"/> is is a zero-length 
        /// string, contains only white space, or contains one or more invalid characters as 
        /// defined by <see cref="System.IO.Path.InvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="filepath"/> is Nothing.
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or 
        /// both exceed the system-defined maximum length. For example, on Windows-based platforms,
        /// paths must be less than 248 characters, and file names must be less than 260 
        /// characters.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid 
        /// (for example, it is on an unmapped drive).</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred while opening the file.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">The file read operation is not supported
        /// on the current platform, <paramref name="filepath"/> specified a directory, or
        /// the caller does not have the required permission.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file specified in 
        /// <paramref name="filepath"/> was not found.</exception>
        /// <exception cref="NotSupportedException"><paramref name="filepath"/> is in an invalid
        /// format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        public Image(string filepath, string collectionFilepath, JObject metadata)
        {
            this.filepath = filepath;
            this.imageUrl = null;
            this.collectionFilepath = collectionFilepath;
            this.metadata = metadata;

            ReadData();
        }

        /// <summary>
        /// Initializes a new instance of the Image class with the image <paramref name="url"/>.
        /// </summary>
        /// <remarks><note>Images created with an image URL are not read and do not have
        /// the <c>filepath</c> set.</note></remarks>
        /// <param name="url">The image URL.</param>
        public Image(Uri url) 
            : this(url, null, null) { }

        /// <summary>
        /// Initializes a new instance of the Image class with the image <paramref name="url"/>
        /// to associate with the <paramref name="collectionFilepath"/> in the API image 
        /// collection.
        /// </summary>
        /// <remarks><note>Images created with an image URL are not read and do not have
        /// the <c>filepath</c> set.</note></remarks>
        /// <param name="url">The image URL.</param>
        /// <param name="collectionFilepath">The filepath to associate with the image in the API
        /// image collection.</param>
        public Image(Uri url, string collectionFilepath) 
            : this(url, collectionFilepath, null) { }

        /// <summary>
        /// Initializes a new instance of the Image class with the image <paramref name="url"/>
        /// to associate with the JSON <paramref name="metadata"/> keywords in the API image 
        /// collection.
        /// </summary>
        /// <remarks>
        /// <note>Images created with an image URL are not read and do not have
        /// the <c>filepath</c> set.</note>
        /// <note>Currently JSON metadata keywords are only available for the MulticolorEngine API.
        /// Refer to the API documentation for more information about JSON metadata keywords.
        /// </note>
        /// </remarks>
        /// <param name="url">The image URL.</param>
        /// <param name="metadata">JSON metadata keywords to associate with the image in the API
        /// image collection.</param>
        public Image(Uri url, JObject metadata)
            : this(url, null, metadata) { }

        /// <summary>
        /// Initializes a new instance of the Image class with the image <paramref name="url"/>
        /// to associate with the <paramref name="collectionFilepath"/> and JSON 
        /// <paramref name="metadata"/> keywords in the API image collection.
        /// </summary>
        /// <remarks>
        /// <note>Images created with an image URL are not read and do not have
        /// the <c>filepath</c> set.</note>
        /// <note>Currently JSON metadata keywords are only available for the MulticolorEngine API.
        /// Refer to the API documentation for more information about JSON metadata keywords.
        /// </note>
        /// </remarks>
        /// <param name="url">The image URL.</param>
        /// <param name="collectionFilepath">The filepath to associate with the image in the API
        /// image collection.</param>
        /// <param name="metadata">JSON metadata keywords to associate with the image in the API
        /// image collection.</param>
        public Image(Uri url, string collectionFilepath, JObject metadata)
        {
            this.filepath = null;
            this.data = null;
            this.imageUrl = url.ToString();
            this.collectionFilepath = collectionFilepath;
            this.metadata = metadata;
        }

        /// <summary>
        /// Read the data for the image.
        /// </summary>
        /// <exception cref="ArgumentException">Image <c>filepath</c> is is a zero-length 
        /// string, contains only white space, or contains one or more invalid characters as 
        /// defined by <see cref="System.IO.Path.InvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException">Image <c>filepath</c> is Nothing.
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or 
        /// both exceed the system-defined maximum length. For example, on Windows-based platforms,
        /// paths must be less than 248 characters, and file names must be less than 260 
        /// characters.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid 
        /// (for example, it is on an unmapped drive).</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred while opening the file.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">The file read operation is not supported
        /// on the current platform, the Image <c>filepath</c> specified a directory, or
        /// the caller does not have the required permission.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file specified by the 
        /// Image <c>filepath</c> was not found.</exception>
        /// <exception cref="NotSupportedException">Image <c>filepath</c> is in an invalid
        /// format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        protected void ReadData()
        {
            if (File.Exists(filepath))
            {
                data = File.ReadAllBytes(filepath);
            }
            else
            {
                throw new IOException("Image " + filepath + " not found");
            }
        }
    }
}
