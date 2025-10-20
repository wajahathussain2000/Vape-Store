using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;

namespace Vape_Store
{
    public partial class PurchaseReportForm : Form
    {
        private PurchaseRepository _purchaseRepository;
        private SupplierRepository _supplierRepository;
        private ProductRepository _productRepository;
        private ReportingService _reportingService;
        
        private List<PurchaseReportItem> _purchaseReportItems;
        private List<Supplier> _suppliers;
        private List<Product> _products;

        public PurchaseReportForm()
        {
            InitializeComponent();
            _purchaseRepository = new PurchaseRepository();
            _supplierRepository = new SupplierRepository();
            _productRepository = new ProductRepository();
            _reportingService = new ReportingService();
            
            _purchaseReportItems = new List<PurchaseReportItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadSuppliers();
            LoadProducts();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnGenerateReport.Click += BtnGenerateReport_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnExportPDF.Click += BtnExportPDF_Click;
            btnPrint.Click += BtnPrint_Click;
            btnClear.Click += BtnClear_Click;
            btnClose.Click += BtnClose_Click;
            
            // Filter event handlers
            cmbSupplier.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;
            cmbPaymentMethod.SelectedIndexChanged += CmbPaymentMethod_SelectedIndexChanged;
            
            // Date range event handlers
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpToDate.ValueChanged += DtpToDate_ValueChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += PurchaseReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvPurchaseReport.AutoGenerateColumns = false;
                dgvPurchaseReport.AllowUserToAddRows = false;
                dgvPurchaseReport.AllowUserToDeleteRows = false;
                dgvPurchaseReport.ReadOnly = true;
                dgvPurchaseReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvPurchaseReport.MultiSelect = false;

                // Define columns
                dgvPurchaseReport.Columns.Clear();
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "InvoiceNumber",
                    HeaderText = "Invoice #",
                    DataPropertyName = "InvoiceNumber",
                    Width = 120
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PurchaseDate",
                    HeaderText = "Purchase Date",
                    DataPropertyName = "PurchaseDate",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SupplierName",
                    HeaderText = "Supplier",
                    DataPropertyName = "SupplierName",
                    Width = 150
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Product",
                    DataPropertyName = "ProductName",
                    Width = 150
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Qty",
                    DataPropertyName = "Quantity",
                    Width = 60
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Bonus",
                    HeaderText = "Bonus",
                    DataPropertyName = "Bonus",
                    Width = 60
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UnitPrice",
                    HeaderText = "Unit Price",
                    DataPropertyName = "UnitPrice",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SubTotal",
                    HeaderText = "Sub Total",
                    DataPropertyName = "SubTotal",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TaxAmount",
                    HeaderText = "Tax",
                    DataPropertyName = "TaxAmount",
                    Width = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalAmount",
                    HeaderText = "Total",
                    DataPropertyName = "TotalAmount",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PaymentMethod",
                    HeaderText = "Payment",
                    DataPropertyName = "PaymentMethod",
                    Width = 100
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PaidAmount",
                    HeaderText = "Paid",
                    DataPropertyName = "PaidAmount",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Balance",
                    HeaderText = "Balance",
                    DataPropertyName = "Balance",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadSuppliers()
        {
            try
            {
                _suppliers = _supplierRepository.GetAllSuppliers();
                cmbSupplier.DataSource = null;
                cmbSupplier.Items.Clear();
                
                // Add "All Suppliers" option
                cmbSupplier.Items.Add(new { SupplierID = 0, SupplierName = "All Suppliers" });
                
                // Add suppliers
                foreach (var supplier in _suppliers)
                {
                    cmbSupplier.Items.Add(new { SupplierID = supplier.SupplierID, SupplierName = supplier.SupplierName });
                }
                
                cmbSupplier.DisplayMember = "SupplierName";
                cmbSupplier.ValueMember = "SupplierID";
                cmbSupplier.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading suppliers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                _products = _productRepository.GetAllProducts();
                cmbProduct.DataSource = null;
                cmbProduct.Items.Clear();
                
                // Add "All Products" option
                cmbProduct.Items.Add(new { ProductID = 0, ProductName = "All Products" });
                
                // Add products
                foreach (var product in _products)
                {
                    cmbProduct.Items.Add(new { ProductID = product.ProductID, ProductName = product.ProductName });
                }
                
                cmbProduct.DisplayMember = "ProductName";
                cmbProduct.ValueMember = "ProductID";
                cmbProduct.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading products: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            // Set default date range (last 30 days)
            dtpToDate.Value = DateTime.Now;
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            
            // Initialize payment method filter
            cmbPaymentMethod.Items.Clear();
            cmbPaymentMethod.Items.Add("All Payment Methods");
            cmbPaymentMethod.Items.Add("Cash");
            cmbPaymentMethod.Items.Add("Credit Card");
            cmbPaymentMethod.Items.Add("Bank Transfer");
            cmbPaymentMethod.Items.Add("Cheque");
            cmbPaymentMethod.SelectedIndex = 0;
            
            // Clear search
            txtSearch.Clear();
            
            // Clear report data
            _purchaseReportItems.Clear();
            dgvPurchaseReport.DataSource = null;
            
            // Clear summary labels
            UpdateSummaryLabels();
        }

        private void UpdateSummaryLabels()
        {
            try
            {
                if (_purchaseReportItems == null || _purchaseReportItems.Count == 0)
                {
                    lblTotalPurchases.Text = "Total Purchases: Rs 0.00";
                    lblTotalQuantity.Text = "Total Quantity: 0";
                    lblTotalTax.Text = "Total Tax: Rs 0.00";
                    lblTotalPaid.Text = "Total Paid: Rs 0.00";
                    lblTotalBalance.Text = "Total Balance: Rs 0.00";
                    lblUniqueSuppliers.Text = "Unique Suppliers: 0";
                    lblUniqueProducts.Text = "Unique Products: 0";
                    return;
                }

                var totalPurchases = _purchaseReportItems.Sum(x => x.TotalAmount);
                var totalQuantity = _purchaseReportItems.Sum(x => x.Quantity);
                var totalTax = _purchaseReportItems.Sum(x => x.TaxAmount);
                var totalPaid = _purchaseReportItems.Sum(x => x.PaidAmount);
                var totalBalance = _purchaseReportItems.Sum(x => x.Balance);
                var uniqueSuppliers = _purchaseReportItems.Select(x => x.SupplierName).Distinct().Count();
                var uniqueProducts = _purchaseReportItems.Select(x => x.ProductName).Distinct().Count();

                lblTotalPurchases.Text = $"Total Purchases: Rs {totalPurchases:F2}";
                lblTotalQuantity.Text = $"Total Quantity: {totalQuantity}";
                lblTotalTax.Text = $"Total Tax: Rs {totalTax:F2}";
                lblTotalPaid.Text = $"Total Paid: Rs {totalPaid:F2}";
                lblTotalBalance.Text = $"Total Balance: Rs {totalBalance:F2}";
                lblUniqueSuppliers.Text = $"Unique Suppliers: {uniqueSuppliers}";
                lblUniqueProducts.Text = $"Unique Products: {uniqueProducts}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating summary labels: {ex.Message}");
            }
        }

        private void GenerateReport()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                
                // Get purchases by date range
                var purchases = _purchaseRepository.GetPurchasesByDateRange(fromDate, toDate);
                
                // Filter by supplier if selected
                if (cmbSupplier.SelectedItem != null)
                {
                    var selectedSupplier = (dynamic)cmbSupplier.SelectedItem;
                    if (selectedSupplier.SupplierID != 0)
                    {
                        purchases = purchases.Where(p => p.SupplierID == selectedSupplier.SupplierID).ToList();
                    }
                }
                
                // Filter by payment method if selected
                if (cmbPaymentMethod.SelectedItem != null && cmbPaymentMethod.SelectedItem.ToString() != "All Payment Methods")
                {
                    var selectedPaymentMethod = cmbPaymentMethod.SelectedItem.ToString();
                    purchases = purchases.Where(p => p.PaymentMethod == selectedPaymentMethod).ToList();
                }
                
                // Convert to report items
                _purchaseReportItems.Clear();
                
                foreach (var purchase in purchases)
                {
                    if (purchase.PurchaseItems != null && purchase.PurchaseItems.Count > 0)
                    {
                        foreach (var item in purchase.PurchaseItems)
                        {
                            // Filter by product if selected
                            if (cmbProduct.SelectedItem != null)
                            {
                                var selectedProduct = (dynamic)cmbProduct.SelectedItem;
                                if (selectedProduct.ProductID != 0 && item.ProductID != selectedProduct.ProductID)
                                {
                                    continue;
                                }
                            }
                            
                            var reportItem = new PurchaseReportItem
                            {
                                InvoiceNumber = purchase.InvoiceNumber,
                                PurchaseDate = purchase.PurchaseDate,
                                SupplierName = purchase.SupplierName ?? "Unknown",
                                ProductName = item.ProductName ?? "Unknown",
                                Quantity = item.Quantity,
                                Bonus = item.Bonus,
                                UnitPrice = item.UnitPrice,
                                SubTotal = item.SubTotal,
                                TaxAmount = purchase.TaxAmount,
                                TotalAmount = purchase.TotalAmount,
                                PaymentMethod = purchase.PaymentMethod ?? "Unknown",
                                PaidAmount = purchase.PaidAmount,
                                Balance = purchase.TotalAmount - purchase.PaidAmount
                            };
                            
                            _purchaseReportItems.Add(reportItem);
                        }
                    }
                    else
                    {
                        // Handle purchases without items
                        var reportItem = new PurchaseReportItem
                        {
                            InvoiceNumber = purchase.InvoiceNumber,
                            PurchaseDate = purchase.PurchaseDate,
                            SupplierName = purchase.SupplierName ?? "Unknown",
                            ProductName = "No Items",
                            Quantity = 0,
                            Bonus = 0,
                            UnitPrice = 0,
                            SubTotal = purchase.SubTotal,
                            TaxAmount = purchase.TaxAmount,
                            TotalAmount = purchase.TotalAmount,
                            PaymentMethod = purchase.PaymentMethod ?? "Unknown",
                            PaidAmount = purchase.PaidAmount,
                            Balance = purchase.TotalAmount - purchase.PaidAmount
                        };
                        
                        _purchaseReportItems.Add(reportItem);
                    }
                }
                
                // Apply search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    _purchaseReportItems = _purchaseReportItems.Where(x => 
                        x.InvoiceNumber.ToLower().Contains(searchTerm) ||
                        x.SupplierName.ToLower().Contains(searchTerm) ||
                        x.ProductName.ToLower().Contains(searchTerm) ||
                        x.PaymentMethod.ToLower().Contains(searchTerm)
                    ).ToList();
                }
                
                // Bind to DataGridView
                dgvPurchaseReport.DataSource = _purchaseReportItems;
                
                // Update summary
                UpdateSummaryLabels();
                
                ShowMessage($"Report generated successfully. Found {_purchaseReportItems.Count} records.", "Success", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        // Event Handlers
        private void PurchaseReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_purchaseReportItems == null || _purchaseReportItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Implement Excel export functionality
                ShowMessage("Excel export functionality will be implemented in the next version.", "Feature Coming Soon", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting to Excel: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (_purchaseReportItems == null || _purchaseReportItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Implement PDF export functionality
                ShowMessage("PDF export functionality will be implemented in the next version.", "Feature Coming Soon", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting to PDF: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (_purchaseReportItems == null || _purchaseReportItems.Count == 0)
                {
                    ShowMessage("No data to print. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Implement print functionality
                ShowMessage("Print functionality will be implemented in the next version.", "Feature Coming Soon", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            SetInitialState();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Auto-generate report when filter changes
            if (_purchaseReportItems != null && _purchaseReportItems.Count > 0)
            {
                GenerateReport();
            }
        }

        private void CmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Auto-generate report when filter changes
            if (_purchaseReportItems != null && _purchaseReportItems.Count > 0)
            {
                GenerateReport();
            }
        }

        private void CmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Auto-generate report when filter changes
            if (_purchaseReportItems != null && _purchaseReportItems.Count > 0)
            {
                GenerateReport();
            }
        }

        private void DtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            // Validate date range
            if (dtpFromDate.Value > dtpToDate.Value)
            {
                dtpToDate.Value = dtpFromDate.Value;
            }
        }

        private void DtpToDate_ValueChanged(object sender, EventArgs e)
        {
            // Validate date range
            if (dtpToDate.Value < dtpFromDate.Value)
            {
                dtpFromDate.Value = dtpToDate.Value;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Auto-filter when search text changes
            if (_purchaseReportItems != null && _purchaseReportItems.Count > 0)
            {
                GenerateReport();
            }
        }
    }

    // Report item class for purchase report
    public class PurchaseReportItem
    {
        public string InvoiceNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SupplierName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int Bonus { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
    }
}