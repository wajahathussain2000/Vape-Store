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
using Vape_Store.DataAccess;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;
using Vape_Store.Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Vape_Store
{
    public partial class DailySaleReportForm : Form
    {
        private SaleRepository _saleRepository;
        private ReportingService _reportingService;
        
        private List<DailySaleReportItem> _dailySaleReportItems;
        private List<DailySaleReportItem> _originalDailySaleReportItems; // Store original unfiltered data

        public DailySaleReportForm()
        {
            InitializeComponent();
            _saleRepository = new SaleRepository();
            _reportingService = new ReportingService();
            
            _dailySaleReportItems = new List<DailySaleReportItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnGenerateReport.Click += BtnGenerateReport_Click;
            btnExportPDF.Click += BtnExportPDF_Click;
            btnViewHTML.Click += BtnViewHTML_Click;
            btnPrint.Click += BtnPrint_Click;
            btnClear.Click += BtnClear_Click;
            btnClose.Click += BtnClose_Click;
            
            // Date event handler
            dtpDate.ValueChanged += DtpDate_ValueChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += DailySaleReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvDailySaleReport.AutoGenerateColumns = false;
                dgvDailySaleReport.AllowUserToAddRows = false;
                dgvDailySaleReport.AllowUserToDeleteRows = false;
                dgvDailySaleReport.ReadOnly = true;
                dgvDailySaleReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvDailySaleReport.MultiSelect = false;
                dgvDailySaleReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvDailySaleReport.AllowUserToResizeColumns = true;
                dgvDailySaleReport.AllowUserToResizeRows = false;
                dgvDailySaleReport.RowHeadersVisible = false;
                dgvDailySaleReport.EnableHeadersVisualStyles = false;
                dgvDailySaleReport.GridColor = Color.FromArgb(236, 240, 241);
                dgvDailySaleReport.BorderStyle = BorderStyle.None;
                
                // Set header styling
                dgvDailySaleReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
                dgvDailySaleReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvDailySaleReport.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                dgvDailySaleReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                // Set row styling
                dgvDailySaleReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                dgvDailySaleReport.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
                dgvDailySaleReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
                dgvDailySaleReport.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvDailySaleReport.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgvDailySaleReport.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgvDailySaleReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                dgvDailySaleReport.ColumnHeadersHeight = 35;

                // Define columns
                dgvDailySaleReport.Columns.Clear();
                
                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "InvoiceNumber",
                    HeaderText = "Invoice",
                    DataPropertyName = "InvoiceNumber",
                    Width = 150,
                    MinimumWidth = 120
                });
                
                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Product Name",
                    DataPropertyName = "ProductName",
                    Width = 250,
                    MinimumWidth = 200
                });
                
                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PurchasePrice",
                    HeaderText = "Purchase Price",
                    DataPropertyName = "PurchasePrice",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SalePrice",
                    HeaderText = "Sale Price",
                    DataPropertyName = "SalePrice",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Qty",
                    DataPropertyName = "Quantity",
                    Width = 80,
                    MinimumWidth = 60,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
                });
                
                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Profit",
                    HeaderText = "Profit",
                    DataPropertyName = "Profit",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", ForeColor = Color.Green }
                });
                
                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProfitPercentage",
                    HeaderText = "Profit %",
                    DataPropertyName = "ProfitPercentage",
                    Width = 100,
                    MinimumWidth = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            try
            {
                // Set default date (today)
                dtpDate.Value = DateTime.Now.Date;
                
                // Clear search
                txtSearch.Clear();
                
                // Clear report data
                _dailySaleReportItems.Clear();
                dgvDailySaleReport.DataSource = null;
                
                // Clear summary labels
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error setting initial state: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadDailySaleData()
        {
            try
            {
                var selectedDate = dtpDate.Value.Date;
                var fromDate = selectedDate;
                var toDate = selectedDate.AddDays(1).AddSeconds(-1); // End of selected day
                
                _dailySaleReportItems.Clear();
                
                // Get daily sale report data with purchase price
                // We need to get the actual purchase price from PurchaseItems, not from Products table
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT 
                            s.InvoiceNumber,
                            p.ProductName,
                            ISNULL(
                                (SELECT TOP 1 pi.UnitPrice 
                                 FROM PurchaseItems pi
                                 INNER JOIN Purchases pur ON pi.PurchaseID = pur.PurchaseID
                                 WHERE pi.ProductID = si.ProductID 
                                   AND pur.PurchaseDate <= s.SaleDate
                                 ORDER BY pur.PurchaseDate DESC, pur.PurchaseID DESC),
                                ISNULL(p.PurchasePrice, ISNULL(p.CostPrice, 0))
                            ) as PurchasePrice,
                            si.UnitPrice as SalePrice,
                            si.Quantity,
                            (si.UnitPrice - ISNULL(
                                (SELECT TOP 1 pi.UnitPrice 
                                 FROM PurchaseItems pi
                                 INNER JOIN Purchases pur ON pi.PurchaseID = pur.PurchaseID
                                 WHERE pi.ProductID = si.ProductID 
                                   AND pur.PurchaseDate <= s.SaleDate
                                 ORDER BY pur.PurchaseDate DESC, pur.PurchaseID DESC),
                                ISNULL(p.PurchasePrice, ISNULL(p.CostPrice, 0))
                            )) * si.Quantity as Profit,
                            s.SaleDate
                        FROM Sales s
                        INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                        INNER JOIN Products p ON si.ProductID = p.ProductID
                        WHERE s.SaleDate >= @FromDate AND s.SaleDate <= @ToDate
                        ORDER BY s.SaleDate DESC, s.InvoiceNumber, p.ProductName";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FromDate", fromDate);
                        command.Parameters.AddWithValue("@ToDate", toDate);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var purchasePrice = Convert.ToDecimal(reader["PurchasePrice"]);
                                var salePrice = Convert.ToDecimal(reader["SalePrice"]);
                                var quantity = Convert.ToInt32(reader["Quantity"]);
                                
                                // Calculate profit: (SalePrice - PurchasePrice) × Quantity
                                var profit = (salePrice - purchasePrice) * quantity;
                                
                                // Calculate profit percentage: ((SalePrice - PurchasePrice) / PurchasePrice) × 100
                                // Handle division by zero: if PurchasePrice is 0, profit % is 0
                                decimal profitPercentage = 0;
                                if (purchasePrice > 0)
                                {
                                    profitPercentage = ((salePrice - purchasePrice) / purchasePrice) * 100;
                                }
                                
                                _dailySaleReportItems.Add(new DailySaleReportItem
                                {
                                    InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                    ProductName = reader["ProductName"].ToString(),
                                    PurchasePrice = purchasePrice,
                                    SalePrice = salePrice,
                                    Quantity = quantity,
                                    Profit = profit,
                                    ProfitPercentage = profitPercentage,
                                    SaleDate = Convert.ToDateTime(reader["SaleDate"])
                                });
                            }
                        }
                    }
                }
                
                if (_dailySaleReportItems.Count == 0)
                {
                    ShowMessage($"No sales found for {selectedDate:yyyy-MM-dd}.", "No Data Found", MessageBoxIcon.Information);
                    return;
                }
                
                // Store original unfiltered data
                _originalDailySaleReportItems = new List<DailySaleReportItem>(_dailySaleReportItems);
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading daily sale data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                // Use original unfiltered data as base
                var filteredItems = _originalDailySaleReportItems?.AsEnumerable() ?? _dailySaleReportItems.AsEnumerable();
                
                // Search filter - search through multiple fields
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        (item.InvoiceNumber?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ProductName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.SaleDate.ToString("yyyy-MM-dd").Contains(searchTerm)) ||
                        (item.SaleDate.ToString("MM/dd/yyyy").Contains(searchTerm)) ||
                        (item.PurchasePrice.ToString("F2").Contains(searchTerm)) ||
                        (item.SalePrice.ToString("F2").Contains(searchTerm)) ||
                        (item.Profit.ToString("F2").Contains(searchTerm)) ||
                        (item.ProfitPercentage.ToString("F2").Contains(searchTerm)) ||
                        (item.Quantity.ToString().Contains(searchTerm)));
                }
                
                // Update filtered list (preserve original)
                _dailySaleReportItems = filteredItems.ToList();
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
                bool hasSearchFilter = !string.IsNullOrWhiteSpace(txtSearch.Text);
                bool hasDataButNoResults = hasSearchFilter && _originalDailySaleReportItems != null && 
                                          _originalDailySaleReportItems.Count > 0 && _dailySaleReportItems.Count == 0;
                
                dgvDailySaleReport.DataSource = null;
                
                if (hasDataButNoResults)
                {
                    // Show empty grid with message - DataGridView doesn't support custom messages easily
                    // So we'll show empty and let UpdateSummaryLabels handle the message
                    dgvDailySaleReport.DataSource = new List<DailySaleReportItem>();
                }
                else
                {
                    dgvDailySaleReport.DataSource = _dailySaleReportItems;
                }
                
                // Format decimal columns
                if (dgvDailySaleReport.Columns["PurchasePrice"] != null)
                    dgvDailySaleReport.Columns["PurchasePrice"].DefaultCellStyle.Format = "F2";
                if (dgvDailySaleReport.Columns["SalePrice"] != null)
                    dgvDailySaleReport.Columns["SalePrice"].DefaultCellStyle.Format = "F2";
                if (dgvDailySaleReport.Columns["Profit"] != null)
                {
                    dgvDailySaleReport.Columns["Profit"].DefaultCellStyle.Format = "F2";
                    // Color profit column - green for positive, red for negative
                    foreach (DataGridViewRow row in dgvDailySaleReport.Rows)
                    {
                        if (row.DataBoundItem is DailySaleReportItem item)
                        {
                            if (item.Profit < 0)
                                row.Cells["Profit"].Style.ForeColor = Color.Red;
                            else if (item.Profit > 0)
                                row.Cells["Profit"].Style.ForeColor = Color.Green;
                        }
                    }
                }
                
                // Format date column if exists
                if (dgvDailySaleReport.Columns["SaleDate"] != null)
                    dgvDailySaleReport.Columns["SaleDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
                
                // Add total row (only if we have results)
                if (!hasDataButNoResults && _dailySaleReportItems.Count > 0)
                {
                    var totalRow = dgvDailySaleReport.Rows[dgvDailySaleReport.Rows.Count - 1];
                    totalRow.DefaultCellStyle.BackColor = Color.LightBlue;
                    totalRow.DefaultCellStyle.Font = new System.Drawing.Font(dgvDailySaleReport.DefaultCellStyle.Font, FontStyle.Bold);
                }
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
                bool hasSearchFilter = !string.IsNullOrWhiteSpace(txtSearch.Text);
                bool hasDataButNoResults = hasSearchFilter && _originalDailySaleReportItems != null && 
                                          _originalDailySaleReportItems.Count > 0 && _dailySaleReportItems.Count == 0;
                
                if (hasDataButNoResults)
                {
                    // Show "No results" message in summary
                    lblTotalProfit.Text = "Total Profit: No results found";
                    lblTotalQuantity.Text = "Total Quantity: No results found";
                    lblTotalSales.Text = $"Total Sales: No results found for '{txtSearch.Text}'";
                    lblTotalCost.Text = "Total Cost: No results found";
                    lblUniqueProducts.Text = "Unique Products: 0";
                    lblUniqueInvoices.Text = "Unique Invoices: 0";
                    lblTotalProfit.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    var totalProfit = _dailySaleReportItems.Sum(item => item.Profit);
                    var totalQuantity = _dailySaleReportItems.Sum(item => item.Quantity);
                    var totalSales = _dailySaleReportItems.Sum(item => item.SalePrice * item.Quantity);
                    var totalCost = _dailySaleReportItems.Sum(item => item.PurchasePrice * item.Quantity);
                    var uniqueProducts = _dailySaleReportItems.Select(item => item.ProductName).Distinct().Count();
                    var uniqueInvoices = _dailySaleReportItems.Select(item => item.InvoiceNumber).Distinct().Count();
                    
                    lblTotalProfit.Text = $"Total Profit: {totalProfit:F2}";
                    lblTotalQuantity.Text = $"Total Quantity: {totalQuantity}";
                    lblTotalSales.Text = $"Total Sales: {totalSales:F2}";
                    lblTotalCost.Text = $"Total Cost: {totalCost:F2}";
                    lblUniqueProducts.Text = $"Unique Products: {uniqueProducts}";
                    lblUniqueInvoices.Text = $"Unique Invoices: {uniqueInvoices}";
                    
                    // Color total profit label
                    if (totalProfit < 0)
                        lblTotalProfit.ForeColor = Color.Red;
                    else if (totalProfit > 0)
                        lblTotalProfit.ForeColor = Color.Green;
                    else
                        lblTotalProfit.ForeColor = SystemColors.ControlText;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating summary labels: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadDailySaleData();
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dailySaleReportItems == null || _dailySaleReportItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate the report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"DailySaleReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToPDF(saveFileDialog.FileName);
                    ShowMessage("Report exported to PDF successfully!", "Success", MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting to PDF: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnViewHTML_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dailySaleReportItems == null || _dailySaleReportItems.Count == 0)
                {
                    ShowMessage("No data to view. Please generate the report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                var htmlContent = GenerateHTMLReport();
                var htmlViewer = new HTMLReportViewerForm();
                htmlViewer.LoadReport(htmlContent);
                htmlViewer.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating HTML report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dailySaleReportItems == null || _dailySaleReportItems.Count == 0)
                {
                    ShowMessage("No data to print. Please generate the report first.", "No Data", MessageBoxIcon.Warning);
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

        private void BtnClear_Click(object sender, EventArgs e)
        {
            SetInitialState();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DtpDate_ValueChanged(object sender, EventArgs e)
        {
            // No validation needed for single date picker
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void DailySaleReportForm_Load(object sender, EventArgs e)
        {
            // Form loaded
        }

        private void ExportToPDF(string filePath)
        {
            try
            {
                Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(baseFont, 18, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font smallFont = new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL);

                Paragraph title = new Paragraph("DAILY SALE REPORT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 10f;
                document.Add(title);

                Paragraph dateInfo = new Paragraph($"Date: {dtpDate.Value:yyyy-MM-dd}", normalFont);
                dateInfo.Alignment = Element.ALIGN_CENTER;
                dateInfo.SpacingAfter = 20f;
                document.Add(dateInfo);

                // Summary
                var totalProfit = _dailySaleReportItems.Sum(item => item.Profit);
                var totalQuantity = _dailySaleReportItems.Sum(item => item.Quantity);
                var totalSales = _dailySaleReportItems.Sum(item => item.SalePrice * item.Quantity);
                var totalCost = _dailySaleReportItems.Sum(item => item.PurchasePrice * item.Quantity);

                Paragraph summaryTitle = new Paragraph("SUMMARY", headerFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                PdfPTable summaryTable = new PdfPTable(2);
                summaryTable.WidthPercentage = 50;
                summaryTable.SetWidths(new float[] { 1, 1 });

                PdfPCell cell1 = new PdfPCell(new Phrase("Total Sales:", normalFont));
                cell1.Border = 0;
                summaryTable.AddCell(cell1);
                PdfPCell cell2 = new PdfPCell(new Phrase($"{totalSales:F2}", normalFont));
                cell2.Border = 0;
                cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.AddCell(cell2);
                PdfPCell cell3 = new PdfPCell(new Phrase("Total Cost:", normalFont));
                cell3.Border = 0;
                summaryTable.AddCell(cell3);
                PdfPCell cell4 = new PdfPCell(new Phrase($"{totalCost:F2}", normalFont));
                cell4.Border = 0;
                cell4.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.AddCell(cell4);
                PdfPCell cell5 = new PdfPCell(new Phrase("Total Profit:", normalFont));
                cell5.Border = 0;
                summaryTable.AddCell(cell5);
                PdfPCell cell6 = new PdfPCell(new Phrase($"{totalProfit:F2}", normalFont));
                cell6.Border = 0;
                cell6.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.AddCell(cell6);
                PdfPCell cell7 = new PdfPCell(new Phrase("Total Quantity:", normalFont));
                cell7.Border = 0;
                summaryTable.AddCell(cell7);
                PdfPCell cell8 = new PdfPCell(new Phrase(totalQuantity.ToString(), normalFont));
                cell8.Border = 0;
                cell8.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.AddCell(cell8);

                summaryTable.SpacingAfter = 20f;
                document.Add(summaryTable);

                // Sales details section
                Paragraph detailsTitle = new Paragraph("SALES DETAILS", headerFont);
                detailsTitle.SpacingAfter = 10f;
                document.Add(detailsTitle);

                // Create sales table
                PdfPTable salesTable = new PdfPTable(7);
                salesTable.WidthPercentage = 100;
                salesTable.SetWidths(new float[] { 1.5f, 2.5f, 1.2f, 1.2f, 0.8f, 1.2f, 1.0f });

                // Add headers
                string[] headers = { "Invoice", "Product Name", "Purchase Price", "Sale Price", "Qty", "Profit", "Profit %" };
                foreach (string header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.Padding = 5f;
                    headerCell.Border = iTextSharp.text.Rectangle.BOX;
                    salesTable.AddCell(headerCell);
                }

                // Add data rows
                foreach (var item in _dailySaleReportItems)
                {
                    PdfPCell cellInvoice = new PdfPCell(new Phrase(item.InvoiceNumber, smallFont));
                    cellInvoice.Padding = 3f;
                    salesTable.AddCell(cellInvoice);
                    
                    PdfPCell cellProduct = new PdfPCell(new Phrase(item.ProductName.Length > 30 ? item.ProductName.Substring(0, 30) + "..." : item.ProductName, smallFont));
                    cellProduct.Padding = 3f;
                    salesTable.AddCell(cellProduct);
                    
                    PdfPCell cellPurchase = new PdfPCell(new Phrase($"{item.PurchasePrice:F2}", smallFont));
                    cellPurchase.Padding = 3f;
                    cellPurchase.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(cellPurchase);
                    
                    PdfPCell cellSale = new PdfPCell(new Phrase($"{item.SalePrice:F2}", smallFont));
                    cellSale.Padding = 3f;
                    cellSale.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(cellSale);
                    
                    PdfPCell cellQty = new PdfPCell(new Phrase(item.Quantity.ToString(), smallFont));
                    cellQty.Padding = 3f;
                    cellQty.HorizontalAlignment = Element.ALIGN_CENTER;
                    salesTable.AddCell(cellQty);
                    
                    PdfPCell cellProfit = new PdfPCell(new Phrase($"{item.Profit:F2}", smallFont));
                    cellProfit.Padding = 3f;
                    cellProfit.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(cellProfit);
                    
                    PdfPCell cellProfitPct = new PdfPCell(new Phrase($"{item.ProfitPercentage:F2}%", smallFont));
                    cellProfitPct.Padding = 3f;
                    cellProfitPct.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(cellProfitPct);
                }

                // Add total row
                if (_dailySaleReportItems.Count > 0)
                {
                    PdfPCell totalCell1 = new PdfPCell(new Phrase("TOTAL", headerFont));
                    totalCell1.Padding = 3f;
                    totalCell1.Colspan = 2;
                    totalCell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    salesTable.AddCell(totalCell1);
                    
                    PdfPCell totalCell2 = new PdfPCell(new Phrase($"{totalCost:F2}", headerFont));
                    totalCell2.Padding = 3f;
                    totalCell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(totalCell2);
                    
                    PdfPCell totalCell3 = new PdfPCell(new Phrase($"{totalSales:F2}", headerFont));
                    totalCell3.Padding = 3f;
                    totalCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(totalCell3);
                    
                    PdfPCell totalCell4 = new PdfPCell(new Phrase(totalQuantity.ToString(), headerFont));
                    totalCell4.Padding = 3f;
                    totalCell4.HorizontalAlignment = Element.ALIGN_CENTER;
                    salesTable.AddCell(totalCell4);
                    
                    PdfPCell totalCell5 = new PdfPCell(new Phrase($"{totalProfit:F2}", headerFont));
                    totalCell5.Padding = 3f;
                    totalCell5.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(totalCell5);
                    
                    // Calculate average profit percentage
                    var avgProfitPct = totalCost > 0 ? ((totalSales - totalCost) / totalCost) * 100 : 0;
                    PdfPCell totalCell6 = new PdfPCell(new Phrase($"{avgProfitPct:F2}%", headerFont));
                    totalCell6.Padding = 3f;
                    totalCell6.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(totalCell6);
                }

                document.Add(salesTable);
                document.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to PDF: {ex.Message}", ex);
            }
        }

        private string GenerateHTMLReport()
        {
            try
            {
                var html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html>");
                html.AppendLine("<head>");
                html.AppendLine("<meta charset='utf-8'>");
                html.AppendLine("<title>Daily Sale Report</title>");
                html.AppendLine("<style>");
                html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
                html.AppendLine("h1 { color: #3498db; text-align: center; }");
                html.AppendLine("h2 { color: #2c3e50; border-bottom: 2px solid #3498db; padding-bottom: 5px; }");
                html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
                html.AppendLine("th { background-color: #3498db; color: white; padding: 10px; text-align: left; }");
                html.AppendLine("td { padding: 8px; border-bottom: 1px solid #ddd; }");
                html.AppendLine("tr:nth-child(even) { background-color: #f2f2f2; }");
                html.AppendLine(".summary { background-color: #ecf0f1; padding: 15px; margin: 20px 0; border-radius: 5px; }");
                html.AppendLine(".summary-item { margin: 5px 0; font-weight: bold; }");
                html.AppendLine(".profit-positive { color: green; }");
                html.AppendLine(".profit-negative { color: red; }");
                html.AppendLine("</style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");
                html.AppendLine("<h1>DAILY SALE REPORT</h1>");
                html.AppendLine($"<p style='text-align: center;'>Date: {dtpDate.Value:yyyy-MM-dd}</p>");
                
                var totalProfit = _dailySaleReportItems.Sum(item => item.Profit);
                var totalQuantity = _dailySaleReportItems.Sum(item => item.Quantity);
                var totalSales = _dailySaleReportItems.Sum(item => item.SalePrice * item.Quantity);
                var totalCost = _dailySaleReportItems.Sum(item => item.PurchasePrice * item.Quantity);
                var uniqueProducts = _dailySaleReportItems.Select(item => item.ProductName).Distinct().Count();
                var uniqueInvoices = _dailySaleReportItems.Select(item => item.InvoiceNumber).Distinct().Count();
                
                html.AppendLine("<div class='summary'>");
                html.AppendLine("<h2>Summary</h2>");
                html.AppendLine($"<div class='summary-item'>Total Sales: {totalSales:F2}</div>");
                html.AppendLine($"<div class='summary-item'>Total Cost: {totalCost:F2}</div>");
                html.AppendLine($"<div class='summary-item {(totalProfit >= 0 ? "profit-positive" : "profit-negative")}'>Total Profit: {totalProfit:F2}</div>");
                html.AppendLine($"<div class='summary-item'>Total Quantity: {totalQuantity}</div>");
                html.AppendLine($"<div class='summary-item'>Unique Products: {uniqueProducts}</div>");
                html.AppendLine($"<div class='summary-item'>Unique Invoices: {uniqueInvoices}</div>");
                html.AppendLine("</div>");
                
                // Data table
                html.AppendLine("<h2>Sales Details</h2>");
                html.AppendLine("<table>");
                html.AppendLine("<tr>");
                html.AppendLine("<th>Invoice</th>");
                html.AppendLine("<th>Product Name</th>");
                html.AppendLine("<th>Purchase Price</th>");
                html.AppendLine("<th>Sale Price</th>");
                html.AppendLine("<th>Qty</th>");
                html.AppendLine("<th>Profit</th>");
                html.AppendLine("<th>Profit %</th>");
                html.AppendLine("</tr>");
                
                foreach (var item in _dailySaleReportItems)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{item.InvoiceNumber}</td>");
                    html.AppendLine($"<td>{item.ProductName}</td>");
                    html.AppendLine($"<td>{item.PurchasePrice:F2}</td>");
                    html.AppendLine($"<td>{item.SalePrice:F2}</td>");
                    html.AppendLine($"<td>{item.Quantity}</td>");
                    html.AppendLine($"<td class='{(item.Profit >= 0 ? "profit-positive" : "profit-negative")}'>{item.Profit:F2}</td>");
                    html.AppendLine($"<td class='{(item.ProfitPercentage >= 0 ? "profit-positive" : "profit-negative")}'>{item.ProfitPercentage:F2}%</td>");
                    html.AppendLine("</tr>");
                }
                
                // Total row
                var avgProfitPct = totalCost > 0 ? ((totalSales - totalCost) / totalCost) * 100 : 0;
                html.AppendLine("<tr style='background-color: #3498db; color: white; font-weight: bold;'>");
                html.AppendLine("<td colspan='2'>TOTAL</td>");
                html.AppendLine($"<td>{totalCost:F2}</td>");
                html.AppendLine($"<td>{totalSales:F2}</td>");
                html.AppendLine($"<td>{totalQuantity}</td>");
                html.AppendLine($"<td class='{(totalProfit >= 0 ? "profit-positive" : "profit-negative")}'>{totalProfit:F2}</td>");
                html.AppendLine($"<td class='{(avgProfitPct >= 0 ? "profit-positive" : "profit-negative")}'>{avgProfitPct:F2}%</td>");
                html.AppendLine("</tr>");
                
                html.AppendLine("</table>");
                html.AppendLine("</body>");
                html.AppendLine("</html>");
                
                return html.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating HTML report: {ex.Message}", ex);
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }
    }

    public class DailySaleReportItem
    {
        public string InvoiceNumber { get; set; }
        public string ProductName { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitPercentage { get; set; }
        public DateTime SaleDate { get; set; }
    }
}

