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
    public partial class SalesReportForm : Form
    {
        private SaleRepository _saleRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;
        private ReportingService _reportingService;
        
        private List<SalesReportItem> _salesReportItems;
        private List<Customer> _customers;
        private List<Product> _products;

        public SalesReportForm()
        {
            InitializeComponent();
            _saleRepository = new SaleRepository();
            _customerRepository = new CustomerRepository();
            _productRepository = new ProductRepository();
            _reportingService = new ReportingService();
            
            _salesReportItems = new List<SalesReportItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadCustomers();
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
            cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;
            cmbPaymentMethod.SelectedIndexChanged += CmbPaymentMethod_SelectedIndexChanged;
            
            // Date range event handlers
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpToDate.ValueChanged += DtpToDate_ValueChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += SalesReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvSalesReport.AutoGenerateColumns = false;
                dgvSalesReport.AllowUserToAddRows = false;
                dgvSalesReport.AllowUserToDeleteRows = false;
                dgvSalesReport.ReadOnly = true;
                dgvSalesReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvSalesReport.MultiSelect = false;

                // Define columns
                dgvSalesReport.Columns.Clear();
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "InvoiceNumber",
                    HeaderText = "Invoice #",
                    DataPropertyName = "InvoiceNumber",
                    Width = 120
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SaleDate",
                    HeaderText = "Sale Date",
                    DataPropertyName = "SaleDate",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CustomerName",
                    HeaderText = "Customer",
                    DataPropertyName = "CustomerName",
                    Width = 150
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Product",
                    DataPropertyName = "ProductName",
                    Width = 150
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Qty",
                    DataPropertyName = "Quantity",
                    Width = 60
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UnitPrice",
                    HeaderText = "Unit Price",
                    DataPropertyName = "UnitPrice",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SubTotal",
                    HeaderText = "Sub Total",
                    DataPropertyName = "SubTotal",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TaxAmount",
                    HeaderText = "Tax",
                    DataPropertyName = "TaxAmount",
                    Width = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalAmount",
                    HeaderText = "Total",
                    DataPropertyName = "TotalAmount",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PaymentMethod",
                    HeaderText = "Payment",
                    DataPropertyName = "PaymentMethod",
                    Width = 100
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PaidAmount",
                    HeaderText = "Paid",
                    DataPropertyName = "PaidAmount",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadCustomers()
        {
            try
            {
                _customers = _customerRepository.GetAllCustomers();
                cmbCustomer.DataSource = new List<Customer> { new Customer { CustomerID = 0, CustomerName = "All Customers" } }.Concat(_customers).ToList();
                cmbCustomer.DisplayMember = "CustomerName";
                cmbCustomer.ValueMember = "CustomerID";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                _products = _productRepository.GetAllProducts();
                cmbProduct.DataSource = new List<Product> { new Product { ProductID = 0, ProductName = "All Products" } }.Concat(_products).ToList();
                cmbProduct.DisplayMember = "ProductName";
                cmbProduct.ValueMember = "ProductID";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading products: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadSalesData()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                
                var sales = _saleRepository.GetSalesByDateRange(fromDate, toDate);
                _salesReportItems.Clear();
                
                foreach (var sale in sales)
                {
                    foreach (var saleItem in sale.SaleItems)
                    {
                        var reportItem = new SalesReportItem
                        {
                            SaleID = sale.SaleID,
                            InvoiceNumber = sale.InvoiceNumber,
                            SaleDate = sale.SaleDate,
                            CustomerName = sale.CustomerName ?? "Walk-in Customer",
                            ProductName = saleItem.ProductName,
                            Quantity = saleItem.Quantity,
                            UnitPrice = saleItem.UnitPrice,
                            SubTotal = saleItem.SubTotal,
                            TaxAmount = sale.TaxAmount / sale.SaleItems.Count, // Distribute tax across items
                            TotalAmount = saleItem.SubTotal + (sale.TaxAmount / sale.SaleItems.Count),
                            PaymentMethod = sale.PaymentMethod,
                            PaidAmount = sale.PaidAmount / sale.SaleItems.Count, // Distribute paid amount across items
                            BalanceAmount = (sale.TotalAmount - sale.PaidAmount) / sale.SaleItems.Count
                        };
                        
                        _salesReportItems.Add(reportItem);
                    }
                }
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading sales data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = _salesReportItems.AsEnumerable();
                
                // Customer filter
                if (cmbCustomer.SelectedItem != null)
                {
                    var selectedCustomer = (Customer)cmbCustomer.SelectedItem;
                    if (selectedCustomer != null)
                    {
                        filteredItems = filteredItems.Where(item => item.CustomerName == selectedCustomer.CustomerName);
                    }
                }
                
                // Product filter
                if (cmbProduct.SelectedItem != null)
                {
                    var selectedProduct = (Product)cmbProduct.SelectedItem;
                    if (selectedProduct != null)
                    {
                        filteredItems = filteredItems.Where(item => item.ProductName == selectedProduct.ProductName);
                    }
                }
                
                // Payment method filter
                if (!string.IsNullOrWhiteSpace(cmbPaymentMethod.SelectedItem?.ToString()) && cmbPaymentMethod.SelectedItem.ToString() != "All Payment Methods")
                {
                    filteredItems = filteredItems.Where(item => item.PaymentMethod == cmbPaymentMethod.SelectedItem.ToString());
                }
                
                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        item.InvoiceNumber.ToLower().Contains(searchTerm) ||
                        item.CustomerName.ToLower().Contains(searchTerm) ||
                        item.ProductName.ToLower().Contains(searchTerm) ||
                        item.PaymentMethod.ToLower().Contains(searchTerm));
                }
                
                _salesReportItems = filteredItems.ToList();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error applying filters: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                dgvSalesReport.DataSource = null;
                dgvSalesReport.DataSource = _salesReportItems;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateSummaryLabels()
        {
            try
            {
                var totalSales = _salesReportItems.Sum(item => item.TotalAmount);
                var totalQuantity = _salesReportItems.Sum(item => item.Quantity);
                var totalTax = _salesReportItems.Sum(item => item.TaxAmount);
                var totalPaid = _salesReportItems.Sum(item => item.PaidAmount);
                var totalBalance = _salesReportItems.Sum(item => item.BalanceAmount);
                var uniqueCustomers = _salesReportItems.Select(item => item.CustomerName).Distinct().Count();
                var uniqueProducts = _salesReportItems.Select(item => item.ProductName).Distinct().Count();
                
                lblTotalSales.Text = $"Total Sales: ${totalSales:F2}";
                lblTotalQuantity.Text = $"Total Quantity: {totalQuantity}";
                lblTotalTax.Text = $"Total Tax: ${totalTax:F2}";
                lblTotalPaid.Text = $"Total Paid: ${totalPaid:F2}";
                lblTotalBalance.Text = $"Total Balance: ${totalBalance:F2}";
                lblUniqueCustomers.Text = $"Unique Customers: {uniqueCustomers}";
                lblUniqueProducts.Text = $"Unique Products: {uniqueProducts}";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating summary labels: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;
            cmbCustomer.SelectedIndex = 0;
            cmbProduct.SelectedIndex = 0;
            cmbPaymentMethod.SelectedIndex = 0;
            txtSearch.Clear();
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadSalesData();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FileName = $"SalesReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveFileDialog.FileName);
                    ShowMessage("Report exported successfully!", "Export Complete", MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting report: {ex.Message}", "Export Error", MessageBoxIcon.Error);
            }
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implement PDF export functionality
                ShowMessage("PDF export functionality will be implemented with iTextSharp package.", "Info", MessageBoxIcon.Information);
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
                // TODO: Implement print functionality
                ShowMessage("Print functionality will be implemented with PrintDocument class.", "Info", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            SetInitialState();
            LoadSalesData();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void CmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void CmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void DtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFromDate.Value > dtpToDate.Value)
            {
                dtpToDate.Value = dtpFromDate.Value;
            }
        }

        private void DtpToDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpToDate.Value < dtpFromDate.Value)
            {
                dtpFromDate.Value = dtpToDate.Value;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void SalesReportForm_Load(object sender, EventArgs e)
        {
            // Set initial state
            SetInitialState();
        }

        private void ExportToCSV(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Invoice Number,Sale Date,Customer Name,Product Name,Quantity,Sale Price,Total Amount,Payment Method");
                
                // Write data
                foreach (var item in _salesReportItems)
                {
                    writer.WriteLine($"{item.InvoiceNumber},{item.SaleDate:yyyy-MM-dd},{item.CustomerName},{item.ProductName},{item.Quantity},{item.UnitPrice:F2},{item.TotalAmount:F2},{item.PaymentMethod}");
                }
            }
        }
    }

    // Data class for sales report items
    public class SalesReportItem
    {
        public int SaleID { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
    }
}
