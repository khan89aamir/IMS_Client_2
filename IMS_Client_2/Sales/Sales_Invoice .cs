﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CoreApp;


namespace IMS_Client_2.Sales
{
    public partial class Sales_Invoice : Form
    {
        public Sales_Invoice()
        {
            InitializeComponent();
        }
        clsConnection_DAL ObjDAL = new clsConnection_DAL(true);
        clsUtility ObjUtil = new clsUtility();

        public bool IsReplaceReturnMode = false;
        public string OldInvoiceID = "";

        DataTable dtItemDetails = new DataTable();
        DataTable dtReplcaeItemDetails = new DataTable();

        Image B_Leave = IMS_Client_2.Properties.Resources.B_click;
        Image B_Enter = IMS_Client_2.Properties.Resources.B_on;
        private void Sales_Invoice_Load(object sender, EventArgs e)
        {
            cboEntryMode.SelectedIndex = 0; // by default

            btnAdd.BackgroundImage = B_Leave;
            btnSave.BackgroundImage = B_Leave;
            btnPrint.BackgroundImage = B_Leave;
            btnSaveData.BackgroundImage = B_Leave;


            BindStoreDetails();

            // GenerateInvoiceNumber();
            InitItemTable();
            dtpSalesDate.Value = DateTime.Now;
            txtBarCode.Focus();
            dtpSalesDate.MaxDate = DateTime.Now;
            if (!IsReplaceReturnMode)
            {
                tabControl1.TabPages.RemoveAt(1);
                radNewItem.Visible = false;
                radReplace.Visible = false;
                lblTitle.Visible = false;

                label16.Enabled = false;
                txtOldBillAmount.Enabled = false;
            }
            else
            {
                radReplace.Checked = true;
                label16.Enabled = true;
                txtOldBillAmount.Enabled = true;    
            }





        }
        private void btnAdd_MouseEnter(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackgroundImage = B_Enter;
        }

        private void btnAdd_MouseLeave(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackgroundImage = B_Leave;
        }
        private void InitItemTable()
        {
            dtItemDetails.Columns.Add("ProductID");
            dtItemDetails.Columns.Add("ProductName");
            dtItemDetails.Columns.Add("BarcodeNo");
            dtItemDetails.Columns.Add("ColorID");
            dtItemDetails.Columns.Add("Color");
            dtItemDetails.Columns.Add("SizeID");
            dtItemDetails.Columns.Add("Size");
            dtItemDetails.Columns.Add("QTY");
            dtItemDetails.Columns.Add("Rate");

            dtItemDetails.Columns.Add("OIRate");
            dtItemDetails.Columns.Add("Adj_Amount");

            dtItemDetails.Columns.Add("Total");
            dtItemDetails.Columns.Add("Delete");


            dtReplcaeItemDetails.Columns.Add("ProductID");
            dtReplcaeItemDetails.Columns.Add("ProductName");
            dtReplcaeItemDetails.Columns.Add("BarcodeNo");
            dtReplcaeItemDetails.Columns.Add("ColorID");
            dtReplcaeItemDetails.Columns.Add("Color");
            dtReplcaeItemDetails.Columns.Add("SizeID");
            dtReplcaeItemDetails.Columns.Add("Size");
            dtReplcaeItemDetails.Columns.Add("QTY");
            dtReplcaeItemDetails.Columns.Add("Rate");
            dtReplcaeItemDetails.Columns.Add("Total");
            dtReplcaeItemDetails.Columns.Add("Delete");

        }
        private void AddRowToItemDetails(string productID, string name, string qty, string rate, string total,
            string BarCode, string SizeID, string Size, string ColorID, string Color)
        {
            DataRow dRow = dtItemDetails.NewRow();
            dRow["ProductID"] = productID;
            dRow["ProductName"] = name;
            dRow["QTY"] = qty;
            dRow["Rate"] = rate;
            dRow["Total"] = total;
            dRow["Delete"] = "Delete";
            dRow["ColorID"] = ColorID;
            dRow["Color"] = Color;
            dRow["SizeID"] = SizeID;
            dRow["Size"] = Size;
            dRow["BarcodeNo"] = BarCode;

            if (radNewItem.Checked && IsReplaceReturnMode)
            {
                SetOldRate(dRow, BarCode, Convert.ToDecimal(rate));
            }
           
            dtItemDetails.Rows.Add(dRow);
            dtItemDetails.AcceptChanges();

            dgvProductDetails.DataSource = dtItemDetails;
        }
        private void SetOldRate(DataRow drow, string _BarCoeNumber, decimal CurrentRate)
        {
            ObjDAL.SetStoreProcedureData("InvoiceID", SqlDbType.Int, OldInvoiceID);
            ObjDAL.SetStoreProcedureData("BarCode", SqlDbType.NVarChar, _BarCoeNumber);

            DataSet dataSet = ObjDAL.ExecuteStoreProcedure_Get("spr_GetReplaceReturnDetails");
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                drow["OIRate"] = dataSet.Tables[0].Rows[0]["Rate"].ToString();

                decimal odlRate = Convert.ToDecimal(dataSet.Tables[0].Rows[0]["Rate"]);
                decimal adAmount = CurrentRate- odlRate;
                drow["Adj_Amount"] = adAmount.ToString();



              
            }
            
        }
            private void AddRowToReplaceItemDetails(string productID, string name, string qty, string rate, string total,
           string BarCode, string SizeID, string Size, string ColorID, string Color)
        {
            DataRow dRow = dtReplcaeItemDetails.NewRow();
            dRow["ProductID"] = productID;
            dRow["ProductName"] = name;
            dRow["QTY"] = qty;
            dRow["Rate"] = rate;
            dRow["Total"] = total;
            dRow["Delete"] = "Delete";
            dRow["ColorID"] = ColorID;
            dRow["Color"] = Color;
            dRow["SizeID"] = SizeID;
            dRow["Size"] = Size;
            dRow["BarcodeNo"] = BarCode;

            dtReplcaeItemDetails.Rows.Add(dRow);
            dtReplcaeItemDetails.AcceptChanges();

            dgvReplaceReturn.DataSource = dtReplcaeItemDetails;
        }

        private bool IsItemExist(string barCode)
        {
            DataRow[] dRow = dtItemDetails.Select("BarcodeNo='" + barCode + "'");
            if (dRow.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool IsReplaceItemExist(string barCode)
        {
            DataRow[] dRow = dtReplcaeItemDetails.Select("BarcodeNo='" + barCode + "'");
            if (dRow.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool IsItemExist_NonBarCode(string PID, string ColorID, string SizeID)
        {
            DataRow[] dRow = dtItemDetails.Select("ProductID='" + PID + "' AND ColorID='" + ColorID + "' AND SizeID='" + SizeID + "'");
            if (dRow.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void UpdateQTYByOne(string barCode, decimal rate)
        {
            DataRow[] dRow = dtItemDetails.Select("BarcodeNo='" + barCode + "'");
            // add one qty
            decimal NewQTY = Convert.ToDecimal(dRow[0]["QTY"]) + 1;
            if (CheckProductQTY(barCode, Convert.ToDecimal(NewQTY)))
            {
                // set to col
                dRow[0]["QTY"] = NewQTY.ToString();
                // cal total
                decimal total = rate * NewQTY;
                // set the total
                dRow[0]["Total"] = total.ToString();
                dtItemDetails.AcceptChanges();
                dgvProductDetails.DataSource = dtItemDetails;
            }
            else
            {
                clsUtility.ShowInfoMessage("No QTY avaiable for the given Product.", clsUtility.strProjectTitle);
            }
        }
        private void UpdateReplaceQTYByOne(string barCode, decimal rate)
        {
            DataRow[] dRow = dtReplcaeItemDetails.Select("BarcodeNo='" + barCode + "'");
            // add one qty
            decimal NewQTY = Convert.ToDecimal(dRow[0]["QTY"]) + 1;
            if (CheckReplaceProductQTY(barCode, Convert.ToDecimal(NewQTY)))
            {
                // set to col
                dRow[0]["QTY"] = NewQTY.ToString();
                // cal total
                decimal total = rate * NewQTY;
                // set the total
                dRow[0]["Total"] = total.ToString();
                dtReplcaeItemDetails.AcceptChanges();
                dgvReplaceReturn.DataSource = dtReplcaeItemDetails;
            }
            else
            {
                clsUtility.ShowInfoMessage("All QTY for this Item has been added from old invoice. No more QTY Left", clsUtility.strProjectTitle);
            }
        }
        private void UpdateQTYByOne_NonBarCode(string pID, string ColorID, string SizeID, decimal rate)
        {
            DataRow[] dRow = dtItemDetails.Select("ProductID='" + pID + "' AND SizeID='" + SizeID + "' AND ColorID='" + ColorID + "'");
            // add one qty
            decimal NewQTY = Convert.ToDecimal(dRow[0]["QTY"]) + 1;
            if (CheckProductQTY_Non_BarCode(pID, SizeID, ColorID, Convert.ToDecimal(NewQTY)))
            {
                // set to col
                dRow[0]["QTY"] = NewQTY.ToString();
                // cal total
                decimal total = rate * NewQTY;
                // set the total
                dRow[0]["Total"] = total.ToString();
                dtItemDetails.AcceptChanges();
                dgvProductDetails.DataSource = dtItemDetails;
            }
            else
            {
                clsUtility.ShowInfoMessage("No QTY avaiable for the given Product.", clsUtility.strProjectTitle);
            }
        }
        private string GenerateInvoiceNumber()
        {
            //SequenceInvoice : this is a sequance object created in SQL ( this is not a table)
            int LastID = ObjDAL.ExecuteScalarInt("SELECT NEXT VALUE FOR " + clsUtility.DBName + ".[dbo].SequenceInvoice");
            string InvoiceNumber = "INV-" + LastID;

            return InvoiceNumber;
        }
        private void BindStoreDetails()
        {
            DataTable dt = null;
            dt = ObjDAL.GetDataCol(clsUtility.DBName + ".dbo.StoreMaster", "StoreID,StoreName", "ISNULL(ActiveStatus,1)=1", " StoreID");
            cmbShop.DataSource = dt;
            cmbShop.DisplayMember = "StoreName";
            cmbShop.ValueMember = "StoreID";
            cmbShop.SelectedIndex = -1;
            // set Default store
            int deafultStoreID = ObjDAL.ExecuteScalarInt("SELECT Storeid FROM " + clsUtility.DBName + ".[dbo].[DefaultStoreSetting] WITH(NOLOCK) WHERE MachineName = '" + Environment.MachineName + "'");
            cmbShop.SelectedValue = deafultStoreID;
            if (deafultStoreID == 0)
            {
                clsUtility.ShowInfoMessage("Please select the default shop for this client from Setting Window.", clsUtility.strProjectTitle);
                Settings.frmOtherSetting otherSetting = new Settings.frmOtherSetting();
                otherSetting.ShowDialog();
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Masters.Store_Master Obj = new Masters.Store_Master();
            Obj.Show();
        }

        private void btnDrug_Click(object sender, EventArgs e)
        {
            Masters.Employee_Details Obj = new Masters.Employee_Details();
            Obj.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Masters.Customer_Master customer_Master = new Masters.Customer_Master();
            customer_Master.ShowDialog();
        }
        private void txtSalesMan_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = ObjDAL.ExecuteSelectStatement("SELECT Empid,Name FROM " + clsUtility.DBName + ".dbo.employeeDetails WHERE [Name] Like '" + txtSalesMan.Text + "%'");
                if (ObjUtil.ValidateTable(dt))
                {
                    ObjUtil.SetControlData(txtSalesMan, "Name");
                    ObjUtil.SetControlData(txtEmpID, "Empid");
                    ObjUtil.ShowDataPopup(dt, txtSalesMan, this, this);
                    if (ObjUtil.GetDataPopup() != null && ObjUtil.GetDataPopup().DataSource != null)
                    {
                        // if there is only one column                
                        ObjUtil.GetDataPopup().AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        if (ObjUtil.GetDataPopup().ColumnCount > 0)
                        {
                            ObjUtil.GetDataPopup().Columns["Empid"].Visible = false;
                            ObjUtil.SetDataPopupSize(200, 0);
                        }
                    }
                    //ObjUtil.GetDataPopup().CellClick += frmSalecounter_CellClick;
                    //ObjUtil.GetDataPopup().KeyDown += frmSalecounter_KeyDown;
                }
                else
                {
                    ObjUtil.CloseAutoExtender();
                }
            }
            catch (Exception)
            {
            }
        }
        private Image GetProductPhoto(int ProductID)
        {
            Image imgProduct = null;
            DataTable dt = ObjDAL.ExecuteSelectStatement("SELECT Photo FROM " + clsUtility.DBName + ".dbo.ProductMaster WHERE ProductID=" + ProductID);
            if (ObjUtil.ValidateTable(dt))
            {
                if (dt.Rows[0]["Photo"] != DBNull.Value)
                {
                    DataTable dtImagePath = ObjDAL.ExecuteSelectStatement("  select ImagePath, Extension from [IMS_Client_2].dbo.DefaultStoreSetting where MachineName='" + Environment.MachineName + "'");
                    if (dtImagePath.Rows.Count > 0)
                    {
                        if (dtImagePath.Rows[0]["ImagePath"] != DBNull.Value)
                        {
                            string ImgPath = dtImagePath.Rows[0]["ImagePath"].ToString();
                            string extension = dtImagePath.Rows[0]["Extension"].ToString();

                            string imgFile = ImgPath + "//" + dt.Rows[0]["Photo"].ToString() + extension;
                            if (File.Exists(imgFile))
                            {
                                imgProduct = Image.FromFile(imgFile);
                            }
                            else
                            {
                                imgProduct = null;
                            }


                        }
                        else
                        {
                            MessageBox.Show("Image file for the selected product doesn't exist.", clsUtility.strProjectTitle);

                        }
                    }
                }
            }
            return imgProduct;
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            if (cboEntryMode.SelectedIndex == 1) // if manual entry
            {
                if (txtBarCode.Text.Trim().Length == 0)
                {
                    return;
                }

                try
                {
                    //string strQ = "EXEC " + clsUtility.DBName + ".dbo.GetProductDetailsByProductName " + cmbShop.SelectedValue.ToString() + ", '" + txtBarCode.Text + "'";
                    //DataTable dt = ObjDAL.ExecuteSelectStatement(strQ);
                    //if (ObjUtil.ValidateTable(dt))
                    //{
                    //    ObjUtil.SetControlData(txtBarCode, "ProductName");
                    //    ObjUtil.SetControlData(txtProductID, "ProductID");

                    //    ObjUtil.SetControlData(txtColorID, "ColorID");
                    //    ObjUtil.SetControlData(txtSizeID, "SizeID");

                    //    ObjUtil.ShowDataPopup(dt, txtBarCode, this, this);

                    //    if (ObjUtil.GetDataPopup() != null && ObjUtil.GetDataPopup().DataSource != null)
                    //    {
                    //        // if there is only one column                
                    //        ObjUtil.GetDataPopup().AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    //        if (ObjUtil.GetDataPopup().ColumnCount > 0)
                    //        {
                    //            ObjUtil.GetDataPopup().Columns["ProductID"].Visible = false;

                    //            ObjUtil.GetDataPopup().Columns["ColorID"].Visible = false;
                    //            ObjUtil.GetDataPopup().Columns["QTY"].Visible = false;
                    //            ObjUtil.GetDataPopup().Columns["SizeID"].Visible = false;
                    //            ObjUtil.GetDataPopup().Columns["ColorName"].HeaderText = "Color";

                    //            ObjUtil.SetDataPopupSize(450, 0);
                    //        }
                    //    }
                    //    ObjUtil.GetDataPopup().CellClick += Sales_Invoice_CellClick;
                    //    ObjUtil.GetDataPopup().KeyDown += Sales_Invoice_KeyDown;
                    //}
                    //else
                    //{
                    //    ObjUtil.CloseAutoExtender();
                    //}
                }
                catch (Exception)
                {
                }
            }
            else
            {
                if (txtBarCode.Text.Trim().Length != 0 && !ObjUtil.IsNumeric(txtBarCode.Text))
                {
                    clsUtility.ShowInfoMessage("Invalid BarCode Entry. Please check the Product Code.", clsUtility.strProjectTitle);
                }
            }
        }

        private void Sales_Invoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (cboEntryMode.SelectedIndex == 1)
                {
                    DataTable dtSelectedProductDetails = ObjDAL.ExecuteSelectStatement("EXEC " + clsUtility.DBName + ".dbo.GetProductDetails_By_Color_Size " + cmbShop.SelectedValue.ToString() + "," + txtSizeID.Text + "," + txtColorID.Text);

                    string PID = dtSelectedProductDetails.Rows[0]["ProductID"].ToString();
                    string ColorID = dtSelectedProductDetails.Rows[0]["ColorID"].ToString();
                    string SizeID = dtSelectedProductDetails.Rows[0]["SizeID"].ToString();
                    string pName = dtSelectedProductDetails.Rows[0]["ProductName"].ToString();
                    string Rate = dtSelectedProductDetails.Rows[0]["Rate"].ToString();

                    string COlorName = dtSelectedProductDetails.Rows[0]["ColorName"].ToString();
                    string Size = dtSelectedProductDetails.Rows[0]["Size"].ToString();
                    string _barCode = dtSelectedProductDetails.Rows[0]["BarcodeNo"].ToString();

                    GetItemDetailsBy_Non_BarCode(PID, pName, Rate, _barCode, SizeID, Size, ColorID, COlorName);
                }
                else
                {
                }
            }
        }

        private void Sales_Invoice_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataTable dtSelectedProductDetails = ObjDAL.ExecuteSelectStatement("EXEC " + clsUtility.DBName + ".dbo.GetProductDetails_By_Color_Size " + cmbShop.SelectedValue.ToString() + "," + txtSizeID.Text + "," + txtColorID.Text);

            string PID = dtSelectedProductDetails.Rows[0]["ProductID"].ToString();
            string ColorID = dtSelectedProductDetails.Rows[0]["ColorID"].ToString();
            string SizeID = dtSelectedProductDetails.Rows[0]["SizeID"].ToString();
            string pName = dtSelectedProductDetails.Rows[0]["ProductName"].ToString();
            string Rate = dtSelectedProductDetails.Rows[0]["Rate"].ToString();

            string COlorName = dtSelectedProductDetails.Rows[0]["ColorName"].ToString();
            string Size = dtSelectedProductDetails.Rows[0]["Size"].ToString();
            string _barCode = dtSelectedProductDetails.Rows[0]["BarcodeNo"].ToString();

            GetItemDetailsBy_Non_BarCode(PID, pName, Rate, _barCode, SizeID, Size, ColorID, COlorName);
        }
        private bool CheckProductQTY(string _BarCoeNumber, decimal CurQTY)
        {
            string strSQL = "SELECT QTY FROM  " + clsUtility.DBName + ".[dbo].[ProductStockColorSizeMaster] WITH(NOLOCK) WHERE BarcodeNo=" + _BarCoeNumber + " AND StoreID=" + cmbShop.SelectedValue.ToString();

            decimal TotalQTY = Convert.ToDecimal(ObjDAL.ExecuteScalar(strSQL));

            if (CurQTY > TotalQTY)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool CheckReplaceProductQTY(string _BarCoeNumber, decimal CurQTY)
        {
            int TotalQTY = 0;
            ObjDAL.SetStoreProcedureData("InvoiceID", SqlDbType.Int, OldInvoiceID);
            ObjDAL.SetStoreProcedureData("BarCode", SqlDbType.NVarChar, _BarCoeNumber);

            DataSet dataSet = ObjDAL.ExecuteStoreProcedure_Get("spr_GetReplaceReturnDetails");
            if (dataSet.Tables.Count > 0)
            {
                TotalQTY = Convert.ToInt32(dataSet.Tables[0].Rows[0]["QTY"]);
                if (CurQTY > TotalQTY)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }


            return false;



        }

        private bool CheckProductQTY_Non_BarCode(string PID, string SizeID, string ColorID, decimal CurQTY)
        {
            string strSQL = "SELECT QTY FROM  " + clsUtility.DBName + ".[dbo].[ProductStockColorSizeMaster] WITH(NOLOCK) WHERE ProductID=" + PID + " AND  SizeID=" + SizeID + " AND ColorID=" + ColorID + " AND StoreID=" + cmbShop.SelectedValue.ToString();

            decimal TotalQTY = Convert.ToDecimal(ObjDAL.ExecuteScalar(strSQL));
            if (CurQTY > TotalQTY)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void GetItemDetailsByProductID(string _BarCodeValue)
        {
            DataTable dt = ObjDAL.ExecuteSelectStatement("EXEC " + clsUtility.DBName + ".dbo.GetProductDetailsByBarCode " + cmbShop.SelectedValue + ", " + _BarCodeValue);
            if (ObjUtil.ValidateTable(dt))
            {
                string pID = dt.Rows[0]["ProductID"].ToString();
                string name = dt.Rows[0]["ProductName"].ToString();
                string rate = dt.Rows[0]["Rate"].ToString();
                string barCode = dt.Rows[0]["BarcodeNo"].ToString();
                string qty = "1";
                string SizeID = dt.Rows[0]["SizeID"].ToString();
                string Size = dt.Rows[0]["Size"].ToString();
                string ColorID = dt.Rows[0]["ColorID"].ToString();
                string ColorName = dt.Rows[0]["ColorName"].ToString();
                decimal total = Convert.ToDecimal(rate) * Convert.ToDecimal(qty);

                if (CheckProductQTY(barCode, Convert.ToDecimal(qty)))
                {
                    // if Item already there in the grid, then just increase the QTY
                    if (IsItemExist(barCode))
                    {
                        UpdateQTYByOne(barCode.ToString(), Convert.ToDecimal(rate));
                        picProduct.Image = GetProductPhoto(Convert.ToInt32(pID));
                    }
                    else
                    {
                        AddRowToItemDetails(pID, name, qty, rate, total.ToString(), barCode, SizeID, Size, ColorID, ColorName);
                        picProduct.Image = GetProductPhoto(Convert.ToInt32(pID));

                    }
                    txtProductID.Clear();
                    txtBarCode.Clear();

                    CalculateGrandTotal();
                    dgvProductDetails.ClearSelection();
                    txtBarCode.Focus();
                }
                else
                {
                    clsUtility.ShowInfoMessage("No QTY avaiable for the Product : " + txtBarCode.Text, clsUtility.strProjectTitle);
                }
            }
            else
            {
                clsUtility.ShowInfoMessage("No Product Found for the barcode value : " + _BarCodeValue, clsUtility.strProjectTitle);
            }
        }

        private void GetItemDetailsBy_Non_BarCode(string pID, string name, string rate, string barCode, string SizeID, string Size, string ColorID, string ColorName)
        {
            string qty = "1";
            decimal total = Convert.ToDecimal(rate) * Convert.ToDecimal(qty);

            if (CheckProductQTY_Non_BarCode(pID, SizeID, ColorID, Convert.ToDecimal(qty)))
            {
                // if Item already there in the grid, then just increase the QTY
                if (IsItemExist_NonBarCode(pID, ColorID, SizeID))
                {
                    UpdateQTYByOne_NonBarCode(pID, ColorID, SizeID, Convert.ToDecimal(rate));
                    picProduct.Image = GetProductPhoto(Convert.ToInt32(pID));
                }
                else
                {
                    AddRowToItemDetails(pID, name, qty, rate, total.ToString(), barCode, SizeID, Size, ColorID, ColorName);
                    picProduct.Image = GetProductPhoto(Convert.ToInt32(pID));
                }
                txtProductID.Clear();
                txtBarCode.Clear();
                txtColorID.Clear();
                txtSizeID.Clear();
                CalculateGrandTotal();
                dgvProductDetails.ClearSelection();
                txtBarCode.Focus();
            }
            else
            {
                clsUtility.ShowInfoMessage("No QTY avaiable for the Product : " + txtBarCode.Text, clsUtility.strProjectTitle);
            }
        }
        private void dgvProductDetails_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ObjUtil.SetRowNumber(dgvProductDetails);
            
            dgvProductDetails.Columns["ProductID"].Visible = false;

            dgvProductDetails.Columns["ColoriD"].Visible = false;
            dgvProductDetails.Columns["SizeiD"].Visible = false;
            if (IsReplaceReturnMode)
            {
                dgvProductDetails.Columns["OIRate"].Visible = true;
                dgvProductDetails.Columns["Adj_Amount"].Visible = false;
                ObjUtil.SetDataGridProperty(dgvProductDetails, DataGridViewAutoSizeColumnsMode.Fill);

            }
            else
            {
                dgvProductDetails.Columns["OIRate"].Visible = false;
                dgvProductDetails.Columns["Adj_Amount"].Visible = false;
                ObjUtil.SetDataGridProperty(dgvProductDetails, DataGridViewAutoSizeColumnsMode.Fill);
            }
          
        }

        private void dgvProductDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //dgvProductDetails.Columns[e.ColumnIndex].Name == "Rate" ||
            if (dgvProductDetails.Columns[e.ColumnIndex].Name == "QTY")
            {
                string pID = dgvProductDetails.Rows[e.RowIndex].Cells["ProductID"].Value.ToString();
                string sizeid = dgvProductDetails.Rows[e.RowIndex].Cells["SizeID"].Value.ToString();
                string colorid = dgvProductDetails.Rows[e.RowIndex].Cells["ColorID"].Value.ToString();
                decimal QTY = Convert.ToDecimal(dgvProductDetails.Rows[e.RowIndex].Cells["QTY"].Value);
                if (QTY < 0)
                {
                    clsUtility.ShowInfoMessage("The QTY can not be negative. Default 1 QTY will be set.", clsUtility.strProjectTitle);
                    dgvProductDetails.Rows[e.RowIndex].Cells["QTY"].Value = "1";
                    QTY = 1;
                }
                decimal Rate = Convert.ToDecimal(dgvProductDetails.Rows[e.RowIndex].Cells["Rate"].Value);
                decimal Total = QTY * Rate;
                string _barNo = dgvProductDetails.Rows[e.RowIndex].Cells["Barcodeno"].Value.ToString();

                if (_barNo.Trim().Length == 0) // if barcode not found
                {
                    if (CheckProductQTY_Non_BarCode(pID, sizeid, colorid, QTY))
                    {
                        dgvProductDetails.Rows[e.RowIndex].Cells["Total"].Value = Total.ToString();
                        CalculateGrandTotal();
                    }
                    else
                    {
                        dgvProductDetails.Rows[e.RowIndex].Cells["QTY"].Value = _StartValue;
                        CalculateGrandTotal();

                        clsUtility.ShowInfoMessage("QTY : " + QTY + " NOT avaiable for the Product : " + dgvProductDetails.Rows[e.RowIndex].Cells["ProductName"].Value, clsUtility.strProjectTitle);
                    }
                }
                else
                {
                    if (CheckProductQTY(_barNo, Convert.ToDecimal(QTY)))
                    {
                        dgvProductDetails.Rows[e.RowIndex].Cells["Total"].Value = Total.ToString();
                        CalculateGrandTotal();
                    }
                    else
                    {
                        dgvProductDetails.Rows[e.RowIndex].Cells["QTY"].Value = _StartValue;
                        CalculateGrandTotal();

                        clsUtility.ShowInfoMessage("QTY : " + QTY + " NOT avaiable for the Product : " + dgvProductDetails.Rows[e.RowIndex].Cells["ProductName"].Value, clsUtility.strProjectTitle);
                    }
                }
            }
        }
        private void CalculateGrandTotal()
        {
            decimal NewBillAmount = 0.0M;
            decimal OldBillAmount = 0.0M;
            decimal Discount = 0.0M;
            decimal Deliverycharges = 0.0M;
            decimal GrandTotal = 0.0M;

            for (int i = 0; i < dgvProductDetails.Rows.Count; i++)
            {
                NewBillAmount += Convert.ToDecimal(dgvProductDetails.Rows[i].Cells["Total"].Value);
            }

            for (int i = 0; i < dgvReplaceReturn.Rows.Count; i++)
            {
                OldBillAmount += Convert.ToDecimal(dgvReplaceReturn.Rows[i].Cells["ColReplaceTotal"].Value);
            }
            txtOldBillAmount.Text = Math.Round(OldBillAmount, 2).ToString();
            txtNewBillAmount.Text= Math.Round(NewBillAmount, 2).ToString();

            decimal MainSubTotal = NewBillAmount - OldBillAmount;

            txtSubTotal.Text = Math.Round(MainSubTotal, 2).ToString();

            Discount = txtDiscount.Text.Length > 0 ? Convert.ToDecimal(txtDiscount.Text) : 0;
            Deliverycharges = txtDeliveryCharges.Text.Length > 0 ? Convert.ToDecimal(txtDeliveryCharges.Text) : 0;

            GrandTotal = MainSubTotal - (MainSubTotal * Discount * 0.01M) + Deliverycharges;
            txtGrandTotal.Text = Math.Round(GrandTotal, 2).ToString();
        }
       

        private void dgvProductDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex==-1)
            {
                return;
            }
            if (dgvProductDetails.Columns[e.ColumnIndex].Name == "ColDelete")
            {
                DialogResult d = MessageBox.Show("Are you sure want to delete ? ", clsUtility.strProjectTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (d == DialogResult.Yes)
                {
                    dgvProductDetails.Rows.RemoveAt(e.RowIndex);
                    dgvProductDetails.EndEdit();
                    CalculateGrandTotal();
                }
            }
            else
            {
                int _PID = Convert.ToInt32(dgvProductDetails.Rows[e.RowIndex].Cells["ProductID"].Value);
                picProduct.Image = GetProductPhoto(Convert.ToInt32(_PID));
            }
        }

        private void ClearAll()
        {
            txtOldBillAmount.Clear();
            txtNewBillAmount.Clear();
            Other_Forms.frmPayment.strPaymentAutoID = "";
            lblPMode.Text = "";
            txtCustomerID.Clear();
            txtCustomerMobile.Clear();
            dtItemDetails.Clear();
            dtReplcaeItemDetails.Clear();
            txtProductID.Clear();
            txtEmpID.Clear();

            txtInvoiceNumber.Clear();
            txtBarCode.Clear();
            txtCustomerMobile.Clear();
            cmbShop.SelectedIndex = -1;
            txtTotalItems.Text ="Total Items : 0";

            txtSubTotal.Text = "0";
            txtDeliveryCharges.Text = "0";
            txtDiscount.Text = "0";
            txtGrandTotal.Text = "0";
            picProduct.Image = null;
            BindStoreDetails();

            txtSalesMan.Clear();
            txtColorID.Clear();
            txtSizeID.Clear();
        }
        private bool SalesValidation()
        {
            if (txtCustomerID.Text.Trim().Length == 0)
            {
                clsUtility.ShowInfoMessage("Please Enter Customer Mobile No.", clsUtility.strProjectTitle);
                txtCustomerMobile.Focus();
                return false;
            }
            if (txtEmpID.Text.Trim().Length == 0)
            {
                clsUtility.ShowInfoMessage("Please Enter Sales Man Name.", clsUtility.strProjectTitle);
                txtSalesMan.Focus();
                return false;
            }
            if (Other_Forms.frmPayment.strPaymentAutoID.Trim().Length == 0)
            {
                clsUtility.ShowInfoMessage("Please Select Payment Mode.", clsUtility.strProjectTitle);
                return false;
            }
            return true;
        }
      
        private int DoNewSales()
        {
            dgvProductDetails.EndEdit();

            // Before sales invocing make sure you have available qty for particular store

            string InvoiceDateTime = dtpSalesDate.Value.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss");

            
            #region SalesInvoiceDetails
            ObjDAL.SetColumnData("InvoiceNumber", SqlDbType.NVarChar, GenerateInvoiceNumber());
            ObjDAL.SetColumnData("InvoiceDate", SqlDbType.DateTime, InvoiceDateTime);
            ObjDAL.SetColumnData("SubTotal", SqlDbType.Decimal, txtSubTotal.Text);
            ObjDAL.SetColumnData("Discount", SqlDbType.Decimal, txtDiscount.Text);
            ObjDAL.SetColumnData("Tax", SqlDbType.Decimal, txtDeliveryCharges.Text);
            ObjDAL.SetColumnData("GrandTotal", SqlDbType.Decimal, txtGrandTotal.Text);
            ObjDAL.SetColumnData("CreatedBy", SqlDbType.Int, clsUtility.LoginID);
            ObjDAL.SetColumnData("CustomerID", SqlDbType.Int, txtCustomerID.Text);
            ObjDAL.SetColumnData("SalesMan", SqlDbType.Int, txtEmpID.Text);
            ObjDAL.SetColumnData("ShopeID", SqlDbType.Int, cmbShop.SelectedValue.ToString());

            ObjDAL.SetColumnData("PaymentMode", SqlDbType.NVarChar, lblPMode.Text);
            ObjDAL.SetColumnData("PaymentAutoID", SqlDbType.NVarChar, Other_Forms.frmPayment.strPaymentAutoID);

            int InvoiceID = ObjDAL.InsertData(clsUtility.DBName + ".dbo.SalesInvoiceDetails", true);
            if (InvoiceID == -1)
            {
                ObjDAL.ResetData();
                return InvoiceID;
            }
            #endregion

            for (int i = 0; i < dgvProductDetails.Rows.Count; i++)
            {
                string Total = dgvProductDetails.Rows[i].Cells["Total"].Value.ToString();
                string ProductID = dgvProductDetails.Rows[i].Cells["ProductID"].Value.ToString();
                string QTY = dgvProductDetails.Rows[i].Cells["QTY"].Value.ToString();
                string Rate = dgvProductDetails.Rows[i].Cells["Rate"].Value.ToString();
                string ColorID = dgvProductDetails.Rows[i].Cells["ColorID"].Value.ToString();
                string SizeID = dgvProductDetails.Rows[i].Cells["SizeID"].Value.ToString();

                ObjDAL.SetColumnData("InvoiceID", SqlDbType.Int, InvoiceID);
                ObjDAL.SetColumnData("ProductID", SqlDbType.Int, ProductID);
                ObjDAL.SetColumnData("QTY", SqlDbType.Decimal, QTY);
                ObjDAL.SetColumnData("CreatedBy", SqlDbType.Int, clsUtility.LoginID);
                ObjDAL.SetColumnData("Rate", SqlDbType.Decimal, Rate);
                ObjDAL.SetColumnData("ColorID", SqlDbType.Int, ColorID);
                ObjDAL.SetColumnData("SizeID", SqlDbType.Int, SizeID);

                ObjDAL.InsertData(clsUtility.DBName + ".dbo.SalesDetails", false);

                ObjDAL.ExecuteNonQuery("UPDATE " + clsUtility.DBName + ".dbo.ProductStockColorSizeMaster " +
                                        "SET QTY=QTY-" + QTY + " WHERE ProductID=" + ProductID + " AND StoreID=" + cmbShop.SelectedValue.ToString() + " AND ColorID=" + ColorID + " AND SizeID=" + SizeID);
            }
            // save the payment details
            if (lblPMode.Text== "K Net" || lblPMode.Text == "Visa" || lblPMode.Text== "Master Card" || lblPMode.Text== "Other")
            {
                // check if any entry is there
                string strQ = " select MasterCashClosingID from " + clsUtility.DBName + ".[dbo].[tblMasterCashClosing] where Convert(Date,cashboxDateTime)=Convert(date,getdate())";
                int MasterCashClosingID = ObjDAL.ExecuteScalarInt(strQ);


                int count = ObjDAL.ExecuteScalarInt("select count(*) from "+ clsUtility.DBName + ".[dbo].[tblCreditClosing] where MasterCashClosingID=" + MasterCashClosingID);
                if (count == 0)  // if NOT found for today
                {
                    ObjDAL.SetColumnData("MasterCashClosingID", SqlDbType.Int, MasterCashClosingID);

                    ObjDAL.SetColumnData("Type", SqlDbType.NVarChar, lblPMode.Text);
                   
                    ObjDAL.SetColumnData("Count", SqlDbType.Int, 1);
                    ObjDAL.SetColumnData("Value", SqlDbType.Decimal, txtGrandTotal.Text);
                    ObjDAL.SetColumnData("CreatedOn", SqlDbType.DateTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    ObjDAL.SetColumnData("CreatedBy", SqlDbType.Int, clsUtility.LoginID);
                    ObjDAL.InsertData(clsUtility.DBName + ".[dbo].[tblCreditClosing]", false);
                }
                else
                {
                    // update the tblCreditClosing, Add count and amount to the exsting value
                    string strUpdate = "  update "+ clsUtility.DBName + ".[dbo].[tblCreditClosing] set Count=Count+1 , Value=value+" + txtGrandTotal.Text + 
                                    " where MasterCashClosingID=" + MasterCashClosingID;

                    ObjDAL.ExecuteNonQuery(strUpdate);
                }

            }

            return InvoiceID;
        }
        private void DoReplaceReturn()
        {
           
            // Take the return and add to stock
            for (int i = 0; i < dgvReplaceReturn.Rows.Count; i++)
            {
                string Total = dgvReplaceReturn.Rows[i].Cells["ColReplaceTotal"].Value.ToString();
                string ProductID = dgvReplaceReturn.Rows[i].Cells["ReplaceProductID"].Value.ToString();
                string QTY = dgvReplaceReturn.Rows[i].Cells["ReplaceQTY"].Value.ToString();
                string Rate = dgvReplaceReturn.Rows[i].Cells["ReplaceRate"].Value.ToString();
                string ColorID = dgvReplaceReturn.Rows[i].Cells["RepalceColorID"].Value.ToString();
                string SizeID = dgvReplaceReturn.Rows[i].Cells["RepalceSizeID"].Value.ToString();
                string barcodeNumber= dgvReplaceReturn.Rows[i].Cells["ReplaceBarcode"].Value.ToString();


                ObjDAL.SetColumnData("OldInvoiceID", SqlDbType.Int, OldInvoiceID);
                ObjDAL.SetColumnData("ProductID", SqlDbType.Int, ProductID);
                ObjDAL.SetColumnData("QTY", SqlDbType.Decimal, QTY);
                
                ObjDAL.SetColumnData("Rate", SqlDbType.Decimal, Rate);
                ObjDAL.SetColumnData("ColorID", SqlDbType.Int, ColorID);
                ObjDAL.SetColumnData("SizeID", SqlDbType.Int, SizeID);
                ObjDAL.SetColumnData("CreatedOn", SqlDbType.DateTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                ObjDAL.SetColumnData("CreatedBy", SqlDbType.Int, clsUtility.LoginID);
                ObjDAL.SetColumnData("BarCode", SqlDbType.NVarChar, barcodeNumber);

                ObjDAL.InsertData(clsUtility.DBName + ".dbo.tblReplaceReturn", false);

                // add the QTY as we are getting back the stock
                ObjDAL.ExecuteNonQuery("UPDATE " + clsUtility.DBName + ".dbo.ProductStockColorSizeMaster " +
                                        "SET QTY=QTY+" + QTY + " WHERE ProductID=" + ProductID + " AND StoreID=" + cmbShop.SelectedValue.ToString() + " AND ColorID=" + ColorID + " AND SizeID=" + SizeID);
            }

            //Cash Return Entry
            string strQ = " select MasterCashClosingID from "+ clsUtility.DBName + ".[dbo].[tblMasterCashClosing] where Convert(Date,cashboxDateTime)=Convert(date,getdate())";
            int MasterCashClosingID = ObjDAL.ExecuteScalarInt(strQ);

           
             int count=  ObjDAL.ExecuteScalarInt("select count(*) from [IMS_Client_2].[dbo].[tblCashReturn] where MasterCashClosingID="+ MasterCashClosingID);
            if (count==0)  // if NOT found for today
            {
                ObjDAL.SetColumnData("MasterCashClosingID", SqlDbType.Int, MasterCashClosingID);
                ObjDAL.SetColumnData("Value", SqlDbType.Decimal, txtOldBillAmount.Text);
                ObjDAL.SetColumnData("CreatedOn", SqlDbType.Date, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                ObjDAL.SetColumnData("CreatedBy", SqlDbType.Int, clsUtility.LoginID);
                ObjDAL.InsertData(clsUtility.DBName+".[dbo].[tblCashReturn]", false);
            }
            else
            {
                // update the tblCashReturn, Add value the exsting value

                string strUpdate = "  update " + clsUtility.DBName + ".[dbo].[tblCashReturn] set Value=value+" + txtOldBillAmount.Text +
                                      "where MasterCashClosingID=" + MasterCashClosingID;

                ObjDAL.ExecuteNonQuery(strUpdate);
            }


        }
       
        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            if (clsFormRights.HasFormRight(clsFormRights.Forms.Sales_Invoice, clsFormRights.Operation.Save) || clsUtility.IsAdmin)
            {
                if (SalesValidation())
                {
                    int NewInvoiceID = 0;

                    // If its New sales invoice
                    if (!IsReplaceReturnMode)
                    {
                        NewInvoiceID= DoNewSales();
                    }
                    else if (IsReplaceReturnMode) // For reaplce and retrun mode
                    {
                        DoReplaceReturn();
                        // if new item purchased ;
                        if (dgvProductDetails.Rows.Count>0)
                        {
                            NewInvoiceID= DoNewSales();

                            // update the crossponding old invoice ID
                            ObjDAL.ExecuteNonQuery("update [IMS_Client_2].[dbo].[tblReplaceReturn] set NewInvoiceID=" + NewInvoiceID + " where OldInvoiceID=" + OldInvoiceID);
                        }

                    }
                  
                    clsUtility.ShowInfoMessage("Data has been saved successfully.", clsUtility.strProjectTitle);
                    ClearAll();

                    Button button = (Button)sender;

                    if (button.Name == "btnSaveData")
                    {
                        this.Close();
                    }
                    else if (button.Name == "btnPrint")
                    {
                        Report.frmSalesInvoiceReport frmSalesInvoice = new Report.frmSalesInvoiceReport();
                        frmSalesInvoice.InvoiceID = NewInvoiceID;
                        frmSalesInvoice.IsDirectPrint = true;
                        frmSalesInvoice.Show();
                    }
                    else
                    {
                        Report.frmSalesInvoiceReport frmSalesInvoice = new Report.frmSalesInvoiceReport();
                        frmSalesInvoice.InvoiceID = NewInvoiceID;
                        frmSalesInvoice.IsDirectPrint = false;
                        frmSalesInvoice.Show();
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool result = clsUtility.ShowQuestionMessage("Are you sure, you want to cancel?", clsUtility.strProjectTitle);
            if (result)
            {
                ClearAll();
            }
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            CalculateGrandTotal();
        }

        decimal _StartValue = 0;
        private void dgvProductDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                _StartValue = Convert.ToDecimal(dgvProductDetails.Rows[e.RowIndex].Cells["QTY"].Value);
            }
            catch (Exception)
            {
            }
        }

        private void txtProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (cboEntryMode.SelectedIndex == 0)
            {
                if (e.KeyData == Keys.Enter)
                {
                 
                    if (IsReplaceReturnMode && radReplace.Checked)
                    {
                        // get the data froms sales.
                        GetDataFromSales(txtBarCode.Text);
                    }
                    else
                    {   //txtProductName contains barcode.
                        GetItemDetailsByProductID(txtBarCode.Text);
                    }
                    
                }
            }
            else
            {
                if (ObjUtil.GetDataPopup() != null)
                {
                    ObjUtil.GetDataPopup().Focus();
                }
            }
        }

        private void GetDataFromSales(string barcode)
        {

            string strQ = "select * from (select InvoiceID, ProductID, " +
                        " (select ProductName from ProductMaster where ProductID = s1.ProductID) as ProductName, " +
                        " QTY, Rate, ColorID, " +
                        " (select ColorName from ColorMaster where ColorID = s1.ColorID) as Color, " +
                        " (select Size from SizeMaster where SizeID = s1.SizeID) as Size,SizeID, " +
                        " (select BarcodeNo from ProductStockColorSizeMaster where ColorID = s1.ColorID and SizeID = s1.SizeID and ProductID = s1.ProductID ) as BarCode from SalesDetails s1" +
                        " where InvoiceID = "+OldInvoiceID+" ) as tb where BarCode = '"+barcode+"'";


           DataTable dtSalesDetails=  ObjDAL.ExecuteSelectStatement(strQ);
            if (dtSalesDetails.Rows.Count>0)
            {
                try
                {
                    string pID = dtSalesDetails.Rows[0]["ProductID"].ToString();
                    string name = dtSalesDetails.Rows[0]["ProductName"].ToString();
                    string rate = dtSalesDetails.Rows[0]["Rate"].ToString();
                    string barCode = dtSalesDetails.Rows[0]["Barcode"].ToString();
                    string qty = "1";
                    string SizeID = dtSalesDetails.Rows[0]["SizeID"].ToString();
                    string Size = dtSalesDetails.Rows[0]["Size"].ToString();
                    string ColorID = dtSalesDetails.Rows[0]["ColorID"].ToString();
                    string ColorName = dtSalesDetails.Rows[0]["Color"].ToString();
                    decimal total = Convert.ToDecimal(rate) * Convert.ToDecimal(qty);
                   
                    // if Item already there in the grid, then just increase the QTY
                    if (IsReplaceItemExist(barCode))
                    {
                        UpdateReplaceQTYByOne(barCode.ToString(), Convert.ToDecimal(rate));
                        picProduct.Image = GetProductPhoto(Convert.ToInt32(pID));
                    }
                    else
                    {
                        AddRowToReplaceItemDetails(pID, name, qty, rate, total.ToString(), barCode, SizeID, Size, ColorID, ColorName);
                        picProduct.Image = GetProductPhoto(Convert.ToInt32(pID));

                    }
                    txtProductID.Clear();
                    txtBarCode.Clear();

                    CalculateGrandTotal();
                    dgvReplaceReturn.ClearSelection();
                    txtBarCode.Focus();

                

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());

                }
              
            }
            else
            {
                clsUtility.ShowInfoMessage("No Data found.", clsUtility.strProjectTitle);
            }
        }

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtCustomerMobile.Text.Trim().Length > 0)
                {
                    string query = "SELECT CustomerID,[Name],PhoneNo FROM " + clsUtility.DBName + ".dbo.CustomerMaster WITH(NOLOCK) WHERE PhoneNo like '%" + txtCustomerMobile.Text + "%'";
                    DataTable dt = ObjDAL.ExecuteSelectStatement(query);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        ObjUtil.SetControlData(txtCustomerName, "Name");
                        ObjUtil.SetControlData(txtCustomerMobile, "PhoneNo");
                        ObjUtil.SetControlData(txtCustomerID, "CustomerID");

                        ObjUtil.ShowDataPopup(dt, txtCustomerMobile, this, this);

                        if (ObjUtil.GetDataPopup() != null && ObjUtil.GetDataPopup().DataSource != null)
                        {
                            // if there is only one column                
                            ObjUtil.GetDataPopup().AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            if (ObjUtil.GetDataPopup().ColumnCount > 0)
                            {
                                ObjUtil.GetDataPopup().Columns["CustomerID"].Visible = false;
                                ObjUtil.SetDataPopupSize(450, 0);
                            }
                        }
                        //ObjUtil.GetDataPopup().CellClick += Sales_Invoice_CellClick;
                        //ObjUtil.GetDataPopup().KeyDown += Sales_Invoice_KeyDown;
                    }
                    else
                    {
                        ObjUtil.CloseAutoExtender();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void picCash_Click(object sender, EventArgs e)
        {
            lblPMode.Text = "Cash";
            //deafult auto ID for cash is four times zero
            Other_Forms.frmPayment.strPaymentAutoID = "0000";
        }
        private void picKnet_Click(object sender, EventArgs e)
        {
            lblPMode.Text = "K Net";
            Other_Forms.frmPayment frmPayment = new Other_Forms.frmPayment();
            Other_Forms.frmPayment.strPaymentAutoID = "";
            frmPayment.lblPaymentMode.Text = "K Net";
            frmPayment.picPaymentMode.Image = picKnet.Image;
            frmPayment.ShowDialog();
        }

        private void picVisa_Click(object sender, EventArgs e)
        {
            lblPMode.Text = "Visa";
            Other_Forms.frmPayment frmPayment = new Other_Forms.frmPayment();
            Other_Forms.frmPayment.strPaymentAutoID = "";
            frmPayment.lblPaymentMode.Text = "Visa";
            frmPayment.picPaymentMode.Image = picVisa.Image;
            frmPayment.ShowDialog();
        }
        private void PicMaster_Click(object sender, EventArgs e)
        {
            lblPMode.Text = "Master Card";
            Other_Forms.frmPayment frmPayment = new Other_Forms.frmPayment();
            Other_Forms.frmPayment.strPaymentAutoID = "";
            frmPayment.lblPaymentMode.Text = "Master Card";
            frmPayment.picPaymentMode.Image = PicMaster.Image;
            frmPayment.ShowDialog();
        }

        private void picOther_Click(object sender, EventArgs e)
        {
            lblPMode.Text = "Other";
            Other_Forms.frmPayment frmPayment = new Other_Forms.frmPayment();
            Other_Forms.frmPayment.strPaymentAutoID = "";
            frmPayment.lblPaymentMode.Text = "Other";
            frmPayment.picPaymentMode.Image = picOther.Image;
            frmPayment.ShowDialog();
        }
        private void lblActiveStatus_Click(object sender, EventArgs e)
        {
            txtInvoiceNumber.ReadOnly = false;
        }

        private void Sales_Invoice_Activated(object sender, EventArgs e)
        {
            txtBarCode.Focus();
        }

        private void cboEntryMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cboEntryMode.SelectedIndex == 1)
            {
                label4.Text = "Product Name :";
            }
            else
            {
                label4.Text = "Barcode :";
            }
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            e.Handled = ObjUtil.IsDecimal(txt, e);
            if (e.Handled == true)
            {
                clsUtility.ShowInfoMessage("Enter Only Numbers...", clsUtility.strProjectTitle);
            }
        }

        private void radNewItem_CheckedChanged(object sender, EventArgs e)
        {
            if (isFromTabChanged)
            {
                isFromTabChanged = false;
                return;
            }
            tabControl1.SelectedIndex = 0;
            lblTitle.Text = "New Item Details";
            txtBarCode.Focus();
        }

        private void radReplace_CheckedChanged(object sender, EventArgs e)
        {
            if (isFromTabChanged)
            {
                isFromTabChanged = false;
                return;
            }

            tabControl1.SelectedIndex = 1;
            lblTitle.Text = "Replace/Return";
            txtBarCode.Focus();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.BringToFront();
            timer1.Stop();
            timer1.Enabled = false;
        }

        private void dgvReplaceReturn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            if (dgvReplaceReturn.Columns[e.ColumnIndex].Name == "ColDelete")
            {
                DialogResult d = MessageBox.Show("Are you sure want to delete ? ", clsUtility.strProjectTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (d == DialogResult.Yes)
                {
                    dgvReplaceReturn.Rows.RemoveAt(e.RowIndex);
                    dgvReplaceReturn.EndEdit();
                   // CalculateGrandTotal();
                }
            }
            else
            {
                int _PID = Convert.ToInt32(dgvReplaceReturn.Rows[e.RowIndex].Cells[0].Value);
                picProduct.Image = GetProductPhoto(Convert.ToInt32(_PID));
            }
        }
        bool isFromTabChanged = false;
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex==0)
            {
                isFromTabChanged = true;
                radNewItem.Checked = true;

                txtTotalItems.Text ="New Items Count :"+ dgvProductDetails.Rows.Count.ToString();

            }
            else if (tabControl1.SelectedIndex==1)
            {
                isFromTabChanged = true;
                radReplace.Checked = true;
                txtTotalItems.Text = "Replace/Return Items :" + dgvReplaceReturn.Rows.Count.ToString();

            }
        }
    }
}