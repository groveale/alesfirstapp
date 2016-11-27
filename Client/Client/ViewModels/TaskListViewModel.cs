using Client.Abstractions;
using Client.Model;
using System;
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Client.ViewModels
{
    class TaskListViewModel : BaseViewModel
    {
        public TaskListViewModel()
        {
            Title = "Task List";

            AddNewFileCommand = new Command(async () => await AddNewFileAsync());

            RefreshList();
        }

       // public ICloudService CloudService => ServiceLocator.Get<ICloudService>();

        ObservableCollection<TodoItem> items = new ObservableCollection<TodoItem>();
        public ObservableCollection<TodoItem> Items
        {
            get { return items; }
            set { SetProperty(ref items, value, "Items"); }
        }

        TodoItem selectedItem;
        public TodoItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                SetProperty(ref selectedItem, value, "SelectedItem");
                if (selectedItem != null)
                {
                    Application.Current.MainPage.Navigation.PushAsync(new Pages.TaskDetail(selectedItem));
                    SelectedItem = null;
                }
            }
        }

        Command refreshCmd;
        public Command RefreshCommand => refreshCmd ?? (refreshCmd = new Command(async () => await ExecuteRefreshCommand()));

        async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                var cloudService = App.CloudService;
                await cloudService.SyncOfflineCacheAsync();
                var table = await cloudService.GetTableAsync<TodoItem>();
                var list = await table.ReadAllItemsAsync();

                Items.Clear();
                foreach (var item in list)
                {
                    Items.Add(item);
                }
                    
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TaskList] Error loading items: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        Command addNewCmd;
        public Command AddNewItemCommand => addNewCmd ?? (addNewCmd = new Command(async () => await ExecuteAddNewItemCommand()));

        async Task ExecuteAddNewItemCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new Pages.TaskDetail());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TaskList] Error in AddNewItem: {ex.Message}");
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
                var storageToken = await cloudService.GetSasTokenAsync();

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


        async Task RefreshList()
        {
            await ExecuteRefreshCommand();
            MessagingCenter.Subscribe<TaskDetailViewModel>(this, "ItemsChanged", async (sender) =>
            {
                await ExecuteRefreshCommand();
            });
        }
    }
}
