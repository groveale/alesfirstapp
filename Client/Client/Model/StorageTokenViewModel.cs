using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class StorageTokenViewModel
    {
        public string Name { get; set; }
        public Uri Uri { get; set; }
        public string SasToken { get; set; }
    }
}
