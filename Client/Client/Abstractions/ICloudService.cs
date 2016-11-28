using Client.Model;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Abstractions
{
    public interface ICloudService
    {
        Task<ICloudTable<T>> GetTableAsync<T>() where T : TableData;

        Task InitializeAsync();
        Task SyncOfflineCacheAsync();

        Task<StorageTokenViewModel> GetSasTokenAsync(string directory, string fileName);
        MobileServiceClient GetMobileClient();
    }
}
