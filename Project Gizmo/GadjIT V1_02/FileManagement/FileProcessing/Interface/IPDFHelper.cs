using GadjIT.GadjitContext.GadjIT_App.Custom;
using System.Collections.Generic;

namespace GadjIT_V1_02.FileManagement.FileProcessing.Interface
{
    public interface IPDFHelper
    {
        byte[] GenerateReport(List<CompanyAccountObject> companyAccountObjects);
    }
}