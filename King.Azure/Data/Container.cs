﻿namespace King.Azure.Data
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Container
    /// </summary>
    public class Container : AzureStorage, IContainer
    {
        #region Members
        /// <summary>
        /// Client
        /// </summary>
        private readonly CloudBlobClient client;

        /// <summary>
        /// Reference
        /// </summary>
        private readonly CloudBlobContainer reference;

        /// <summary>
        /// Is Public
        /// </summary>
        private readonly bool isPublic = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Queue
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="connectionString">Connection String</param>
        /// <param name="isPublic">Is Public</param>
        public Container(string name, string connectionString, bool isPublic = false)
            : base(connectionString)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            this.client = this.Account.CreateCloudBlobClient();
            this.reference = this.client.GetContainerReference(name);
            this.isPublic = isPublic;
        }

        /// <summary>
        /// Queue
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="account">Storage Account</param>
        /// <param name="isPublic">Is Public</param>
        public Container(string name, CloudStorageAccount account, bool isPublic = false)
            : base(account)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            this.client = this.Account.CreateCloudBlobClient();
            this.reference = this.client.GetContainerReference(name);
            this.isPublic = isPublic;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Table Name
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.reference.Name;
            }
        }

        /// <summary>
        /// Is Public
        /// </summary>
        public virtual bool IsPublic
        {
            get
            {
                return this.isPublic;
            }
        }

        /// <summary>
        /// Client
        /// </summary>
        public virtual CloudBlobClient Client
        {
            get
            {
                return this.client;
            }
        }

        /// <summary>
        /// Reference
        /// </summary>
        public virtual CloudBlobContainer Reference
        {
            get
            {
                return this.reference;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create If Not Exists
        /// </summary>
        /// <returns>Created</returns>
        public virtual async Task<bool> CreateIfNotExists()
        {
            var result = await this.reference.CreateIfNotExistsAsync();
            if (result)
            {
                var permissions = new BlobContainerPermissions()
                {
                    PublicAccess = this.isPublic ? BlobContainerPublicAccessType.Blob : BlobContainerPublicAccessType.Off
                };

                await this.reference.SetPermissionsAsync(permissions);
            }

            return result;
        }

        /// <summary>
        /// Delete Container
        /// </summary>
        /// <returns>Task</returns>
        public virtual async Task Delete()
        {
            await this.reference.DeleteAsync();
        }

        /// <summary>
        /// Delete from Blob Storage
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Object</returns>
        public virtual async Task Delete(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            var blob = this.GetReference(blobName);
            await blob.DeleteAsync();
        }

        /// <summary>
        /// Blob Exists
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>bool</returns>
        public virtual async Task<bool> Exists(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            var blob = this.GetReference(blobName);
            return await blob.ExistsAsync();
        }

        /// <summary>
        /// Save Object as Json to Blob Storage
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="blobName">Blob Name</param>
        /// <param name="obj">Object</param>
        /// <returns>Task</returns>
        public virtual async Task Save(string blobName, object obj)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }

            var json = JsonConvert.SerializeObject(obj);

            await this.Save(blobName, json, "application/json");
        }

        /// <summary>
        /// Save Text
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="text">Text</param>
        /// <param name="contentType">Content Type</param>
        /// <returns>Task</returns>
        public virtual async Task Save(string blobName, string text, string contentType = "text/plain")
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("text");
            }

            var blob = this.GetReference(blobName);
            await blob.UploadTextAsync(text);

            blob.Properties.ContentType = contentType;
            await blob.SetPropertiesAsync();
        }

        /// <summary>
        /// Get Object from Blob Storage
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Object</returns>
        public virtual async Task<T> Get<T>(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            var blob = this.GetReference(blobName);
            var json = await blob.DownloadTextAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Get Bytes
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>bytes</returns>
        public virtual async Task<byte[]> Get(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            var blob = this.GetReference(blobName);
            await blob.FetchAttributesAsync();
            var bytes = new byte[blob.Properties.Length];
            await blob.DownloadToByteArrayAsync(bytes, 0);

            return bytes;
        }

        /// <summary>
        /// Get Bytes
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Text</returns>
        public virtual async Task<string> GetText(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            var blob = this.GetReference(blobName);
            return await blob.DownloadTextAsync();
        }

        /// <summary>
        /// Save Bytes
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="bytes">bytes</param>
        /// <returns>Task</returns>
        public virtual async Task Save(string blobName, byte[] bytes, string contentType = "application/octet-stream")
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }
            if (null == bytes)
            {
                throw new ArgumentNullException("bytes");
            }

            var blob = this.GetReference(blobName);
            await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
            blob.Properties.ContentType = contentType;
            await blob.SetPropertiesAsync();
        }

        /// <summary>
        /// Blob Properties
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Blob Container Properties</returns>
        public virtual async Task<BlobProperties> Properties(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            var blob = this.GetReference(blobName);
            await blob.FetchAttributesAsync();
            return blob.Properties;
        }

        /// <summary>
        /// Set Cache Control
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="cacheDuration">Cache Duration (Default 1 year)</param>
        /// <returns>Task</returns>
        public virtual async Task SetCacheControl(string blobName, int cacheDuration = 31536000)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            cacheDuration = cacheDuration < 1 ? 31536000 : cacheDuration;

            var blob = this.GetReference(blobName);
            blob.Properties.CacheControl = string.Format("public, max-age={0}", cacheDuration);
            await blob.SetPropertiesAsync();
        }

        /// <summary>
        /// Get Reference
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Cloud Block Blob</returns>
        public virtual CloudBlockBlob GetReference(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            return this.reference.GetBlockBlobReference(blobName);
        }

        /// <summary>
        /// Get Stream
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Stream</returns>
        public virtual async Task<Stream> Stream(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            var properties = await this.Properties(blobName);
            var blob = this.GetReference(blobName);
            var stream = new MemoryStream();
            await blob.DownloadRangeToStreamAsync(stream, 0, properties.Length);
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// List Blobs
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="useFlatBlobListing">Use Flat Blob Listing</param>
        /// <returns>Blobs</returns>
        public IEnumerable<IListBlobItem> List(string prefix = null, bool useFlatBlobListing = false)
        {
            return this.reference.ListBlobs(prefix, useFlatBlobListing);
        }

        /// <summary>
        /// Create Snapshot
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Task</returns>
        public async Task<ICloudBlob> Snapshot(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            var exists = await this.Exists(blobName);
            if (exists)
            {
                var blob = await this.reference.GetBlobReferenceFromServerAsync(blobName);

                var block = blob as CloudBlockBlob;
                if (null != block)
                {
                    return await block.CreateSnapshotAsync();
                }

                var page = blob as CloudPageBlob;
                if (null != page)
                {
                    return await page.CreateSnapshotAsync();
                }

            }

            return null;
        }
        #endregion
    }
}