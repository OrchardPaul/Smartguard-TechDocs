using BlazorInputFile;
using Gizmo_V1_02.Services;

namespace Gizmo_V1_02.Pages.Chapters
{
    public interface IChapterFileUpload
    {
        IFileUpload FileUpload { get; set; }

        void ChapterFileIsValid(string path);
        void UploadChapterFiles(IFileListEntry files);
    }
}