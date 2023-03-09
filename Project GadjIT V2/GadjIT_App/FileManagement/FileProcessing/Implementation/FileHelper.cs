using BlazorInputFile;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow;

namespace GadjIT_App.FileManagement.FileProcessing.Implementation
{
    public class FileHelper : IFileHelper
    {
        public class FeeDefinition
        {
            public string PostingCode { get; set; }
            public string Account { get; set; }
            public string TransactionType { get; set; }
            public string Destination { get; set; }
            public string TransactionDescription { get; set; }
        }

        public class Document
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IJSRuntime jsRuntime;

        public bool IsFileValid { get; set; }

        public Action<string> ValidationAction { get; set; }

        public string CustomPath { get; set; }
        private string CustomFullPath => string.IsNullOrEmpty(CustomPath) ?
                                                Path.Combine(webHostEnvironment.ContentRootPath, "FileManagement\\FileStorage\\Default")
                                                :
                                                Path.Combine(webHostEnvironment.ContentRootPath, CustomPath);
        private string CustomURLPath => string.IsNullOrEmpty(CustomPath) ?
                                                "/FileManagement/FileStorage/Default"
                                                :
                                                '/' + CustomPath.Replace("\\","/") + "/";
        

        public List<FeeDefinition> FeeDefinitions = new List<FeeDefinition>
        {
            new FeeDefinition { PostingCode = "DSO", Account = "Office" , TransactionType = "Payout"        , Destination = "Bank"          , TransactionDescription = "Disbursement" },
            new FeeDefinition { PostingCode = "DSP", Account = "Office" , TransactionType = "Payout"        , Destination = "Petty Cash"    , TransactionDescription = "Disbursement" },
            new FeeDefinition { PostingCode = "WOD", Account = "Office" , TransactionType = "Write Off Debt", Destination = ""              , TransactionDescription = "Write Off Debts" },
            new FeeDefinition { PostingCode = "O/N", Account = "Office" , TransactionType = "Payout"        , Destination = "Non-Bank"      , TransactionDescription = "Office to Nominal Transfer" },
            new FeeDefinition { PostingCode = "OCR", Account = "Office" , TransactionType = "Received"      , Destination = "Bank"          , TransactionDescription = "Office Credit" },
            new FeeDefinition { PostingCode = "OCP", Account = "Office" , TransactionType = "Received"      , Destination = "Petty Cash"    , TransactionDescription = "Office Credit" },
            new FeeDefinition { PostingCode = "OTO", Account = "Office" , TransactionType = "Transfer"      , Destination = "Office"        , TransactionDescription = "Office to Office Transfer" },
            new FeeDefinition { PostingCode = "OTC", Account = "Office" , TransactionType = "Transfer"      , Destination = "Client"        , TransactionDescription = "Office to Client Transfer" },
            new FeeDefinition { PostingCode = "OCR", Account = "Office" , TransactionType = "Received"      , Destination = "Bank"          , TransactionDescription = "Office Credit" },
            new FeeDefinition { PostingCode = "CDR", Account = "Client" , TransactionType = "Payout"        , Destination = ""              , TransactionDescription = "Client Debit" },
            new FeeDefinition { PostingCode = "CCR", Account = "Client" , TransactionType = "Received"      , Destination = ""              , TransactionDescription = "Client Credit" },
            new FeeDefinition { PostingCode = "CIN", Account = "Client" , TransactionType = "Interest"      , Destination = ""              , TransactionDescription = "Client Interest" },
            new FeeDefinition { PostingCode = "CTO", Account = "Client" , TransactionType = "Transfer"      , Destination = "Office"        , TransactionDescription = "Client to Office Transfer" },
            new FeeDefinition { PostingCode = "CTC", Account = "Client" , TransactionType = "Transfer"      , Destination = "Client"        , TransactionDescription = "Client to Client Transfer" },
            new FeeDefinition { PostingCode = "CTD", Account = "Client" , TransactionType = "Transfer"      , Destination = "Deposit"       , TransactionDescription = "Client to Designated Deposit" },
            new FeeDefinition { PostingCode = "DFD", Account = "Deposit", TransactionType = "Payout"        , Destination = ""              , TransactionDescription = "Direct from Designated Deposit" },
            new FeeDefinition { PostingCode = "DOD", Account = "Deposit", TransactionType = "Received"      , Destination = ""              , TransactionDescription = "Direct on Deposit" }
        };


        public FileHelper(IWebHostEnvironment webHost, IJSRuntime jsRuntime)
        {
            webHostEnvironment = webHost;
            this.jsRuntime = jsRuntime;

        }

        public async Task<bool> DownloadFile(string FileName, byte[] data)
        {
            try
            {
                await jsRuntime.InvokeAsync<object>(
                     "DownloadTextFile",
                     FileName,
                     data);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }


        public async Task<bool> Upload(IFileListEntry file)
        {
            var filePath = Path.Combine(CustomFullPath, file.Name);

            var MemStream = new MemoryStream();

            try
            {
                await file.Data.CopyToAsync(MemStream);

                ValidationAction.Invoke(filePath);

            }
            catch
            {
                return false;
            }


            if (IsFileValid)
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        MemStream.WriteTo(fs);
                    }

                    return true;
                }
                catch
                {

                    try
                    {
                        Directory.CreateDirectory(CustomFullPath);

                        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            MemStream.WriteTo(fs);
                        }

                        return true;
                    }
                    catch(Exception e)
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
                    .GetFiles(CustomFullPath)                                                                                                             
                    .Select(F => new FileDesc
                    {
                        FileName = Path.GetFileName(F)
                    ,
                        FilePath = Path.GetFullPath(F)
                    ,
                        FileURL = CustomURLPath + Path.GetFileName(F)
                    ,
                        FileDirectory = Path.GetDirectoryName(F)
                    ,
                        FileDate = File.GetCreationTime(F)
                    }).ToList());

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



        public void Write(List<string> output, string fileName)
        {
            // Write the string array to a new file 
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

        public List<string> ValidateSmartflowExcel(string FilePath)
        {
            var isExcelValid = new List<string>();

            try
            {
                FileInfo fileInfo = new FileInfo(FilePath);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
                {

                    try
                    {
                        ExcelWorksheet worksheetLookups = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Lookups").SingleOrDefault();
                        if (worksheetLookups is null)
                        {
                            isExcelValid.Add("Missing Lookups Worksheet");
                        }
                    }
                    catch
                    {
                        isExcelValid.Add("Missing Lookups Worksheet");
                    }


                    try
                    {
                        ExcelWorksheet worksheetAgenda = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Agenda").SingleOrDefault();
                        if (worksheetAgenda is null)
                        {
                            isExcelValid.Add("Missing Agenda Worksheet");
                        }
                    }
                    catch
                    {
                        isExcelValid.Add("Missing Agenda Worksheet");
                    }


                    try
                    {
                        ExcelWorksheet worksheetStatus = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Status").SingleOrDefault();
                        if (worksheetStatus is null)
                        {
                            isExcelValid.Add("Missing Status Worksheet");
                        }
                    }
                    catch
                    {
                        isExcelValid.Add("Missing Status Worksheet");
                    }


                    try
                    {
                        ExcelWorksheet worksheetFees = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Fees").SingleOrDefault();
                        if (worksheetFees is null)
                        {
                            isExcelValid.Add("Missing Fees Worksheet");
                        }
                    }
                    catch
                    {
                        isExcelValid.Add("Missing Fees Worksheet");
                    }

                    try
                    {
                        ExcelWorksheet worksheetDocuments = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Documents").FirstOrDefault();
                        if (worksheetDocuments is null)
                        {
                            isExcelValid.Add("Missing Documents Worksheet");
                        }
                    }
                    catch
                    {
                        isExcelValid.Add("Missing Documents Worksheet");
                    }

                    try
                    {
                        ExcelWorksheet worksheetAttachments = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Linked Items").SingleOrDefault();
                        if (worksheetAttachments is null)
                        {
                            isExcelValid.Add("Missing Link Items Worksheet");
                        }
                    }
                    catch
                    {
                        isExcelValid.Add("Missing Documents Worksheet");
                    }

                    try
                    {
                        ExcelWorksheet worksheetDataViews = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Data Views").SingleOrDefault();
                        if (worksheetDataViews is null)
                        {
                            isExcelValid.Add("Missing Data Views Worksheet");
                        }
                    }
                    catch
                    {
                        isExcelValid.Add("Missing Data Views Worksheet");
                    }

                }

            }
            catch
            {
                isExcelValid.Add("Error Processing Excel");
            }


            return isExcelValid;
        }

        public async Task<string> WriteSmartflowDataToExcel(SmartflowV2 selectedChapter, List<P4W_DmDocuments> documents, List<P4W_CaseTypeGroups> caseTypeGroups)
        {
            List<string> docTypes = new List<string> { "Doc", "Letter", "Form", "Step", "Date", "Email" };
            Dictionary<int?, string> docP4WTypes = new Dictionary<int?, string> { { 1, "Doc" }, { 4, "Form" }, { 6, "Step" }, { 8, "Date" }, { 9, "Email" }, { 11, "Doc" }, { 12, "Email" }, { 13, "Doc" }, { 19, "Doc" } };

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new ExcelPackage();

            var workSheetHeader = excel.Workbook.Worksheets.Add("Overview");
            workSheetHeader.TabColor = System.Drawing.Color.RoyalBlue;
            workSheetHeader.DefaultRowHeight = 12;

            workSheetHeader.Cells[1, 1, 1, 16].Merge = true;

            workSheetHeader.Row(1).Height = 82;
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

            workSheetHeader.Cells[4, 1].Value = "Smartflow Name:";
            workSheetHeader.Cells[4, 2].Value = selectedChapter.Name;

            workSheetHeader.Cells[6, 1].Value = "Colour:";
            workSheetHeader.Cells[6, 2].Value = selectedChapter.BackgroundColourName;

            workSheetHeader.Cells[7, 1].Value = "General Notes: ";
            workSheetHeader.Cells[7, 2].Value = selectedChapter.GeneralNotes;

            workSheetHeader.Cells[8, 1].Value = "Show Partner Notes: ";
            workSheetHeader.Cells[8, 2].Value = selectedChapter.ShowPartnerNotes;

            workSheetHeader.Cells[9, 1].Value = "Show Document Tracking: ";
            workSheetHeader.Cells[9, 2].Value = selectedChapter.ShowDocumentTracking;

            /*
             * 
             * Agenda
             * 
             * 
             */

            var workSheetAgenda = excel.Workbook.Worksheets.Add("Agenda");

            workSheetAgenda.TabColor = System.Drawing.Color.Black;
            workSheetAgenda.DefaultRowHeight = 12;

            workSheetAgenda.Row(1).Height = 82;
            workSheetAgenda.Row(1).Style.Font.Size = 8;
            workSheetAgenda.Row(1).Style.Font.Color.SetColor(System.Drawing.Color.DarkGray);
            workSheetAgenda.Cells[1, 1].Style.WrapText = true;
            workSheetAgenda.Cells[1, 1].Value = "I would like and Agenda folder (Progress File) where  I can store outgoing and incoming correspondence. The folder should be called:";

            //Header of table
            workSheetAgenda.Row(2).Height = 14;
            workSheetAgenda.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetAgenda.Row(2).Style.Font.Bold = true;
            workSheetAgenda.Cells[2, 1].Value = "Agenda Name";


            //Body of table
            int recordIndex = 3;
            foreach (var chapterItem in selectedChapter.Agendas.OrderBy(C => C.Name).ToList())
            {
                workSheetAgenda.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(chapterItem.Name) ? "" : chapterItem.Name;

                recordIndex++;
            }

            workSheetAgenda.Column(1).Width = 30;


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
            workSheetStatus.Row(1).Height = 82;
            workSheetStatus.Row(1).Style.Font.Size = 8;
            workSheetStatus.Row(1).Style.Font.Color.SetColor(System.Drawing.Color.DarkGray);
            workSheetStatus.Cells[1, 1].Style.WrapText = true;
            workSheetStatus.Cells[1, 1].Value = "I would like the following Statuses to be available in the Smartflow:";
            workSheetStatus.Cells[1, 2].Style.WrapText = true;
            workSheetStatus.Cells[1, 2].Value = "I would like the Status to update the Matter Milestone to:";
            workSheetStatus.Cells[1, 3].Style.WrapText = true;
            workSheetStatus.Cells[1, 3].Value = "The Smartflow will no longer reschedule when this status has been reached.";

            workSheetStatus.Row(2).Height = 14;
            workSheetStatus.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetStatus.Row(2).Style.Font.Bold = true;
            workSheetStatus.Cells[2, 1].Value = "Status Name";
            workSheetStatus.Cells[2, 2].Value = "Milestone";
            workSheetStatus.Cells[2, 3].Value = "End Of Flow (Y or Blank)";


            //Body of table
            recordIndex = 3;
            foreach (var chapterItem in selectedChapter.Status.OrderBy(C => C.SeqNo).ToList())
            {
                workSheetStatus.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(chapterItem.Name) ? "" : chapterItem.Name;
                workSheetStatus.Cells[recordIndex, 2].Value = string.IsNullOrEmpty(chapterItem.MilestoneStatus) ? "" : chapterItem.MilestoneStatus;
                workSheetStatus.Cells[recordIndex, 3].Value = string.IsNullOrEmpty(chapterItem.SuppressStep) ? "" : chapterItem.SuppressStep;

                recordIndex++;
            }

            workSheetStatus.Column(1).Width = 22;
            workSheetStatus.Column(2).Width = 22;
            workSheetStatus.Column(3).Width = 31;


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
            workSheetFees.Row(1).Height = 82;
            workSheetFees.Row(1).Style.Font.Size = 8;
            workSheetFees.Row(1).Style.Font.Color.SetColor(System.Drawing.Color.DarkGray);
            workSheetFees.Cells[1, 1].Style.WrapText = true;
            workSheetFees.Cells[1, 1].Value = "I would like to be able to automatically create a posting slip for the following fees/costs from the Smartflow screen";
            workSheetFees.Cells[1, 2].Style.WrapText = true;
            workSheetFees.Cells[1, 2].Value = "e.g. Disbursement, Our Fee or Additional Fee.";
            workSheetFees.Cells[1, 4].Style.WrapText = true;
            workSheetFees.Cells[1, 4].Value = "Is the fee/cost vateable?";
            workSheetFees.Cells[1, 5].Style.WrapText = true;
            workSheetFees.Cells[1, 5].Value = "(if unsure please see the Lookup sheet)\r\nThe posting type is:";

            workSheetFees.Row(2).Height = 14;
            workSheetFees.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetFees.Row(2).Style.Font.Bold = true;
            workSheetFees.Cells[2, 1].Value = "Fee Name";
            workSheetFees.Cells[2, 2].Value = "Fee Category";
            workSheetFees.Cells[2, 3].Value = "Fee Amount";
            workSheetFees.Cells[2, 4].Value = "VATable";
            workSheetFees.Cells[2, 5].Value = "Posting Type";

            //Body of table
            recordIndex = 3;
            foreach (var feeItem in selectedChapter.Fees.OrderBy(C => C.SeqNo).ToList())
            {
                workSheetFees.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(feeItem.FeeName) ? "" : feeItem.FeeName;
                workSheetFees.Cells[recordIndex, 2].Value = string.IsNullOrEmpty(feeItem.FeeName) ? "" : feeItem.FeeCategory;
                workSheetFees.Cells[recordIndex, 3].Value = feeItem.Amount.ToString();
                workSheetFees.Cells[recordIndex, 4].Value = string.IsNullOrEmpty(feeItem.FeeName) ? "" : feeItem.VATable;
                workSheetFees.Cells[recordIndex, 5].Value = string.IsNullOrEmpty(feeItem.FeeName) ? "" : feeItem.PostingType;

                recordIndex++;
            }

            var postingTypeValidation = workSheetFees.DataValidations.AddListValidation("E3:E300");
            postingTypeValidation.Formula.ExcelFormula = $"= Lookups!$D$3:$D${FeeDefinitions.Count()}";


            workSheetFees.Column(1).AutoFit();
            workSheetFees.Column(2).AutoFit();
            workSheetFees.Column(3).AutoFit();
            workSheetFees.Column(4).AutoFit();
            workSheetFees.Column(5).AutoFit();



            /*
             * 
             * Documents
             * 
             * 
             */

            var workSheetDocument = excel.Workbook.Worksheets.Add("Documents");

            workSheetDocument.TabColor = System.Drawing.Color.Black;
            workSheetDocument.DefaultRowHeight = 12;
            workSheetDocument.Row(1).Style.Font.Size = 8;
            workSheetDocument.Row(1).Style.Font.Color.SetColor(System.Drawing.Color.DarkGray);
            workSheetDocument.Row(1).Height = 82;
            workSheetDocument.Cells[1, 1].Style.WrapText = true;
            workSheetDocument.Cells[1, 1].Value = "Mandatory: \r\nI would like the following documents available from the Smartflow screen: ";
            workSheetDocument.Cells[1, 2].Style.WrapText = true;
            workSheetDocument.Cells[1, 2].Value = "Optional: \r\nI would like the documents to be displayed using the following name: ";
            workSheetDocument.Cells[1, 3].Style.WrapText = true;
            workSheetDocument.Cells[1, 3].Value = "Optional: \r\n'Y' or 'N'. The item will be a custom item:";
            workSheetDocument.Cells[1, 4].Style.WrapText = true;
            workSheetDocument.Cells[1, 4].Value = "Optional: \r\nThe item schedule in the Case Agenda should be named:";
            workSheetDocument.Cells[1, 5].Style.WrapText = true;
            workSheetDocument.Cells[1, 5].Value = "Optional: \r\nI would like the next item to be scheduled for this many days:";
            workSheetDocument.Cells[1, 6].Style.WrapText = true;
            workSheetDocument.Cells[1, 6].Value = "Optional: \r\nI would like the reschedule days to be based on this table field: ";
            workSheetDocument.Cells[1, 7].Style.WrapText = true;
            workSheetDocument.Cells[1, 7].Value = "Optional: \r\nOn running the document I would also like a history item created called: ";
            workSheetDocument.Cells[1, 8].Style.WrapText = true;
            workSheetDocument.Cells[1, 8].Value = "Optional: \r\nWhen the document is selected the Smartflow Status should be changed to:";
            workSheetDocument.Cells[1, 9].Style.WrapText = true;
            workSheetDocument.Cells[1, 9].Value = "Optional: \r\nThe document will be saved in the following agenda:";
            workSheetDocument.Cells[1, 10].Style.WrapText = true;
            workSheetDocument.Cells[1, 10].Value = "Optional: \r\nI would like the document to be inserted or taken automatically [INSERT or TAKE]:";
            workSheetDocument.Cells[1, 11].Style.WrapText = true;
            workSheetDocument.Cells[1, 11].Value = "Optional: \r\nI would like to track the document via the following method [Send Only or Response Required]";
            workSheetDocument.Cells[1, 12].Style.WrapText = true;
            workSheetDocument.Cells[1, 12].Value = "Optional: \r\nWhen chasing tracked documents. I would like the letter to display the following text:";

            workSheetDocument.Cells[1, 13].Style.WrapText = true;
            workSheetDocument.Cells[1, 13].Value = "Optional: \r\nWhen the document is selected the following user message should appear: ";
            workSheetDocument.Cells[1, 14].Style.WrapText = true;
            workSheetDocument.Cells[1, 14].Value = "Optional: \r\nWhen the document is selected the following pop up alert should appear:";
            workSheetDocument.Cells[1, 15].Style.WrapText = true;
            workSheetDocument.Cells[1, 15].Value = "Optional: \r\nWhen the document is processed the following field should be updated: ";
            workSheetDocument.Cells[1, 16].Style.WrapText = true;
            workSheetDocument.Cells[1, 16].Value = "Optional: \r\n'Y' or 'N'. The document will appear on the smartflow as optional: ";



            //Header of table
            workSheetDocument.Row(2).Height = 30;
            workSheetDocument.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetDocument.Row(2).Style.Font.Bold = true;
            workSheetDocument.Cells[2, 1].Value = "Item Name";
            workSheetDocument.Cells[2, 2].Value = "Alternative Item Name";
            workSheetDocument.Cells[2, 3].Value = "Custom Item";
            workSheetDocument.Cells[2, 4].Value = "Reschedule As Description";
            workSheetDocument.Cells[2, 5].Value = "Reschedule Days";
            workSheetDocument.Cells[2, 6].Value = "Reschedule Data Item";
            workSheetDocument.Cells[2, 7].Value = "Step History Description";
            workSheetDocument.Cells[2, 8].Value = "Status Change";
            workSheetDocument.Cells[2, 9].Value = "Agenda";
            workSheetDocument.Cells[2, 10].Value = "Action";
            workSheetDocument.Cells[2, 11].Value = "Tracking Method";
            workSheetDocument.Cells[2, 12].Value = "Chaser Description";
            workSheetDocument.Cells[2, 13].Value = "Item User Message";
            workSheetDocument.Cells[2, 14].Value = "Popup Alert";
            workSheetDocument.Cells[2, 15].Value = "Notes to Developer";
            workSheetDocument.Cells[2, 16].Value = "Optional Document";

            //Body of table
            recordIndex = 3;
            foreach (var chapterItem in selectedChapter.Documents.OrderBy(C => C.SeqNo).ToList())
            {
                workSheetDocument.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(chapterItem.Name) ? "" : chapterItem.Name;
                workSheetDocument.Cells[recordIndex, 2].Value = string.IsNullOrEmpty(chapterItem.AltDisplayName) ? "" : chapterItem.AltDisplayName;
                workSheetDocument.Cells[recordIndex, 3].Value = string.IsNullOrEmpty(chapterItem.CustomItem) ? "" : chapterItem.CustomItem;

                workSheetDocument.Cells[recordIndex, 4].Value = string.IsNullOrEmpty(chapterItem.AsName) ? "" : chapterItem.AsName;
                workSheetDocument.Cells[recordIndex, 5].Value = string.IsNullOrEmpty(chapterItem.AsName) ? "" : chapterItem.RescheduleDays is null ? "0" : chapterItem.RescheduleDays.ToString();
                workSheetDocument.Cells[recordIndex, 6].Value = string.IsNullOrEmpty(chapterItem.RescheduleDataItem) ? "" : chapterItem.RescheduleDataItem;
                workSheetDocument.Cells[recordIndex, 7].Value = string.IsNullOrEmpty(chapterItem.CompleteName) ? "" : chapterItem.CompleteName;
                workSheetDocument.Cells[recordIndex, 8].Value = string.IsNullOrEmpty(chapterItem.NextStatus) ? "" : chapterItem.NextStatus;
                workSheetDocument.Cells[recordIndex, 9].Value = string.IsNullOrEmpty(chapterItem.Agenda) ? "" : chapterItem.Agenda;
                workSheetDocument.Cells[recordIndex, 10].Value = string.IsNullOrEmpty(chapterItem.Action) ? "" : chapterItem.Action;
                workSheetDocument.Cells[recordIndex, 11].Value = string.IsNullOrEmpty(chapterItem.TrackingMethod) ? "" : chapterItem.TrackingMethod;
                workSheetDocument.Cells[recordIndex, 12].Value = string.IsNullOrEmpty(chapterItem.ChaserDesc) ? "" : chapterItem.ChaserDesc;

                workSheetDocument.Cells[recordIndex, 13].Value = string.IsNullOrEmpty(chapterItem.UserMessage) ? "" : chapterItem.UserMessage;
                workSheetDocument.Cells[recordIndex, 14].Value = string.IsNullOrEmpty(chapterItem.PopupAlert) ? "" : chapterItem.PopupAlert;
                workSheetDocument.Cells[recordIndex, 15].Value = string.IsNullOrEmpty(chapterItem.DeveloperNotes) ? "" : chapterItem.DeveloperNotes;
                workSheetDocument.Cells[recordIndex, 16].Value = string.IsNullOrEmpty(chapterItem.OptionalDocument) ? "" : chapterItem.OptionalDocument;


                //workSheetDocument.Cells[recordIndex, 6].DataValidation.AddListDataValidation().Formula.ExcelFormula = $"= Status!A3:A{selectedChapter.Items.Where(C => C.Type == "Status").ToList().Count() + 3}";

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
            workSheetDocument.Column(10).AutoFit();
            workSheetDocument.Column(11).AutoFit();
            workSheetDocument.Column(12).AutoFit();
            workSheetDocument.Column(13).AutoFit();
            workSheetDocument.Column(14).AutoFit();
            workSheetDocument.Column(15).AutoFit();
            workSheetDocument.Column(16).AutoFit();

            /*
             * 
             * Attachments
             * 
             * 
             */

            var workSheetAttachments = excel.Workbook.Worksheets.Add("Linked Items");

            workSheetAttachments.TabColor = System.Drawing.Color.DarkGray;
            workSheetAttachments.DefaultRowHeight = 12;


            //Header of table
            workSheetAttachments.Row(1).Height = 82;
            workSheetAttachments.Row(1).Style.Font.Size = 8;
            workSheetAttachments.Row(1).Style.Font.Color.SetColor(System.Drawing.Color.LightGray);
            workSheetAttachments.Cells[1, 1].Style.WrapText = true;
            workSheetAttachments.Cells[1, 1].Value = "Document name from which item to be attached to (Documents > Item Name).";
            workSheetAttachments.Cells[1, 2].Style.WrapText = true;
            workSheetAttachments.Cells[1, 2].Value = "Document name to be attached.";
            workSheetAttachments.Cells[1, 3].Style.WrapText = true;
            workSheetAttachments.Cells[1, 3].Value = "Alternative name to be shown in agenda.";
            workSheetAttachments.Cells[1, 4].Style.WrapText = true;
            workSheetAttachments.Cells[1, 4].Value = "Optional: \r\n'Y' or 'N'. The item will be a custom item:";
            workSheetAttachments.Cells[1, 5].Style.WrapText = true;
            workSheetAttachments.Cells[1, 5].Value = "The document will be saved in the following agenda:";
            workSheetAttachments.Cells[1, 6].Style.WrapText = true;
            workSheetAttachments.Cells[1, 6].Value = "TAKE / INSERT / SCHEDULE.";
            workSheetAttachments.Cells[1, 7].Style.WrapText = true;
            workSheetAttachments.Cells[1, 7].Value = "Send Only / Response Required";
            workSheetAttachments.Cells[1, 8].Style.WrapText = true;
            workSheetAttachments.Cells[1, 8].Value = "";
            workSheetAttachments.Cells[1, 9].Style.WrapText = true;
            workSheetAttachments.Cells[1, 9].Value = "If action is SCHEDULE, how many days the item will be scheduled for";
            workSheetAttachments.Cells[1, 10].Style.WrapText = true;
            workSheetAttachments.Cells[1, 10].Value = "If action is SCHEDULE, date field the schedule days are base from";
            workSheetAttachments.Cells[1, 11].Style.WrapText = true;
            workSheetAttachments.Cells[1, 11].Value = "Optional: \r\n'Y' or 'N'. The document will appear on the smartflow as optional: ";



            workSheetAttachments.Row(2).Height = 20;
            workSheetAttachments.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetAttachments.Row(2).Style.Font.Bold = true;
            workSheetAttachments.Cells[2, 1].Value = "Document Item Name ";
            workSheetAttachments.Cells[2, 2].Value = "Linked Item Name";
            workSheetAttachments.Cells[2, 3].Value = "Linked Item Display Name";
            workSheetAttachments.Cells[2, 4].Value = "Custom Item";
            workSheetAttachments.Cells[2, 5].Value = "Agenda";
            workSheetAttachments.Cells[2, 6].Value = "Action";
            workSheetAttachments.Cells[2, 7].Value = "Tracking Method";
            workSheetAttachments.Cells[2, 8].Value = "Chaser Description";
            workSheetAttachments.Cells[2, 9].Value = "Schedule Days";
            workSheetAttachments.Cells[2, 10].Value = "Schedule Data Item";
            workSheetAttachments.Cells[2, 11].Value = "Optional Document";

            //Body of table
            recordIndex = 3;
            foreach (var chapterItem in selectedChapter
                                            .Documents
                                            .Where(C => !(C.LinkedItems is null) && C.LinkedItems.Count() > 0)
                                            .OrderBy(C => C.SeqNo)
                                            .ToList())
            {
                foreach (var doc in chapterItem.LinkedItems)
                {
                    workSheetAttachments.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(chapterItem.AltDisplayName) ? (string.IsNullOrEmpty(chapterItem.Name) ? "" : chapterItem.Name) : chapterItem.AltDisplayName;
                    workSheetAttachments.Cells[recordIndex, 2].Value = string.IsNullOrEmpty(doc.DocName) ? "" : doc.DocName;
                    workSheetAttachments.Cells[recordIndex, 3].Value = string.IsNullOrEmpty(doc.DocAsName) ? "" : doc.DocAsName;
                    workSheetAttachments.Cells[recordIndex, 4].Value = string.IsNullOrEmpty(doc.CustomItem) ? "" : doc.CustomItem;

                    workSheetAttachments.Cells[recordIndex, 5].Value = string.IsNullOrEmpty(doc.Agenda) ? "" : doc.Agenda;

                    workSheetAttachments.Cells[recordIndex, 6].Value = string.IsNullOrEmpty(doc.Action) ? "" : doc.Action;
                    workSheetAttachments.Cells[recordIndex, 7].Value = string.IsNullOrEmpty(doc.TrackingMethod) ? "" : doc.TrackingMethod;
                    workSheetAttachments.Cells[recordIndex, 8].Value = string.IsNullOrEmpty(doc.ChaserDesc) ? "" : doc.ChaserDesc;
                    workSheetAttachments.Cells[recordIndex, 9].Value = doc.ScheduleDays;
                    workSheetAttachments.Cells[recordIndex, 10].Value = string.IsNullOrEmpty(doc.ScheduleDataItem) ? "" : doc.ScheduleDataItem; ;
                    workSheetAttachments.Cells[recordIndex, 11].Value = string.IsNullOrEmpty(doc.OptionalDocument) ? "" : doc.OptionalDocument; ;

                    recordIndex++;
                }
            }

            workSheetAttachments.Column(1).Width = 31;
            workSheetAttachments.Column(2).Width = 19;
            workSheetAttachments.Column(3).Width = 25;
            workSheetAttachments.Column(4).Width = 10;
            workSheetAttachments.Column(5).Width = 22;
            workSheetAttachments.Column(6).Width = 22;
            workSheetAttachments.Column(7).Width = 22;
            workSheetAttachments.Column(8).Width = 22;
            workSheetAttachments.Column(9).Width = 22;
            workSheetAttachments.Column(10).Width = 22;
            workSheetAttachments.Column(11).Width = 22;

            /*
           * 
           * Data Views
           * 
           * 
           */

            var workSheetDataViews = excel.Workbook.Worksheets.Add("Data Views");

            workSheetDataViews.TabColor = System.Drawing.Color.Black;
            workSheetDataViews.DefaultRowHeight = 12;

            //Header of table
            workSheetDataViews.Row(1).Height = 82;
            workSheetDataViews.Row(1).Style.Font.Size = 8;
            workSheetDataViews.Row(1).Style.Font.Color.SetColor(System.Drawing.Color.DarkGray);
            workSheetDataViews.Cells[1, 1].Style.WrapText = true;
            workSheetDataViews.Cells[1, 1].Value = "I would like the following P4W View(s) to be available in the Smartflow Screen:";
            workSheetDataViews.Cells[1, 2].Style.WrapText = true;
            workSheetDataViews.Cells[1, 2].Value = "Please name the view:";

            workSheetDataViews.Row(2).Height = 14;
            workSheetDataViews.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetDataViews.Row(2).Style.Font.Bold = true;
            workSheetDataViews.Cells[2, 1].Value = "View Name";
            workSheetDataViews.Cells[2, 2].Value = "Display Name";


            //Body of table
            recordIndex = 3;
            foreach (var viewItem in selectedChapter.DataViews.OrderBy(C => C.SeqNo).ToList())
            {
                workSheetDataViews.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(viewItem.ViewName) ? "" : viewItem.ViewName;
                workSheetDataViews.Cells[recordIndex, 2].Value = string.IsNullOrEmpty(viewItem.DisplayName) ? "" : viewItem.DisplayName;

                recordIndex++;
            }

            workSheetDataViews.Column(1).Width = 50;
            workSheetDataViews.Column(2).Width = 50;

            /*
             * 
             * Lookups
             * 
             * 
             */

            var workSheetLookUp = excel.Workbook.Worksheets.Add("Lookups");

            workSheetLookUp.DefaultRowHeight = 12;

            //Header of table

            workSheetLookUp.Row(1).Height = 30;
            workSheetLookUp.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetLookUp.Row(1).Style.Font.Bold = true;
            workSheetLookUp.Row(1).Style.Font.Color.SetColor(System.Drawing.Color.White);

            workSheetLookUp.Cells[1, 1, 1, 2].Merge = true;

            workSheetLookUp.Cells[1, 1].Style.Fill.SetBackground(System.Drawing.Color.Black);
            workSheetLookUp.Cells[1, 1].Value = $"Documents for Case Type Group: '{selectedChapter.P4WCaseTypeGroup}'";
            workSheetLookUp.Cells[2, 1].Value = "Document Type";
            workSheetLookUp.Cells[2, 2].Value = "Document Name";


            workSheetLookUp.Cells[1, 4, 1, 8].Merge = true;

            workSheetLookUp.Cells[1, 4].Style.Fill.SetBackground(System.Drawing.Color.Black);
            workSheetLookUp.Cells[1, 4].Value = "Posting Types For Fees";

            workSheetLookUp.Cells[2, 4].Value = "Posting Code";
            workSheetLookUp.Cells[2, 5].Value = "Account";
            workSheetLookUp.Cells[2, 6].Value = "Transaction Type";
            workSheetLookUp.Cells[2, 7].Value = "Destination";
            workSheetLookUp.Cells[2, 8].Value = "Transaction Description";


            //Body of table
            recordIndex = 3;
            foreach (var doc in documents
                .Where(D => D.CaseTypeGroupRef == caseTypeGroups
                                                    .Where(C => C.Name == selectedChapter.P4WCaseTypeGroup)
                                                    .Select(C => C.Id)
                                                    .SingleOrDefault()
                                                    ||
                            D.CaseTypeGroupRef == 0
                                                    )
                .OrderBy(D => D.Name).ToList())
            {
                workSheetLookUp.Cells[recordIndex, 1].Value = doc.DocumentType is null ? "" : docP4WTypes.Where(D => D.Key == doc.DocumentType).Select(D => D.Value).SingleOrDefault();
                workSheetLookUp.Cells[recordIndex, 2].Value = string.IsNullOrEmpty(doc.Name) ? "" : doc.Name;

                recordIndex++;
            }

            recordIndex = 3;
            foreach (var definition in FeeDefinitions.OrderBy(F => F.PostingCode).ToList())
            {
                workSheetLookUp.Cells[recordIndex, 4].Value = string.IsNullOrEmpty(definition.PostingCode) ? "" : definition.PostingCode;
                workSheetLookUp.Cells[recordIndex, 5].Value = string.IsNullOrEmpty(definition.Account) ? "" : definition.Account;
                workSheetLookUp.Cells[recordIndex, 6].Value = string.IsNullOrEmpty(definition.TransactionType) ? "" : definition.TransactionType;
                workSheetLookUp.Cells[recordIndex, 7].Value = string.IsNullOrEmpty(definition.Destination) ? "" : definition.Destination;
                workSheetLookUp.Cells[recordIndex, 8].Value = string.IsNullOrEmpty(definition.TransactionDescription) ? "" : definition.TransactionDescription;
                recordIndex++;
            }

            workSheetLookUp.Column(1).AutoFit();
            workSheetLookUp.Column(2).AutoFit();
            workSheetLookUp.Column(3).AutoFit();
            workSheetLookUp.Column(4).AutoFit();
            workSheetLookUp.Column(5).AutoFit();
            workSheetLookUp.Column(6).AutoFit();
            workSheetLookUp.Column(7).AutoFit();
            workSheetLookUp.Column(8).AutoFit();

            //Download SpreadSheet
            string excelName = $"{selectedChapter.Name}.xlsx";

            await jsRuntime.InvokeAsync<object>(
                 "DownloadTextFile",
                 excelName,
                 excel.GetAsByteArray());

            return "Success";
        }


        public SmartflowV2 ReadChapterDataFromExcel(string FilePath)
        {
            SmartflowV2 readChapters = new SmartflowV2 { Agendas = new List<SmartflowAgenda>()
                                                        , Status = new List<SmartflowStatus>()
                                                        , Documents = new List<SmartflowDocument>()
                                                        , Fees = new List<SmartflowFee>()
                                                        , DataViews = new List<SmartflowDataView>() };
            SmartflowAgenda agendaObject;
            SmartflowStatus statusObject;
            SmartflowDocument documentObject;
            SmartflowFee feeObject;
            SmartflowDataView readView;

            FileInfo fileInfo = new FileInfo(FilePath);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet workSheetHeader = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Overview").SingleOrDefault();
                int totalColumns = workSheetHeader.Dimension.End.Column;
                int totalRows = workSheetHeader.Dimension.End.Row;

                readChapters.ShowPartnerNotes = workSheetHeader.Cells[8, 2].FirstOrDefault() is null
                                            ? "N"
                                            : workSheetHeader.Cells[8, 2].Value is null
                                            ? "N"
                                            : workSheetHeader.Cells[8, 2].Value.ToString() == "Y"
                                            ? "Y"
                                            : "N";


                readChapters.ShowDocumentTracking = workSheetHeader.Cells[9, 2].FirstOrDefault() is null
                                            ? "N"
                                            : workSheetHeader.Cells[9, 2].Value is null
                                            ? "N"
                                            : workSheetHeader.Cells[9, 2].Value.ToString() == "Y"
                                            ? "Y"
                                            : "N";


                //Agenda
                ExcelWorksheet worksheetAgenda = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Agenda").SingleOrDefault();
                totalColumns = worksheetAgenda.Dimension.End.Column;
                totalRows = worksheetAgenda.Dimension.End.Row;

                for (int row = 3; row <= totalRows; row++)
                {
                    agendaObject = new SmartflowAgenda();

                    for (int column = 1; column <= totalColumns; column++)
                    {
                        if (column == 1) agendaObject.Name = worksheetAgenda.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetAgenda.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetAgenda.Cells[row, column].Value.ToString();
                    }

                    

                    if (!string.IsNullOrEmpty(agendaObject.Name))
                    {
                        readChapters.Agendas.Add(agendaObject);
                    }
                }

                //Status
                ExcelWorksheet worksheetStatus = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Status").SingleOrDefault();
                totalColumns = worksheetStatus.Dimension.End.Column;
                totalRows = worksheetStatus.Dimension.End.Row;

                for (int row = 3; row <= totalRows; row++)
                {
                    statusObject = new SmartflowStatus();

                    for (int column = 1; column <= totalColumns; column++)
                    {
                        if (column == 1) statusObject.Name = worksheetStatus.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetStatus.Cells[row, column].Value is null
                                            ? ""
                                            : Regex.Replace(worksheetStatus.Cells[row, column].Value.ToString(), "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                        if (column == 2) statusObject.MilestoneStatus = worksheetStatus.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetStatus.Cells[row, column].Value is null
                                            ? ""
                                            : Regex.Replace(worksheetStatus.Cells[row, column].Value.ToString(), "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                        if (column == 3) statusObject.SuppressStep = worksheetStatus.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetStatus.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetStatus.Cells[row, column].Value.ToString();
                    }

                    statusObject.SeqNo = row - 2;

                    if (!string.IsNullOrEmpty(statusObject.Name))
                    {
                        if (readChapters.Status.Where(I => I.Name == statusObject.Name).Count() == 0)
                        {
                            readChapters.Status.Add(statusObject);
                        }
                    }


                }

                //Fees
                ExcelWorksheet worksheetFees = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Fees").SingleOrDefault();
                totalColumns = worksheetFees.Dimension.End.Column;
                totalRows = worksheetFees.Dimension.End.Row;

                for (int row = 3; row <= totalRows; row++)
                {
                    feeObject = new SmartflowFee();

                    for (int column = 1; column <= totalColumns; column++)
                    {
                        if (column == 1) feeObject.FeeName = worksheetFees.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetFees.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetFees.Cells[row, column].Value.ToString();
                        if (column == 2) feeObject.FeeCategory = worksheetFees.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetFees.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetFees.Cells[row, column].Value.ToString();
                        try
                        {
                            if (column == 3) feeObject.Amount = worksheetFees.Cells[row, column].FirstOrDefault() is null
                                                                        ? 0
                                                                        : worksheetFees.Cells[row, column].Value is null
                                                                        ? 0
                                                                        : Convert.ToDecimal(worksheetFees.Cells[row, column].Value.ToString());
                        }
                        catch
                        {
                            feeObject.Amount = 0;
                        }
                        if (column == 4) feeObject.VATable = worksheetFees.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetFees.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetFees.Cells[row, column].Value.ToString();
                        if (column == 5) feeObject.PostingType = worksheetFees.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetFees.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetFees.Cells[row, column].Value.ToString();
                    }

                    feeObject.SeqNo = row - 2;

                    readChapters.Fees.Add(feeObject);
                }

                //Documents
                ExcelWorksheet worksheetDocuments = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Documents").FirstOrDefault();
                totalColumns = worksheetDocuments.Dimension.End.Column;
                totalRows = worksheetDocuments.Dimension.End.Row;

                for (int row = 3; row <= totalRows; row++)
                {
                    documentObject = new SmartflowDocument();

                    for (int column = 1; column <= totalColumns; column++)
                    {

                        if (column == 1) documentObject.Name = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : Regex.Replace(worksheetDocuments.Cells[row, column].Value.ToString(), "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                        if (column == 2) documentObject.AltDisplayName = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : Regex.Replace(worksheetDocuments.Cells[row, column].Value.ToString(), "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                        if (column == 3) documentObject.CustomItem = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? "N"
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? "N"
                                            : worksheetDocuments.Cells[row, column].Value.ToString() == "Y"
                                            ? "Y"
                                            : "N";
                        if (column == 4) documentObject.AsName = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : Regex.Replace(worksheetDocuments.Cells[row, column].Value.ToString(), "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");

                        try
                        {
                            if (column == 5) documentObject.RescheduleDays = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                                                            ? 0
                                                                            : worksheetDocuments.Cells[row, column].Value is null
                                                                            ? 0
                                                                            : Convert.ToInt32(worksheetDocuments.Cells[row, column].Value.ToString());
                        }
                        catch
                        {
                            documentObject.RescheduleDays = null;
                        }
                        if (column == 6) documentObject.RescheduleDataItem = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : Regex.Replace(worksheetDocuments.Cells[row, column].Value.ToString(), "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                        if (column == 7) documentObject.CompleteName = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : Regex.Replace(worksheetDocuments.Cells[row, column].Value.ToString(), "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                        if (column == 8) documentObject.NextStatus = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value.ToString();
                        if (column == 9) documentObject.Agenda = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value.ToString();                                            
                        if (column == 10) documentObject.Action = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? "INSERT"
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? "INSERT"
                                            : worksheetDocuments.Cells[row, column].Value.ToString().ToUpper();
                        if (column == 11) documentObject.TrackingMethod = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value.ToString();
                        if (column == 12) documentObject.ChaserDesc = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value.ToString();
                        if (column == 13) documentObject.UserMessage = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value.ToString();
                        if (column == 14) documentObject.PopupAlert = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value.ToString();
                        if (column == 15) documentObject.DeveloperNotes = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetDocuments.Cells[row, column].Value.ToString();
                        if (column == 16) documentObject.OptionalDocument = worksheetDocuments.Cells[row, column].FirstOrDefault() is null
                                            ? "N"
                                            : worksheetDocuments.Cells[row, column].Value is null
                                            ? "N"
                                            : worksheetDocuments.Cells[row, column].Value.ToString() == "Y"
                                            ? "Y"
                                            : "N";
                    }

                    documentObject.SeqNo = row - 2;


                    //
                    //Add Agenda not set on Agenda tab but exists on Documents tab
                    //
                    if (!string.IsNullOrEmpty(documentObject.Agenda))
                    {
                        if(!readChapters.Agendas.Select(I => I.Name).Contains(documentObject.Agenda)){
                            
                            readChapters.Agendas.Add(new SmartflowAgenda 
                                                    { 
                                                        Name = documentObject.Agenda, 
                                                    });
                        }
                    }

                    //
                    //Add Status not set on Status tab but exists on Documents tab
                    //
                    if (!string.IsNullOrEmpty(documentObject.NextStatus))
                    {
                        if(!readChapters.Status.Select(I => I.Name).Contains(documentObject.NextStatus)){
                            var seqNo = readChapters.Status.Select(I => I.SeqNo)
                                        .OrderByDescending(I => I)
                                        .FirstOrDefault();

                            seqNo = seqNo ?? -1;            
                            seqNo += 1;

                            readChapters.Status.Add(new SmartflowStatus 
                                                    { 
                                                        Name = documentObject.NextStatus, 
                                                        SeqNo = seqNo,
                                                        MilestoneStatus = ""
                                                    });
                        }
                    }

                    if (!string.IsNullOrEmpty(documentObject.Name))
                    {
                        readChapters.Documents.Add(documentObject);
                    }

                }

                //Attachments
                ExcelWorksheet worksheetAttachments = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Linked Items").SingleOrDefault();
                totalColumns = worksheetAttachments.Dimension.End.Column;
                totalRows = worksheetAttachments.Dimension.End.Row;

                string documentName = "";

                for (int row = 3; row <= totalRows; row++)
                {
                    documentObject = null;
                    LinkedDocument newAttachment = new LinkedDocument();

                    for (int column = 1; column <= totalColumns; column++)
                    {
                        if (column == 1)
                        {
                            documentName = worksheetAttachments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value.ToString();

                            documentObject = readChapters
                                .Documents
                                .Where(C => !string.IsNullOrEmpty(C.AltDisplayName) & C.AltDisplayName == documentName)
                                .FirstOrDefault();

                            if (documentObject is null)
                            {
                                documentObject = readChapters
                                .Documents
                                .Where(C => C.Name == documentName)
                                .FirstOrDefault();
                            }
                        }

                        if (!(documentObject is null))
                        {
                            if (column == 2) newAttachment.DocName = worksheetAttachments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value.ToString();
                            if (column == 3) newAttachment.DocAsName = worksheetAttachments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value.ToString();
                            if (column == 4) newAttachment.CustomItem = worksheetAttachments.Cells[row, column].FirstOrDefault() is null
                                            ? "N"
                                            : worksheetAttachments.Cells[row, column].Value is null
                                            ? "N"
                                            : worksheetAttachments.Cells[row, column].Value.ToString() == "Y"
                                            ? "Y"
                                            : "N";
                            if (column == 5) newAttachment.Agenda = worksheetAttachments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value.ToString();                
                            if (column == 6) newAttachment.Action = worksheetAttachments.Cells[row, column].FirstOrDefault() is null
                                            ? "INSERT"
                                            : worksheetAttachments.Cells[row, column].Value is null
                                            ? "INSERT"
                                            : worksheetAttachments.Cells[row, column].Value.ToString().ToUpper();
                            if (column == 7) newAttachment.TrackingMethod = worksheetAttachments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value.ToString();
                            if (column == 8) newAttachment.ChaserDesc = worksheetAttachments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value.ToString();
                            try
                            {
                                if (column == 9) newAttachment.ScheduleDays = worksheetAttachments.Cells[row, column].FirstOrDefault() is null
                                                                                ? 0
                                                                                : worksheetAttachments.Cells[row, column].Value is null
                                                                                ? 0
                                                                                : Convert.ToInt32(worksheetAttachments.Cells[row, column].Value.ToString());
                            }
                            catch
                            {
                                newAttachment.ScheduleDays = 0;
                            }

                            if (column == 10) newAttachment.ScheduleDataItem = worksheetAttachments.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetAttachments.Cells[row, column].Value.ToString();

                            if (column == 11) newAttachment.OptionalDocument = worksheetAttachments.Cells[row, column].FirstOrDefault() is null
                                            ? "N"
                                            : worksheetAttachments.Cells[row, column].Value is null
                                            ? "N"
                                            : worksheetAttachments.Cells[row, column].Value.ToString() == "Y"
                                            ? "Y"
                                            : "N";

                        }
                    }

                    if (!(documentObject is null))
                    {
                        if (!string.IsNullOrEmpty(newAttachment.Agenda))
                        {
                            if(!readChapters.Agendas.Select(A => A.Name).Contains(newAttachment.Agenda)){
                                
                                readChapters.Agendas.Add(new SmartflowAgenda 
                                                        { 
                                                            Name = newAttachment.Agenda, 
                                                        });
                            }
                        }

                        if (documentObject.LinkedItems is null)
                        {
                            documentObject.LinkedItems = new List<LinkedDocument>();
                        }
                        documentObject.LinkedItems.Add(newAttachment);
                    }

                }

                ExcelWorksheet worksheetDataViews = excelPackage.Workbook.Worksheets.Where(W => W.Name == "Data Views").SingleOrDefault();
                totalColumns = worksheetDataViews.Dimension.End.Column;
                totalRows = worksheetDataViews.Dimension.End.Row;

                for (int row = 3; row <= totalRows; row++)
                {
                    readView = new SmartflowDataView();

                    for (int column = 1; column <= totalColumns; column++)
                    {
                        if (column == 1) readView.ViewName = worksheetDataViews.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDataViews.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetDataViews.Cells[row, column].Value.ToString();
                        if (column == 2) readView.DisplayName = worksheetDataViews.Cells[row, column].FirstOrDefault() is null
                                            ? ""
                                            : worksheetDataViews.Cells[row, column].Value is null
                                            ? ""
                                            : worksheetDataViews.Cells[row, column].Value.ToString();
                    }

                    readView.SeqNo = row - 2;

                    readChapters.DataViews.Add(readView);
                }

            }




            return readChapters;
        }

    }
}
