using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;
using Vape_Store.Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Vape_Store
{
    public partial class SupplierDueReportForm : Form
    {
        private SupplierRepository _supplierRepository;
        private SupplierLedgerRepository _supplierLedgerRepository;
        private ReportingService _reportingService;
        
        private List<SupplierDueReportItem> _supplierDueItems;
        private List<Supplier> _suppliers;

        public SupplierDueReportForm()
        {
            InitializeComponent();
            _supplierRepository = new SupplierRepository();
            _supplierLedgerRepository = new SupplierLedgerRepository();
            _reportingService = new ReportingService();
            
            _supplierDueItems = new List<SupplierDueReportItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadSuppliers();
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
            cmbSupplier.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += SupplierDueReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvSupplierDueReport.AutoGenerateColumns = false;
                dgvSupplierDueReport.AllowUserToAddRows = false;
                dgvSupplierDueReport.AllowUserToDeleteRows = false;
                dgvSupplierDueReport.ReadOnly = true;
                dgvSupplierDueReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvSupplierDueReport.MultiSelect = false;

                // Define columns
                dgvSupplierDueReport.Columns.Clear();
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SupplierCode",
                    HeaderText = "Code",
                    DataPropertyName = "SupplierCode",
                    Width = 80
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SupplierName",
                    HeaderText = "Supplier Name",
                    DataPropertyName = "SupplierName",
                    Width = 200
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Phone",
                    HeaderText = "Phone",
                    DataPropertyName = "Phone",
                    Width = 120
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalPurchases",
                    HeaderText = "Total Purchases",
                    DataPropertyName = "TotalPurchases",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalPaid",
                    HeaderText = "Total Paid",
                    DataPropertyName = "TotalPaid",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalDue",
                    HeaderText = "Total Due",
                    DataPropertyName = "TotalDue",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LastPurchaseDate",
                    HeaderText = "Last Purchase",
                    DataPropertyName = "LastPurchaseDate",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
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

        private void LoadSuppliers()
        {
            try
            {
                _suppliers = _supplierRepository.GetAllSuppliers();
                var supplierList = new List<Supplier> { new Supplier { SupplierID = 0, SupplierName = "All Suppliers" } };
                if (_suppliers != null && _suppliers.Count > 0)
                {
                    supplierList.AddRange(_suppliers);
                }
                SearchableComboBoxHelper.MakeSearchable(cmbSupplier, supplierList, "SupplierName", "SupplierID", "SupplierName");
                cmbSupplier.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading suppliers: {ex.Message}", "Error", MessageBoxIcon.Error);
                var fallbackList = new List<Supplier> { new Supplier { SupplierID = 0, SupplierName = "All Suppliers" } };
                SearchableComboBoxHelper.MakeSearchable(cmbSupplier, fallbackList, "SupplierName", "SupplierID", "SupplierName");
                cmbSupplier.SelectedIndex = 0;
            }
        }

        private void LoadSupplierDueData()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                
                int? selectedSupplierId = null;
                if (cmbSupplier.SelectedItem is Supplier selectedSupplier && selectedSupplier.SupplierID != 0)
                {
                    selectedSupplierId = selectedSupplier.SupplierID;
                }

                var summaries = _supplierLedgerRepository.GetSupplierSummaries(fromDate, toDate, selectedSupplierId);
                _supplierDueItems = new List<SupplierDueReportItem>();

                foreach (var summary in summaries)
                {
                    _supplierDueItems.Add(new SupplierDueReportItem
                    {
                        SupplierID = summary.SupplierID,
                        SupplierCode = summary.SupplierCode,
                        SupplierName = summary.SupplierName,
                        Phone = summary.Phone,
                        TotalPurchases = summary.TotalCredit,
                        TotalPaid = summary.TotalDebit,
                        TotalDue = summary.ClosingBalance,
                        LastPurchaseDate = summary.LastPurchaseDate,
                        LastPaymentDate = summary.LastPaymentDate
                    });
                }

                if (_supplierDueItems.Count == 0)
                {
                    ShowMessage($"No supplier ledger activity found for the date range {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}.", "No Data Found", MessageBoxIcon.Information);
                }
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading supplier due data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = _supplierDueItems.AsEnumerable();
                
                // Supplier filter
                if (cmbSupplier.SelectedItem is Supplier selectedSupplier && selectedSupplier.SupplierID != 0)
                {
                    filteredItems = filteredItems.Where(item => item.SupplierID == selectedSupplier.SupplierID);
                }
                
                // Search filter - search through multiple fields
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        (item.SupplierName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.SupplierCode?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.Phone?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.TotalPurchases.ToString("F2").Contains(searchTerm)) ||
                        (item.TotalPaid.ToString("F2").Contains(searchTerm)) ||
                        (item.TotalDue.ToString("F2").Contains(searchTerm)) ||
                        (item.LastPurchaseDate?.ToString("yyyy-MM-dd").Contains(searchTerm) ?? false) ||
                        (item.LastPurchaseDate?.ToString("MM/dd/yyyy").Contains(searchTerm) ?? false) ||
                        (item.LastPaymentDate?.ToString("yyyy-MM-dd").Contains(searchTerm) ?? false) ||
                        (item.LastPaymentDate?.ToString("MM/dd/yyyy").Contains(searchTerm) ?? false));
                }
                
                dgvSupplierDueReport.DataSource = filteredItems.ToList();
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
                dgvSupplierDueReport.DataSource = null;
                dgvSupplierDueReport.DataSource = _supplierDueItems;
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
                var totalPurchases = _supplierDueItems.Sum(item => item.TotalPurchases);
                var totalPaid = _supplierDueItems.Sum(item => item.TotalPaid);
                var totalDue = _supplierDueItems.Sum(item => item.TotalDue);
                var suppliersWithDue = _supplierDueItems.Count(item => item.TotalDue > 0);
                var totalSuppliers = _supplierDueItems.Count;
                
                lblTotalPurchases.Text = $"Total Purchases: {totalPurchases:F2}";
                lblTotalPaid.Text = $"Total Paid: {totalPaid:F2}";
                lblTotalDue.Text = $"Total Due: {totalDue:F2}";
                lblSuppliersWithDue.Text = $"Suppliers with Due: {suppliersWithDue}";
                lblTotalSuppliers.Text = $"Total Suppliers: {totalSuppliers}";
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
            cmbSupplier.SelectedIndex = 0;
            txtSearch.Clear();
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadSupplierDueData();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_supplierDueItems == null || _supplierDueItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FileName = $"SupplierDueReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
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
                if (_supplierDueItems == null || _supplierDueItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"SupplierDueReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
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
            try
            {
                if (_supplierDueItems == null || _supplierDueItems.Count == 0)
                {
                    ShowMessage("No data to print. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    if (_supplierDueItems == null || _supplierDueItems.Count == 0)
                    {
                        ShowMessage("No data to print. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                        return;
                    }

                    var htmlContent = GenerateHTMLReport();
                    var htmlViewer = new HTMLReportViewerForm();
                    htmlViewer.LoadReport(htmlContent);
                    htmlViewer.ShowDialog();
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error printing report: {ex.Message}", "Error", MessageBoxIcon.Error);
                }
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

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
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
                writer.WriteLine("Supplier Code,Supplier Name,Phone,Total Purchases,Total Paid,Total Due,Last Purchase,Last Payment");
                
                // Write data
                foreach (var item in _supplierDueItems)
                {
                    writer.WriteLine($"{item.SupplierCode},{item.SupplierName},{item.Phone},{item.TotalPurchases:F2},{item.TotalPaid:F2},{item.TotalDue:F2},{item.LastPurchaseDate?.ToString("yyyy-MM-dd")},{item.LastPaymentDate?.ToString("yyyy-MM-dd")}");
                }
            }
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
                Paragraph title = new Paragraph("Attock Mobiles Rwp - SUPPLIER DUE REPORT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Report info
                Paragraph reportInfo = new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nDate Range: {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}", normalFont);
                reportInfo.SpacingAfter = 15f;
                document.Add(reportInfo);

                // Create table for report data
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 2f, 1.5f, 1.5f, 1.5f, 1f });

                // Table headers
                string[] headers = { "ID", "Supplier Name", "Phone", "Total Purchases", "Total Paid", "Due Amount" };
                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    table.AddCell(cell);
                }

                // Add data rows
                foreach (var item in _supplierDueItems)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.SupplierID.ToString(), normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.SupplierName, normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.Phone, normalFont)) { Padding = 3f });
                    
                    PdfPCell purchasesCell = new PdfPCell(new Phrase(item.TotalPurchases.ToString("F2"), normalFont));
                    purchasesCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    purchasesCell.Padding = 3f;
                    table.AddCell(purchasesCell);
                    
                    PdfPCell paidCell = new PdfPCell(new Phrase(item.TotalPaid.ToString("F2"), normalFont));
                    paidCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    paidCell.Padding = 3f;
                    table.AddCell(paidCell);
                    
                    PdfPCell dueCell = new PdfPCell(new Phrase(item.TotalDue.ToString("F2"), normalFont));
                    dueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    dueCell.Padding = 3f;
                    table.AddCell(dueCell);
                }

                document.Add(table);

                // Add summary section
                document.Add(new Paragraph("\n", normalFont));
                
                Paragraph summaryTitle = new Paragraph("SUMMARY", headerFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                // Calculate totals
                var totalSuppliers = _supplierDueItems.Count;
                var totalPurchases = _supplierDueItems.Sum(x => x.TotalPurchases);
                var totalPaid = _supplierDueItems.Sum(x => x.TotalPaid);
                var totalDue = _supplierDueItems.Sum(x => x.TotalDue);

                document.Add(new Paragraph($"Total Suppliers: {totalSuppliers}", normalFont));
                document.Add(new Paragraph($"Total Purchases: {totalPurchases:F2}", normalFont));
                document.Add(new Paragraph($"Total Paid: {totalPaid:F2}", normalFont));
                document.Add(new Paragraph($"Total Due: {totalDue:F2}", normalFont));

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

        private string GenerateHTMLReport()
        {
            try
            {
                var html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html><head>");
                html.AppendLine("<title>Supplier Due Report</title>");
                html.AppendLine("<style>");
                html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
                html.AppendLine("h1 { color: #2c3e50; text-align: center; }");
                html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
                html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
                html.AppendLine("th { background-color: #f2f2f2; font-weight: bold; }");
                html.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
                html.AppendLine(".summary { background-color: #e8f4f8; padding: 15px; margin: 20px 0; border-radius: 5px; }");
                html.AppendLine("</style>");
                html.AppendLine("</head><body>");
                
                html.AppendLine("<h1>Attock Mobiles Rwp - SUPPLIER DUE REPORT</h1>");
                html.AppendLine($"<p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
                
                // Summary
                html.AppendLine("<div class='summary'>");
                html.AppendLine("<h2>Summary</h2>");
                var totalDue = _supplierDueItems.Sum(x => x.TotalDue);
                html.AppendLine($"<p><strong>Total Suppliers with Due:</strong> {_supplierDueItems.Count}</p>");
                html.AppendLine($"<p><strong>Total Due Amount:</strong> {totalDue:F2}</p>");
                html.AppendLine("</div>");
                
                // Table
                html.AppendLine("<table>");
                html.AppendLine("<tr><th>Supplier Name</th><th>Phone</th><th>Email</th><th>Total Purchases</th><th>Total Paid</th><th>Total Due</th></tr>");
                
                foreach (var item in _supplierDueItems)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{item.SupplierName}</td>");
                    html.AppendLine($"<td>{item.Phone}</td>");
                    html.AppendLine($"<td>N/A</td>");
                    html.AppendLine($"<td>{item.TotalPurchases:F2}</td>");
                    html.AppendLine($"<td>{item.TotalPaid:F2}</td>");
                    html.AppendLine($"<td>{item.TotalDue:F2}</td>");
                    html.AppendLine("</tr>");
                }
                
                html.AppendLine("</table>");
                html.AppendLine("</body></html>");
                
                return html.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating HTML report: {ex.Message}");
            }
        }

        private void SupplierDueReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
            LoadSupplierDueData(); // Automatically load data when form opens
        }
    }

    public class SupplierDueReportItem
    {
        public int SupplierID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Phone { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
    }
}

