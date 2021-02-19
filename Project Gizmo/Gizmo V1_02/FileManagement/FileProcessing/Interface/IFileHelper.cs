using BlazorInputFile;
using Gizmo_V1_02.FileManagement.FileClassObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.FileManagement.FileProcessing.Interface
{
    public interface IFileHelper
    {
        bool IsFileValid { get; set; }
        Action<string> ValidationAction { get; set; }
        string CustomPath { get; set; }
        Task<bool> Upload(IFileListEntry file);
        List<FileDesc> GetFileList();
        void Write(List<string> output);
        string ReadFileIntoString(string path);
    }
}
