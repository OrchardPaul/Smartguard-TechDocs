using BlazorInputFile;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT.AppContext.GadjIT_App.Custom;
using GadjIT_V1_02.FileManagement.FileClassObjects;
using GadjIT_V1_02.FileManagement.FileProcessing.Interface;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GadjIT_V1_02.FileManagement.FileProcessing.Implementation
{
    public class ExcelHelper : IExcelHelper
    {
        private readonly IJSRuntime jsRuntime;

        public ExcelHelper(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task<string> WriteChapterDataToExcel(CompanyAccountObject companyAccount, List<BillingItem> billingItems)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new ExcelPackage();

            var workSheetHeader = excel.Workbook.Worksheets.Add("Overview");
            workSheetHeader.TabColor = System.Drawing.Color.RoyalBlue;
            workSheetHeader.DefaultRowHeight = 12;

            workSheetHeader.Cells[1, 1, 1, 16].Merge = true;

            workSheetHeader.Row(1).Height = 82;
            workSheetHeader.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetHeader.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheetHeader.Cells[1, 1].Style.Font.Name = "Arial Black";
            workSheetHeader.Cells[1, 1].Style.Font.Size = 16;
            workSheetHeader.Cells[1, 1].Value = companyAccount.CompanyObject.CompanyName;

            workSheetHeader.Column(1).Width = 40;
            workSheetHeader.Column(1).Style.Font.Bold = true;

            workSheetHeader.Cells[2, 1].Value = "Subscription Charge:";
            workSheetHeader.Cells[2, 2].Value = "£" + companyAccount.AccountObject.Subscription.ToString("F2");

            workSheetHeader.Cells[3, 1].Value = "Smartflow Charge:";
            workSheetHeader.Cells[3, 2].Value = "£" + billingItems.Select(S => S.MonthlyCharge).ToList().Sum().ToString("F2");

            var subTotal = billingItems.Select(S => S.MonthlyCharge).ToList().Sum() + companyAccount.AccountObject.Subscription;

            workSheetHeader.Cells[4, 1].Value = "Subtotal:";
            workSheetHeader.Cells[4, 2].Value = "£" + subTotal.ToString("F2");

            var VAT = subTotal / 100 * 20;

            workSheetHeader.Cells[5, 1].Value = "VAT:";
            workSheetHeader.Cells[5, 2].Value = "£" + VAT.ToString("F2");

            var Total = subTotal + VAT;

            workSheetHeader.Cells[6, 1].Value = "Total:";
            workSheetHeader.Cells[6, 2].Value = "£" + Total.ToString("F2");

            workSheetHeader.Cells[7, 1].Value = "Bill Date:";
            workSheetHeader.Cells[7, 2].Value = companyAccount.AccountObject.LastBilledDate.ToString("dd/MM/yyyy");

            //workSheetHeader.Cells[6, 1].Value = "Total Billed:";
            //workSheetHeader.Cells[6, 2].Value = "£" + companyAccount.AccountObject.TotalBilled.ToString("F2");

            //workSheetHeader.Cells[7, 1].Value = "Total Outstanding:";
            //workSheetHeader.Cells[7, 2].Value = "£" + companyAccount.SmartflowAccounts.Sum(S => S.Outstanding).ToString("F2");

            /*
             * 
             * BreakDown
             * 
             * 
             */

            var workSheetBreakdown = excel.Workbook.Worksheets.Add("Breakdown");

            workSheetBreakdown.TabColor = System.Drawing.Color.Black;
            workSheetBreakdown.DefaultRowHeight = 12;

            //Header of table
            workSheetBreakdown.Row(1).Height = 82;
            workSheetBreakdown.Row(1).Style.Font.Size = 8;
            workSheetBreakdown.Row(1).Style.Font.Color.SetColor(System.Drawing.Color.DarkGray);
            workSheetBreakdown.Cells[1, 1].Style.WrapText = true;
            workSheetBreakdown.Cells[1, 1].Value = ":";
            workSheetBreakdown.Cells[1, 2].Style.WrapText = true;
            workSheetBreakdown.Cells[1, 2].Value = "The Smartflow will no longer reschedule when this status has been reached.";

            workSheetBreakdown.Row(2).Height = 14;
            workSheetBreakdown.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheetBreakdown.Row(2).Style.Font.Bold = true;
            workSheetBreakdown.Cells[2, 1].Value = "Smartflow Name";
            workSheetBreakdown.Cells[2, 2].Value = "Bill";
            workSheetBreakdown.Cells[2, 3].Value = "Months Remaining";
            //workSheetBreakdown.Cells[2, 4].Value = "Outstanding";
            //workSheetBreakdown.Cells[2, 5].Value = "Total Billed";

            //Body of table
            int recordIndex = 3;
            foreach (var billingItem in billingItems.OrderBy(B => B.SmartflowName).ToList())
            {
                workSheetBreakdown.Cells[recordIndex, 1].Value = string.IsNullOrEmpty(billingItem.SmartflowName) ? "" : billingItem.SmartflowName;
                workSheetBreakdown.Cells[recordIndex, 2].Value = string.IsNullOrEmpty(billingItem.MonthlyCharge.ToString()) ? "" : "£" + billingItem.MonthlyCharge.ToString("F2");
                workSheetBreakdown.Cells[recordIndex, 3].Value = string.IsNullOrEmpty(billingItem.MonthsRemaing.ToString()) ? "" : billingItem.MonthsRemaing.ToString();
                //workSheetBreakdown.Cells[recordIndex, 4].Value = string.IsNullOrEmpty(billingItem.Outstanding.ToString()) ? "" : "£" + billingItem.Outstanding.ToString("F2");
                //workSheetBreakdown.Cells[recordIndex, 5].Value = string.IsNullOrEmpty(billingItem.Billed.ToString()) ? "" : "£" + billingItem.Billed.ToString("F2");

                recordIndex++;
            }

            workSheetBreakdown.Column(1).Width = 22;
            workSheetBreakdown.Column(2).Width = 31;
            workSheetBreakdown.Column(3).Width = 31;
            //workSheetBreakdown.Column(4).Width = 31;
            //workSheetBreakdown.Column(5).Width = 31;


            //Download SpreadSheet
            string excelName = $"{companyAccount.CompanyObject.CompanyName}.xlsx";

            await jsRuntime.InvokeAsync<object>(
                 "DownloadTextFile",
                 excelName,
                 excel.GetAsByteArray());

            return "Success";
        }
    }
}
