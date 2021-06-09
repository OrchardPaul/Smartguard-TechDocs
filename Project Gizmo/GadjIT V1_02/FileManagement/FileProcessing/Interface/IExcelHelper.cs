using GadjIT.GadjitContext.GadjIT_App.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GadjIT_V1_02.FileManagement.FileProcessing.Interface
{
    public interface IExcelHelper
    {
        Task<string> WriteChapterDataToExcel(CompanyAccountObject companyAccount, List<BillingItem> billingItems);
    }
}