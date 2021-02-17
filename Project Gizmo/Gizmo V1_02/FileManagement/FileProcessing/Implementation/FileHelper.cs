using BlazorInputFile;
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

        public async Task Upload(IFileListEntry file)
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
                    }
                    catch
                    {
                        string fail = "Failed";
                    }
                }
                
            }
        }

        public string Read(IFileListEntry file)
        {
            using FileStream fs = File.OpenRead(file.Name);

            byte[] buf = new byte[1024];
            int c;
            var test = "";

            while ((c = fs.Read(buf, 0, buf.Length)) > 0)
            {
                test = Encoding.UTF8.GetString(buf, 0, c);
            }

            return test;
        }

        public string Write(IFileListEntry file)
        {
            using FileStream fs = File.OpenWrite(file.Name);

            byte[] buf = new byte[1024];
            int c;
            var test = "";

            while ((c = fs.Read(buf, 0, buf.Length)) > 0)
            {
                test = Encoding.UTF8.GetString(buf, 0, c);
            }

            return test;
        }

    }
}
