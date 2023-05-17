using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace PrintFeatureDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void CalculateTotalPrice ()
        {
            double total = 0;
            for (int i = 0; i < this.gridProducts.Rows.Count; i++)
            {
                total += double.Parse(this.gridProducts.Rows[i].Cells[1].Value.ToString()) * int.Parse(this.gridProducts.Rows[i].Cells[2].Value.ToString());
            }

            this.lblTotalPrice.Text = string.Format("Total Price: {0:n}", total);
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allows 0-9 and backspace
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46)
            {
                e.Handled = true;
                return;
            }
            // Checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if ((sender as Guna.UI2.WinForms.Guna2TextBox).Text.IndexOf(e.KeyChar) != -1)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allows 0-9 and backspace
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46)
            {
                e.Handled = true;
                return;
            }
            // Checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if ((sender as Guna.UI2.WinForms.Guna2TextBox).Text.IndexOf(e.KeyChar) != -1)
                {
                    e.Handled = true;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataGridViewButtonColumn clmBtnRemove = new DataGridViewButtonColumn();
            clmBtnRemove.HeaderText = "Action";
            clmBtnRemove.Name = "clmBtnRemove";
            clmBtnRemove.Text = "Remove";
            clmBtnRemove.UseColumnTextForButtonValue = true;
            this.gridProducts.Columns.Add(clmBtnRemove);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int n = this.gridProducts.Rows.Add();
            this.gridProducts.Rows[n].Cells[0].Value = this.txtProductName.Text;

            double price = double.Parse(this.txtPrice.Text);
            this.gridProducts.Rows[n].Cells[1].Value = string.Format("{0:n}", price);
            
            this.gridProducts.Rows[n].Cells[2].Value = this.txtQuantity.Text;

            double totalPriceEachItem = double.Parse(this.txtPrice.Text) * int.Parse(this.txtQuantity.Text);
            this.gridProducts.Rows[n].Cells[3].Value = string.Format("{0:n}", totalPriceEachItem);

            this.gridProducts.ClearSelection();
            CalculateTotalPrice();

            this.txtProductName.ResetText();
            this.txtPrice.ResetText();
            this.txtQuantity.ResetText();

            this.txtProductName.Focus();
        }

        private void gridProducts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = this.gridProducts.Rows[e.RowIndex];

            if (row.Cells[4].Selected)
            {
                this.gridProducts.Rows.Remove(row);
            }

            this.gridProducts.ClearSelection();
            CalculateTotalPrice();

            this.txtProductName.Focus();
        }

        public class Cart
        {
            public string ProductName { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
            public double Total { get; set; }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string fullName = string.Empty;
            if (String.IsNullOrWhiteSpace(this.txtMiddleName.Text))
            {
                fullName = string.Format("{0} {1}", this.txtFirstName.Text, this.txtLastName.Text);
            }
            else
            {
                fullName = string.Format("{0} {1}. {2}", this.txtFirstName.Text, this.txtMiddleName.Text[0], this.txtLastName.Text);
            }

            List<Cart> cart = new List<Cart>();
            cart.Clear();

            for (int i = 0; i < this.gridProducts.Rows.Count; i++)
            {
                cart.Add(new Cart()
                {
                    ProductName = this.gridProducts.Rows[i].Cells[0].Value.ToString(),
                    Price = double.Parse(this.gridProducts.Rows[i].Cells[1].Value.ToString()),
                    Quantity = int.Parse(this.gridProducts.Rows[i].Cells[2].Value.ToString()),
                    Total = double.Parse(this.gridProducts.Rows[i].Cells[3].Value.ToString())
                });
            }

            Form2 frm2 = new Form2();
            ReportViewer rprtReceipt = (ReportViewer)frm2.Controls["rprtReceipt"];

            ReportDataSource source = new ReportDataSource("dsCart", cart);
            rprtReceipt.LocalReport.DataSources.Clear();
            rprtReceipt.LocalReport.DataSources.Add(source);

            ReportParameterCollection parameters = new ReportParameterCollection();
            parameters.Add(new ReportParameter("pCustomerName", fullName));
            parameters.Add(new ReportParameter("pTotalPrice", this.lblTotalPrice.Text));
            rprtReceipt.LocalReport.SetParameters(parameters);

            frm2.ShowDialog();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit this application?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
