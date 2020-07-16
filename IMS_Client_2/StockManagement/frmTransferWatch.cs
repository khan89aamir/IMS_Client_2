﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CoreApp;

namespace IMS_Client_2.StockManagement
{
    public partial class frmTransferWatch : Form
    {
        public frmTransferWatch()
        {
            InitializeComponent();
        }

        clsConnection_DAL ObjDAL = new clsConnection_DAL(true);
        clsUtility ObjUtil = new clsUtility();

        Image B_Leave = IMS_Client_2.Properties.Resources.B_click;
        Image B_Enter = IMS_Client_2.Properties.Resources.B_on;

        private void dgvTransferWatch_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ObjUtil.SetRowNumber(dgvTransferWatch);
            ObjUtil.SetDataGridProperty(dgvTransferWatch, DataGridViewAutoSizeColumnsMode.Fill);
        }

        private void LoadData()
        {
            DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(clsUtility.DBName + ".dbo.SPR_Get_StoreTransfer_List");
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (ObjUtil.ValidateTable(dt))
                {
                    dgvTransferWatch.DataSource = dt;
                }
                else
                {
                    dgvTransferWatch.DataSource = null;
                }
            }
        }
        private void frmTransferWatch_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdByDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rdByDate.Checked)
            {
                dtpFromDate.Enabled = true;
                dtpToDate.Enabled = true;
                cmbBillStatus.SelectedIndex = -1;
                cmbBillStatus.Enabled = false;
            }
            else
            {
                dtpFromDate.Enabled = false;
                dtpToDate.Enabled = false;
            }
        }

        private void rdByBillStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (rdByBillStatus.Checked)
            {
                cmbBillStatus.Enabled = true;
                dtpFromDate.Enabled = false;
                dtpToDate.Enabled = false;
            }
            else
            {
                cmbBillStatus.SelectedIndex = -1;
                cmbBillStatus.Enabled = false;
            }
        }
    }
}