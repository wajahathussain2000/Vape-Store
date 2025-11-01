using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;
using Vape_Store.DataAccess;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
                // Suppress default DataGridView data error dialogs
                dgvCustomerReport.DataError += (s, e) => { e.ThrowException = false; };

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
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
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
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvCustomerReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalPaid",
                    HeaderText = "Total Paid",
                    DataPropertyName = "TotalPaid",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvCustomerReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalDue",
                    HeaderText = "Total Due",
                    DataPropertyName = "TotalDue",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
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
                lblTotalSales.Text = $"Total Sales: {totalSales:F2}";
                lblActiveCustomers.Text = $"Active Customers: {totalCustomers}";
                lblAverageSale.Text = $"Average Sale: {avgOrderValue:F2}";
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
            try
            {
                if (_customerReportData == null || _customerReportData.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"CustomerReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToPDF(saveFileDialog.FileName);
                    ShowMessage("Report exported to PDF successfully!", "Export Complete", MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting to PDF: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
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

        private void ExportToPDF(string filePath)
        {
            try
            {
                // Create PDF document
                Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                // Set up fonts
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(baseFont, 18, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font smallFont = new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL);

                // Title
                Paragraph title = new Paragraph("VAPE STORE - CUSTOMER REPORT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Report info
                Paragraph reportInfo = new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nDate Range: {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}", normalFont);
                reportInfo.SpacingAfter = 15f;
                document.Add(reportInfo);

                // Create table for report data
                PdfPTable table = new PdfPTable(9);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 2f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f });

                // Table headers
                string[] headers = { "Code", "Customer Name", "Phone", "Total Purchases", "Total Orders", "Avg Order Value", "Total Paid", "Total Due", "Last Purchase" };
                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    table.AddCell(cell);
                }

                // Add data rows
                foreach (var item in _customerReportData)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.CustomerCode, normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.CustomerName, normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.Phone, normalFont)) { Padding = 3f });
                    
                    PdfPCell purchasesCell = new PdfPCell(new Phrase(item.TotalPurchases.ToString("F2"), normalFont));
                    purchasesCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    purchasesCell.Padding = 3f;
                    table.AddCell(purchasesCell);
                    
                    PdfPCell ordersCell = new PdfPCell(new Phrase(item.TotalOrders.ToString(), normalFont));
                    ordersCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    ordersCell.Padding = 3f;
                    table.AddCell(ordersCell);
                    
                    PdfPCell avgCell = new PdfPCell(new Phrase(item.AverageOrderValue.ToString("F2"), normalFont));
                    avgCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    avgCell.Padding = 3f;
                    table.AddCell(avgCell);
                    
                    PdfPCell paidCell = new PdfPCell(new Phrase(item.TotalPaid.ToString("F2"), normalFont));
                    paidCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    paidCell.Padding = 3f;
                    table.AddCell(paidCell);
                    
                    PdfPCell dueCell = new PdfPCell(new Phrase(item.TotalDue.ToString("F2"), normalFont));
                    dueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    dueCell.Padding = 3f;
                    table.AddCell(dueCell);
                    
                    string lastPurchase = item.LastPurchaseDate?.ToString("MM/dd/yyyy") ?? "N/A";
                    table.AddCell(new PdfPCell(new Phrase(lastPurchase, normalFont)) { Padding = 3f });
                }

                document.Add(table);

                // Add summary section
                document.Add(new Paragraph("\n", normalFont));
                
                Paragraph summaryTitle = new Paragraph("SUMMARY", headerFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                // Calculate totals
                var totalCustomers = _customerReportData.Count;
                var totalSales = _customerReportData.Sum(x => x.TotalPurchases);
                var totalOrders = _customerReportData.Sum(x => x.TotalOrders);
                var totalPaid = _customerReportData.Sum(x => x.TotalPaid);
                var totalDue = _customerReportData.Sum(x => x.TotalDue);
                var avgOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0;

                document.Add(new Paragraph($"Total Customers: {totalCustomers}", normalFont));
                document.Add(new Paragraph($"Total Sales: {totalSales:F2}", normalFont));
                document.Add(new Paragraph($"Total Orders: {totalOrders}", normalFont));
                document.Add(new Paragraph($"Total Paid: {totalPaid:F2}", normalFont));
                document.Add(new Paragraph($"Total Due: {totalDue:F2}", normalFont));
                document.Add(new Paragraph($"Average Order Value: {avgOrderValue:F2}", normalFont));

                document.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating PDF: {ex.Message}");
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void CustomerReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
            LoadCustomerReportData(); // Automatically load data when form opens
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