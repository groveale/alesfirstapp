using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Abstractions
{
    public interface IPlatform
    {
        Task<Stream> GetUploadFileAsync();

    }
}
