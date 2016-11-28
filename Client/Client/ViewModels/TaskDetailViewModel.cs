using Client.Abstractions;
using Client.Model;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Client.ViewModels
{
    class TaskDetailViewModel : BaseViewModel
    {
        

        public TaskDetailViewModel(TodoItem item = null)
        {
            if (item != null)
            {
                Item = item;
                Title = item.Text;
            }
            else
            {
                Item = new TodoItem { Text = "New Item", Complete = false };
                Title = "New Item";
            }

            AddNewFileCommand = new Command(async () => await AddNewFileAsync());

        }

        public ICloudService CloudService => ServiceLocator.Get<ICloudService>();

        public TodoItem Item { get; set; }

        Command cmdSave;
        public Command SaveCommand => cmdSave ?? (cmdSave = new Command(async () => await ExecuteSaveCommand()));

        async Task ExecuteSaveCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                var table = await CloudService.GetTableAsync<TodoItem>();

                if (Item.Id == null)
                {
                    await table.CreateItemAsync(Item);
                }
                else
                {
                    await table.UpdateItemAsync(Item);
                }
                MessagingCenter.Send<TaskDetailViewModel>(this, "ItemsChanged");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TaskDetail] Save error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        Command cmdDelete;
        public Command DeleteCommand => cmdDelete ?? (cmdDelete = new Command(async () => await ExecuteDeleteCommand()));

        async Task ExecuteDeleteCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                var table = await CloudService.GetTableAsync<TodoItem>();
                if (Item.Id != null)
                {
                    await table.DeleteItemAsync(Item);
                }
                MessagingCenter.Send<TaskDetailViewModel>(this, "ItemsChanged");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TaskDetail] Save error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }


        // <summary>
        /// Reference to the Platform Provider
        /// </summary>
        public IPlatform PlatformProvider => DependencyService.Get<IPlatform>();


        /// <summary>
        /// Bindable property for the AddNewFile Command
        /// </summary>
        public ICommand AddNewFileCommand { get; }

        /// <summary>
        /// User clicked on the Add New File button
        /// </summary>
        private async Task AddNewFileAsync()
        {
            if (IsBusy)
            {
                return;
            }
            IsBusy = true;

            try
            {
                // Get a stream for the file

                var mediaStream = await PlatformProvider.GetUploadFileAsync();
                if (mediaStream == null)
                {
                    IsBusy = false;
                    return;
                }

                var cloudService = App.CloudService;

                // Get the SAS token from the backend
                var storageToken = await cloudService.GetSasTokenAsync("event", Item.Id);

                // Use the SAS token to upload the file
                var storageUri = new Uri($"{storageToken.Uri}{storageToken.SasToken}");
                var blobStorage = new CloudBlockBlob(storageUri);
                await blobStorage.UploadFromStreamAsync(mediaStream);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error Uploading File", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }


    }
}
