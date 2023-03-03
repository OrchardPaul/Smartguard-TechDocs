using GadjIT_AppContext.GadjIT_App.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GadjIT_App.FileManagement.FileProcessing.Interface
{
    public interface IExcelHelper
    {
        Task<string> WriteSmartflowDataToExcel(CompanyAccountObject companyAccount, List<BillingItem> billingItems);
    }
}