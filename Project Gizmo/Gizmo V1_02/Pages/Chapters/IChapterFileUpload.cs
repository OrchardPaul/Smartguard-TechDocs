using BlazorInputFile;
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
        void WriteChapterToFile(string JSON);
        string readJson(string path);
    }
}