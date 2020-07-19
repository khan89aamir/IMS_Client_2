﻿using CoreApp;
using IMS_Client_2.Barcode;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IMS_Client_2.Report.Report_Forms
{
    public partial class frmClosingCashReport : Form
    {
        public frmClosingCashReport()
        {
            InitializeComponent();
        }

        public DataTable dtCashDetails = new DataTable();
        public DataTable dtCreditDetails = new DataTable();

        public string ShopName, ShopAddress, CashierNo, CashNumber, TotalCash, TotalCredit, GrandToatl;
        public bool IsDirectPrint = false;

        private void frmClosingCashReport_Load(object sender, EventArgs e)
        {
            if (IsDirectPrint)
            {
                LoadReport();
                ReportPrinting reportPrinting = new ReportPrinting();
                reportPrinting.Export(this.reportViewer1.LocalReport);

                PrinterSettings printerSetting = new PrinterSettings();
                if (clsBarCodeUtility.GetPrinterName(clsBarCodeUtility.PrinterType.InvoicePrinter).Trim().Length == 0)
                {
                    bool b = clsUtility.ShowQuestionMessage("Printer Not Configured for barcode. Do you want to print on default printer?", clsUtility.strProjectTitle);
                    if (b == false)
                    {
                        return;
                    }

                }
                printerSetting.PrinterName = clsBarCodeUtility.GetPrinterName(clsBarCodeUtility.PrinterType.BarCodePrinter);



                reportPrinting.Print(printerSetting);

                this.Close();
            }
            else
            {
                // this.reportViewer1.LocalReport.ReportEmbeddedResource = "IMS_Client_2.Report.RDLC_Files.CashClosingReport.rdlc";
                LoadReport();

                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;
                this.reportViewer1.RefreshReport();
            }

        }
        private void LoadReport()
        {
            reportViewer1.LocalReport.DataSources.Clear();

            ReportDataSource rds = new ReportDataSource("dsCashClosing", dtCashDetails);
            ReportDataSource rds2 = new ReportDataSource("dsCreditClosing", dtCreditDetails);

            // creating the parameter with the extact name as in the report.
            ReportParameter param1 = new ReportParameter("ShopeName", ShopName, true);
            ReportParameter param2 = new ReportParameter("Address", ShopAddress, true);
            ReportParameter param3 = new ReportParameter("Date", DateTime.Now.Date.ToString("yyyy-MM-dd"), true);
            ReportParameter param4 = new ReportParameter("Time", DateTime.Now.ToShortTimeString(), true);
            ReportParameter param5 = new ReportParameter("CasheirNo", CashierNo, true);
            ReportParameter param6 = new ReportParameter("CashNo", CashNumber, true);
            ReportParameter param9 = new ReportParameter("TotalCredit", TotalCredit, true);
            ReportParameter param7 = new ReportParameter("TotalCash", TotalCash, true);
            ReportParameter param8 = new ReportParameter("GrandCashValue", GrandToatl, true);


            // adding the parameter in the report dynamically
            reportViewer1.LocalReport.SetParameters(param1);
            reportViewer1.LocalReport.SetParameters(param2);
            reportViewer1.LocalReport.SetParameters(param3);
            reportViewer1.LocalReport.SetParameters(param4);
            reportViewer1.LocalReport.SetParameters(param5);
            reportViewer1.LocalReport.SetParameters(param6);
            reportViewer1.LocalReport.SetParameters(param7);
            reportViewer1.LocalReport.SetParameters(param8);
            reportViewer1.LocalReport.SetParameters(param9);


            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.LocalReport.DataSources.Add(rds2);

            this.reportViewer1.RefreshReport();
        }
    }
}