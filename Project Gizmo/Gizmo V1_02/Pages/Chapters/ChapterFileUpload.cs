using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorInputFile;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Gizmo_V1_02.Data;
using Gizmo_V1_02.FileManagement.FileClassObjects;
using Gizmo_V1_02.FileManagement.FileClassObjects.FileOptions;
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
        public IFileHelper FileHelper { get; set; }

        public ChapterFileUpload(IFileHelper fileUpload)
        {
            FileHelper = fileUpload;
        }

        public void SetChapterOptions(ChapterFileOptions chapterFileOptions)
        {
            FileHelper.CustomPath = $"FileManagement/FileStorage/{chapterFileOptions.Company}/Chapters/{chapterFileOptions.CaseTypeGroup}/{chapterFileOptions.CaseType}/{chapterFileOptions.Chapter}";
        }

        public async Task<bool> UploadChapterFiles(IFileListEntry files)
        {
            try
            {

                FileHelper.ValidationAction = ChapterFileIsValid;

                return await FileHelper.Upload(files);

            }
            catch
            {
                return false;
            }
        }

        public List<FileDesc> GetFileListForChapter()
        {
            return FileHelper.GetFileList();
        }

        public void WriteChapterToFile(string JSON)
        {
            List<string> output = new List<string> { JSON };

            FileHelper.Write(output);
        }

        public string readJson(string path)
        {
            return FileHelper.ReadFileIntoString(path);
        }

        public byte[] ReadFileToByteArray(string path)
        {
            return FileHelper.ReadFileIntoByteArray(path);
        }

        public string DeleteFile(string path)
        {
            return FileHelper.DeleteFile(path);
        }


        public List<UsrOrDefChapterManagement> readChapterItemsFromExcel(string path)
        {
            return FileHelper.ReadChapterDataFromExcel(path);
        }

        public bool ValidateChapterJSON(string JSON)
        {
            var IsJsonValid = true;

            JSchemaGenerator generator = new JSchemaGenerator();
            try
            {
                JSchema schema = generator.Generate(typeof(VmChapter));
                JObject jObject = JObject.Parse(JSON);

                IsJsonValid = jObject.IsValid(schema);

            }
            catch
            {
                IsJsonValid = false;
            }

            return IsJsonValid;
        }


        public void ChapterFileIsValid(string path)
        {
            bool isValid = true;

            FileHelper.IsFileValid = isValid;
        }

    }
}
