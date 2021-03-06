﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Files;
using Microsoft.Azure.Mobile.Server.Files.Controllers;
using Backend.DataObjects;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Security.Claims;
using System.Configuration;

namespace Backend.Controllers
{
    [Route("api/getstoragetoken")]
    [MobileAppController]
    [Authorize]
    public class GetStorageTokenController : ApiController
    {
        private const string connString = "MS_AzureStorageAccountConnectionString";
        private const string localConnString = "LOCALMS_AzureStorageAccountConnectionString";

        public GetStorageTokenController()
        {
            ConnectionString = ConfigurationManager.AppSettings[connString];
            StorageAccount = CloudStorageAccount.Parse(ConnectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        public string ConnectionString { get; }

        public CloudStorageAccount StorageAccount { get; }

        public CloudBlobClient BlobClient { get; }

        private string containerName = "userdata";


        [HttpGet]
        public async Task<StorageTokenViewModel> GetAsync(string directoryName, string fileName)
        {
            // The userId is the SID without the sid: prefix
            //var claimsPrincipal = User as ClaimsPrincipal;
           // var userId = claimsPrincipal
            //    .FindFirst(ClaimTypes.NameIdentifier)
            //    .Value.Substring(4);

            // Errors creating the storage container result in a 500 Internal Server Error
            var container = BlobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            // Get the user directory within the container
            var directory = container.GetDirectoryReference(directoryName);
            var blobName = fileName;
            var blob = directory.GetBlockBlobReference(blobName);

            // Create a policy for accessing the defined blob
            var blobPolicy = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(60),
                Permissions = SharedAccessBlobPermissions.Read
                            | SharedAccessBlobPermissions.Write
                            | SharedAccessBlobPermissions.Create
            };

            return new StorageTokenViewModel
            {
                Name = blobName,
                Uri = blob.Uri,
                SasToken = blob.GetSharedAccessSignature(blobPolicy)
            };
        }

        public class StorageTokenViewModel
        {
            public string Name { get; set; }
            public Uri Uri { get; set; }
            public string SasToken { get; set; }
        }
    }
}
