using BlazorInputFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services
{
    public interface IFileUpload
    {
        Task Upload(IFileListEntry file);
    }

    
}
