using BlazorInputFile;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Gizmo_V1_02.FileManagement.FileClassObjects;
using Gizmo_V1_02.FileManagement.FileProcessing.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo_V1_02.FileManagement.FileProcessing.Implementation
{
    public class FileHelper : IFileHelper
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IJSRuntime jsRuntime;

        public bool IsFileValid { get; set; }

        public Action<string> ValidationAction { get; set; }

        public string CustomPath { get; set; }

        public FileHelper(IWebHostEnvironment webHost, IJSRuntime jsRuntime)
        {
            webHostEnvironment = webHost;
            this.jsRuntime = jsRuntime;
        }



        public async Task<bool> Upload(IFileListEntry file)
        {
            var path = string.IsNullOrEmpty(CustomPath) ?
                                    Path.Combine(webHostEnvironment.ContentRootPath, "FileManagement/FileStorage/Default", file.Name)
                                    :
                                    Path.Combine(webHostEnvironment.ContentRootPath, CustomPath, file.Name);

            var MemStream = new MemoryStream();

            await file.Data.CopyToAsync(MemStream);

            ValidationAction.Invoke(path);

            if (IsFileValid)
            {
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        MemStream.WriteTo(fs);
                    }

                    return true;
                }
                catch
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                    try
                    {
                        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            MemStream.WriteTo(fs);
                        }

                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

            }
            return false;
        }

        public List<FileDesc> GetFileList()
        {
            var fileList = new List<FileDesc>();

            try
            {
                fileList.AddRange(Directory
                    .GetFiles(CustomPath)
                    .Select(F => new FileDesc 
                    { FileName = Path.GetFileName(F)
                    , FilePath = Path.GetFullPath(F)
                    , FileDirectory = Path.GetDirectoryName(F)
                    , FileDate = File.GetCreationTime(F)}).ToList());

                return fileList;
            }
            catch
            {
                return fileList;
            }

        }


        //public string ReadFileIntoString(string path)
        //{
        //    using FileStream fs = File.OpenRead(path);

        //    byte[] buf = new byte[50000];
        //    int c;
        //    var result = "";

        //    while ((c = fs.Read(buf, 0, buf.Length)) > 0)
        //    {
        //        result = Encoding.UTF8.GetString(buf, 0, c);
        //    }

        //    return result;
        //}



        public string ReadFileIntoString(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    return File.ReadAllText(path);
                }
                catch
                {
                    return "Failed Read";
                }
            }

            return "File not found";
        }

        public byte[] ReadFileIntoByteArray(string path)
        {
            byte[] returnBytes = new byte[5000];

            if (File.Exists(path))
            {
                try
                {
                    return File.ReadAllBytes(path);
                }
                catch
                {
                   
                    return returnBytes;
                }
            }

            return returnBytes;
        }

        public string MoveFile(string oldPath, string newPath)
        {
            if (File.Exists(oldPath))
            {
                try
                {
                    File.Move(oldPath, newPath);
                    return "Success";
                }
                catch
                {
                    return "Failed Move";
                }
            }

            return "File not found";
        }

        public string RenameFile(string path, string oldName, string newName)
        {
            string newPath = path.Replace(oldName, newName);

            if (File.Exists(path))
            {
                try
                {
                    File.Move(path, newPath);
                    return "Success";
                }
                catch
                {
                    return "Failed Rename";
                }
            }

            return "File not found";
        }

        public string DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    return "Success";
                }
                catch
                {
                    return "Failed Delete";
                }
            }

            return "File not found";
        }

        public string MoveFolder(string oldPath, string newPath)
        {
            if (Directory.Exists(oldPath))
            {
                try
                {
                    Directory.Move(oldPath, newPath);
                    return "Success";
                }
                catch
                {
                    return "Failed Move";
                }
            }

            return "Folder not found";
        }


        public string RenameFolder(string path, string oldName, string newName)
        {
            string newPath = path.Replace(oldName, newName);

            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Move(path, newPath);
                    return "Success";
                }
                catch
                {
                    return "Failed Rename";
                }
            }

            return "Folder not found";
        }

        public string DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path);
                    return "Success";
                }
                catch
                {
                    return "Failed Delete";
                }
            }

            return "Folder not found";

        }



        public void Write(List<string> output,string fileName)
        {
            // Write the string array to a new file named "WriteLines.txt".
            try
            {
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(CustomPath, fileName)))
                {
                    foreach (string line in output)
                        outputFile.WriteLine(line);
                }
            }
            catch
            {
                Directory.CreateDirectory(Path.GetFullPath(CustomPath));

                using (StreamWriter outputFile = new StreamWriter(Path.Combine(CustomPath, fileName)))
                {
                    foreach (string line in output)
                        outputFile.WriteLine(line);
                }
            }

        }

        public List<string> ValidateChapterExcel(string FilePath)
        {
            var isExcelValid = new List<string>();

            try
            {
                FileInfo fileInfo = new FileInfo(FilePath);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                    int totalColumns = worksheet.Dimension.End.Column;
                    int totalRows = worksheet.Dimension.End.Row;

                    if (totalRows <= 2) isExcelValid.Add("No data in spreadsheet to import");
                    if (totalColumns != 16) isExcelValid.Add("Spreadsheet has an invalid number of columns");
                }

            }
            catch
            {
                isExcelValid.Add("Error Processing Excel");
            }


            return isExcelValid;
        }

        public async Task<string> WriteChapterDataToExcel(VmChapter selectedChapter)
        {
            List<string> docTypes = new List<string> { "Doc", "Letter", "Form", "Step", "Date", "Email" };

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new ExcelPackage();
            
           

            var workSheetHeader = excel.Workbook.Worksheets.Add(selectedChapter.Name);
            workSheetHeader.TabColor = System.Drawing.Color.RoyalBlue;
            workSheetHeader.DefaultRowHeight = 12;

           

            workSheetHeader.Cells[1, 1, 1, 16].Merge = true;

            workSheetHeader.Row(1).Height = 30;
            workSheetHeader.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetHeader.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheetHeader.Cells[1, 1].Style.Font.Name = "Arial Black";
            workSheetHeader.Cells[1, 1].Style.Font.Size = 16;
            workSheetHeader.Cells[1, 1].Value = selectedChapter.Name;

            workSheetHeader.Column(1).Width = 40;
            workSheetHeader.Column(1).Style.Font.Bold = true;

            workSheetHeader.Cells[2, 1].Value = "Case Type Group:";
            workSheetHeader.Cells[2, 2].Value = selectedChapter.CaseTypeGroup;

            workSheetHeader.Cells[3, 1].Value = "Case Type:";
            workSheetHeader.Cells[3, 2].Value = selectedChapter.CaseType;

            workSheetHeader.Cells[4, 1].Value = "Case Type Group:";
            workSheetHeader.Cells[4, 2].Value = selectedChapter.Name;

            workSheetHeader.Cells[6, 1].Value = "Colour:";
            workSheetHeader.Cells[6, 2].Value = selectedChapter.BackgroundColourName;

            workSheetHeader.Cells[7, 1].Value = "General Notes (Ticker): ";
            workSheetHeader.Cells[7, 2].Value = selectedChapter.GeneralNotes;

            /*
             * 
             * Agenda
             * 
             * 
             */

            var workSheetAgenda = excel.Workbook.Worksheets.Add("Agenda");

            workSheetAgenda.TabColor = System.Drawing.Color.Black;
            workSheetAgenda.DefaultRowHeight = 12;

            //Header of table
            workSheetAgenda.Row(1).Height = 20;
            workSheetAgenda.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetAgenda.Row(1).Style.Font.Bold = true;
            workSheetAgenda.Cells[1, 1].Value = "Agenda Name";

            
            //Body of table
            int recordIndex = 2;
            foreach (var chapterItem in selectedChapter.ChapterItems.Where(C => C.Type == "Agenda").OrderBy(C => C.SeqNo).ToList())
            {
                workSheetAgenda.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(chapterItem.Name) ? "" : chapterItem.Name;

                recordIndex++;
            }

            workSheetAgenda.Column(1).AutoFit();


            /*
             * 
             * Status
             * 
             * 
             */

            var workSheetStatus = excel.Workbook.Worksheets.Add("Status");

            workSheetStatus.TabColor = System.Drawing.Color.Black;
            workSheetStatus.DefaultRowHeight = 12;

            //Header of table
            workSheetStatus.Row(1).Height = 20;
            workSheetStatus.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetStatus.Row(1).Style.Font.Bold = true;
            workSheetStatus.Cells[1, 1].Value = "Status Name";
            workSheetStatus.Cells[1, 2].Value = "End Of Flow (Y or Blank)";


            //Body of table
            recordIndex = 2;
            foreach (var chapterItem in selectedChapter.ChapterItems.Where(C => C.Type == "Status").OrderBy(C => C.SeqNo).ToList())
            {
                workSheetStatus.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(chapterItem.Name) ? "" : chapterItem.Name;
                workSheetStatus.Cells[recordIndex, 2].Value = string.IsNullOrEmpty(chapterItem.SuppressStep) ? "" : chapterItem.SuppressStep;

                recordIndex++;
            }

            workSheetStatus.Column(1).AutoFit();
            workSheetStatus.Column(2).AutoFit();


            /*
             * 
             * Documents
             * 
             * 
             */

            var workSheetDocument = excel.Workbook.Worksheets.Add("Documents");

            workSheetDocument.TabColor = System.Drawing.Color.Black;
            workSheetDocument.DefaultRowHeight = 12;

            //Header of table
            workSheetDocument.Row(1).Height = 20;
            workSheetDocument.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetDocument.Row(1).Style.Font.Bold = true;
            workSheetDocument.Cells[1, 1].Value = "Item Name";
            workSheetDocument.Cells[1, 2].Value = "Alternative Item Name";
            workSheetDocument.Cells[1, 3].Value = "Reschedule Days";
            workSheetDocument.Cells[1, 4].Value = "Reschedule As Description";
            workSheetDocument.Cells[1, 5].Value = "Step History Description";
            workSheetDocument.Cells[1, 6].Value = "Status Change";
            workSheetDocument.Cells[1, 7].Value = "Item User Message";
            workSheetDocument.Cells[1, 8].Value = "Popup Alert";
            workSheetDocument.Cells[1, 9].Value = "Notes to Developer";

            //Body of table
            recordIndex = 2;
            foreach (var chapterItem in selectedChapter.ChapterItems.Where(C => docTypes.Contains(C.Type)).OrderBy(C => C.SeqNo).ToList())
            {
                workSheetDocument.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(chapterItem.Name) ? "" : chapterItem.Name;
                workSheetDocument.Cells[recordIndex, 2].Value = string.IsNullOrEmpty(chapterItem.AltDisplayName) ? "" : chapterItem.AltDisplayName;
                workSheetDocument.Cells[recordIndex, 3].Value = chapterItem.RescheduleDays is null ? "" : chapterItem.RescheduleDays.ToString();
                workSheetDocument.Cells[recordIndex, 4].Value = string.IsNullOrEmpty(chapterItem.AsName) ? "" : chapterItem.AsName;
                workSheetDocument.Cells[recordIndex, 5].Value = string.IsNullOrEmpty(chapterItem.CompleteName) ? "" : chapterItem.CompleteName;
                workSheetDocument.Cells[recordIndex, 6].Value = string.IsNullOrEmpty(chapterItem.NextStatus) ? "" : chapterItem.NextStatus;
                workSheetDocument.Cells[recordIndex, 7].Value = string.IsNullOrEmpty(chapterItem.UserMessage) ? "" : chapterItem.UserMessage;
                workSheetDocument.Cells[recordIndex, 8].Value = string.IsNullOrEmpty(chapterItem.PopupAlert) ? "" : chapterItem.PopupAlert;
                workSheetDocument.Cells[recordIndex, 9].Value = string.IsNullOrEmpty(chapterItem.DeveloperNotes) ? "" : chapterItem.DeveloperNotes;

                recordIndex++;
            }

            workSheetDocument.Column(1).AutoFit();
            workSheetDocument.Column(2).AutoFit();
            workSheetDocument.Column(3).AutoFit();
            workSheetDocument.Column(4).AutoFit();
            workSheetDocument.Column(5).AutoFit();
            workSheetDocument.Column(6).AutoFit();
            workSheetDocument.Column(7).AutoFit();
            workSheetDocument.Column(8).AutoFit();
            workSheetDocument.Column(9).AutoFit();



            /*
             * 
             * Attachments
             * 
             * 
             */

            var workSheetAttachments = excel.Workbook.Worksheets.Add("Attachments");

            workSheetAttachments.TabColor = System.Drawing.Color.Black;
            workSheetAttachments.DefaultRowHeight = 12;

            //Header of table
            workSheetAttachments.Row(1).Height = 20;
            workSheetAttachments.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetAttachments.Row(1).Style.Font.Bold = true;
            workSheetAttachments.Cells[1, 1].Value = "Item Name";
            workSheetAttachments.Cells[1, 2].Value = "Alternative Item Name";
            workSheetAttachments.Cells[1, 3].Value = "Reschedule Days";
            workSheetAttachments.Cells[1, 4].Value = "Reschedule As Description";
            workSheetAttachments.Cells[1, 5].Value = "Step History Description";
            workSheetAttachments.Cells[1, 6].Value = "Status Change";
            workSheetAttachments.Cells[1, 7].Value = "Item User Message";
            workSheetAttachments.Cells[1, 8].Value = "Popup Alert";
            workSheetAttachments.Cells[1, 9].Value = "Notes to Developer";

            workSheetAttachments.Row(1).Height = 20;
            workSheetAttachments.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetAttachments.Row(1).Style.Font.Bold = true;
            workSheetAttachments.Cells[1, 1].Value = "Item Name";
            workSheetAttachments.Cells[1, 2].Value = "Alternative Item Name";
            workSheetAttachments.Cells[1, 3].Value = "Reschedule Days";
            workSheetAttachments.Cells[1, 4].Value = "Reschedule As Description";
            workSheetAttachments.Cells[1, 5].Value = "Step History Description";
            workSheetAttachments.Cells[1, 6].Value = "Status Change";
            workSheetAttachments.Cells[1, 7].Value = "Item User Message";
            workSheetAttachments.Cells[1, 8].Value = "Popup Alert";
            workSheetAttachments.Cells[1, 9].Value = "Notes to Developer";

            //Body of table
            recordIndex = 2;
            foreach (var chapterItem in selectedChapter.ChapterItems.Where(C => docTypes.Contains(C.Type)).OrderBy(C => C.SeqNo).ToList())
            {
                workSheetAttachments.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(chapterItem.Name) ? "" : chapterItem.Name;
                workSheetAttachments.Cells[recordIndex, 2].Value = string.IsNullOrEmpty(chapterItem.AltDisplayName) ? "" : chapterItem.AltDisplayName;
                workSheetAttachments.Cells[recordIndex, 3].Value = chapterItem.RescheduleDays is null ? "" : chapterItem.RescheduleDays.ToString();
                workSheetAttachments.Cells[recordIndex, 4].Value = string.IsNullOrEmpty(chapterItem.AsName) ? "" : chapterItem.AsName;
                workSheetAttachments.Cells[recordIndex, 5].Value = string.IsNullOrEmpty(chapterItem.CompleteName) ? "" : chapterItem.CompleteName;
                workSheetAttachments.Cells[recordIndex, 6].Value = string.IsNullOrEmpty(chapterItem.NextStatus) ? "" : chapterItem.NextStatus;
                workSheetAttachments.Cells[recordIndex, 7].Value = string.IsNullOrEmpty(chapterItem.UserMessage) ? "" : chapterItem.UserMessage;
                workSheetAttachments.Cells[recordIndex, 8].Value = string.IsNullOrEmpty(chapterItem.PopupAlert) ? "" : chapterItem.PopupAlert;
                workSheetAttachments.Cells[recordIndex, 9].Value = string.IsNullOrEmpty(chapterItem.DeveloperNotes) ? "" : chapterItem.DeveloperNotes;

                recordIndex++;
            }

            workSheetAttachments.Column(1).AutoFit();
            workSheetAttachments.Column(2).AutoFit();
            workSheetAttachments.Column(3).AutoFit();
            workSheetAttachments.Column(4).AutoFit();
            workSheetAttachments.Column(5).AutoFit();
            workSheetAttachments.Column(6).AutoFit();
            workSheetAttachments.Column(7).AutoFit();
            workSheetAttachments.Column(8).AutoFit();
            workSheetAttachments.Column(9).AutoFit();




            /*
             * 
             * Fee
             * 
             * 
             */

            var workSheetFees = excel.Workbook.Worksheets.Add("Fees");

            workSheetFees.TabColor = System.Drawing.Color.Black;
            workSheetFees.DefaultRowHeight = 12;

            //Header of table
            workSheetFees.Row(1).Height = 20;
            workSheetFees.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetFees.Row(1).Style.Font.Bold = true;
            workSheetFees.Cells[1, 1].Value = "Case Type Group";
            workSheetFees.Cells[1, 2].Value = "Case Type";
            workSheetFees.Cells[1, 3].Value = "Smartflow Name";
            workSheetFees.Cells[1, 4].Value = "Item Type";
            workSheetFees.Cells[1, 5].Value = "Sequence";
            workSheetFees.Cells[1, 6].Value = "Item Name";
            workSheetFees.Cells[1, 7].Value = "Alternative Item Name";
            workSheetFees.Cells[1, 8].Value = "Reschedule Days";
            workSheetFees.Cells[1, 9].Value = "Reschedule As Description";
            workSheetFees.Cells[1, 10].Value = "Step History Description";
            workSheetFees.Cells[1, 11].Value = "Status Change";
            workSheetFees.Cells[1, 12].Value = "End of Flow";
            workSheetFees.Cells[1, 13].Value = "Item User Message";
            workSheetFees.Cells[1, 14].Value = "Popup Alert";
            workSheetFees.Cells[1, 15].Value = "Notes to Developer";
            workSheetFees.Cells[1, 16].Value = "Story Information Notes";

            //Body of table
            recordIndex = 2;
            foreach (var chapterItem in selectedChapter.ChapterItems.Where(C => C.Type == "Fee").OrderBy(C => C.SeqNo).ToList())
            {
                workSheetFees.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(chapterItem.CaseTypeGroup) ? "" : chapterItem.CaseTypeGroup;
                workSheetFees.Cells[recordIndex, 2].Value = string.IsNullOrEmpty(chapterItem.CaseType) ? "" : chapterItem.CaseType;
                workSheetFees.Cells[recordIndex, 3].Value = string.IsNullOrEmpty(chapterItem.ChapterName) ? "" : chapterItem.ChapterName;
                workSheetFees.Cells[recordIndex, 4].Value = string.IsNullOrEmpty(chapterItem.Type) ? "" : chapterItem.Type;
                workSheetFees.Cells[recordIndex, 5].Value = chapterItem.SeqNo is null ? "" : chapterItem.SeqNo.ToString();
                workSheetFees.Cells[recordIndex, 6].Value = string.IsNullOrEmpty(chapterItem.Name) ? "" : chapterItem.Name;
                workSheetFees.Cells[recordIndex, 7].Value = string.IsNullOrEmpty(chapterItem.AltDisplayName) ? "" : chapterItem.AltDisplayName;
                workSheetFees.Cells[recordIndex, 8].Value = chapterItem.RescheduleDays is null ? "" : chapterItem.RescheduleDays.ToString();
                workSheetFees.Cells[recordIndex, 9].Value = string.IsNullOrEmpty(chapterItem.AsName) ? "" : chapterItem.AsName;
                workSheetFees.Cells[recordIndex, 10].Value = string.IsNullOrEmpty(chapterItem.CompleteName) ? "" : chapterItem.CompleteName;
                workSheetFees.Cells[recordIndex, 11].Value = string.IsNullOrEmpty(chapterItem.NextStatus) ? "" : chapterItem.NextStatus;
                workSheetFees.Cells[recordIndex, 12].Value = string.IsNullOrEmpty(chapterItem.SuppressStep) ? "" : chapterItem.SuppressStep;
                workSheetFees.Cells[recordIndex, 13].Value = string.IsNullOrEmpty(chapterItem.UserMessage) ? "" : chapterItem.UserMessage;
                workSheetFees.Cells[recordIndex, 14].Value = string.IsNullOrEmpty(chapterItem.PopupAlert) ? "" : chapterItem.PopupAlert;
                workSheetFees.Cells[recordIndex, 15].Value = string.IsNullOrEmpty(chapterItem.DeveloperNotes) ? "" : chapterItem.DeveloperNotes;
                workSheetFees.Cells[recordIndex, 16].Value = string.IsNullOrEmpty(chapterItem.StoryNotes) ? "" : chapterItem.StoryNotes;

                recordIndex++;
            }

            workSheetFees.Column(1).AutoFit();
            workSheetFees.Column(2).AutoFit();
            workSheetFees.Column(3).AutoFit();
            workSheetFees.Column(4).AutoFit();
            workSheetFees.Column(5).AutoFit();
            workSheetFees.Column(6).AutoFit();
            workSheetFees.Column(7).AutoFit();
            workSheetFees.Column(8).AutoFit();
            workSheetFees.Column(9).AutoFit();
            workSheetFees.Column(10).AutoFit();
            workSheetFees.Column(11).AutoFit();
            workSheetFees.Column(12).AutoFit();
            workSheetFees.Column(13).AutoFit();
            workSheetFees.Column(14).AutoFit();
            workSheetFees.Column(15).AutoFit();
            workSheetFees.Column(16).AutoFit();





            //Download SpreadSheet
            string excelName = $"{selectedChapter.Name}.xlsx";

            await jsRuntime.InvokeAsync<object>(
                 "DownloadTextFile",
                 excelName,
                 excel.GetAsByteArray());

            return "Success";
        }




        public List<UsrOrDefChapterManagement> ReadChapterDataFromExcel(string FilePath)
        {
            List<UsrOrDefChapterManagement> readChapters = new List<UsrOrDefChapterManagement>();

            FileInfo fileInfo = new FileInfo(FilePath);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                int totalColumns = worksheet.Dimension.End.Column;
                int totalRows = worksheet.Dimension.End.Row;

                for (int row = 3; row <= totalRows; row++)
                {
                    UsrOrDefChapterManagement readObject = new UsrOrDefChapterManagement();

                    for (int column = 3; column <= totalColumns; column++)
                    {
                        if (column == 1) readObject.CaseTypeGroup = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 2) readObject.CaseType = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 3) readObject.ChapterName = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 4) readObject.Type = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        try
                        {
                            if (column == 5) readObject.SeqNo = worksheet.Cells[row, column].FirstOrDefault() is null ? 0 : Convert.ToInt32(worksheet.Cells[row, column].Value.ToString());
                        }
                        catch
                        {
                            readObject.SeqNo = null;
                        }
                        if (column == 6) readObject.Name = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 7) readObject.AltDisplayName = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        try
                        {
                            if (column == 8) readObject.RescheduleDays = worksheet.Cells[row, column].FirstOrDefault() is null ? 0 : Convert.ToInt32(worksheet.Cells[row, column].Value.ToString());
                        }
                        catch
                        {
                            readObject.RescheduleDays = null;
                        }
                        if (column == 9) readObject.AsName = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 10) readObject.CompleteName = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 11) readObject.NextStatus = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 12) readObject.SuppressStep = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 13) readObject.UserMessage = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 14) readObject.PopupAlert = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 15) readObject.DeveloperNotes = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 16) readObject.StoryNotes = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                    }

                    readChapters.Add(readObject);
                }
            }




            return readChapters;
        }

    }
}
