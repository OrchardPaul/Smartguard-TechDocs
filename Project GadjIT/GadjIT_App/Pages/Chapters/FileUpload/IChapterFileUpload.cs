using BlazorInputFile;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using GadjIT_App.Shared.StaticObjects;

namespace GadjIT_App.Pages.Chapters.FileUpload
{
    public interface IChapterFileUpload
    {
        IFileHelper FileHelper { get; set; }

        void ChapterFileIsValid(string path);
        List<FileDesc> GetFileListForChapter();
        void SetFileHelperCustomPath(ChapterFileOptions chapterFileOptions, FileStorageType _storageType);
        Task<bool> UploadChapterFiles(IFileListEntry files);
        void WriteChapterToFile(string JSON, string fileName);
        string readJson(string path);
        IList<string> ValidateChapterJSON(string JSON);
        Task<string> WriteChapterDataToExcel(VmSmartflow selectedChapter, List<DmDocuments> documents, List<CaseTypeGroups> caseTypeGroups);
        VmSmartflow readChapterItemsFromExcel(string path);
        byte[] ReadFileToByteArray(string path);
        string DeleteFile(string path);
        List<string> ValidateChapterExcel(string filePath);


    }
}