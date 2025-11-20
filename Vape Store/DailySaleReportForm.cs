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
                    HeaderText = "Products",
                    DataPropertyName = "ProductName",
                    Width = 320,
                    MinimumWidth = 220
                });
                
                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PurchasePrice",
                    HeaderText = "Purchase Total",
                    DataPropertyName = "PurchasePrice",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });

                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SubTotal",
                    HeaderText = "Sub Total",
                    DataPropertyName = "SubTotal",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "DiscountAmount",
                    HeaderText = "Discount",
                    DataPropertyName = "DiscountAmount",
                    Width = 110,
                    MinimumWidth = 90,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });

                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TaxAmount",
                    HeaderText = "Tax",
                    DataPropertyName = "TaxAmount",
                    Width = 100,
                    MinimumWidth = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvDailySaleReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SalePrice",
                    HeaderText = "Grand Total",
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
                        
                        // First pass: Collect all items per sale to get accurate sale totals
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

                                // Store sale-level info (same for all items in the sale)
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

                                // Collect item data
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

                        // Second pass: Calculate profit with correct discount distribution
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
                                // Calculate profit: Actual Revenue After Discount and Tax - Cost
                                // Distribute discount and tax proportionally to each item based on its share of subtotal
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
                        (item.SubTotal.ToString("F2").Contains(searchTerm)) ||
                        (item.SalePrice.ToString("F2").Contains(searchTerm)) ||
                        (item.TaxAmount.ToString("F2").Contains(searchTerm)) ||
                        (item.Profit.ToString("F2").Contains(searchTerm)) ||
                        (item.DiscountAmount.ToString("F2").Contains(searchTerm)) ||
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
                if (dgvDailySaleReport.Columns["SubTotal"] != null)
                    dgvDailySaleReport.Columns["SubTotal"].DefaultCellStyle.Format = "F2";
                if (dgvDailySaleReport.Columns["SalePrice"] != null)
                    dgvDailySaleReport.Columns["SalePrice"].DefaultCellStyle.Format = "F2";
                if (dgvDailySaleReport.Columns["DiscountAmount"] != null)
                    dgvDailySaleReport.Columns["DiscountAmount"].DefaultCellStyle.Format = "F2";
                if (dgvDailySaleReport.Columns["TaxAmount"] != null)
                    dgvDailySaleReport.Columns["TaxAmount"].DefaultCellStyle.Format = "F2";
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
                var totalSubTotal = _dailySaleReportItems.Sum(item => item.SubTotal);
                var totalDiscount = _dailySaleReportItems.Sum(item => item.DiscountAmount);
                var totalTax = _dailySaleReportItems.Sum(item => item.TaxAmount);
                var totalSales = _dailySaleReportItems.Sum(item => item.SalePrice);
                var totalCost = _dailySaleReportItems.Sum(item => item.PurchasePrice);

                Paragraph summaryTitle = new Paragraph("SUMMARY", headerFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                PdfPTable summaryTable = new PdfPTable(2);
                summaryTable.WidthPercentage = 50;
                summaryTable.SetWidths(new float[] { 1, 1 });

                PdfPCell cell1 = new PdfPCell(new Phrase("Subtotal:", normalFont));
                cell1.Border = 0;
                summaryTable.AddCell(cell1);
                PdfPCell cell2 = new PdfPCell(new Phrase($"{totalSubTotal:F2}", normalFont));
                cell2.Border = 0;
                cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.AddCell(cell2);
                PdfPCell cell3 = new PdfPCell(new Phrase("Discount:", normalFont));
                cell3.Border = 0;
                summaryTable.AddCell(cell3);
                PdfPCell cell4 = new PdfPCell(new Phrase($"{totalDiscount:F2}", normalFont));
                cell4.Border = 0;
                cell4.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.AddCell(cell4);
                PdfPCell cell5 = new PdfPCell(new Phrase("Tax:", normalFont));
                cell5.Border = 0;
                summaryTable.AddCell(cell5);
                PdfPCell cell6 = new PdfPCell(new Phrase($"{totalTax:F2}", normalFont));
                cell6.Border = 0;
                cell6.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.AddCell(cell6);
                PdfPCell cell7 = new PdfPCell(new Phrase("Grand Total:", normalFont));
                cell7.Border = 0;
                summaryTable.AddCell(cell7);
                PdfPCell cell8 = new PdfPCell(new Phrase($"{totalSales:F2}", normalFont));
                cell8.Border = 0;
                cell8.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.AddCell(cell8);
                PdfPCell cell9 = new PdfPCell(new Phrase("Total Cost:", normalFont));
                cell9.Border = 0;
                summaryTable.AddCell(cell9);
                PdfPCell cell10 = new PdfPCell(new Phrase($"{totalCost:F2}", normalFont));
                cell10.Border = 0;
                cell10.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.AddCell(cell10);
                PdfPCell cell11 = new PdfPCell(new Phrase("Total Profit:", normalFont));
                cell11.Border = 0;
                summaryTable.AddCell(cell11);
                PdfPCell cell12 = new PdfPCell(new Phrase($"{totalProfit:F2}", normalFont));
                cell12.Border = 0;
                cell12.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.AddCell(cell12);
                PdfPCell cell13 = new PdfPCell(new Phrase("Total Quantity:", normalFont));
                cell13.Border = 0;
                summaryTable.AddCell(cell13);
                PdfPCell cell14 = new PdfPCell(new Phrase(totalQuantity.ToString(), normalFont));
                cell14.Border = 0;
                cell14.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.AddCell(cell14);

                summaryTable.SpacingAfter = 20f;
                document.Add(summaryTable);

                // Sales details section
                Paragraph detailsTitle = new Paragraph("SALES DETAILS", headerFont);
                detailsTitle.SpacingAfter = 10f;
                document.Add(detailsTitle);

                // Create sales table
                PdfPTable salesTable = new PdfPTable(10);
                salesTable.WidthPercentage = 100;
                salesTable.SetWidths(new float[] { 1.2f, 2.6f, 1.0f, 1.0f, 0.9f, 0.9f, 1.0f, 0.7f, 1.0f, 0.9f });

                // Add headers
                string[] headers = { "Invoice", "Product Name", "Purchase Total", "Sub Total", "Discount", "Tax", "Grand Total", "Qty", "Profit", "Profit %" };
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
                    
                    PdfPCell cellSubTotal = new PdfPCell(new Phrase($"{item.SubTotal:F2}", smallFont));
                    cellSubTotal.Padding = 3f;
                    cellSubTotal.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(cellSubTotal);
                    
                    PdfPCell cellDiscount = new PdfPCell(new Phrase($"{item.DiscountAmount:F2}", smallFont));
                    cellDiscount.Padding = 3f;
                    cellDiscount.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(cellDiscount);
                    
                    PdfPCell cellTax = new PdfPCell(new Phrase($"{item.TaxAmount:F2}", smallFont));
                    cellTax.Padding = 3f;
                    cellTax.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(cellTax);
                    
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
                    
                    PdfPCell totalCell3 = new PdfPCell(new Phrase($"{totalSubTotal:F2}", headerFont));
                    totalCell3.Padding = 3f;
                    totalCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(totalCell3);

                    PdfPCell totalCellDiscount = new PdfPCell(new Phrase($"{totalDiscount:F2}", headerFont));
                    totalCellDiscount.Padding = 3f;
                    totalCellDiscount.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(totalCellDiscount);

                    PdfPCell totalCellTax = new PdfPCell(new Phrase($"{totalTax:F2}", headerFont));
                    totalCellTax.Padding = 3f;
                    totalCellTax.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(totalCellTax);

                    PdfPCell totalCellTotal = new PdfPCell(new Phrase($"{totalSales:F2}", headerFont));
                    totalCellTotal.Padding = 3f;
                    totalCellTotal.HorizontalAlignment = Element.ALIGN_RIGHT;
                    salesTable.AddCell(totalCellTotal);
                    
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
                var totalSubTotal = _dailySaleReportItems.Sum(item => item.SubTotal);
                var totalDiscount = _dailySaleReportItems.Sum(item => item.DiscountAmount);
                var totalTax = _dailySaleReportItems.Sum(item => item.TaxAmount);
                var totalSales = _dailySaleReportItems.Sum(item => item.SalePrice);
                var totalCost = _dailySaleReportItems.Sum(item => item.PurchasePrice);
                var uniqueProducts = CalculateUniqueProducts(_dailySaleReportItems);
                var uniqueInvoices = _dailySaleReportItems.Select(item => item.InvoiceNumber).Distinct().Count();
                
                html.AppendLine("<div class='summary'>");
                html.AppendLine("<h2>Summary</h2>");
                html.AppendLine($"<div class='summary-item'>Subtotal: {totalSubTotal:F2}</div>");
                html.AppendLine($"<div class='summary-item'>Discount: {totalDiscount:F2}</div>");
                html.AppendLine($"<div class='summary-item'>Tax: {totalTax:F2}</div>");
                html.AppendLine($"<div class='summary-item'>Grand Total: {totalSales:F2}</div>");
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
                html.AppendLine("<th>Purchase Total</th>");
                html.AppendLine("<th>Sub Total</th>");
                html.AppendLine("<th>Discount</th>");
                html.AppendLine("<th>Tax</th>");
                html.AppendLine("<th>Grand Total</th>");
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
                    html.AppendLine($"<td>{item.SubTotal:F2}</td>");
                    html.AppendLine($"<td>{item.DiscountAmount:F2}</td>");
                    html.AppendLine($"<td>{item.TaxAmount:F2}</td>");
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
                html.AppendLine($"<td>{totalSubTotal:F2}</td>");
                html.AppendLine($"<td>{totalDiscount:F2}</td>");
                html.AppendLine($"<td>{totalTax:F2}</td>");
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

        private int CalculateUniqueProducts(IEnumerable<DailySaleReportItem> items)
        {
            var uniqueProducts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items ?? Enumerable.Empty<DailySaleReportItem>())
            {
                if (string.IsNullOrWhiteSpace(item.ProductName))
                    continue;

                var parts = item.ProductName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var cleaned = part.Trim();
                    var idx = cleaned.LastIndexOf("(x", StringComparison.OrdinalIgnoreCase);
                    if (idx > 0)
                    {
                        cleaned = cleaned.Substring(0, idx).Trim();
                    }

                    if (!string.IsNullOrEmpty(cleaned))
                    {
                        uniqueProducts.Add(cleaned);
                    }
                }
            }

            return uniqueProducts.Count;
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
        public decimal SubTotal { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public DateTime SaleDate { get; set; }
    }

    // Helper classes for profit calculation
    internal class SaleItemData
    {
        public string ProductName { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public decimal ItemSubTotal { get; set; }
    }

    internal class SaleInfo
    {
        public decimal SaleSubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SaleDate { get; set; }
    }
}

