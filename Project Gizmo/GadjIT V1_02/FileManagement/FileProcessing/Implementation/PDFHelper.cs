using GadjIT.GadjitContext.GadjIT_App.Custom;
using GadjIT_V1_02.FileManagement.FileProcessing.Interface;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.FileManagement.FileProcessing.Implementation
{
    public class PDFHelper : IPDFHelper
    {
        private readonly IJSRuntime jsRuntime;

        public PDFHelper(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }


        int MaxColumn = 4;
        Document Document;
        PdfPTable PdfPTable = new PdfPTable(4);
        PdfPCell PdfPCell;
        Font FontStyle;
        MemoryStream MemoryStream = new MemoryStream();
        public List<CompanyAccountObject> CompanyAccountObjects;

        public byte[] GenerateReport(List<CompanyAccountObject> companyAccountObjects)
        {
            this.CompanyAccountObjects = companyAccountObjects;

            Document = new Document(PageSize.A4, 10f, 10f, 20f, 30f);

            PdfPTable.WidthPercentage = 90;
            PdfPTable.HorizontalAlignment = Element.ALIGN_LEFT;
            FontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfWriter.GetInstance(Document, MemoryStream);
            Document.Open();

            float[] sizes = new float[MaxColumn];
            for (var i = 0; i < MaxColumn; i++)
            {
                if (i == 0) sizes[i] = 50;
                else sizes[i] = 100;
            }
            PdfPTable.SetWidths(sizes);

            this.ReportHeader();
            this.ReportBody();

            PdfPTable.HeaderRows = 2;
            Document.Add(PdfPTable);
            Document.Close();


            return MemoryStream.ToArray();
        }

        private void ReportHeader()
        {
            FontStyle = FontFactory.GetFont("Tahoma", 18f, 1);
            PdfPCell = new PdfPCell(new Phrase("Accounts Overview", FontStyle));
            PdfPCell.Colspan = MaxColumn;
            PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            PdfPCell.Border = 0;
            PdfPCell.ExtraParagraphSpace = 0;
            PdfPTable.AddCell(PdfPCell);
            PdfPTable.CompleteRow();
        }

        private void ReportBody()
        {
            FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);

            PdfPCell = new PdfPCell(new Phrase("Company Name", FontStyle));
            PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            PdfPCell.BackgroundColor = BaseColor.GRAY;
            PdfPTable.AddCell(PdfPCell);

            PdfPCell = new PdfPCell(new Phrase("Total SmartFlows", FontStyle));
            PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            PdfPCell.BackgroundColor = BaseColor.GRAY;
            PdfPTable.AddCell(PdfPCell);

            PdfPCell = new PdfPCell(new Phrase("SmartFlows Live", FontStyle));
            PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            PdfPCell.BackgroundColor = BaseColor.GRAY;
            PdfPTable.AddCell(PdfPCell);

            PdfPCell = new PdfPCell(new Phrase("SmartFlows Dev", FontStyle));
            PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            PdfPCell.BackgroundColor = BaseColor.GRAY;
            PdfPTable.AddCell(PdfPCell);

            PdfPTable.CompleteRow();

            foreach (var account in CompanyAccountObjects)
            {
                PdfPCell = new PdfPCell(new Phrase(account.CompanyObject.CompanyName, FontStyle));
                PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                PdfPCell.BackgroundColor = BaseColor.WHITE;
                PdfPTable.AddCell(PdfPCell);

                PdfPCell = new PdfPCell(new Phrase(account.SmartflowAccounts.Count().ToString(), FontStyle));
                PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                PdfPCell.BackgroundColor = BaseColor.WHITE;
                PdfPTable.AddCell(PdfPCell);

                PdfPCell = new PdfPCell(new Phrase(account.SmartflowAccounts.Where(S => S.System == "Live").Count().ToString(), FontStyle));
                PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                PdfPCell.BackgroundColor = BaseColor.WHITE;
                PdfPTable.AddCell(PdfPCell);

                PdfPCell = new PdfPCell(new Phrase(account.SmartflowAccounts.Where(S => S.System == "Dev").Count().ToString(), FontStyle));
                PdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                PdfPCell.BackgroundColor = BaseColor.WHITE;
                PdfPTable.AddCell(PdfPCell);

                PdfPTable.CompleteRow();
            }

        }

    }
}
