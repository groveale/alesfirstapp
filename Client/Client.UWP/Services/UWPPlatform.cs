using Client.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Plugin.Media;
using Client.UWP.Services;

[assembly: Xamarin.Forms.Dependency(typeof(UWPPlatform))]
namespace Client.UWP.Services
{
    public class UWPPlatform : IPlatform
    {
        public async Task<Stream> GetUploadFileAsync()
        {
            var mediaPlugin = CrossMedia.Current;
            var mainPage = Xamarin.Forms.Application.Current.MainPage;

            await mediaPlugin.Initialize();

            if (mediaPlugin.IsPickPhotoSupported)
            {
                var mediaFile = await mediaPlugin.PickPhotoAsync();
                return mediaFile.GetStream();
            }
            else
            {
                await mainPage.DisplayAlert("Media Service Unavailable", "Cannot pick photo", "OK");
                return null;
            }
        }
    }
}
