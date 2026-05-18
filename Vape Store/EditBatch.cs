using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Models;

namespace Vape_Store
{
    public partial class EditBatch : Form
    {
        string cs = ConfigurationManager.ConnectionStrings["dbs"].ConnectionString;
        private List<string> allProducts = new List<string>();
        private bool suppressTextChanged = false;
        private Timer barcodeTimer = new Timer();

        public EditBatch()
        {
            InitializeComponent();

            cmbProducts.TextUpdate += cmbProducts_TextUpdate;
            cmbProducts.KeyDown += cmbProducts_KeyDown;

            txtBarcode.TextChanged += txtBarcode_TextChanged;

            txtPurchasePrice.TextChanged += txtPurchasePrice_TextChanged;
            txtSalePrice.TextChanged += txtSalePrice_TextChanged;

           
            barcodeTimer.Interval = 200; 
            barcodeTimer.Tick += BarcodeTimer_Tick;
        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            // Restart timer every time user types
            barcodeTimer.Stop();
            barcodeTimer.Start();
        }

        private void BarcodeTimer_Tick(object sender, EventArgs e)
        {
            barcodeTimer.Stop();

            string barcode = txtBarcode.Text.Trim();

            if (!string.IsNullOrEmpty(barcode))
            {
                LoadProductByBarcode(barcode);

                // Clear after scan
                txtBarcode.Clear();

                // Focus again for next scan
                txtBarcode.Focus();
            }
        }

        private void LoadProductByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
            SELECT ProductID, ProductName 
            FROM Products 
            WHERE Barcode = @barcode", con);

                cmd.Parameters.AddWithValue("@barcode", barcode);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int productId = Convert.ToInt32(reader["ProductID"]);
                        string productName = reader["ProductName"].ToString();

                        cmbProducts.Text = productName;

                        LoadBatchData(productId);
                    }
                    else
                    {
                        dgvBatches.DataSource = null;

                        // 🔔 OPTIONAL: alert if not found
                        MessageBox.Show("Barcode not found!");
                    }
                }
            }
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                LoadProductByBarcode(txtBarcode.Text.Trim());

                // 🔥 CLEAR AFTER SCAN (VERY IMPORTANT)
                txtBarcode.Clear();

                // 🔥 KEEP CURSOR READY FOR NEXT SCAN
                txtBarcode.Focus();
            }
        }





        private void EditBatch_Load(object sender, EventArgs e)
        {
            LoadProductNames();
            cmbProducts.AutoCompleteMode = AutoCompleteMode.None;
            cmbProducts.AutoCompleteSource = AutoCompleteSource.None;
        }



        private void LoadProductNames()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                {
                    conn.Open();
                    string query = "SELECT ProductName FROM Products";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    allProducts.Clear();
                    while (reader.Read())
                        allProducts.Add(reader["ProductName"].ToString());

                    reader.Close();
                }


                cmbProducts.DataSource = new List<string>(allProducts);
                cmbProducts.SelectedIndex = -1;   
                cmbProducts.Text = "";            
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void cmbProducts_TextUpdate(object sender, EventArgs e)
        {
            if (suppressTextChanged) return;

            string typed = cmbProducts.Text;

            var filtered = allProducts
                .Where(p => p.IndexOf(typed, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            suppressTextChanged = true;

            if (filtered.Count > 0)
            {
                cmbProducts.DataSource = null;
                cmbProducts.DataSource = filtered;

                cmbProducts.SelectedIndex = 0;  
                cmbProducts.SelectedIndex = -1; 

                cmbProducts.DroppedDown = true;
            }
            else
            {
                cmbProducts.DroppedDown = false;
            }

            cmbProducts.Text = typed;
            cmbProducts.SelectionStart = typed.Length;
            cmbProducts.SelectionLength = 0;


            suppressTextChanged = false;
        }

        private void cmbProducts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cmbProducts.SelectedItem != null)
                {
                    cmbProducts.Text = cmbProducts.SelectedItem.ToString();
                }

                string productName = cmbProducts.Text;
                int productId = GetProductIdByName(productName);

                if (productId > 0)
                {
                    LoadBatchData(productId);
                }

                cmbProducts.DroppedDown = false;
                e.Handled = true;
            }
        }

        private void cmbProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProducts.SelectedItem == null) return;

            string productName = cmbProducts.Text;

            int productId = GetProductIdByName(productName);

            if (productId > 0)
            {
                LoadBatchData(productId);
            }
        }

        private void LoadBatchData(int productId)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(@"
             SELECT 
                PurchaseItemID,
                BatchNumber,
                RemainingQuantity,
                UnitPrice,
                SellingPrice,
                ExpiryDate
               FROM PurchaseItems
               WHERE ProductID = @pid AND RemainingQuantity > 0
                ORDER BY PurchaseItemID ASC", con);

                da.SelectCommand.Parameters.AddWithValue("@pid", productId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvBatches.DataSource = dt;

                SetupGrid();
            }
        }

        private void SetupGrid()
        {
            dgvBatches.Columns["PurchaseItemID"].Visible = false;

            dgvBatches.Columns["BatchNumber"].ReadOnly = true;
            dgvBatches.Columns["RemainingQuantity"].ReadOnly = true;
            dgvBatches.Columns["ExpiryDate"].ReadOnly = true;

            dgvBatches.Columns["UnitPrice"].HeaderText = "Purchase Price";
            dgvBatches.Columns["SellingPrice"].HeaderText = "Sale Price";

            dgvBatches.Columns["UnitPrice"].ReadOnly = false;
            dgvBatches.Columns["SellingPrice"].ReadOnly = false;
        }

        private void UpdateBatchPrices()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    foreach (DataGridViewRow row in dgvBatches.Rows)
                    {
                        if (row.Cells["PurchaseItemID"].Value == null) continue;

                        decimal purchasePrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
                        decimal salePrice = Convert.ToDecimal(row.Cells["SellingPrice"].Value);

                        SqlCommand cmd = new SqlCommand(@"
                    UPDATE PurchaseItems
                    SET UnitPrice = @pp,
                        SellingPrice = @sp
                    WHERE PurchaseItemID = @id", con, tran);

                        cmd.Parameters.AddWithValue("@pp", purchasePrice);
                        cmd.Parameters.AddWithValue("@sp", salePrice);
                        cmd.Parameters.AddWithValue("@id", row.Cells["PurchaseItemID"].Value);

                        cmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                    MessageBox.Show("Batch Prices Updated Successfully!");
                    Clear();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateBatchPrices();

        }



        private int GetProductIdByName(string productName)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT ProductID FROM Products WHERE ProductName = @name", con);

                cmd.Parameters.AddWithValue("@name", productName);

                object result = cmd.ExecuteScalar();

                if (result != null)
                    return Convert.ToInt32(result);
                else
                    return 0;
            }
        }


        private void cmbProducts_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmbProducts.SelectedItem == null) return;

            string productName = cmbProducts.SelectedItem.ToString();

            int productId = GetProductIdByName(productName);

            if (productId > 0)
            {
                LoadBatchData(productId);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbProducts.Text))
            {
                MessageBox.Show("Please select a product first.");
                return;
            }

            string productName = cmbProducts.Text;
            int productId = GetProductIdByName(productName);

            if (productId > 0)
            {
                LoadBatchData(productId);
            }
            else
            {
                MessageBox.Show("Product not found.");
            }
        }

        private void Clear()
        {
            cmbProducts.DataSource = null;
            cmbProducts.DataSource = new List<string>(allProducts);
            cmbProducts.SelectedIndex = -1;
            cmbProducts.Text = "";

            dgvBatches.DataSource = null;
            dgvBatches.Rows.Clear();
            dgvBatches.Refresh();
            txtPurchasePrice.Text = string.Empty;
            txtSalePrice.Text = string.Empty;
            suppressTextChanged = false;


        }
        private void button3_Click(object sender, EventArgs e)
        {
          Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtPurchasePrice_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPurchasePrice.Text))
                return;

            if (!decimal.TryParse(txtPurchasePrice.Text, out decimal newPrice))
                return;

            foreach (DataGridViewRow row in dgvBatches.Rows)
            {
                if (row.IsNewRow) continue;

                row.Cells["UnitPrice"].Value = newPrice;
            }

            
        }

        private void txtSalePrice_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSalePrice.Text))
                return;

            if (!decimal.TryParse(txtSalePrice.Text, out decimal newPrice))
                return;

            foreach (DataGridViewRow row in dgvBatches.Rows)
            {
                if (row.IsNewRow) continue;

                row.Cells["SellingPrice"].Value = newPrice;
            }
            
        }


    }
}
