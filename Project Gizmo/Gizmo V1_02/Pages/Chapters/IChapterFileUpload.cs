using BlazorInputFile;
using GadjIT.ClientContext.P4W;
using Gizmo_V1_02.FileManagement.FileClassObjects;
using Gizmo_V1_02.FileManagement.FileClassObjects.FileOptions;
using Gizmo_V1_02.FileManagement.FileProcessing.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Chapters
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
        bool ValidateChapterJSON(string JSON);
        List<UsrOrDefChapterManagement> readChapterItemsFromExcel(string path);
        byte[] ReadFileToByteArray(string path);
        string DeleteFile(string path);
        List<string> ValidateChapterExcel(string filePath);


    }
}