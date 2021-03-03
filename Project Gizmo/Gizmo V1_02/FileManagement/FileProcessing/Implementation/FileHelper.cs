using BlazorInputFile;
using GadjIT.ClientContext.P4W;
using Gizmo_V1_02.FileManagement.FileClassObjects;
using Gizmo_V1_02.FileManagement.FileProcessing.Interface;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo_V1_02.FileManagement.FileProcessing.Implementation
{
    public class FileHelper : IFileHelper
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public bool IsFileValid { get; set; }

        public Action<string> ValidationAction { get; set; }

        public string CustomPath { get; set; }

        public FileHelper(IWebHostEnvironment webHost)
        {
            webHostEnvironment = webHost;
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
                fileList.AddRange(Directory.GetFiles(CustomPath).Select(F => new FileDesc { FileName = Path.GetFileName(F), FilePath = Path.GetFullPath(F), FileDirectory = Path.GetDirectoryName(F)}).ToList());

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



        public void Write(List<string> output)
        {
            // Write the string array to a new file named "WriteLines.txt".
            try
            {
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(CustomPath, "JSON.txt")))
                {
                    foreach (string line in output)
                        outputFile.WriteLine(line);
                }
            }
            catch
            {
                Directory.CreateDirectory(Path.GetFullPath(CustomPath));

                using (StreamWriter outputFile = new StreamWriter(Path.Combine(CustomPath, "JSON.txt")))
                {
                    foreach (string line in output)
                        outputFile.WriteLine(line);
                }
            }

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
                        if (column == 5) readObject.SeqNo = worksheet.Cells[row, column].FirstOrDefault() is null ? 0 : Convert.ToInt32(worksheet.Cells[row, column].Value.ToString());
                        if (column == 6) readObject.Name = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 7) readObject.AltDisplayName = worksheet.Cells[row, column].FirstOrDefault() is null ? "" : worksheet.Cells[row, column].Value.ToString();
                        if (column == 8) readObject.RescheduleDays = worksheet.Cells[row, column].FirstOrDefault() is null ? 0 : Convert.ToInt32(worksheet.Cells[row, column].Value.ToString());
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
