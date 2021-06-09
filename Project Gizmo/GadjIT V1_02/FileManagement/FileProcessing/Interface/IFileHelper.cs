using BlazorInputFile;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT_V1_02.FileManagement.FileClassObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GadjIT_V1_02.FileManagement.FileProcessing.Interface
{
    public interface IFileHelper
    {
        string CustomPath { get; set; }
        bool IsFileValid { get; set; }
        Action<string> ValidationAction { get; set; }

        string DeleteFile(string path);
        string DeleteFolder(string path);
        List<FileDesc> GetFileList();
        string MoveFile(string oldPath, string newPath);
        string MoveFolder(string oldPath, string newPath);
        VmChapter ReadChapterDataFromExcel(string FilePath);
        string ReadFileIntoString(string path);
        string RenameFile(string path, string oldName, string newName);
        string RenameFolder(string path, string oldName, string newName);
        Task<bool> Upload(IFileListEntry file);
        List<string> ValidateChapterExcel(string FilePath);
        byte[] ReadFileIntoByteArray(string path);
        void Write(List<string> output, string fileName);
        Task<bool> DownloadFile(string FileName, byte[] data);

        Task<string> WriteChapterDataToExcel(VmChapter selectedChapter, List<DmDocuments> documents, List<CaseTypeGroups> caseTypeGroups);
    }
}