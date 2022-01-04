using GadjIT.AppContext.GadjIT_App.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GadjIT_App.FileManagement.FileProcessing.Interface
{
    public interface IExcelHelper
    {
        Task<string> WriteChapterDataToExcel(CompanyAccountObject companyAccount, List<BillingItem> billingItems);
    }
}