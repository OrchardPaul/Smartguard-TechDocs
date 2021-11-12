using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_01.Services
{
    public interface IChapterManagementService 
    {
        Task<List<string>> GetCaseTypeGroup();
        Task<List<string>> GetCaseTypes();
    }
}
