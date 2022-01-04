using GadjIT.AppContext.GadjIT_App.Custom;
using System.Collections.Generic;

namespace GadjIT_App.FileManagement.FileProcessing.Interface
{
    public interface IPDFHelper
    {
        byte[] GenerateReport(List<CompanyAccountObject> companyAccountObjects);
    }
}