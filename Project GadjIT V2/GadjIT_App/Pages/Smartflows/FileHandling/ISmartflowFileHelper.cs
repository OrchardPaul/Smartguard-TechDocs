using BlazorInputFile;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using GadjIT_App.Shared.StaticObjects;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow;

namespace GadjIT_App.Pages.Smartflows.FileHandling
{
    public interface ISmartflowFileHelper
    {
        IFileHelper FileHelper { get; set; }

        void SmartflowFileIsValid(string path);
        List<FileDesc> GetFileListForSmartflow();
        void SetFileHelperCustomPath(FileOptions FileOptions, FileStorageType _storageType);
        Task<bool> UploadSmartflowFiles(IFileListEntry files);
        void WriteSmartflowToFile(string JSON, string fileName);
        string ReadJson(string path);
        IList<string> ValidateSmartflowJSON(string JSON);
        Task<string> WriteSmartflowDataToExcel(SmartflowV2 selectedChapter, List<P4W_DmDocuments> documents, List<P4W_CaseTypeGroups> caseTypeGroups);
        SmartflowV2 ReadSmartflowItemsFromExcel(string path);
        byte[] ReadFileToByteArray(string path);
        string DeleteFile(string path);
        List<string> ValidateSmartflowExcel(string filePath);


    }
}