using BlazorInputFile;
using Gizmo_V1_02.Services;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Data      
{
    public class FileUpload : IFileUpload
    {
        private readonly IWebHostEnvironment webHostEnvironment;


        public FileUpload(IWebHostEnvironment webHost)
        {
            webHostEnvironment = webHost;
        }


        public async Task Upload(IFileListEntry file)
        {
            var path = Path.Combine(webHostEnvironment.ContentRootPath, "UploadedFiles", file.Name);

            var MemStream = new MemoryStream();

            await file.Data.CopyToAsync(MemStream);

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                MemStream.WriteTo(fs);
            }
                
        }
    }
}
