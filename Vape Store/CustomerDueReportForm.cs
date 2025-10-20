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
    public partial class CustomerDueReportForm : Form
    {
        private CustomerRepository _customerRepository;
        private SaleRepository _saleRepository;
        private CustomerPaymentRepository _customerPaymentRepository;
        private ReportingService _reportingService;
        
        private List<CustomerDueReportItem> _customerDueItems;
        private List<Customer> _customers;

        public CustomerDueReportForm()
        {
            InitializeComponent();
            _customerRepository = new CustomerRepository();
            _saleRepository = new SaleRepository();
            _customerPaymentRepository = new CustomerPaymentRepository();
            _reportingService = new ReportingService();
            
            _customerDueItems = new List<CustomerDueReportItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadCustomers();
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
            
            // Date range event handlers
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpToDate.ValueChanged += DtpToDate_ValueChanged;
            
            // Filter event handlers
            cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += CustomerDueReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvCustomerDueReport.AutoGenerateColumns = false;
                dgvCustomerDueReport.AllowUserToAddRows = false;
                dgvCustomerDueReport.AllowUserToDeleteRows = false;
                dgvCustomerDueReport.ReadOnly = true;
                dgvCustomerDueReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvCustomerDueReport.MultiSelect = false;

                // Define columns
                dgvCustomerDueReport.Columns.Clear();
                
                dgvCustomerDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CustomerCode",
                    HeaderText = "Code",
                    DataPropertyName = "CustomerCode",
                    Width = 80
                });
                
                dgvCustomerDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CustomerName",
                    HeaderText = "Customer Name",
                    DataPropertyName = "CustomerName",
                    Width = 200
                });
                
                dgvCustomerDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Phone",
                    HeaderText = "Phone",
                    DataPropertyName = "Phone",
                    Width = 120
                });
                
                dgvCustomerDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalSales",
                    HeaderText = "Total Sales",
                    DataPropertyName = "TotalSales",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvCustomerDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalPaid",
                    HeaderText = "Total Paid",
                    DataPropertyName = "TotalPaid",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvCustomerDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalDue",
                    HeaderText = "Total Due",
                    DataPropertyName = "TotalDue",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvCustomerDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LastSaleDate",
                    HeaderText = "Last Sale",
                    DataPropertyName = "LastSaleDate",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvCustomerDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LastPaymentDate",
                    HeaderText = "Last Payment",
                    DataPropertyName = "LastPaymentDate",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
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
                cmbCustomer.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadCustomerDueData()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                
                _customerDueItems.Clear();
                
                foreach (var customer in _customers)
                {
                    // Get sales for this customer
                    var sales = _saleRepository.GetSalesByCustomerAndDateRange(customer.CustomerID, fromDate, toDate);
                    var totalSales = sales.Sum(s => s.TotalAmount);
                    
                    // Get payments for this customer
                    var payments = _customerPaymentRepository.GetPaymentsByCustomerAndDateRange(customer.CustomerID, fromDate, toDate);
                    var totalPaid = payments.Sum(p => p.PaidAmount);
                    
                    var totalDue = totalSales - totalPaid;
                    var lastSaleDate = sales.Any() ? sales.Max(s => s.SaleDate) : (DateTime?)null;
                    var lastPaymentDate = payments.Any() ? payments.Max(p => p.PaymentDate) : (DateTime?)null;
                    
                    var reportItem = new CustomerDueReportItem
                    {
                        CustomerID = customer.CustomerID,
                        CustomerCode = customer.CustomerCode,
                        CustomerName = customer.CustomerName,
                        Phone = customer.Phone,
                        TotalSales = totalSales,
                        TotalPaid = totalPaid,
                        TotalDue = totalDue,
                        LastSaleDate = lastSaleDate,
                        LastPaymentDate = lastPaymentDate
                    };
                    
                    _customerDueItems.Add(reportItem);
                }
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customer due data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = _customerDueItems.AsEnumerable();
                
                // Customer filter
                if (cmbCustomer.SelectedItem != null)
                {
                    var selectedCustomer = (Customer)cmbCustomer.SelectedItem;
                    filteredItems = filteredItems.Where(item => item.CustomerID == selectedCustomer.CustomerID);
                }
                
                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        item.CustomerName.ToLower().Contains(searchTerm) ||
                        item.CustomerCode.ToLower().Contains(searchTerm) ||
                        item.Phone.ToLower().Contains(searchTerm));
                }
                
                dgvCustomerDueReport.DataSource = filteredItems.ToList();
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
                dgvCustomerDueReport.Refresh();
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
                var totalSales = _customerDueItems.Sum(item => item.TotalSales);
                var totalPaid = _customerDueItems.Sum(item => item.TotalPaid);
                var totalDue = _customerDueItems.Sum(item => item.TotalDue);
                var customersWithDue = _customerDueItems.Count(item => item.TotalDue > 0);
                var totalCustomers = _customerDueItems.Count;
                
                lblTotalSales.Text = $"Total Sales: ${totalSales:F2}";
                lblTotalPaid.Text = $"Total Paid: ${totalPaid:F2}";
                lblTotalDue.Text = $"Total Due: ${totalDue:F2}";
                lblCustomersWithDue.Text = $"Customers with Due: {customersWithDue}";
                lblTotalCustomers.Text = $"Total Customers: {totalCustomers}";
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
            txtSearch.Clear();
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadCustomerDueData();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_customerDueItems == null || _customerDueItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FileName = $"CustomerDueReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
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
                if (_customerDueItems == null || _customerDueItems.Count == 0)
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
                if (_customerDueItems == null || _customerDueItems.Count == 0)
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

        private void DtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFromDate.Value > dtpToDate.Value)
                dtpToDate.Value = dtpFromDate.Value;
        }

        private void DtpToDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpToDate.Value < dtpFromDate.Value)
                dtpFromDate.Value = dtpToDate.Value;
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void ExportToCSV(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Customer Code,Customer Name,Phone,Total Sales,Total Paid,Total Due,Last Sale,Last Payment");
                
                // Write data
                foreach (var item in _customerDueItems)
                {
                    writer.WriteLine($"{item.CustomerCode},{item.CustomerName},{item.Phone},{item.TotalSales:F2},{item.TotalPaid:F2},{item.TotalDue:F2},{item.LastSaleDate?.ToString("yyyy-MM-dd")},{item.LastPaymentDate?.ToString("yyyy-MM-dd")}");
                }
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void CustomerDueReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
        }
    }

    public class CustomerDueReportItem
    {
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
        public DateTime? LastSaleDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
    }
}

