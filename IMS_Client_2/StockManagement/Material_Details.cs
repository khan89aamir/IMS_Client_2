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

namespace IMS_Client_2.StockManagement
{
    public partial class Material_Details : Form
    {
        public Material_Details()
        {
            InitializeComponent();
        }

        clsUtility ObjUtil = new clsUtility();
        clsConnection_DAL ObjDAL = new clsConnection_DAL(true);

        private void FillStoreData()
        {
            DataTable dt = ObjDAL.GetDataCol(clsUtility.DBName + ".[dbo].[StoreMaster]", "StoreID,StoreName", "ISNULL(ActiveStatus,1)=1", "StoreName ASC");
            if (ObjUtil.ValidateTable(dt))
            {
                cmbShop.DataSource = dt;
                cmbShop.DisplayMember = "StoreName";
                cmbShop.ValueMember = "StoreID";
                cmbShop.SelectedIndex = -1;
            }
        }

        private void txtSearchByProductName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtSearchByProductName.Text.Length > 0)
                {
                    DataTable dt = ObjDAL.ExecuteSelectStatement("EXEC " + clsUtility.DBName + ".dbo.Get_ProductDetails_Popup '" + txtSearchByProductName.Text + "'");
                    if (ObjUtil.ValidateTable(dt))
                    {
                        ObjUtil.SetControlData(txtSearchByProductName, "ItemName");
                        ObjUtil.SetControlData(txtProductID, "ProductID");
                        ObjUtil.ShowDataPopup(dt, txtSearchByProductName, this, groupBox1);

                        if (ObjUtil.GetDataPopup() != null && ObjUtil.GetDataPopup().DataSource != null)
                        {
                            ObjUtil.GetDataPopup().AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            if (ObjUtil.GetDataPopup().ColumnCount > 0)
                            {
                                ObjUtil.GetDataPopup().Columns["ProductID"].Visible = false;
                                ObjUtil.GetDataPopup().Columns["CategoryID"].Visible = false;
                                ObjUtil.SetDataPopupSize(300, 0);
                            }
                        }
                        ObjUtil.GetDataPopup().CellClick += Material_Details_SearchByProductName_CellClick;
                        ObjUtil.GetDataPopup().KeyDown += Material_Details_SearchByProductName_KeyDown;
                    }
                    else
                    {
                        ObjUtil.CloseAutoExtender();
                    }
                }
                else
                {
                    txtProductID.Clear();
                    ObjUtil.CloseAutoExtender();
                }
            }
            catch (Exception ex)
            {
                clsUtility.ShowErrorMessage(ex.ToString(), clsUtility.strProjectTitle);
            }
        }

        private void Material_Details_SearchByProductName_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.DataSource != null)
            {
                txtSearchByProductName.SelectionStart = txtSearchByProductName.MaxLength;
                txtSearchByProductName.Focus();
                SearchByProductID();
            }
        }

        private void Material_Details_SearchByProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                txtSearchByProductName.SelectionStart = txtSearchByProductName.MaxLength;
                txtSearchByProductName.Focus();
                SearchByProductID();
            }
        }

        private void Material_Details_SearchByStyleNo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.DataSource != null)
            {
                txtSearchByStyleNo.SelectionStart = txtSearchByStyleNo.MaxLength;
                txtSearchByStyleNo.Focus();
                SearchByStyleNo();
            }
        }

        private void Material_Details_SearchByStyleNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                txtSearchByStyleNo.SelectionStart = txtSearchByStyleNo.MaxLength;
                txtSearchByStyleNo.Focus();
                SearchByStyleNo();
            }
        }

        private void SearchByProductID()
        {
            //DataTable dt = ObjDAL.ExecuteSelectStatement("EXEC " + clsUtility.DBName + ".dbo.Get_Material_Details " + txtProductID.Text + ",NULL");

            ObjDAL.SetStoreProcedureData("ProductID", SqlDbType.Int, txtProductID.Text.Trim(), clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("StoreID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("BarcodeNo", SqlDbType.BigInt, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ColorID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ModelNo", SqlDbType.NVarChar, DBNull.Value, clsConnection_DAL.ParamType.Input);
            DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(clsUtility.DBName + ".dbo.Get_Material_Details");
            DataTable dt = ds.Tables[0];
            if (ObjUtil.ValidateTable(dt))
            {
                dgvProductDetails.DataSource = dt;
            }
            else
            {
                dgvProductDetails.DataSource = null;
            }
            ObjDAL.ResetData();
        }

        private void SearchByShopID()
        {
            //DataTable dt = ObjDAL.ExecuteSelectStatement("EXEC " + clsUtility.DBName + ".dbo.Get_Material_Details NULL," + cmbShop.SelectedValue);

            ObjDAL.SetStoreProcedureData("ProductID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("StoreID", SqlDbType.Int, cmbShop.SelectedValue, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("BarcodeNo", SqlDbType.BigInt, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ColorID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ModelNo", SqlDbType.NVarChar, DBNull.Value, clsConnection_DAL.ParamType.Input);
            DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(clsUtility.DBName + ".dbo.Get_Material_Details");
            DataTable dt = ds.Tables[0];
            if (ObjUtil.ValidateTable(dt))
            {
                dgvProductDetails.DataSource = dt;
            }
            else
            {
                dgvProductDetails.DataSource = null;
            }
            ObjDAL.ResetData();
        }

        private void SearchByBarcode()
        {
            //DataTable dt = ObjDAL.ExecuteSelectStatement("EXEC " + clsUtility.DBName + ".dbo.Get_Material_Details NULL," + txtSearchByBarcode.Text.Trim());

            ObjDAL.SetStoreProcedureData("ProductID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("StoreID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("BarcodeNo", SqlDbType.BigInt, txtSearchByBarcode.Text.Trim(), clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ColorID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ModelNo", SqlDbType.NVarChar, DBNull.Value, clsConnection_DAL.ParamType.Input);
            DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(clsUtility.DBName + ".dbo.Get_Material_Details");
            DataTable dt = ds.Tables[0];
            if (ObjUtil.ValidateTable(dt))
            {
                dgvProductDetails.DataSource = dt;
            }
            else
            {
                dgvProductDetails.DataSource = null;
            }
            ObjDAL.ResetData();
        }
        private void SearchByColor()
        {
            //DataTable dt = ObjDAL.ExecuteSelectStatement("EXEC " + clsUtility.DBName + ".dbo.Get_Material_Details NULL," + txtSearchByBarcode.Text.Trim());

            ObjDAL.SetStoreProcedureData("ProductID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("StoreID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("BarcodeNo", SqlDbType.BigInt, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ColorID", SqlDbType.Int, cmbColor.SelectedValue, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ModelNo", SqlDbType.NVarChar, DBNull.Value, clsConnection_DAL.ParamType.Input);
            DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(clsUtility.DBName + ".dbo.Get_Material_Details");
            DataTable dt = ds.Tables[0];
            if (ObjUtil.ValidateTable(dt))
            {
                dgvProductDetails.DataSource = dt;
            }
            else
            {
                dgvProductDetails.DataSource = null;
            }
            ObjDAL.ResetData();
        }

        private void SearchByStyleNo()
        {
            //DataTable dt = ObjDAL.ExecuteSelectStatement("EXEC " + clsUtility.DBName + ".dbo.Get_Material_Details " + txtProductID.Text + ",NULL");

            ObjDAL.SetStoreProcedureData("ProductID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("StoreID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("BarcodeNo", SqlDbType.BigInt, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ColorID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ModelNo", SqlDbType.NVarChar, txtSearchByStyleNo.Text.Trim(), clsConnection_DAL.ParamType.Input);
            DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(clsUtility.DBName + ".dbo.Get_Material_Details");
            DataTable dt = ds.Tables[0];
            if (ObjUtil.ValidateTable(dt))
            {
                dgvProductDetails.DataSource = dt;
            }
            else
            {
                dgvProductDetails.DataSource = null;
            }
            ObjDAL.ResetData();
        }

        private void Material_Details_KeyDown(object sender, KeyEventArgs e)
        {
            txtSearchByProductName.Focus();
        }

        private void Material_Details_Load(object sender, EventArgs e)
        {
            FillColorData();
            FillStoreData();
            LoadData();
            cmbShop.SelectedValue = frmHome.Home_StoreID;
            rdShowAll.Checked = true;
        }
        private void LoadData()
        {
            //DataTable dt = ObjDAL.ExecuteSelectStatement("EXEC " + clsUtility.DBName + ".dbo.Get_Material_Details 0,0");

            ObjDAL.SetStoreProcedureData("ProductID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("StoreID", SqlDbType.Int, cmbShop.SelectedValue, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("BarcodeNo", SqlDbType.BigInt, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ColorID", SqlDbType.Int, DBNull.Value, clsConnection_DAL.ParamType.Input);
            ObjDAL.SetStoreProcedureData("ModelNo", SqlDbType.NVarChar, DBNull.Value, clsConnection_DAL.ParamType.Input);
            DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(clsUtility.DBName + ".dbo.Get_Material_Details");
            DataTable dt = ds.Tables[0];
            if (ObjUtil.ValidateTable(dt))
            {
                dgvProductDetails.DataSource = dt;
            }
            else
            {
                dgvProductDetails.DataSource = null;
            }
            ObjDAL.ResetData();
        }

        private void FillColorData()
        {
            DataTable dt = ObjDAL.GetDataCol(clsUtility.DBName + ".dbo.ColorMaster", "ColorID,ColorName", "ISNULL(ActiveStatus,1)=1", "ColorName ASC");
            if (ObjUtil.ValidateTable(dt))
            {
                cmbColor.DataSource = dt;
                cmbColor.DisplayMember = "ColorName";
                cmbColor.ValueMember = "ColorID";
            }
            cmbColor.SelectedIndex = -1;
        }

        private void cmbShop_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SearchByShopID();
        }

        private void rdSearchByStore_CheckedChanged(object sender, EventArgs e)
        {
            if (rdSearchByStore.Checked)
            {
                cmbShop.Enabled = true;
                cmbShop.Focus();
            }
            else
            {
                cmbShop.Enabled = false;
                cmbShop.SelectedIndex = -1;
            }
        }

        private void rdSearchByItem_CheckedChanged(object sender, EventArgs e)
        {
            if (rdSearchByItem.Checked)
            {
                txtSearchByProductName.Enabled = true;
                txtSearchByProductName.Focus();
            }
            else
            {
                txtSearchByProductName.Enabled = false;
                txtSearchByProductName.Clear();
            }
        }

        private void rdShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rdShowAll.Checked)
            {
                LoadData();
            }
            else
            {
                cmbShop.SelectedIndex = -1;
                cmbColor.SelectedIndex = -1;
                txtSearchByProductName.Clear();
                txtSearchByBarcode.Clear();
                txtSearchByStyleNo.Clear();
            }
        }

       

        private void dgvProductDetails_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ObjUtil.SetRowNumber(dgvProductDetails);
            //ObjUtil.SetDataGridProperty(dgvProductDetails, DataGridViewAutoSizeColumnsMode.ColumnHeader);
            ObjUtil.SetDataGridProperty(dgvProductDetails, DataGridViewAutoSizeColumnsMode.Fill);
            dgvProductDetails.Columns["ProductID"].Visible = false;
            dgvProductDetails.Columns["Photo"].Visible = false;
            dgvProductDetails.Columns["StoreID"].Visible = false;
            dgvProductDetails.Columns["CategoryID"].Visible = false;
            dgvProductDetails.Columns["SizeTypeID"].Visible = false;
            dgvProductDetails.Columns["SizeID"].Visible = false;

            lblTotalRecords.Text = "Total Records : " + dgvProductDetails.Rows.Count.ToString();
            dgvProductDetails.MultiSelect = true;
        }

        private void txtSearchByProductName_Enter(object sender, EventArgs e)
        {
            ObjUtil.SetTextHighlightColor(sender);
        }

        private void txtSearchByProductName_Leave(object sender, EventArgs e)
        {
            ObjUtil.SetTextHighlightColor(sender, Color.White);
        }

        private void rdSearchByBarCode_CheckedChanged(object sender, EventArgs e)
        {
            if (rdSearchByBarCode.Checked)
            {
                txtSearchByBarcode.Enabled = true;
                txtSearchByBarcode.Focus();
            }
            else
            {
                txtSearchByBarcode.Enabled = false;
                txtSearchByBarcode.Clear();
            }
        }

        private void rdSearchByColor_CheckedChanged(object sender, EventArgs e)
        {
            if (rdSearchByColor.Checked)
            {
                cmbColor.Enabled = true;
                cmbColor.Focus();
            }
            else
            {
                cmbColor.Enabled = false;
                cmbColor.SelectedIndex = -1;
            }
        }

        private void cmbColor_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SearchByColor();
        }

        private void dgvProductDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (dgvProductDetails.SelectedRows.Count > 0)
            {
               // GetProductImage(dgvProductDetails.SelectedRows[0].Cells["ProductID"].Value.ToString());
            }
        }

        private void printBarcodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DataGridViewSelectedRowCollection dgvRows = dgvProductDetails.SelectedRows;
            //if (dgvProductDetails.SelectedRows != null && dgvProductDetails.SelectedRows.Count > 0)
            //{
            //    Barcode.frmBarCodeStockPopup frmBarCode = new Barcode.frmBarCodeStockPopup();
            //    frmBarCode.dgvRows = dgvRows;
            //    frmBarCode.ShowDialog();
            //}
            //else
            //{
            //    clsUtility.ShowInfoMessage("Please select record.", clsUtility.strProjectTitle);
            //}
        }

        private void txtSearchByStyleNo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtSearchByStyleNo.Text.Length > 0)
                {
                    ObjDAL.SetStoreProcedureData("ModelNo", SqlDbType.NVarChar, txtSearchByStyleNo.Text.Trim(), clsConnection_DAL.ParamType.Input);
                    DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(clsUtility.DBName + ".dbo.SPR_Get_StyleNo_Popup");
                    DataTable dt = ds.Tables[0];
                    if (ObjUtil.ValidateTable(dt))
                    {
                        ObjUtil.SetControlData(txtSearchByStyleNo, "StyleNo");
                        ObjUtil.ShowDataPopup(dt, txtSearchByStyleNo, this, groupBox1);

                        if (ObjUtil.GetDataPopup() != null && ObjUtil.GetDataPopup().DataSource != null)
                        {
                            ObjUtil.GetDataPopup().AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            if (ObjUtil.GetDataPopup().ColumnCount > 0)
                            {
                                ObjUtil.SetDataPopupSize(180, 0);
                            }
                        }
                        ObjUtil.GetDataPopup().CellClick += Material_Details_SearchByStyleNo_CellClick;
                        ObjUtil.GetDataPopup().KeyDown += Material_Details_SearchByStyleNo_KeyDown;
                    }
                    else
                    {
                        ObjUtil.CloseAutoExtender();
                    }
                }
                else
                {
                    ObjUtil.CloseAutoExtender();
                }
            }
            catch (Exception ex)
            {
                clsUtility.ShowErrorMessage(ex.ToString(), clsUtility.strProjectTitle);
            }
        }

        private void rdSearchByStyleNo_CheckedChanged(object sender, EventArgs e)
        {
            if (rdSearchByStyleNo.Checked)
            {
                txtSearchByStyleNo.Enabled = true;
                txtSearchByStyleNo.Focus();
            }
            else
            {
                txtSearchByStyleNo.Enabled = false;
                txtSearchByStyleNo.Clear();
            }
        }

        private void txtSearchByBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!ObjUtil.IsControlTextEmpty(txtSearchByBarcode))
                {
                    SearchByBarcode();
                }
            }
        }
    }
}