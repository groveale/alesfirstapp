using Client.Abstractions;
using Client.Model;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class AzureCloudService : ICloudService
    {
        public MobileServiceClient client { get; }

        public AzureCloudService()
        {
            client = new MobileServiceClient("http://alesfirstapp.azurewebsites.net");
        }
        public MobileServiceClient GetMobileClient()
        {
            return client;
        }

        public async Task<ICloudTable<T>> GetTableAsync<T>() where T : TableData
        {
            await InitializeAsync();
            return new AzureCloudTable<T>(client);
        }

        public async Task<StorageTokenViewModel> GetSasTokenAsync()
        {
            var parameters = new Dictionary<string, string>();
            var storageToken = await client.InvokeApiAsync<StorageTokenViewModel>("GetStorageToken", HttpMethod.Get, parameters);
            return storageToken;
        }

        #region Offline Sync Initialization
        public async Task InitializeAsync()
        {
            // Short circuit - local database is already initialized
            if (client.SyncContext.IsInitialized)
                return;

            // Create a reference to the local sqlite store
            var store = new MobileServiceSQLiteStore("offlinecache.db");

            // Define the database schema
            store.DefineTable<TodoItem>();

            // Actually create the store and update the schema
            await client.SyncContext.InitializeAsync(store);
        }

        public async Task SyncOfflineCacheAsync()
        {
            await InitializeAsync();

            if (!(await CrossConnectivity.Current.IsRemoteReachable(client.MobileAppUri.Host, 443)))
            {
                Debug.WriteLine($"Cannot connect to {client.MobileAppUri} right now - offline");
                return;
            }

            // Push the Operations Queue to the mobile backend
            await client.SyncContext.PushAsync();



            // Pull each sync table
            //var taskTable = await GetTableAsync<TodoItem>(); await taskTable.PullAsync();
            var halfTaskTable = client.GetSyncTable<TodoItem>(); await halfTaskTable.PullAsync("Task Table 123", halfTaskTable.CreateQuery().Where(u => u.Text == "First item"));
        }

        #endregion
    }
}
