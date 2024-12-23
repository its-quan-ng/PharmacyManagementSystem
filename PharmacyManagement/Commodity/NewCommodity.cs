﻿using DevExpress.XtraEditors;
using PharmacyManagement.DB_query;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PharmacyManagement.Commodity
{
    public partial class NewCommodity : XtraForm
    {
        PharmacyMgtDatabase dataTable = new PharmacyMgtDatabase();
        public NewCommodity()
        {
            dataTable.OpenConnection();
            InitializeComponent();

            txtCommodityID.CausesValidation = true;
            txtCommodityName.CausesValidation = true;
            this.AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void NewCommodity_Load(object sender, EventArgs e)
        {
            FetchData();
            txtCommodityID.Select();
        }

        public void FetchData()
        {
            PharmacyMgtDatabase categogyTable = new PharmacyMgtDatabase();
            categogyTable.OpenConnection();
            string typeSql = "SELECT * FROM CATEGORIES";
            SqlCommand typeCmd = new SqlCommand(typeSql);
            categogyTable.Fill(typeCmd);
            cboCommodityType.DataSource = categogyTable;
            cboCommodityType.DisplayMember = "CategoryName";
            cboCommodityType.ValueMember = "CategoryID";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (ValidateChildren(ValidationConstraints.Enabled))
            {
                CreateNewCommodity();
            }
        }

        private void CreateNewCommodity()
        {
            try
            {
                string sql = @"INSERT INTO COMMODITY VALUES(@CommodityID, @CommodityName, @Manufacturer, 
                      @Quantity, @BaseUnit, @PurchasePrice, @SellingPrice, @MfgDate, @ExpDate, @CategoryID)";
                SqlCommand cmd = new SqlCommand(sql);

                if (!int.TryParse(txtQuantity.Text, out int quantity) ||
                    !decimal.TryParse(txtPurchasePrice.Text, out decimal purchasePrice) ||
                    !decimal.TryParse(txtSellingPrice.Text, out decimal sellingPrice))
                {
                    MessageBox.Show("Invalid numeric values", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                cmd.Parameters.Add("@CommodityID", SqlDbType.NVarChar, 5).Value = txtCommodityID.Text;
                cmd.Parameters.Add("@CommodityName", SqlDbType.NVarChar, 200).Value = txtCommodityName.Text;
                cmd.Parameters.Add("@Manufacturer", SqlDbType.NVarChar, 200).Value = txtManufacturer.Text;
                cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = quantity;
                cmd.Parameters.Add("@BaseUnit", SqlDbType.NVarChar, 30).Value = txtBaseUnit.Text;
                cmd.Parameters.Add("@PurchasePrice", SqlDbType.Money).Value = purchasePrice;
                cmd.Parameters.Add("@SellingPrice", SqlDbType.Money).Value = sellingPrice;
                cmd.Parameters.Add("@MfgDate", SqlDbType.Date).Value = dtpMfgDate.Value;
                cmd.Parameters.Add("@ExpDate", SqlDbType.Date).Value = dtpExpDate.Value;
                cmd.Parameters.Add("@CategoryID", SqlDbType.TinyInt).Value = Convert.ToByte(cboCommodityType.SelectedValue);

                dataTable.Update(cmd);
                MessageBox.Show("Commodity added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        #region validating input
        private void txtCommodityID_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCommodityID.Text))
            {
                errorProvider1.SetError(txtCommodityID, "Please enter the Commodity ID!");
                txtCommodityID.Focus();
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(txtCommodityID, null);
                e.Cancel = false;
            }
        }

        private void txtCommodityName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCommodityName.Text))
            {
                errProviderName.SetError(txtCommodityName, "Please enter the Commodity Name!");
                txtCommodityName.Focus();
                e.Cancel = true;
            }
            else
            {
                errProviderName.SetError(txtCommodityName, null);
                e.Cancel = false;
            }
        }


        private void txtManufacturer_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtManufacturer.Text))
            {
                errProviderFacturer.SetError(txtManufacturer, "Please enter the Commodity ID!");
                txtManufacturer.Focus();
                e.Cancel = true;
            }
            else
            {
                errProviderFacturer.SetError(txtManufacturer, null);
                e.Cancel = false;
            }
        }

        private void txtBaseUnit_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBaseUnit.Text))
            {
                errProviderUnit.SetError(txtBaseUnit, "Please enter the Commodity ID!");
                txtBaseUnit.Focus();
                e.Cancel = true;
            }
            else
            {
                errProviderUnit.SetError(txtBaseUnit, null);
                e.Cancel = false;
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to cancel the changes?",
                "Cancel Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                NewCommodity_Load(sender, e);
            }
        }

        private void txtQuantity_Validating(object sender, CancelEventArgs e)
        {
            if (!int.TryParse(txtQuantity.Text, out _))
            {
                errProviderQuantity.SetError(txtQuantity, "Please enter a valid number!");
                e.Cancel = true;
            }
            else
            {
                errProviderQuantity.SetError(txtQuantity, null);
                e.Cancel = false;
            }
        }

        private void dtpExpDate_Validating(object sender, CancelEventArgs e)
        {
            if (dtpExpDate.Value <= dtpMfgDate.Value)
            {
                errProviderExpDate.SetError(dtpExpDate, "Expiry date must be after manufacturing date!");
                e.Cancel = true;
            }
            else
            {
                errProviderExpDate.SetError(dtpExpDate, null);
                e.Cancel = false;
            }
        }
        #endregion
    }
}