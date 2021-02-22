using BlazorInputFile;
using Gizmo_V1_02.FileManagement.FileClassObjects;
using Gizmo_V1_02.FileManagement.FileProcessing.Interface;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var test = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            return false;
        }

        public List<FileDesc> GetFileList()
        {
            var fileList = new List<FileDesc>();

            try
            {
                fileList.AddRange(Directory.GetFiles(CustomPath).Select(F => new FileDesc { FileName = Path.GetFileName(F), FilePath = Path.GetFullPath(F) }).ToList());

                return fileList;
            }
            catch
            {
                return fileList;
            }

        }

        public string ReadFileIntoString(string path)
        {
            using FileStream fs = File.OpenRead(path);

            byte[] buf = new byte[50000];
            int c;
            var result = "";

            while ((c = fs.Read(buf, 0, buf.Length)) > 0)
            {
                result = Encoding.UTF8.GetString(buf, 0, c);
            }

            return result;
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

    }
}
