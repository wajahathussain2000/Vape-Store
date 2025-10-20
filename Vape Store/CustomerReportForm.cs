using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;
using Vape_Store.DataAccess;

namespace Vape_Store
{
    public partial class CustomerReportForm : Form
    {
        private CustomerRepository _customerRepository;
        private SaleRepository _saleRepository;
        private ReportingService _reportingService;
        
        private List<CustomerReportData> _customerReportData;

        public CustomerReportForm()
        {
            InitializeComponent();
            _customerRepository = new CustomerRepository();
            _saleRepository = new SaleRepository();
            _reportingService = new ReportingService();
            
            _customerReportData = new List<CustomerReportData>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            btnGenerateReport.Click += BtnGenerateReport_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnExportPDF.Click += BtnExportPDF_Click;
            btnPrint.Click += BtnPrint_Click;
            btnClear.Click += BtnClear_Click;
            btnClose.Click += BtnClose_Click;
            
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpToDate.ValueChanged += DtpToDate_ValueChanged;
            
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            this.Load += CustomerReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvCustomerReport.AutoGenerateColumns = false;
                dgvCustomerReport.AllowUserToAddRows = false;
                dgvCustomerReport.AllowUserToDeleteRows = false;
                dgvCustomerReport.ReadOnly = true;
                dgvCustomerReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvCustomerReport.MultiSelect = false;

                dgvCustomerReport.Columns.Clear();
                
                dgvCustomerReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CustomerCode",
                    HeaderText = "Code",
                    DataPropertyName = "CustomerCode",
                    Width = 80
                });
                
                dgvCustomerReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CustomerName",
                    HeaderText = "Customer Name",
                    DataPropertyName = "CustomerName",
                    Width = 150
                });
                
                dgvCustomerReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Phone",
                    HeaderText = "Phone",
                    DataPropertyName = "Phone",
                    Width = 100
                });
                
                dgvCustomerReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalPurchases",
                    HeaderText = "Total Purchases",
                    DataPropertyName = "TotalPurchases",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvCustomerReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalOrders",
                    HeaderText = "Total Orders",
                    DataPropertyName = "TotalOrders",
                    Width = 100
                });
                
                dgvCustomerReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "AverageOrderValue",
                    HeaderText = "Avg Order Value",
                    DataPropertyName = "AverageOrderValue",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvCustomerReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalPaid",
                    HeaderText = "Total Paid",
                    DataPropertyName = "TotalPaid",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvCustomerReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalDue",
                    HeaderText = "Total Due",
                    DataPropertyName = "TotalDue",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvCustomerReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LastPurchaseDate",
                    HeaderText = "Last Purchase",
                    DataPropertyName = "LastPurchaseDate",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadCustomerReportData()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1);
                
                _customerReportData.Clear();
                
                var customers = _customerRepository.GetAllCustomers();
                
                foreach (var customer in customers)
                {
                    var customerData = GetCustomerData(customer.CustomerID, fromDate, toDate);
                    customerData.CustomerCode = customer.CustomerCode;
                    customerData.CustomerName = customer.CustomerName;
                    customerData.Phone = customer.Phone;
                    customerData.Email = customer.Email;
                    customerData.Address = customer.Address;
                    
                    _customerReportData.Add(customerData);
                }
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customer report data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private CustomerReportData GetCustomerData(int customerId, DateTime fromDate, DateTime toDate)
        {
            var data = new CustomerReportData { CustomerID = customerId };
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    
                    var query = @"
                        SELECT 
                            COUNT(*) as TotalOrders,
                            ISNULL(SUM(TotalAmount), 0) as TotalPurchases,
                            ISNULL(SUM(PaidAmount), 0) as TotalPaid,
                            ISNULL(MAX(SaleDate), NULL) as LastPurchaseDate
                        FROM Sales
                        WHERE CustomerID = @CustomerID
                        AND SaleDate BETWEEN @FromDate AND @ToDate";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customerId);
                        command.Parameters.AddWithValue("@FromDate", fromDate);
                        command.Parameters.AddWithValue("@ToDate", toDate);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                data.TotalOrders = Convert.ToInt32(reader["TotalOrders"]);
                                data.TotalPurchases = Convert.ToDecimal(reader["TotalPurchases"]);
                                data.TotalPaid = Convert.ToDecimal(reader["TotalPaid"]);
                                data.LastPurchaseDate = reader["LastPurchaseDate"] != DBNull.Value 
                                    ? Convert.ToDateTime(reader["LastPurchaseDate"]) 
                                    : (DateTime?)null;
                            }
                        }
                    }
                }
                
                data.TotalDue = data.TotalPurchases - data.TotalPaid;
                data.AverageOrderValue = data.TotalOrders > 0 ? data.TotalPurchases / data.TotalOrders : 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting customer data: {ex.Message}", ex);
            }
            
            return data;
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredData = _customerReportData.AsEnumerable();
                
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    filteredData = filteredData.Where(item => 
                        item.CustomerCode.ToLower().Contains(searchTerm) ||
                        item.CustomerName.ToLower().Contains(searchTerm) ||
                        item.Phone.ToLower().Contains(searchTerm));
                }
                
                _customerReportData = filteredData.OrderByDescending(x => x.TotalPurchases).ToList();
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
                dgvCustomerReport.DataSource = null;
                dgvCustomerReport.DataSource = _customerReportData;
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
                var totalCustomers = _customerReportData.Count;
                var totalSales = _customerReportData.Sum(x => x.TotalPurchases);
                var totalOrders = _customerReportData.Sum(x => x.TotalOrders);
                var totalPaid = _customerReportData.Sum(x => x.TotalPaid);
                var totalDue = _customerReportData.Sum(x => x.TotalDue);
                var avgOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0;
                
                lblTotalCustomers.Text = $"Total Customers: {totalCustomers}";
                lblTotalSales.Text = $"Total Sales: ${totalSales:F2}";
                lblTotalOrders.Text = $"Total Orders: {totalOrders}";
                lblTotalPaid.Text = $"Total Paid: ${totalPaid:F2}";
                lblTotalDue.Text = $"Total Due: ${totalDue:F2}";
                lblAvgOrderValue.Text = $"Avg Order Value: ${avgOrderValue:F2}";
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
            txtSearch.Clear();
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadCustomerReportData();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            ShowMessage("Excel export functionality will be implemented.", "Info", MessageBoxIcon.Information);
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            ShowMessage("PDF export functionality will be implemented.", "Info", MessageBoxIcon.Information);
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            ShowMessage("Print functionality will be implemented.", "Info", MessageBoxIcon.Information);
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

        private void CustomerReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
        }
    }

    public class CustomerReportData
    {
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal TotalPurchases { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
    }
}