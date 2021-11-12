using BlazorInputFile;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT_V1_02.FileManagement.FileClassObjects;
using GadjIT_V1_02.FileManagement.FileClassObjects.FileOptions;
using GadjIT_V1_02.FileManagement.FileProcessing.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.Chapters
{
    public interface IChapterFileUpload
    {
        IFileHelper FileHelper { get; set; }

        void ChapterFileIsValid(string path);
        List<FileDesc> GetFileListForChapter();
        void SetChapterOptions(ChapterFileOptions chapterFileOptions);
        Task<bool> UploadChapterFiles(IFileListEntry files);
        void WriteChapterToFile(string JSON, string fileName);
        string readJson(string path);
        IList<string> ValidateChapterJSON(string JSON);
        Task<string> WriteChapterDataToExcel(VmChapter selectedChapter, List<DmDocuments> documents, List<CaseTypeGroups> caseTypeGroups);
        VmChapter readChapterItemsFromExcel(string path);
        byte[] ReadFileToByteArray(string path);
        string DeleteFile(string path);
        List<string> ValidateChapterExcel(string filePath);


    }
}