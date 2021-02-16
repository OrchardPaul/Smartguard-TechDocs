using BlazorInputFile;
using Gizmo_V1_02.FileManagement.FileProcessing.Interface;

namespace Gizmo_V1_02.Pages.Chapters
{
    public interface IChapterFileUpload
    {
        IFileHelper FileUpload { get; set; }

        void ChapterFileIsValid(string path);
        void UploadChapterFiles(IFileListEntry files);
    }
}