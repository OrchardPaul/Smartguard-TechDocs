using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorInputFile;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using Newtonsoft.Json.Linq;
using GadjIT_App.Shared.StaticObjects;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow;

namespace GadjIT_App.Pages.Smartflows.FileHandling
{
    public class SmartflowFileHelper : ISmartflowFileHelper
    {
        public IFileHelper FileHelper { get; set; }

        
        public SmartflowFileHelper(IFileHelper fileUpload)
        {
            FileHelper = fileUpload;
        }

        public List<string> ValidateSmartflowExcel(string filePath)
        {
            return FileHelper.ValidateSmartflowExcel(filePath);
        }

        public void SetFileHelperCustomPath(FileOptions FileOptions, FileStorageType _storageType)
        {
            
            switch(_storageType)
            {
            
                case (FileStorageType)FileStorageType.TempUploads:
                    FileHelper.CustomPath = $"FileManagement\\FileStorage\\{FileOptions.Company}\\Smartflows\\{FileOptions.CaseTypeGroup}\\{FileOptions.CaseType}\\Temp";
                    break;
                case (FileStorageType)FileStorageType.BackupsCaseType:
                    FileHelper.CustomPath = $"FileManagement\\FileStorage\\{FileOptions.Company}\\Smartflows\\{FileOptions.CaseTypeGroup}\\{FileOptions.CaseType}\\{FileOptions.SelectedSystem}";
                    break;
                case (FileStorageType)FileStorageType.BackupsSmartflow:
                    FileHelper.CustomPath = $"FileManagement\\FileStorage\\{FileOptions.Company}\\Smartflows\\{FileOptions.CaseTypeGroup}\\{FileOptions.CaseType}\\{FileOptions.SmartflowName}\\{FileOptions.SelectedSystem}";
                    break;
                case (FileStorageType)FileStorageType.BackgroundImages:
                    FileHelper.CustomPath = $"wwwroot\\images\\Companies\\{FileOptions.Company}\\BackgroundImages";
                    break;
                default:
                    FileHelper.CustomPath = $"";
                    break;
            }
        }

        public async Task<bool> UploadSmartflowFiles(IFileListEntry files)
        {
            try
            {

                FileHelper.ValidationAction = SmartflowFileIsValid;

                return await FileHelper.Upload(files);

            }
            catch
            {
                return false;
            }
        }

        public List<FileDesc> GetFileListForSmartflow()
        {
            return FileHelper.GetFileList();
        }

        public void WriteSmartflowToFile(string JSON, string fileName)
        {
            List<string> output = new List<string> { JSON };

            FileHelper.Write(output, fileName);
        }

        public string ReadJson(string path)
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

        public async Task<string> WriteSmartflowDataToExcel(Smartflow selectedChapter, List<P4W_DmDocuments> documents, List<P4W_CaseTypeGroups> caseTypeGroups)
        {
            return await FileHelper.WriteSmartflowDataToExcel(selectedChapter, documents, caseTypeGroups);
        }

        public Smartflow ReadSmartflowItemsFromExcel(string path)
        {
            return FileHelper.ReadChapterDataFromExcel(path);
        }

        public IList<string> ValidateSmartflowJSON(string JSON)
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


        public void SmartflowFileIsValid(string path)
        {
            bool isValid = true;

            FileHelper.IsFileValid = isValid;
        }

    }
}
