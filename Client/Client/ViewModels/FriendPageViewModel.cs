using Client.Abstractions;
using Plugin.Contacts;
using Plugin.Contacts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class FriendPageViewModel : BaseViewModel
    {
        public FriendPageViewModel()
        {
            Title = "Friends List";
            getContacts();
        }

        public async void getContacts()
        {
            if (await CrossContacts.Current.RequestPermission())
            {

                List<Contact> contacts = null;
                CrossContacts.Current.PreferContactAggregation = false;//recommended
                                                                       //run in background
                await Task.Run(() =>
                {
                    if (CrossContacts.Current.Contacts == null)
                        return;

                    contacts = CrossContacts.Current.Contacts
                      .Where(c => !string.IsNullOrWhiteSpace(c.LastName) && c.Phones.Count > 0)
                      .ToList();

                    contacts = contacts.OrderBy(c => c.LastName).ToList();
                });
            }
        }
        
    }
}
