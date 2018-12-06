namespace Azure.Data.Wrappers
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Blob Container
    /// </summary>
    public class Container : AzureStorage, IContainer
    {
        #region Members
        /// <summary>
        /// Default Cache Duration
        /// </summary>
        public const uint DefaultCacheDuration = 31536000;

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
        /// Container Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="connectionString">Connection String</param>
        /// <param name="isPublic">Is Public</param>
        /// <param name="location">Location Mode</param>
        public Container(string name, string connectionString, bool isPublic = false, LocationMode location = LocationMode.PrimaryThenSecondary)
            : this(name, CloudStorageAccount.Parse(connectionString), isPublic, location)
        {
        }

        /// <summary>
        /// Container Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="account">Storage Account</param>
        /// <param name="isPublic">Is Public</param>
        /// <param name="location">Location Mode</param>
        public Container(string name, CloudStorageAccount account, bool isPublic = false, LocationMode location = LocationMode.PrimaryThenSecondary)
            : base(account)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            this.client = this.Account.CreateCloudBlobClient();
            this.client.DefaultRequestOptions.LocationMode = location;
            this.reference = this.client.GetContainerReference(name);
            this.isPublic = isPublic;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Name
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
            var result = await this.reference.CreateIfNotExistsAsync().ConfigureAwait(false);
            if (result)
            {
                var permissions = new BlobContainerPermissions()
                {
                    PublicAccess = this.isPublic ? BlobContainerPublicAccessType.Blob : BlobContainerPublicAccessType.Off
                };

                await this.reference.SetPermissionsAsync(permissions).ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Delete Container
        /// </summary>
        /// <returns>Task</returns>
        public virtual async Task Delete()
        {
            await this.reference.DeleteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Delete from Blob Storage
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="deleteHistory">Delete History (Snapshots)</param>
        /// <returns>Object</returns>
        public virtual async Task Delete(string blobName, bool deleteHistory = true)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            var delSnapshots = deleteHistory ? DeleteSnapshotsOption.IncludeSnapshots : DeleteSnapshotsOption.None;
            var blob = this.GetBlockReference(blobName);
            await blob.DeleteAsync(delSnapshots, AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions(), new OperationContext()).ConfigureAwait(false);
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

            var blob = this.GetBlockReference(blobName);
            return await blob.ExistsAsync().ConfigureAwait(false);
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

            await this.Save(blobName, json, "application/json").ConfigureAwait(false);
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

            var blob = this.GetBlockReference(blobName);
            var cacheProperties = await this.Properties(blobName).ConfigureAwait(false);

            await blob.UploadTextAsync(text).ConfigureAwait(false);

            await this.Set(blob, cacheProperties, contentType).ConfigureAwait(false);
        }

        /// <summary>
        /// Set Properties on Blob
        /// </summary>
        /// <param name="blob">Blob</param>
        /// <param name="cached">Cached Properties</param>
        /// <param name="type">Content Type</param>
        /// <param name="cacheControl">Cache Control</param>
        /// <param name="disposition">Content Disposition</param>
        /// <param name="encoding">Content Encoding</param>
        /// <param name="language">Content Language</param>
        /// <returns></returns>
        public virtual async Task Set(CloudBlockBlob blob, BlobProperties cached, string type = null, string cacheControl = null, string disposition = null, string encoding = null, string language = null)
        {
            await blob.FetchAttributesAsync().ConfigureAwait(false);

            if (null != cached)
            {
                blob.Properties.CacheControl = cached.CacheControl;
                blob.Properties.ContentDisposition = cached.ContentDisposition;
                blob.Properties.ContentEncoding = cached.ContentEncoding;
                blob.Properties.ContentLanguage = cached.ContentLanguage;
                blob.Properties.ContentType = cached.ContentType;
            }

            blob.Properties.CacheControl = string.IsNullOrWhiteSpace(cacheControl) ? blob.Properties.CacheControl : cacheControl;
            blob.Properties.ContentDisposition = string.IsNullOrWhiteSpace(disposition) ? blob.Properties.ContentDisposition : disposition;
            blob.Properties.ContentEncoding = string.IsNullOrWhiteSpace(encoding) ? blob.Properties.ContentEncoding : encoding;
            blob.Properties.ContentLanguage = string.IsNullOrWhiteSpace(language) ? blob.Properties.ContentLanguage : language;
            blob.Properties.ContentType = string.IsNullOrWhiteSpace(type) ? blob.Properties.ContentType : type;

            await blob.SetPropertiesAsync().ConfigureAwait(false);
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

            var json = await this.GetText(blobName).ConfigureAwait(false);
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

            var blob = this.GetBlockReference(blobName);
            await blob.FetchAttributesAsync().ConfigureAwait(false);

            var bytes = new byte[blob.Properties.Length];
            await blob.DownloadToByteArrayAsync(bytes, 0).ConfigureAwait(false);

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

            var blob = this.GetBlockReference(blobName);
            return await blob.DownloadTextAsync().ConfigureAwait(false);
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

            var blob = this.GetBlockReference(blobName);
            var cached = await this.Properties(blobName).ConfigureAwait(false);

            await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length).ConfigureAwait(false);

            await this.Set(blob, cached, contentType).ConfigureAwait(false);
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

            var exists = await this.Exists(blobName).ConfigureAwait(false);
            if (exists)
            {
                var blob = this.GetBlockReference(blobName);
                await blob.FetchAttributesAsync().ConfigureAwait(false);
                return blob.Properties;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set Cache Control
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="cacheDuration">Cache Duration (Default 1 year)</param>
        /// <returns>Task</returns>
        public virtual async Task SetCacheControl(string blobName, uint cacheDuration = DefaultCacheDuration)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            cacheDuration = cacheDuration < 1 ? DefaultCacheDuration : cacheDuration;

            var blob = this.GetBlockReference(blobName);
            await this.Set(blob, null, null, string.Format("public, max-age={0}", cacheDuration)).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Reference
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="snapshot">Snapshot time</param>
        /// <returns>Cloud Block Blob</returns>
        public virtual CloudBlockBlob GetBlockReference(string blobName, DateTimeOffset? snapshot = null)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            return this.reference.GetBlockBlobReference(blobName, snapshot);
        }

        /// <summary>
        /// Get Reference
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="snapshot">Snapshot time</param>
        /// <returns>Cloud Block Blob</returns>
        public virtual CloudPageBlob GetPageReference(string blobName, DateTimeOffset? snapshot = null)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            return this.reference.GetPageBlobReference(blobName, snapshot);
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

            var properties = await this.Properties(blobName).ConfigureAwait(false);
            var blob = this.GetBlockReference(blobName);
            var stream = new MemoryStream();
            await blob.DownloadRangeToStreamAsync(stream, 0, properties.Length).ConfigureAwait(false);
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// List Blobs
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="useFlatBlobListing">Use Flat Blob Listing</param>
        /// <returns>Blobs</returns>
        public async Task<IEnumerable<IListBlobItem>> List(string prefix = null, bool useFlatBlobListing = true, BlobListingDetails details = BlobListingDetails.All, int? maxResults = int.MaxValue)
        {
            BlobContinuationToken token = null;
            var blobs = new List<IListBlobItem>();
            var options = new BlobRequestOptions();
            var operationContext = new OperationContext();

            do
            {
                var segments = await this.reference.ListBlobsSegmentedAsync(prefix, useFlatBlobListing, details, maxResults, token, options, operationContext).ConfigureAwait(false);
                blobs.AddRange(segments.Results);
                token = segments.ContinuationToken;
            }
            while (null != token);

            return blobs;
        }

        /// <summary>
        /// Create Snapshot
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Task</returns>
        public virtual async Task<ICloudBlob> Snapshot(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            var options = new BlobRequestOptions
            {
                LocationMode = LocationMode.PrimaryOnly,
            };

            var blobs = await this.List(blobName).ConfigureAwait(false);
            var blob = blobs.FirstOrDefault();
            var block = blob as CloudBlockBlob;
            if (null != block)
            {
                return await block.CreateSnapshotAsync(null, null, options, null).ConfigureAwait(false);
            }
            var page = blob as CloudPageBlob;
            if (null != page)
            {
                return await page.CreateSnapshotAsync(null, null, options, null).ConfigureAwait(false);
            }

            return null;
        }

        /// <summary>
        /// Copy From Blob to Blob
        /// </summary>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <returns>Blob Uri</returns>
        public virtual async Task<string> Copy(string from, string to)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentException("Source blob address");
            }
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("Target blob address");
            }

            var source = this.GetBlockReference(from);
            var target = this.GetBlockReference(to);
            return await target.StartCopyAsync(source).ConfigureAwait(false);
        }

        /// <summary>
        /// Copy from, to seperate container/blob
        /// </summary>
        /// <param name="from">From</param>
        /// <param name="target">Target</param>
        /// <param name="to">To</param>
        /// <returns>Blob Uri</returns>
        public virtual async Task<string> Copy(string from, string target, string to)
        {
            return await this.Copy(from, new Container(target, this.Account), to).ConfigureAwait(false);
        }

        /// <summary>
        /// Copy from, to seperate container/blob
        /// </summary>
        /// <param name="from">From</param>
        /// <param name="target">Target</param>
        /// <param name="to">To</param>
        /// <returns>Blob Uri</returns>
        public virtual async Task<string> Copy(string from, IContainer target, string to)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentException("from");
            }
            if (null == target)
            {
                throw new ArgumentNullException("target");
            }
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("to");
            }

            var source = this.GetBlockReference(from);
            var targetBlockBlob = target.GetBlockReference(to);
            return await targetBlockBlob.StartCopyAsync(source).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a shared access signature for the container.
        /// </summary>
        /// <param name="policy">the access policy for the shared access signature.</param>
        /// <param name="groupPolicyIdentifier">A container-level access policy</param>
        /// <returns>A shared access signature, as a URI query string.</returns>
        public virtual string GetSharedAccessSignature(SharedAccessBlobPolicy policy, string groupPolicyIdentifier = null)
        {
            return string.IsNullOrEmpty(groupPolicyIdentifier) ? reference.GetSharedAccessSignature(policy) : reference.GetSharedAccessSignature(policy, groupPolicyIdentifier);
        }

        /// <summary>
        /// Get Reference
        /// </summary>
        /// <param name="relativeAddress">Directory Name</param>
        /// <returns>Cloud Block Directory</returns>
        public virtual CloudBlobDirectory GetDirectoryReference(string relativeAddress)
        {
            if (string.IsNullOrWhiteSpace(relativeAddress))
            {
                throw new ArgumentException("blobName");
            }

            return reference.GetDirectoryReference(relativeAddress);
        }

        /// <summary>
        /// Returns a shared access signature for the container..
        /// </summary>
        /// <param name="policy">the access policy for the shared access signature.</param>
        /// <param name="groupPolicyIdentifier">A container-level access policy</param>
        /// <returns>A shared access signature, as a URI.</returns>
        public virtual string GetContainerSasUriWithPolicy(SharedAccessBlobPolicy policy, string groupPolicyIdentifier)
        {
            var sasContainerToken = GetSharedAccessSignature(policy, groupPolicyIdentifier);
            return reference.Uri + sasContainerToken;
        }

        /// <summary>
        /// Returns a shared access signature for a blob in the container.
        /// </summary>
        /// <param name="blobName">the name of the blob in the container.</param>
        /// <param name="policy">The name of the shared access policy.</param>
        /// <param name="groupPolicyIdentifier">A container-level access policy</param>
        /// <returns>A shared access signature, as a URI</returns>
        public virtual string GetBlobSasUriWithPolicy(string blobName, SharedAccessBlobPolicy policy, string groupPolicyIdentifier)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("blobName");
            }

            var blob = reference.GetBlockBlobReference(blobName);

            //Generate the shared access signature on the blob.
            var sasBlobToken = blob.GetSharedAccessSignature(null, groupPolicyIdentifier);

            //Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;
        }

        /// <summary>
        /// Set Shared Access Policy
        /// </summary>
        /// <param name="policyName">The shared signature policy name</param>
        /// <param name="minutesToExpire">The amount in time for the shared access signature to expire.</param>
        /// <param name="policyPermissions">The shared access policy permissions</param>
        /// <returns>Task</returns>
        public virtual async Task SetSharedAccessPolicy(string policyName, double minutesToExpire, List<SharedAccessBlobPermissions> policyPermissions)
        {
            if (string.IsNullOrEmpty(policyName))
            {
                throw new ArgumentException("policyName");
            }

            if (minutesToExpire <= 0)
            {
                throw new ArgumentException("minutesToExpire must be greater than 0");
            }

            await ClearContainerPermissions();

            var permissions = await reference.GetPermissionsAsync().ConfigureAwait(false);

            var sharedPolicy = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTimeOffset.Now.AddMinutes(minutesToExpire)
            };

            // Loop through list of inputted permissions and add them to the policy.
            foreach (var perm in policyPermissions)
            {
                sharedPolicy.Permissions = sharedPolicy.Permissions | perm;
            }

            permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
            await reference.SetPermissionsAsync(permissions).ConfigureAwait(false);
        }

        /// <summary>
        /// Clears the shared access policies on the container.
        /// </summary>
        /// <returns>Task</returns>
        private async Task ClearContainerPermissions()
        {
            var perms = await reference.GetPermissionsAsync().ConfigureAwait(false);
            perms.SharedAccessPolicies.Clear();
            await reference.SetPermissionsAsync(perms).ConfigureAwait(false);
        }

        #endregion
    }
}
