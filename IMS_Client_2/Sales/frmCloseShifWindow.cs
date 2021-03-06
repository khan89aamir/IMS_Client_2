﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Web.UI.WebControls;
using System.Windows.Forms;
using CoreApp;

namespace IMS_Client_2.Sales
{
    public partial class frmCloseShifWindow : Form
    {
        public frmCloseShifWindow()
        {
            InitializeComponent();
        }

        clsUtility ObjUtil = new clsUtility();
        clsConnection_DAL ObjDAL = new clsConnection_DAL(true);

        public int pMasterCashClosingID = 0;
        int pStoreID = 0;
        decimal ExpensesAmt = 0, PettyCashBAL = 0;
        object Cashtotal = 0;
        DateTime CashBoxDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

        DataTable dtCash = new DataTable();
        DataTable dtCredit = new DataTable();
        DataTable dtExpenses = new DataTable();

        Image B_Leave = IMS_Client_2.Properties.Resources.B_click;
        Image B_Enter = IMS_Client_2.Properties.Resources.B_on;

        private void ClearAll()
        {
            Cashtotal = 0;
            txtTotalValue.Clear();
            dtCash = null;
            dtExpenses = null;
        }

        private void LoadData()
        {
            try
            {
                ObjDAL.SetStoreProcedureData("MasterCashClosingID", SqlDbType.Int, pMasterCashClosingID, clsConnection_DAL.ParamType.Input);
                ObjDAL.SetStoreProcedureData("StoreID", SqlDbType.Int, frmHome.Home_StoreID, clsConnection_DAL.ParamType.Input);
                ObjDAL.SetStoreProcedureData("PettyCashBAL", SqlDbType.Decimal, lblPettyCashBAL.Text, clsConnection_DAL.ParamType.Output);
                DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(clsUtility.DBName + ".dbo.SPR_Get_CashClosing_Details");
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    dtCredit = ds.Tables[1];
                    dtExpenses = ds.Tables[2];

                    DataTable dtoutput = ObjDAL.GetOutputParmData();
                    if (ObjUtil.ValidateTable(dtoutput))
                    {
                        PettyCashBAL = Convert.ToDecimal(dtoutput.Rows[0][1]);
                        lblPettyCashBAL.Text = PettyCashBAL.ToString();
                    }

                    if (ObjUtil.ValidateTable(dt))
                    {
                        dgvCloseCash.DataSource = dt;
                        dtCash = dt;
                        if (Convert.ToInt32(dt.Rows[0]["CashStatus"]) == 0)
                        {
                            dgvCloseCash.ReadOnly = false;
                            btnOpenCash.Enabled = false;
                            btnCloseCash.Enabled = true;
                            btnPreview.Enabled = false;
                            btnPrint.Enabled = false;

                            dgvCloseCash.DataSource = dtCash;
                        }
                        else
                        {
                            if (DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(dt.Rows[0]["CashBoxDate"]).ToString("yyyy-MM-dd"))
                            {
                                dgvCloseCash.ReadOnly = true;
                                btnOpenCash.Enabled = false;
                                btnCloseCash.Enabled = false;
                                btnPreview.Enabled = true;
                                btnPrint.Enabled = true;
                            }
                            else
                            {
                                dgvCloseCash.ReadOnly = true;
                                btnOpenCash.Enabled = true;
                                btnCloseCash.Enabled = false;
                                btnPreview.Enabled = true;
                                btnPrint.Enabled = true;
                            }
                        }
                        txtCashNo.Text = dt.Rows[0]["CashNo"].ToString();
                        txtCashierName.Text = dt.Rows[0]["Name"].ToString();
                        cmbShop.SelectedValue = dt.Rows[0]["StoreID"];
                        lblReturnedAmount.Text = dt.Rows[0]["CashReturn"].ToString();
                        txtTotalValue.Text = dt.Rows[0]["TotalCashValue"].ToString();
                    }
                    else
                    {
                        dgvCloseCash.DataSource = null;
                        dtCash = null;
                        dtExpenses = null;
                        btnOpenCash.Enabled = true;
                        btnCloseCash.Enabled = false;
                        btnPreview.Enabled = false;
                        btnPrint.Enabled = false;
                    }
                    ObjDAL.ResetData();
                }
                else
                {
                    dtCash = null;
                    dtExpenses = null;
                    dgvCloseCash.DataSource = null;
                    //btnOpenCash.Enabled = false;
                    btnOpenCash.Enabled = true;
                    btnCloseCash.Enabled = false;
                    btnPreview.Enabled = false;
                    btnPrint.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                clsUtility.ShowErrorMessage(ex.ToString());
            }
        }

        private void frmCloseShifWindow_Load(object sender, EventArgs e)
        {
            btnOpenCash.BackgroundImage = B_Leave;
            btnCloseCash.BackgroundImage = B_Leave;
            btnPreview.BackgroundImage = B_Leave;
            btnPrint.BackgroundImage = B_Leave;

            pStoreID = frmHome.Home_StoreID;
            FillStoreData();

            LoadListViewImage();

            listView1.Items[0].Selected = true;
            cmbShop.SelectedValue = pStoreID;
            //LoadData();
        }
        ImageList imageList = new ImageList();
        private void LoadListViewImage()
        {
            Image imgCash = Properties.Resources.Cash_in_Hand_light;
            Image imgCredit = Properties.Resources.Credit_card_1;
            Image imgExpense = Properties.Resources.expenses_1;

            imageList.Images.Add(imgCash);
            imageList.Images.Add(imgCredit);
            imageList.Images.Add(imgExpense);
            imageList.ImageSize = new Size(48, 48);
            listView1.View = System.Windows.Forms.View.LargeIcon;
            listView1.LargeImageList = imageList;

            listView1.Items[0].ImageIndex = 0;
            listView1.Items[1].ImageIndex = 1;
            listView1.Items[2].ImageIndex = 2;
        }

        private void btnOpenCash_MouseEnter(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackgroundImage = B_Enter;
        }

        private void btnOpenCash_MouseLeave(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackgroundImage = B_Leave;
        }

        private void dgvCloseCash_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ObjUtil.SetRowNumber(dgvCloseCash);
            ObjUtil.SetDataGridProperty(dgvCloseCash, DataGridViewAutoSizeColumnsMode.Fill, Color.White);
            if (listView1.Items[0].Selected)
            {
                dgvCloseCash.Columns["CashBandID"].Visible = false;
                dgvCloseCash.Columns["CashBand"].Visible = false;

                dgvCloseCash.Columns["CashBoxDate"].Visible = false;
                dgvCloseCash.Columns["CashStatus"].Visible = false;
                dgvCloseCash.Columns["StoreID"].Visible = false;
                dgvCloseCash.Columns["MasterCashClosingID"].Visible = false;
                dgvCloseCash.Columns["CashNo"].Visible = false;
                dgvCloseCash.Columns["EmployeeID"].Visible = false;
                dgvCloseCash.Columns["TotalCashValue"].Visible = false;
                dgvCloseCash.Columns["Name"].Visible = false;
                dgvCloseCash.Columns["CashReturn"].Visible = false;

                CalcTotalCashBand();

                dgvCloseCash.RowsDefaultCellStyle.SelectionBackColor = Color.White;
                dgvCloseCash.RowsDefaultCellStyle.SelectionForeColor = Color.Black;
            }
            else if (listView1.Items[1].Selected)
            {
                if (dgvCloseCash.Columns.Contains("CreditClosingID"))
                {
                    dgvCloseCash.Columns["CreditClosingID"].Visible = false;
                }
                if (dgvCloseCash.Columns.Contains("MasterCashClosingID"))
                {
                    dgvCloseCash.Columns["MasterCashClosingID"].Visible = false;
                }

                CalcTotalCredit();
            }
            else if (listView1.Items[2].Selected)
            {
                if (dgvCloseCash.Columns.Contains("PettyCashExpID"))
                {
                    dgvCloseCash.Columns["PettyCashExpID"].Visible = false;
                }
                if (dgvCloseCash.Columns.Contains("MasterCashClosingID"))
                {
                    dgvCloseCash.Columns["MasterCashClosingID"].Visible = false;
                }
                CalcTotalPettyCashExp();

                dgvCloseCash.RowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
                dgvCloseCash.RowsDefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            }
            dgvCloseCash.ClearSelection();
        }

        private string GetCashNumber()
        {
            //SequenceInvoice : this is a sequance object created in SQL ( this is not a table)
            long LastID = (long)ObjDAL.ExecuteScalar("SELECT NEXT VALUE FOR " + clsUtility.DBName + ".[dbo].Seq_CashNumber");
            return LastID.ToString();
        }

        private void OpenCashBox()
        {
            string NexCashNumber = GetCashNumber();
            int pStoreID = frmHome.Home_StoreID;
            ObjDAL.SetColumnData("CashNo", SqlDbType.NVarChar, NexCashNumber);
            ObjDAL.SetColumnData("StoreID", SqlDbType.Int, pStoreID);
            ObjDAL.SetColumnData("CashBoxDate", SqlDbType.Date, DateTime.Now.ToString("yyyy-MM-dd"));
            ObjDAL.SetColumnData("EmployeeID", SqlDbType.Int, clsUtility.LoginID);
            ObjDAL.SetColumnData("CashStatus", SqlDbType.Bit, false);
            ObjDAL.SetColumnData("CreatedBy", SqlDbType.Int, clsUtility.LoginID);

            pMasterCashClosingID = ObjDAL.InsertData(clsUtility.DBName + ".[dbo].tblMasterCashClosing", true);
            if (pMasterCashClosingID > 0)
            {
                clsUtility.ShowInfoMessage("Cash Box has been Opened !", clsUtility.strProjectTitle);
                ClearAll();
                LoadData();
                btnOpenCash.Enabled = false;
            }
        }

        private void btnOpenCash_Click(object sender, EventArgs e)
        {
            bool result = clsUtility.ShowQuestionMessage("Are you sure, you want to open cash box ?", clsUtility.strProjectTitle);
            if (result)
            {
                this.Cursor = Cursors.WaitCursor;
                System.Threading.Thread.Sleep(2000);
                OpenCashBox();
                this.Cursor = Cursors.Default;
            }
        }
        private void InsertCloseCashBand(DataTable dt)
        {
            DataRow[] drow = dt.Select("Count>0");
            int ID = 0;
            for (int i = 0; i < drow.Length; i++)
            {
                ObjDAL.SetColumnData("MasterCashClosingID", SqlDbType.Int, drow[i]["MasterCashClosingID"]);
                ObjDAL.SetColumnData("CashBandID", SqlDbType.Int, drow[i]["CashBandID"]);
                ObjDAL.SetColumnData("Count", SqlDbType.Int, drow[i]["Count"] == DBNull.Value ? 0 : drow[i]["Count"]);
                ObjDAL.SetColumnData("Value", SqlDbType.Decimal, drow[i]["Value"] == DBNull.Value ? 0 : drow[i]["Value"]);

                ObjDAL.SetColumnData("CreatedBy", SqlDbType.Int, clsUtility.LoginID);

                ID = ObjDAL.InsertData(clsUtility.DBName + ".[dbo].[tblCashClosing]", true);
            }
            if (ID > 0)
            {
                ObjDAL.UpdateColumnData("TotalCashValue", SqlDbType.Decimal, Cashtotal);
                ObjDAL.UpdateColumnData("CashStatus", SqlDbType.Int, 1);
                ObjDAL.UpdateColumnData("UpdatedBy", SqlDbType.Int, clsUtility.LoginID);
                ObjDAL.UpdateColumnData("UpdatedOn", SqlDbType.DateTime, DateTime.Now);
                int a = ObjDAL.UpdateData(clsUtility.DBName + ".[dbo].[tblMasterCashClosing]", "MasterCashClosingID=" + pMasterCashClosingID);
                if (a > 0)
                {
                    clsUtility.ShowInfoMessage("Cash Box has been Closed !");
                }
            }
            ObjDAL.ResetData();
        }

        private void InsertPettyCashExpenses()
        {
            bool b = false;
            if (ObjUtil.ValidateTable(dtExpenses))
            {
                for (int i = 0; i < dtExpenses.Rows.Count; i++)
                {
                    ObjDAL.SetStoreProcedureData("PettyCashExpAmt", SqlDbType.Decimal, dtExpenses.Rows[i]["ExpensesAmt"], clsConnection_DAL.ParamType.Input);
                    ObjDAL.SetStoreProcedureData("StoreID", SqlDbType.Int, frmHome.Home_StoreID, clsConnection_DAL.ParamType.Input);
                    ObjDAL.SetStoreProcedureData("Particulars", SqlDbType.NVarChar, dtExpenses.Rows[i]["Particulars"], clsConnection_DAL.ParamType.Input);
                    ObjDAL.SetStoreProcedureData("MasterCashClosingID", SqlDbType.Int, dtExpenses.Rows[i]["MasterCashClosingID"], clsConnection_DAL.ParamType.Input);
                    ObjDAL.SetStoreProcedureData("CreatedBy", SqlDbType.Int, clsUtility.LoginID, clsConnection_DAL.ParamType.Input);

                    b = ObjDAL.ExecuteStoreProcedure_DML(clsUtility.DBName + ".[dbo].SPR_Insert_PettyCashExpAmt");
                }
                if (b)
                {
                    clsUtility.ShowInfoMessage("Petty Cash Book has been Saved !", clsUtility.strProjectTitle);
                }
                ObjDAL.ResetData();
            }
        }

        private bool ValidatePettyCashExpAmt()
        {
            if (PettyCashBAL >= ExpensesAmt)
            {
                for (int i = 0; i < dtExpenses.Rows.Count; i++)
                {
                    if (dtExpenses.Rows[i]["MasterCashClosingID"] == DBNull.Value || dtExpenses.Rows[i]["MasterCashClosingID"].ToString() == "" || Convert.ToDecimal(dtExpenses.Rows[i]["ExpensesAmt"]) == 0)
                    {
                        dtExpenses.Rows[i].Delete();
                        dtExpenses.AcceptChanges();
                        return true;
                    }
                    else
                    {
                        if (dtExpenses.Rows[i]["Particulars"] == DBNull.Value || dtExpenses.Rows[i]["Particulars"].ToString() == "" || dtExpenses.Rows[i]["ExpensesAmt"] == DBNull.Value)
                        {
                            clsUtility.ShowInfoMessage("Enter valid entry for Expenses", clsUtility.strProjectTitle);
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                clsUtility.ShowInfoMessage("You have added Extra " + (PettyCashBAL - ExpensesAmt) + " Expenses..", clsUtility.strProjectTitle);
                return false;
            }
        }

        private void btnCloseCash_Click(object sender, EventArgs e)
        {
            if (listView1.Items[0].Selected || listView1.Items[2].Selected)
            {
                //DataTable dt = (DataTable)dgvCloseCash.DataSource;
                DataTable dt = dtCash;
                double total = Convert.ToDouble(Cashtotal);
                if (ObjUtil.ValidateTable(dt) && total > 0)
                {
                    if (ValidatePettyCashExpAmt())
                    {
                        InsertCloseCashBand(dt);
                        InsertPettyCashExpenses();
                        ClearAll();
                        listView1.Items[0].Selected = true;
                        listView1_SelectedIndexChanged(sender, e);
                        //LoadData();

                        // Set isfromdefaul = true for closing form without confirmation
                        Sales.Sales_Invoice.isfromdefaul = true;
                       // ObjUtil.CloseAutoExtender(typeof(Sales.Sales_Invoice));
                        Sales.Sales_Invoice.isfromdefaul = false;
                    }
                }
                else
                {
                    clsUtility.ShowInfoMessage("Please Enter CashBand for Closing Today's Cash.");
                }
            }
            else
            {
                clsUtility.ShowInfoMessage("Please Select Cash Option from List.");
            }
        }

        private void dgvCloseCash_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            int column = dgvCloseCash.CurrentCell.ColumnIndex;
            string headerText = dgvCloseCash.Columns[column].HeaderText;

            e.Cancel = false;
            if (listView1.Items[0].Selected)
            {
                if (headerText == "Count" && e.FormattedValue.ToString() != "")
                {
                    if (Convert.ToInt32(e.FormattedValue) == 0)
                    {
                        clsUtility.ShowInfoMessage("Enter Valid Count..");
                        e.Cancel = true;
                    }
                    return;
                }

                //if (e.ColumnIndex == 3 && e.FormattedValue.ToString() != "")
                //{
                //    e.Cancel = false;
                //    dgvCloseCash.Rows[e.RowIndex].ErrorText = "";
                //    int newInteger = 0;
                //    if (dgvCloseCash.Rows[e.RowIndex].IsNewRow) { return; }
                //    if (!int.TryParse(e.FormattedValue.ToString(),
                //        out newInteger) || newInteger < 0)
                //    {
                //        e.Cancel = true;
                //        clsUtility.ShowInfoMessage("Enter Only Numbers..");
                //    }
                //}
            }
            else if (listView1.Items[2].Selected)
            {
                if (headerText == "ExpensesAmt" && e.FormattedValue.ToString() != "")
                { 
                    if (Convert.ToDecimal(e.FormattedValue) == 0)
                    {
                        clsUtility.ShowInfoMessage("Enter Valid Expenses Amount..");
                        e.Cancel = true;
                    }
                    return;
                }
                //if (e.ColumnIndex == 3 && e.FormattedValue.ToString() != "")
                //{
                //    if (dgvCloseCash.Rows[e.RowIndex].IsNewRow) { return; }

                //    decimal newDecimal = 0;
                //    if (!decimal.TryParse(e.FormattedValue.ToString(),
                //        out newDecimal) || newDecimal < 0)
                //    {
                //        e.Cancel = true;
                //        clsUtility.ShowInfoMessage("Enter Only Numbers..");
                //        //dgvQtycolor.Rows[e.RowIndex].ErrorText = "Size must be a Positive integer";
                //    }
                //}
            }
        }
        private void FillStoreData()
        {
            DataTable dt = ObjDAL.GetDataCol(clsUtility.DBName + ".dbo.StoreMaster", "StoreID,StoreName", "ISNULL(ActiveStatus,1)=1", "StoreName ASC");
            cmbShop.DataSource = dt;
            cmbShop.DisplayMember = "StoreName";
            cmbShop.ValueMember = "StoreID";
            cmbShop.SelectedIndex = -1;
        }

        private void CalcTotalCashBand()
        {
            //DataTable dt = (DataTable)dgvCloseCash.DataSource;
            DataTable dt = dtCash;
            if (ObjUtil.ValidateTable(dt))
            {
                Cashtotal = dt.Compute("SUM(Value)", string.Empty);
                txtTotalValue.Text = Cashtotal.ToString();
            }
        }

        private void CalcTotalCredit()
        {
            if (ObjUtil.ValidateTable(dtCredit))
            {
                txtTotalValue.Text = dtCredit.Compute("SUM(Value)", string.Empty).ToString();
            }
        }

        private void CalcTotalPettyCashExp()
        {
            try
            {
                DataTable dt = dtExpenses;
                if (ObjUtil.ValidateTable(dt))
                {
                    //DataRow[] drow = dt.Select("SUM(ExpensesAmt)");
                    //txtTotalValue.Text = drow[0].ToString();

                    //object total = dt.Compute("SUM(ExpensesAmt)", "ExpensesAmt IS NOT NULL");
                    if (dt.Rows[0]["ExpensesAmt"] != DBNull.Value)
                    {
                        ExpensesAmt = dt.AsEnumerable().Sum(x => Convert.ToDecimal(x["ExpensesAmt"]));
                        txtTotalValue.Text = ExpensesAmt.ToString();
                    }
                }
            }
            catch { }
        }

        private void dgvCloseCash_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //if (listView1.Items[0].Selected)
            //{
            //    double CashBand = dgvCloseCash.Rows[e.RowIndex].Cells["CashBand"].Value == DBNull.Value ? 0 : Convert.ToDouble(dgvCloseCash.Rows[e.RowIndex].Cells["CashBand"].Value);
            //    int Count = dgvCloseCash.Rows[e.RowIndex].Cells["Count"].Value == DBNull.Value ? 0 : Convert.ToInt32(dgvCloseCash.Rows[e.RowIndex].Cells["Count"].Value);
            //    dgvCloseCash.Rows[e.RowIndex].Cells["Value"].Value = Math.Round(CashBand * Count, 3);

            //    CalcTotalCashBand();
            //}
            //else if (listView1.Items[2].Selected)
            //{
            //    if (ObjUtil.ValidateTable(dtExpenses))
            //    {
            //        dtExpenses.Rows[e.RowIndex]["MasterCashClosingID"] = pMasterCashClosingID;
            //        dtExpenses.Rows[e.RowIndex]["PettyCashExpID"] = 0;
            //        if (e.RowIndex + 1 == dgvCloseCash.Rows.Count)
            //        {
            //            DataRow Row = dtExpenses.NewRow();
            //            Row["ExpensesAmt"] = 0;
            //            dtExpenses.Rows.Add(Row);
            //        }
            //        dtExpenses.AcceptChanges();
            //        dgvCloseCash.DataSource = dtExpenses;
            //    }
            //    CalcTotalPettyCashExp();
            //}
        }

        private void dgvCloseCash_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            dgvCloseCash.Columns[e.Column.Index].ReadOnly = true;
            if (listView1.Items[0].Selected)
            {
                if (dgvCloseCash.Columns.Contains("Count"))
                {
                    dgvCloseCash.Columns["Count"].ReadOnly = false;
                }
            }
            else if (listView1.Items[2].Selected)
            {
                if (dgvCloseCash.Columns.Contains("Particulars"))
                {
                    dgvCloseCash.Columns["Particulars"].ReadOnly = false;
                }
                if (dgvCloseCash.Columns.Contains("ExpensesAmt"))
                {
                    dgvCloseCash.Columns["ExpensesAmt"].ReadOnly = false;
                }
            }
            dgvCloseCash.Columns[e.Column.Index].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.Items[0].Selected)
                {
                    if (!ObjUtil.ValidateTable(dtCash))
                    {
                        LoadData();
                    }
                    else
                    {
                        dgvCloseCash.DataSource = dtCash;
                    }
                }
                else if (listView1.Items[1].Selected)
                {
                    txtTotalValue.Clear();
                    dgvCloseCash.DataSource = dtCredit;
                }
                else if (listView1.Items[2].Selected)
                {
                    txtTotalValue.Clear();
                    dgvCloseCash.DataSource = dtExpenses;
                }
            }
            catch (Exception ex)
            {
                LoadData();
                clsUtility.ShowErrorMessage(ex.ToString());
            }
        }

        private void PrintReport(bool Direct)
        {
            ObjDAL.SetStoreProcedureData("MasterCashClosingID", SqlDbType.Int, pMasterCashClosingID, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("StoreID", SqlDbType.Int, frmHome.Home_StoreID, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("PettyCashBAL", SqlDbType.Decimal, lblPettyCashBAL.Text, clsConnection_DAL.ParamType.Output);
            DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(clsUtility.DBName + ".dbo.SPR_Get_CashClosing_Details");

            DataTable dtCashDetails = ds.Tables[0];
            DataTable dtCredit = ds.Tables[1];
            string TotalExpenses = "0.00";
            try
            {
                if (ObjUtil.ValidateTable(dtExpenses))
                {
                    TotalExpenses = Convert.ToString(dtExpenses.Compute("Sum(ExpensesAmt)", string.Empty));
                }
            }
            catch (Exception ex)
            {
            }

            Report.Report_Forms.frmClosingCashReport frmClosingCashReport = new Report.Report_Forms.frmClosingCashReport();
            frmClosingCashReport.CashierNo = GetCashierNumber(dtCashDetails.Rows[0]["EmployeeID"].ToString()); ;
            frmClosingCashReport.ShopAddress = GetShopAddress();
            frmClosingCashReport.ShopName = cmbShop.Text.ToString();
            frmClosingCashReport.CashNumber = dtCashDetails.Rows[0]["CashNo"].ToString();
            frmClosingCashReport.TotalCash = dtCashDetails.Rows[0]["TotalCashValue"].ToString();
            frmClosingCashReport.TotalCredit = GetTotalCreditValue();
            frmClosingCashReport.GrandToatl = Convert.ToString(Convert.ToDecimal(frmClosingCashReport.TotalCash) + Convert.ToDecimal(frmClosingCashReport.TotalCredit));
            frmClosingCashReport.dtCashDetails = dtCashDetails;
            frmClosingCashReport.IsDirectPrint = Direct;
            frmClosingCashReport.dtCreditDetails = dtCredit;
            frmClosingCashReport.PettyCash = lblPettyCashBAL.Text;
            frmClosingCashReport.PettyCashBeforeExp = Convert.ToString(Convert.ToDecimal(lblPettyCashBAL.Text) + Convert.ToDecimal(TotalExpenses));
            frmClosingCashReport.dtExpanses = this.dtExpenses;
            frmClosingCashReport.TotalExpenses = TotalExpenses;

            frmClosingCashReport.Show();
        }
        private void btnPreview_Click(object sender, EventArgs e)
        {
            PrintReport(false);
        }
        private string GetCashierNumber(string empID)
        {
            object obj = ObjDAL.ExecuteScalar("SELECT EmployeeCode FROM " + clsUtility.DBName + ".dbo.EmployeeDetails WITH(NOLOCK) WHERE EmpID=" + empID);
            if (obj == null)
            {
                return "NA";
            }
            else
            {
                return obj.ToString();
            }
        }
        private string GetShopAddress()
        {
            return ObjDAL.ExecuteScalar("SELECT Place+' ,Tel :'+Tel FROM " + clsUtility.DBName + ".dbo.StoreMaster WITH(NOLOCK) WHERE StoreID=" + cmbShop.SelectedValue.ToString()).ToString();
        }
        private string GetTotalCreditValue()
        {
            DataTable dt = ObjDAL.ExecuteSelectStatement("SELECT ISNULL(SUM(Value),0) FROM " + clsUtility.DBName + ".[dbo].[tblCreditClosing] WITH(NOLOCK) WHERE MasterCashClosingID=" + pMasterCashClosingID + " AND [Type]!='Cash'");
            if (ObjUtil.ValidateTable(dt))
            {
                return dt.Rows[0][0].ToString();
            }
            return "0";
        }

        private void dgvCloseCash_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //int column = dgvCloseCash.CurrentCell.ColumnIndex;
                //string headerText = dgvCloseCash.Columns[column].HeaderText;

                if (listView1.Items[0].Selected)
                {
                    if (e.ColumnIndex == 3)
                    //if (headerText == "Count") // dont uncomment because it's iterate
                    {
                        double CashBand = dgvCloseCash.Rows[e.RowIndex].Cells["CashBand"].Value == DBNull.Value ? 0 : Convert.ToDouble(dgvCloseCash.Rows[e.RowIndex].Cells["CashBand"].Value);
                        int Count = dgvCloseCash.Rows[e.RowIndex].Cells["Count"].Value.ToString() == "" ? 0 : Convert.ToInt32(dgvCloseCash.Rows[e.RowIndex].Cells["Count"].Value);

                        double total = Math.Round(CashBand * Count, 3);
                        dgvCloseCash.Rows[e.RowIndex].Cells["Value"].Value = total;//Math.Round(CashBand * Count, 3);

                        CalcTotalCashBand();
                    }
                }
                else if (listView1.Items[2].Selected)
                {
                    if (ObjUtil.ValidateTable(dtExpenses) && e.ColumnIndex >= 0)
                    {
                        dtExpenses.Rows[e.RowIndex]["MasterCashClosingID"] = pMasterCashClosingID;
                        dtExpenses.Rows[e.RowIndex]["PettyCashExpID"] = 0;
                        if (e.RowIndex + 1 == dgvCloseCash.Rows.Count)
                        {
                            DataRow Row = dtExpenses.NewRow();
                            Row["ExpensesAmt"] = 0;
                            dtExpenses.Rows.Add(Row);
                        }
                        dtExpenses.AcceptChanges();
                        dgvCloseCash.DataSource = dtExpenses;
                    }
                    CalcTotalPettyCashExp();
                }
            }
            catch (Exception ex)
            {
                clsUtility.ShowErrorMessage(ex.ToString());
            }
        }

        private void dgvCloseCash_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvCloseCash.CurrentCell.ColumnIndex;
            string headerText = dgvCloseCash.Columns[column].HeaderText;

            if (headerText == "ExpensesAmt")
            {
                e.Control.KeyPress += Decimal_Control_KeyPress;
            }
            else if (headerText == "Count")
            {
                e.Control.KeyPress += Int_Control_KeyPress;
            }
            else if (headerText == "Particulars")
            {
                e.Control.KeyPress += String_Control_KeyPress;
            }
        }

        private void Int_Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            //string k = e.KeyChar.ToString();
            //TextBox txt = (TextBox)sender;
            e.Handled = ObjUtil.IsNumeric(e);
        }

        private void Decimal_Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            //string k = e.KeyChar.ToString();
            TextBox txt = (TextBox)sender;
            e.Handled = ObjUtil.IsDecimal(txt, e);
        }

        private void String_Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = ObjUtil.IsAlphaNumeric(e);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintReport(true);
        }

        private bool CloseSalesForm(Type formType)
        {
            foreach (Form form in Application.OpenForms)
                if (form.GetType().Name == formType.Name)
                {
                    form.Close();
                    return true;
                }
                return false;
        }
    }
}