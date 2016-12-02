using Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Client.Pages
{
    public partial class FriendPage : ContentPage
    {
        public FriendPage()
        {
            InitializeComponent();
            BindingContext = new FriendPageViewModel();
        }
    }
}
