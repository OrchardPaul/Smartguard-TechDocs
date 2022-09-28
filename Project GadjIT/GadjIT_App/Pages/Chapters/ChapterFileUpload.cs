using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorInputFile;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.Data;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace GadjIT_App.Pages.Chapters
{
    public class ChapterFileUpload : IChapterFileUpload
    {
        public IFileHelper FileHelper { get; set; }

        public ChapterFileUpload(IFileHelper fileUpload)
        {
            FileHelper = fileUpload;
        }

        public List<string> ValidateChapterExcel(string filePath)
        {
            return FileHelper.ValidateChapterExcel(filePath);
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

        public void WriteChapterToFile(string JSON, string fileName)
        {
            List<string> output = new List<string> { JSON };

            FileHelper.Write(output, fileName);
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

        public async Task<string> WriteChapterDataToExcel(VmChapter selectedChapter, List<DmDocuments> documents, List<CaseTypeGroups> caseTypeGroups)
        {
            return await FileHelper.WriteChapterDataToExcel(selectedChapter, documents, caseTypeGroups);
        }

        public VmChapter readChapterItemsFromExcel(string path)
        {
            return FileHelper.ReadChapterDataFromExcel(path);
        }

        public IList<string> ValidateChapterJSON(string JSON)
        {
            IList<string> lstErrors = new List<string>();

            try
            {
                JObject jObject = JObject.Parse(JSON);

            }
            catch (Exception e)
            {
                lstErrors.Add("Error parsing JSON");
            }

            return lstErrors;
        }


        public void ChapterFileIsValid(string path)
        {
            bool isValid = true;

            FileHelper.IsFileValid = isValid;
        }

    }
}
