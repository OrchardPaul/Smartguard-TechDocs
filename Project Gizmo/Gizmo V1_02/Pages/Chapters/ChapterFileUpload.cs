using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorInputFile;
using Gizmo_V1_02.Data;
using Gizmo_V1_02.FileManagement.FileOptions;
using Gizmo_V1_02.FileManagement.FileProcessing.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace Gizmo_V1_02.Pages.Chapters
{
    public class ChapterFileUpload : IChapterFileUpload
    {
        public IFileHelper FileUpload { get; set; }

        public ChapterFileUpload(IFileHelper fileUpload)
        {
            FileUpload = fileUpload;
        }


        public async void UploadChapterFiles(IFileListEntry files, ChapterFileOptions chapterFileOptions)
        {
            FileUpload.CustomPath = $"FileManagement/FileStorage/{chapterFileOptions.Company}/{chapterFileOptions.CaseTypeGroup}/{chapterFileOptions.CaseType}/{chapterFileOptions.CaseTypeGroup}";
            FileUpload.ValidationAction = ChapterFileIsValid;

            await FileUpload.Upload(files);
        }

        public void ChapterFileIsValid(string path)
        {
            bool isValid = true;

            FileUpload.IsFileValid = isValid;
        }

    }
}
