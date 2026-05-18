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
    public partial class DateRangeReportForm : Form
    {
        private SaleRepository _saleRepository;
        private ReportingService _reportingService;
        
        private List<DailySaleReportItem> _dailySaleReportItems;
        private List<DailySaleReportItem> _originalDailySaleReportItems; // Store original unfiltered data

        public DateRangeReportForm()
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
            
            // Date event handlers
            dtpStartDate.ValueChanged += DtpDate_ValueChanged;
            dtpEndDate.ValueChanged += DtpDate_ValueChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += DateRangeReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvDateRangeSaleReport.AutoGenerateColumns = false;
                dgvDateRangeSaleReport.AllowUserToAddRows = false;
                dgvDateRangeSaleReport.AllowUserToDeleteRows = false;
                dgvDateRangeSaleReport.ReadOnly = true;
                dgvDateRangeSaleReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvDateRangeSaleReport.MultiSelect = false;
                dgvDateRangeSaleReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvDateRangeSaleReport.AllowUserToResizeColumns = true;
                dgvDateRangeSaleReport.AllowUserToResizeRows = false;
                dgvDateRangeSaleReport.RowHeadersVisible = false;
                dgvDateRangeSaleReport.EnableHeadersVisualStyles = false;
                dgvDateRangeSaleReport.GridColor = Color.FromArgb(236, 240, 241);
                dgvDateRangeSaleReport.BorderStyle = BorderStyle.None;
                
                // Set header styling
                dgvDateRangeSaleReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
                dgvDateRangeSaleReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvDateRangeSaleReport.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                dgvDateRangeSaleReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                // Set row styling
                dgvDateRangeSaleReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                dgvDateRangeSaleReport.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
                dgvDateRangeSaleReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
                dgvDateRangeSaleReport.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvDateRangeSaleReport.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgvDateRangeSaleReport.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgvDateRangeSaleReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                dgvDateRangeSaleReport.ColumnHeadersHeight = 35;

                // Define columns
                dgvDateRangeSaleReport.Columns.Clear();
                
                dgvDateRangeSaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "InvoiceNumber",
                    HeaderText = "Invoice",
                    DataPropertyName = "InvoiceNumber",
                    Width = 150,
                    MinimumWidth = 120
                });
                
                dgvDateRangeSaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Products",
                    DataPropertyName = "ProductName",
                    Width = 320,
                    MinimumWidth = 220
                });
                
                dgvDateRangeSaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PurchasePrice",
                    HeaderText = "Purchase Total",
                    DataPropertyName = "PurchasePrice",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });

                dgvDateRangeSaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SubTotal",
                    HeaderText = "Sub Total",
                    DataPropertyName = "SubTotal",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvDateRangeSaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "DiscountAmount",
                    HeaderText = "Discount",
                    DataPropertyName = "DiscountAmount",
                    Width = 110,
                    MinimumWidth = 90,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });

                dgvDateRangeSaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TaxAmount",
                    HeaderText = "Tax",
                    DataPropertyName = "TaxAmount",
                    Width = 100,
                    MinimumWidth = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvDateRangeSaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SalePrice",
                    HeaderText = "Grand Total",
                    DataPropertyName = "SalePrice",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });

                dgvDateRangeSaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Qty",
                    DataPropertyName = "Quantity",
                    Width = 80,
                    MinimumWidth = 60,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
                });
                
                dgvDateRangeSaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Profit",
                    HeaderText = "Profit",
                    DataPropertyName = "Profit",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", ForeColor = Color.Green }
                });
                
                dgvDateRangeSaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProfitPercentage",
                    HeaderText = "Profit %",
                    DataPropertyName = "ProfitPercentage",
                    Width = 100,
                    MinimumWidth = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvDateRangeSaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SaleDate",
                    HeaderText = "Date",
                    DataPropertyName = "SaleDate",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" }
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
                // Set default date range (this month)
                dtpStartDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtpEndDate.Value = DateTime.Now.Date;
                
                // Clear search
                txtSearch.Clear();
                
                // Clear report data
                _dailySaleReportItems.Clear();
                dgvDateRangeSaleReport.DataSource = null;
                
                // Clear summary labels
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error setting initial state: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadDateRangeSaleData()
        {
            try
            {
                var fromDate = dtpStartDate.Value.Date;
                var toDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1); // End of selected end date
                
                _dailySaleReportItems.Clear();
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT 
                            s.SaleID,
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
                            si.SubTotal as ItemSubTotal,
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
                            s.DiscountAmount,
                            s.DiscountPercent,
                            s.TaxAmount as SaleTaxAmount,
                            s.SubTotal as SaleSubTotal,
                            s.TotalAmount as SaleTotalAmount,
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
                        
                        var saleItems = new Dictionary<string, List<SaleItemData>>();
                        var saleInfo = new Dictionary<string, SaleInfo>();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var invoiceNumber = reader["InvoiceNumber"].ToString();
                                var productName = reader["ProductName"].ToString();
                                var saleDate = Convert.ToDateTime(reader["SaleDate"]);

                                var purchasePrice = Convert.ToDecimal(reader["PurchasePrice"]);
                                var salePrice = Convert.ToDecimal(reader["SalePrice"]);
                                var quantity = Convert.ToInt32(reader["Quantity"]);
                                var itemSubTotal = reader["ItemSubTotal"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["ItemSubTotal"])
                                    : salePrice * quantity;
                                var saleDiscount = reader["DiscountAmount"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["DiscountAmount"])
                                    : 0;
                                var saleSubTotal = reader["SaleSubTotal"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["SaleSubTotal"])
                                    : salePrice * quantity;
                                var saleTaxAmount = reader["SaleTaxAmount"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["SaleTaxAmount"])
                                    : 0;
                                var saleTotalAmount = reader["SaleTotalAmount"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["SaleTotalAmount"])
                                    : saleSubTotal - saleDiscount + saleTaxAmount;

                                if (!saleInfo.ContainsKey(invoiceNumber))
                                {
                                    saleInfo[invoiceNumber] = new SaleInfo
                                    {
                                        SaleSubTotal = saleSubTotal,
                                        DiscountAmount = saleDiscount,
                                        TaxAmount = saleTaxAmount,
                                        TotalAmount = saleTotalAmount,
                                        SaleDate = saleDate
                                    };
                                }

                                if (!saleItems.ContainsKey(invoiceNumber))
                                {
                                    saleItems[invoiceNumber] = new List<SaleItemData>();
                                }

                                saleItems[invoiceNumber].Add(new SaleItemData
                                {
                                    ProductName = productName,
                                    PurchasePrice = purchasePrice,
                                    SalePrice = salePrice,
                                    Quantity = quantity,
                                    ItemSubTotal = itemSubTotal
                                });
                            }
                        }

                        var invoiceAggregates = new Dictionary<string, DailySaleReportItem>();
                        var invoiceOrder = new List<string>();

                        foreach (var kvp in saleItems)
                        {
                            var invoiceNumber = kvp.Key;
                            var items = kvp.Value;
                            var sale = saleInfo[invoiceNumber];

                            decimal totalPurchasePrice = 0;
                            decimal totalProfit = 0;
                            int totalQuantity = 0;
                            var productNames = new List<string>();

                            foreach (var item in items)
                            {
                                var itemDiscountShare = sale.SaleSubTotal > 0 
                                    ? (item.ItemSubTotal / sale.SaleSubTotal) * sale.DiscountAmount 
                                    : 0;
                                var itemTaxShare = sale.SaleSubTotal > 0 
                                    ? (item.ItemSubTotal / sale.SaleSubTotal) * sale.TaxAmount 
                                    : 0;
                                var actualRevenue = item.ItemSubTotal - itemDiscountShare - itemTaxShare;
                                var itemCost = item.PurchasePrice * item.Quantity;
                                var itemProfit = actualRevenue - itemCost;

                                totalPurchasePrice += itemCost;
                                totalProfit += itemProfit;
                                totalQuantity += item.Quantity;
                                productNames.Add($"{item.ProductName} (x{item.Quantity})");
                            }

                            invoiceAggregates[invoiceNumber] = new DailySaleReportItem
                            {
                                InvoiceNumber = invoiceNumber,
                                ProductName = string.Join(", ", productNames),
                                PurchasePrice = totalPurchasePrice,
                                Quantity = totalQuantity,
                                Profit = totalProfit,
                                DiscountAmount = sale.DiscountAmount,
                                SubTotal = sale.SaleSubTotal,
                                TaxAmount = sale.TaxAmount,
                                SalePrice = sale.TotalAmount,
                                SaleDate = sale.SaleDate
                            };
                            invoiceOrder.Add(invoiceNumber);
                        }
                        
                        _dailySaleReportItems = new List<DailySaleReportItem>();
                        foreach (var invoiceNumber in invoiceOrder)
                        {
                            var invoiceItem = invoiceAggregates[invoiceNumber];
                            invoiceItem.ProfitPercentage = invoiceItem.PurchasePrice > 0
                                ? (invoiceItem.Profit / invoiceItem.PurchasePrice) * 100
                                : 0;
                            _dailySaleReportItems.Add(invoiceItem);
                        }
                    }
                }
                
                if (_dailySaleReportItems.Count == 0)
                {
                    ShowMessage($"No sales found for the selected date range.", "No Data Found", MessageBoxIcon.Information);
                }
                
                _originalDailySaleReportItems = new List<DailySaleReportItem>(_dailySaleReportItems);
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading report data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = _originalDailySaleReportItems?.AsEnumerable() ?? _dailySaleReportItems.AsEnumerable();
                
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        (item.InvoiceNumber?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ProductName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.SaleDate.ToString("yyyy-MM-dd").Contains(searchTerm)) ||
                        (item.SaleDate.ToString("MM/dd/yyyy").Contains(searchTerm)) ||
                        (item.PurchasePrice.ToString("F2").Contains(searchTerm)) ||
                        (item.SubTotal.ToString("F2").Contains(searchTerm)) ||
                        (item.SalePrice.ToString("F2").Contains(searchTerm)) ||
                        (item.TaxAmount.ToString("F2").Contains(searchTerm)) ||
                        (item.Profit.ToString("F2").Contains(searchTerm)) ||
                        (item.DiscountAmount.ToString("F2").Contains(searchTerm)) ||
                        (item.ProfitPercentage.ToString("F2").Contains(searchTerm)) ||
                        (item.Quantity.ToString().Contains(searchTerm)));
                }
                
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
                
                dgvDateRangeSaleReport.DataSource = null;
                
                if (hasDataButNoResults)
                {
                    dgvDateRangeSaleReport.DataSource = new List<DailySaleReportItem>();
                }
                else
                {
                    dgvDateRangeSaleReport.DataSource = _dailySaleReportItems;
                }
                
                if (dgvDateRangeSaleReport.Columns["Profit"] != null)
                {
                    foreach (DataGridViewRow row in dgvDateRangeSaleReport.Rows)
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
                    var totalSubTotal = _dailySaleReportItems.Sum(item => item.SubTotal);
                    var totalDiscount = _dailySaleReportItems.Sum(item => item.DiscountAmount);
                    var totalTax = _dailySaleReportItems.Sum(item => item.TaxAmount);
                    var totalSales = _dailySaleReportItems.Sum(item => item.SalePrice);
                    var totalCost = _dailySaleReportItems.Sum(item => item.PurchasePrice);
                    var uniqueProducts = CalculateUniqueProducts(_dailySaleReportItems);
                    var uniqueInvoices = _dailySaleReportItems.Select(item => item.InvoiceNumber).Distinct().Count();
                    
                    lblTotalProfit.Text = $"Total Profit: {totalProfit:F2}";
                    lblTotalQuantity.Text = $"Total Quantity: {totalQuantity}";
                    lblTotalSales.Text = $"Subtotal: {totalSubTotal:F2} | Discount: {totalDiscount:F2} | Tax: {totalTax:F2} | Total: {totalSales:F2}";
                    lblTotalCost.Text = $"Total Cost: {totalCost:F2}";
                    lblUniqueProducts.Text = $"Unique Products: {uniqueProducts}";
                    lblUniqueInvoices.Text = $"Unique Invoices: {uniqueInvoices}";
                    
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
            LoadDateRangeSaleData();
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
                saveFileDialog.FileName = $"DateRangeSaleReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

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
            // Optional: Auto-generate report when dates change
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void DateRangeReportForm_Load(object sender, EventArgs e)
        {
        }

        private void ExportToPDF(string filePath)
        {
            try
            {
                Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(baseFont, 9, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font smallFont = new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL);

                Paragraph title = new Paragraph("DATE RANGE SALE REPORT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 10f;
                document.Add(title);

                Paragraph dateInfo = new Paragraph($"From: {dtpStartDate.Value:yyyy-MM-dd} To: {dtpEndDate.Value:yyyy-MM-dd}", normalFont);
                dateInfo.Alignment = Element.ALIGN_CENTER;
                dateInfo.SpacingAfter = 20f;
                document.Add(dateInfo);

                var totalProfit = _dailySaleReportItems.Sum(item => item.Profit);
                var totalQuantity = _dailySaleReportItems.Sum(item => item.Quantity);
                var totalSubTotal = _dailySaleReportItems.Sum(item => item.SubTotal);
                var totalDiscount = _dailySaleReportItems.Sum(item => item.DiscountAmount);
                var totalTax = _dailySaleReportItems.Sum(item => item.TaxAmount);
                var totalSales = _dailySaleReportItems.Sum(item => item.SalePrice);
                var totalCost = _dailySaleReportItems.Sum(item => item.PurchasePrice);

                PdfPTable summaryTable = new PdfPTable(4);
                summaryTable.WidthPercentage = 100;
                summaryTable.SetWidths(new float[] { 1, 1, 1, 1 });

                string[] summaryHeaders = { "Subtotal", $"{totalSubTotal:F2}", "Total Cost", $"{totalCost:F2}" };
                string[] summaryHeaders2 = { "Discount", $"{totalDiscount:F2}", "Total Profit", $"{totalProfit:F2}" };
                string[] summaryHeaders3 = { "Tax", $"{totalTax:F2}", "Total Quantity", totalQuantity.ToString() };
                string[] summaryHeaders4 = { "Grand Total", $"{totalSales:F2}", "Unique Invoices", _dailySaleReportItems.Select(x => x.InvoiceNumber).Distinct().Count().ToString() };

                foreach (var h in summaryHeaders) summaryTable.AddCell(new PdfPCell(new Phrase(h, normalFont)) { Border = 0 });
                foreach (var h in summaryHeaders2) summaryTable.AddCell(new PdfPCell(new Phrase(h, normalFont)) { Border = 0 });
                foreach (var h in summaryHeaders3) summaryTable.AddCell(new PdfPCell(new Phrase(h, normalFont)) { Border = 0 });
                foreach (var h in summaryHeaders4) summaryTable.AddCell(new PdfPCell(new Phrase(h, normalFont)) { Border = 0 });

                summaryTable.SpacingAfter = 20f;
                document.Add(summaryTable);

                PdfPTable salesTable = new PdfPTable(10);
                salesTable.WidthPercentage = 100;
                salesTable.SetWidths(new float[] { 1.2f, 2.6f, 1.0f, 1.0f, 0.9f, 0.9f, 1.0f, 0.7f, 1.0f, 0.9f });

                string[] headers = { "Date", "Invoice", "Product(s)", "Purchase", "Subtotal", "Discount", "Tax", "Total", "Qty", "Profit" };
                foreach (string header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    salesTable.AddCell(headerCell);
                }

                foreach (var item in _dailySaleReportItems)
                {
                    salesTable.AddCell(new PdfPCell(new Phrase(item.SaleDate.ToString("yyyy-MM-dd"), smallFont)));
                    salesTable.AddCell(new PdfPCell(new Phrase(item.InvoiceNumber, smallFont)));
                    salesTable.AddCell(new PdfPCell(new Phrase(item.ProductName, smallFont)));
                    salesTable.AddCell(new PdfPCell(new Phrase($"{item.PurchasePrice:F2}", smallFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    salesTable.AddCell(new PdfPCell(new Phrase($"{item.SubTotal:F2}", smallFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    salesTable.AddCell(new PdfPCell(new Phrase($"{item.DiscountAmount:F2}", smallFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    salesTable.AddCell(new PdfPCell(new Phrase($"{item.TaxAmount:F2}", smallFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    salesTable.AddCell(new PdfPCell(new Phrase($"{item.SalePrice:F2}", smallFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    salesTable.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), smallFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    salesTable.AddCell(new PdfPCell(new Phrase($"{item.Profit:F2}", smallFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
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
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'><title>Date Range Sale Report</title>");
            html.AppendLine("<style>body { font-family: Arial, sans-serif; margin: 20px; } h1 { color: #3498db; text-align: center; } table { width: 100%; border-collapse: collapse; margin-top: 20px; } th { background-color: #3498db; color: white; padding: 10px; } td { padding: 8px; border-bottom: 1px solid #ddd; } tr:nth-child(even) { background-color: #f2f2f2; } .summary { background-color: #ecf0f1; padding: 15px; margin: 20px 0; border-radius: 5px; } .profit-positive { color: green; } .profit-negative { color: red; }</style></head><body>");
            html.AppendLine("<h1>DATE RANGE SALE REPORT</h1>");
            html.AppendLine($"<p style='text-align: center;'>From: {dtpStartDate.Value:yyyy-MM-dd} To: {dtpEndDate.Value:yyyy-MM-dd}</p>");
            
            var totalProfit = _dailySaleReportItems.Sum(item => item.Profit);
            var totalQuantity = _dailySaleReportItems.Sum(item => item.Quantity);
            var totalSales = _dailySaleReportItems.Sum(item => item.SalePrice);
            var totalCost = _dailySaleReportItems.Sum(item => item.PurchasePrice);
            
            html.AppendLine("<div class='summary'>");
            html.AppendLine($"<div>Total Sales: {totalSales:F2}</div>");
            html.AppendLine($"<div>Total Cost: {totalCost:F2}</div>");
            html.AppendLine($"<div class='{(totalProfit >= 0 ? "profit-positive" : "profit-negative")}'>Total Profit: {totalProfit:F2}</div>");
            html.AppendLine($"<div>Total Quantity: {totalQuantity}</div>");
            html.AppendLine("</div>");
            
            html.AppendLine("<table><tr><th>Date</th><th>Invoice</th><th>Product Name</th><th>Purchase</th><th>Total</th><th>Qty</th><th>Profit</th></tr>");
            foreach (var item in _dailySaleReportItems)
            {
                html.AppendLine($"<tr><td>{item.SaleDate:yyyy-MM-dd}</td><td>{item.InvoiceNumber}</td><td>{item.ProductName}</td><td>{item.PurchasePrice:F2}</td><td>{item.SalePrice:F2}</td><td>{item.Quantity}</td><td class='{(item.Profit >= 0 ? "profit-positive" : "profit-negative")}'>{item.Profit:F2}</td></tr>");
            }
            html.AppendLine("</table></body></html>");
            return html.ToString();
        }

        private int CalculateUniqueProducts(IEnumerable<DailySaleReportItem> items)
        {
            var uniqueProducts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items ?? Enumerable.Empty<DailySaleReportItem>())
            {
                if (string.IsNullOrWhiteSpace(item.ProductName)) continue;
                var parts = item.ProductName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var cleaned = part.Trim();
                    var idx = cleaned.LastIndexOf("(x", StringComparison.OrdinalIgnoreCase);
                    if (idx > 0) cleaned = cleaned.Substring(0, idx).Trim();
                    if (!string.IsNullOrEmpty(cleaned)) uniqueProducts.Add(cleaned);
                }
            }
            return uniqueProducts.Count;
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }
    }
}
