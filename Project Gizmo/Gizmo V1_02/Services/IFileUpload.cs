using BlazorInputFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services
{
    public interface IFileUpload
    {
        bool IsFileValid { get; set; }
        Action<string> ValidationAction { get; set; }
        Task Upload(IFileListEntry file);
    }

    
}
