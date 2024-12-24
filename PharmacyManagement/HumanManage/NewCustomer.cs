﻿using DevExpress.XtraEditors;
using PharmacyManagement.DB_query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PharmacyManagement.HumanManage
{
    public partial class NewCustomer : XtraForm
    {
        PharmacyMgtDatabase dataTable = new PharmacyMgtDatabase();
        string customerID = "";
        public NewCustomer()
        {
            dataTable.OpenConnection();
            InitializeComponent();
        }

        private void NewCustomer_Load(object sender, EventArgs e)
        {
            dgvCustomer.AutoGenerateColumns = false;
            FetchData();
            ToggleControls(false);

        }

        public void FetchData()
        {
            string customerSql = @"SELECT * FROM Customer";
            SqlCommand customerCmd = new SqlCommand(customerSql);
            dataTable.Fill(customerCmd);
            BindingSource binding = new BindingSource();
            binding.DataSource = dataTable;

            dgvCustomer.DataSource = binding;
            bindingNavigator.BindingSource = binding;

            txtCustomerID.DataBindings.Clear();
            txtCustomerName.DataBindings.Clear();
            txtContact.DataBindings.Clear();
            txtAddress.DataBindings.Clear();
            radMale.DataBindings.Clear();
            radFemale.DataBindings.Clear();

            txtCustomerID.DataBindings.Add("Text", binding, "CustomerID");
            txtCustomerName.DataBindings.Add("Text", binding, "CustomerName");
            txtContact.DataBindings.Add("Text", binding, "Contact");
            txtAddress.DataBindings.Add("Text", binding, "CustomerAddress");

            // Handle rad button gender
            Binding male = new Binding("Checked", binding, "Sex");
            male.Format += (s, evt) =>
            {
                evt.Value = Convert.ToString(evt.Value) == "M";
            };
            radMale.DataBindings.Add(male);

            Binding female = new Binding("Checked", binding, "Sex");
            female.Format += (s, evt) =>
            {
                evt.Value = Convert.ToString(evt.Value) == "F";
            };
            radFemale.DataBindings.Add(female);
        }

        private void ToggleControls(bool value)
        {
            txtCustomerID.Enabled = value;
            txtCustomerName.Enabled = value;
            txtContact.Enabled = value;
            txtAddress.Enabled = value;
            radMale.Enabled = value;
            radFemale.Enabled = value;

            btnSave.Enabled = value;
            btnReload.Enabled = value;

            btnAdd.Enabled = !value;
            btnEdit.Enabled = !value;
            btnDelete.Enabled = !value;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            customerID = "";
            txtCustomerID.Text = "";
            txtCustomerName.Text = "";
            txtContact.Text = "";
            txtAddress.Text = "";
            radFemale.Checked = true;
            txtCustomerID.Focus();

            ToggleControls(true);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            customerID = txtCustomerName.Text;
            ToggleControls(true);
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            NewCustomer_Load(sender, e);
            dgvCustomer.Sort(dgvCustomer.Columns["CustomerID"], ListSortDirection.Ascending);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult kq;
            kq = MessageBox.Show("Are you sure you want to delete this customer? ", "Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (kq == DialogResult.Yes)
            {
                string deleteCustomerQuery = @"DELETE FROM CUSTOMER WHERE CustomerID = @CustomerID";
                SqlCommand deleteCustomerCmd = new SqlCommand(deleteCustomerQuery);
                deleteCustomerCmd.Parameters.Add("@CustomerID", SqlDbType.NVarChar, 5).Value = txtCustomerID.Text;
                dataTable.Update(deleteCustomerCmd);

                NewCustomer_Load(sender, e);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    string checkCustomerIdSql = @"SELECT COUNT(*) FROM CUSTOMER WHERE CustomerID = @CustomerID";
                    SqlCommand checkCmd = new SqlCommand(checkCustomerIdSql);
                    checkCmd.Parameters.Add("@CustomerID", SqlDbType.NVarChar, 5).Value = txtCustomerID.Text.Trim();
                    int count = (int)dataTable.Update(checkCmd);
                    if (count > 0)
                    {
                        MessageBox.Show("Customer ID already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (customerID == "")
                    {
                        string sql = @"INSERT INTO CUSTOMER VALUES(@CustomerID, @CustomerName, @Sex, 
                      @Contact, @CustomerAddress)";
                        SqlCommand cmd = new SqlCommand(sql);

                        cmd.Parameters.Add("@CustomerID", SqlDbType.NVarChar, 5).Value = txtCustomerID.Text;
                        cmd.Parameters.Add("@CustomerName", SqlDbType.NVarChar, 200).Value = txtCustomerName.Text;
                        cmd.Parameters.Add("@Sex", SqlDbType.Char, 1).Value = radFemale.Checked ? "F" : "M";
                        cmd.Parameters.Add("@Contact", SqlDbType.NVarChar, 10).Value = txtContact.Text;
                        cmd.Parameters.Add("@CustomerAddress", SqlDbType.NVarChar, 200).Value = txtAddress.Text;
                        dataTable.Update(cmd);
                        MessageBox.Show("Customer added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string sql = @"UPDATE CUSTOMER
                                       SET CustomerID = @newCustomerID,
                                       CustomerName = @CustomerName,
                                       Sex = @Sex,
                                       Contact = @Contact,
                                       CustomerAddress = CustomerAddress
                                   WHERE CustomerID = @oldCustomerID";
                        SqlCommand cmd = new SqlCommand(sql);
                        cmd.Parameters.Add("@newCustomerID", SqlDbType.VarChar, 5).Value = txtCustomerID.Text.Trim();
                        cmd.Parameters.Add("@oldCustomerID", SqlDbType.VarChar, 5).Value = customerID;
                        cmd.Parameters.Add("@CustomerName", SqlDbType.VarChar, 200).Value = txtCustomerName.Text;
                        cmd.Parameters.Add("@Sex", SqlDbType.Char, 1).Value = radFemale.Checked ? "F" : "M";
                        cmd.Parameters.Add("@Contact", SqlDbType.VarChar, 10).Value = txtContact.Text;
                        cmd.Parameters.Add("@CustomerAddress", SqlDbType.VarChar, 200).Value = txtAddress.Text;
                        dataTable.Update(cmd);
                        MessageBox.Show("Customer edited successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    NewCustomer_Load(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private bool ValidateInput()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.IsBalloon = true;

            toolTip.Hide(txtCustomerID);
            toolTip.Hide(txtCustomerName);
            toolTip.Hide(txtContact);

            // Validate Customer ID
            if (string.IsNullOrWhiteSpace(txtCustomerID.Text))
            {
                toolTip.Show("Please enter the Customer ID!", txtCustomerID,
                    txtCustomerID.Width - 15, txtCustomerID.Height - 80, 2000);
                txtCustomerID.Focus();
                return false;
            }

            // Validate Commodity Name
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                toolTip.Show("Please enter the Customer Name!", txtCustomerName,
                    txtCustomerName.Width - 15, txtCustomerName.Height - 80, 2000);
                txtCustomerName.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtContact.Text, @"^\d+$"))
            {
                toolTip.Show("Contact number must contain only digits!", txtContact,
                    txtContact.Width - 15, txtContact.Height - 80, 2000);
                txtContact.Focus();
                return false;
            }

            return true;
        }

        private void dgvCustomer_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvCustomer.Columns[e.ColumnIndex].Name == "Sex")
            {
                if (e.Value != null)
                {
                    e.Value = e.Value.ToString() == "F" ? "Nữ" : "Nam";
                };
            }
        }
    }
}