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
    public partial class SalesReportForm : Form
    {
        private SaleRepository _saleRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;
        private ReportingService _reportingService;
        
        private List<SalesReportItem> _salesReportItems;
        private List<SalesReportItem> _originalSalesReportItems; // Store original unfiltered data
        private List<Customer> _customers;
        private List<Product> _products;
        private bool _isItemWiseMode = false;

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
            InitializePaymentMethods();
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
            
            // Filter event handlers
            cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
            cmbPaymentMethod.SelectedIndexChanged += CmbPaymentMethod_SelectedIndexChanged;
            
            // Date range event handlers
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpToDate.ValueChanged += DtpToDate_ValueChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // DataGridView error handler
            dgvSalesReport.DataError += DgvSalesReport_DataError;
            
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
                dgvSalesReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvSalesReport.AllowUserToResizeColumns = true;
                dgvSalesReport.AllowUserToResizeRows = false;
                dgvSalesReport.RowHeadersVisible = false;
                dgvSalesReport.EnableHeadersVisualStyles = false;
                dgvSalesReport.GridColor = Color.FromArgb(236, 240, 241);
                dgvSalesReport.BorderStyle = BorderStyle.None;
                
                // Set header styling
                dgvSalesReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
                dgvSalesReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvSalesReport.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                dgvSalesReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                // Set row styling
                dgvSalesReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                dgvSalesReport.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
                dgvSalesReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
                dgvSalesReport.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvSalesReport.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgvSalesReport.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgvSalesReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                dgvSalesReport.ColumnHeadersHeight = 35;

                // Define columns
                dgvSalesReport.Columns.Clear();
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "InvoiceNumber",
                    HeaderText = "Invoice #",
                    DataPropertyName = "InvoiceNumber",
                    Width = 150,
                    MinimumWidth = 120
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SaleDate",
                    HeaderText = "Sale Date",
                    DataPropertyName = "SaleDate",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CustomerName",
                    HeaderText = "Customer",
                    DataPropertyName = "CustomerName",
                    Width = 200,
                    MinimumWidth = 150
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Product",
                    DataPropertyName = "ProductName",
                    Width = 250,
                    MinimumWidth = 200,
                    Visible = true // Visible by default in normal mode
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Qty",
                    DataPropertyName = "Quantity",
                    Width = 80,
                    MinimumWidth = 60
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UnitPrice",
                    HeaderText = "Unit Price",
                    DataPropertyName = "UnitPrice",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SubTotal",
                    HeaderText = "Sub Total",
                    DataPropertyName = "SubTotal",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });

                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "DiscountAmount",
                    HeaderText = "Discount",
                    DataPropertyName = "DiscountAmount",
                    Width = 110,
                    MinimumWidth = 90,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TaxAmount",
                    HeaderText = "Tax",
                    DataPropertyName = "TaxAmount",
                    Width = 100,
                    MinimumWidth = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalAmount",
                    HeaderText = "Total",
                    DataPropertyName = "TotalAmount",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PaymentMethod",
                    HeaderText = "Payment",
                    DataPropertyName = "PaymentMethod",
                    Width = 120,
                    MinimumWidth = 100
                });
                
                dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PaidAmount",
                    HeaderText = "Paid",
                    DataPropertyName = "PaidAmount",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
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
                var customerList = new List<Customer> { new Customer { CustomerID = 0, CustomerName = "All Customers" } };
                if (_customers != null && _customers.Count > 0)
                {
                    customerList.AddRange(_customers);
                }
                SearchableComboBoxHelper.MakeSearchable(cmbCustomer, customerList, "CustomerName", "CustomerID", "CustomerName");
                cmbCustomer.SelectedIndex = 0; // Select "All Customers"
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customers: {ex.Message}", "Error", MessageBoxIcon.Error);
                // Ensure ComboBox has at least one item
                var fallbackList = new List<Customer> { new Customer { CustomerID = 0, CustomerName = "All Customers" } };
                SearchableComboBoxHelper.MakeSearchable(cmbCustomer, fallbackList, "CustomerName", "CustomerID", "CustomerName");
                cmbCustomer.SelectedIndex = 0;
            }
        }


        private void InitializePaymentMethods()
        {
            try
            {
                var paymentMethods = new List<string> { "All Payment Methods", "Cash", "Card", "Credit Card", "Debit Card", "Bank Transfer", "Check", "Digital Wallet", "Other" };
                SearchableComboBoxHelper.MakeSearchable(cmbPaymentMethod, paymentMethods);
                cmbPaymentMethod.SelectedIndex = 0; // Select "All Payment Methods"
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing payment methods: {ex.Message}", "Error", MessageBoxIcon.Error);
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
                
                if (sales.Count == 0)
                {
                    // Check if database has any data at all
                    var allSales = _saleRepository.GetAllSales();
                    if (allSales.Count == 0)
                    {
                        var result = MessageBox.Show(
                            "No sales data found in the database. Would you like to seed the database with test data?",
                            "No Data Found",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        
                        if (result == DialogResult.Yes)
                        {
                            SeedDatabaseWithTestData();
                            // Reload data after seeding
                            sales = _saleRepository.GetSalesByDateRange(fromDate, toDate);
                        }
                        else
                        {
                            ShowMessage($"No sales found for the date range {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}. Please check your date range or ensure there is data in the database.", "No Data Found", MessageBoxIcon.Information);
                            return;
                        }
                    }
                    else
                    {
                        ShowMessage($"No sales found for the date range {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}. Please check your date range or ensure there is data in the database.", "No Data Found", MessageBoxIcon.Information);
                        return;
                    }
                }
                
                // Check if this is item-wise mode
                if (_isItemWiseMode)
                {
                    LoadItemWiseData(sales);
                }
                else
                {
                    LoadDetailedSalesData(sales);
                }
                
                // Store original unfiltered data
                _originalSalesReportItems = new List<SalesReportItem>(_salesReportItems);
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading sales data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadItemWiseData(List<Sale> sales)
        {
            // Industry-standard Item-wise Sales Report
            // Groups sales by product and shows: Code, Name, Category, Qty, Unit Price, Total Sales, Cost, Profit, Margin, Discounts, Tax, Net Sales
            
            // Load all products to get cost information
            var allProducts = _productRepository.GetAllProducts();
            var productDictionary = allProducts.ToDictionary(p => p.ProductID, p => p);
            
            // Group sales by product (using ProductID as key for accuracy)
            var itemWiseData = new Dictionary<int, SalesReportItem>();
            var productCustomerCount = new Dictionary<int, int>(); // Track number of unique customers
            
            foreach (var sale in sales)
            {
                // Check if sale has items loaded
                if (sale.SaleItems == null || sale.SaleItems.Count == 0)
                {
                    var saleService = new SalesService();
                    var fullSale = saleService.GetSaleById(sale.SaleID);
                    if (fullSale != null && fullSale.SaleItems != null && fullSale.SaleItems.Count > 0)
                    {
                        sale.SaleItems = fullSale.SaleItems;
                    }
                    else
                    {
                        continue;
                    }
                }
                
                var saleDiscount = sale.DiscountAmount;
                var saleSubTotal = sale.SubTotal;
                var saleTaxableAmount = saleSubTotal - saleDiscount;
                var customerName = !string.IsNullOrWhiteSpace(sale.CustomerName) ? sale.CustomerName : "Walk-in Customer";
                
                foreach (var saleItem in sale.SaleItems)
                {
                    var productID = saleItem.ProductID;
                    
                    // Get product information
                    Product product = null;
                    if (productDictionary.ContainsKey(productID))
                    {
                        product = productDictionary[productID];
                    }
                    
                    // Calculate item sales values
                    var itemSubTotal = saleItem.SubTotal > 0 ? saleItem.SubTotal : saleItem.UnitPrice * saleItem.Quantity;
                    
                    // Calculate discount share proportionally
                    decimal discountShare = 0;
                    if (saleDiscount > 0 && saleSubTotal > 0)
                    {
                        discountShare = (saleDiscount * itemSubTotal) / saleSubTotal;
                    }
                    
                    // Calculate item's taxable amount
                    var itemTaxableAmount = itemSubTotal - discountShare;
                    
                    // Calculate tax share proportionally based on taxable amount
                    decimal taxShare = 0;
                    if (sale.TaxAmount > 0 && saleTaxableAmount > 0)
                    {
                        taxShare = (sale.TaxAmount * itemTaxableAmount) / saleTaxableAmount;
                    }
                    
                    // Get product cost (use CostPrice if available, otherwise PurchasePrice)
                    decimal costPerUnit = 0;
                    if (product != null)
                    {
                        costPerUnit = product.CostPrice > 0 ? product.CostPrice : product.PurchasePrice;
                    }
                    
                    // Calculate cost and profit metrics
                    var totalCost = costPerUnit * saleItem.Quantity;
                    var totalSalesAmount = itemSubTotal; // Before discount
                    var netSalesAmount = itemTaxableAmount + taxShare; // After discount and tax
                    var grossProfit = totalSalesAmount - totalCost; // Profit before discount and tax
                    var grossProfitMargin = totalSalesAmount > 0 ? (grossProfit / totalSalesAmount) * 100 : 0;
                    
                    // Track unique customers
                    if (!productCustomerCount.ContainsKey(productID))
                    {
                        productCustomerCount[productID] = 0;
                    }
                    productCustomerCount[productID]++;
                    
                    if (itemWiseData.ContainsKey(productID))
                    {
                        // Aggregate data for existing product
                        var existingItem = itemWiseData[productID];
                        existingItem.Quantity += saleItem.Quantity;
                        existingItem.SubTotal += itemSubTotal;
                        existingItem.DiscountAmount += discountShare;
                        existingItem.TaxAmount += taxShare;
                        existingItem.TotalAmount += netSalesAmount;
                        existingItem.TotalCost += totalCost;
                        existingItem.NetSalesAmount += netSalesAmount;
                        
                        // Recalculate weighted averages and totals
                        existingItem.UnitPrice = existingItem.Quantity > 0 
                            ? existingItem.SubTotal / existingItem.Quantity 
                            : 0;
                        existingItem.CostPerUnit = existingItem.Quantity > 0 
                            ? existingItem.TotalCost / existingItem.Quantity 
                            : 0;
                        existingItem.GrossProfit = existingItem.SubTotal - existingItem.TotalCost;
                        existingItem.GrossProfitMargin = existingItem.SubTotal > 0 
                            ? (existingItem.GrossProfit / existingItem.SubTotal) * 100 
                            : 0;
                    }
                    else
                    {
                        // Create new item for product
                        var reportItem = new SalesReportItem
                        {
                            ProductName = product != null ? product.ProductName : saleItem.ProductName ?? "Unknown Product",
                            ProductCode = product != null ? product.ProductCode : saleItem.ProductCode ?? "",
                            CategoryName = product != null ? product.CategoryName ?? "" : "",
                            Quantity = saleItem.Quantity,
                            UnitPrice = saleItem.UnitPrice,
                            SubTotal = itemSubTotal,
                            DiscountAmount = discountShare,
                            TaxAmount = taxShare,
                            TotalAmount = netSalesAmount,
                            CostPerUnit = costPerUnit,
                            TotalCost = totalCost,
                            GrossProfit = grossProfit,
                            GrossProfitMargin = grossProfitMargin,
                            NetSalesAmount = netSalesAmount,
                            CustomerName = "", // Will be set to customer count
                            SaleDate = sale.SaleDate,
                            InvoiceNumber = "", // Not needed in item-wise
                            PaymentMethod = "", // Not needed in item-wise
                            PaidAmount = 0,
                            BalanceAmount = 0
                        };
                        
                        itemWiseData[productID] = reportItem;
                    }
                }
            }
            
            // Update customer count for each product
            foreach (var kvp in itemWiseData)
            {
                var productID = kvp.Key;
                var reportItem = kvp.Value;
                
                if (productCustomerCount.ContainsKey(productID))
                {
                    var customerCount = productCustomerCount[productID];
                    reportItem.CustomerName = customerCount == 1 ? "1 Customer" : $"{customerCount} Customers";
                }
            }
            
            // Add aggregated items to report
            _salesReportItems.AddRange(itemWiseData.Values);
            
            // Sort by product name
            _salesReportItems = _salesReportItems.OrderBy(x => x.ProductName).ToList();
        }

        private void LoadDetailedSalesData(List<Sale> sales)
        {
            foreach (var sale in sales)
            {
                var itemCount = Math.Max(1, sale.SaleItems.Count);
                var taxSharePerItem = itemCount > 0 ? sale.TaxAmount / itemCount : 0;
                var paidShare = itemCount > 0 ? sale.PaidAmount / itemCount : 0;
                var balanceShare = itemCount > 0 ? (sale.TotalAmount - sale.PaidAmount) / itemCount : 0;
                var saleDiscount = sale.DiscountAmount;
                var saleSubTotal = sale.SubTotal;
                
                foreach (var saleItem in sale.SaleItems)
                {
                    var itemSubTotal = saleItem.SubTotal > 0 ? saleItem.SubTotal : saleItem.UnitPrice * saleItem.Quantity;
                    decimal discountShare = 0;
                    if (saleDiscount > 0 && saleSubTotal > 0)
                    {
                        discountShare = (saleDiscount * itemSubTotal) / saleSubTotal;
                    }
                    var netSubTotal = itemSubTotal - discountShare;
                    var lineTotal = netSubTotal + taxSharePerItem;
                    
                    var reportItem = new SalesReportItem
                    {
                        SaleID = sale.SaleID,
                        InvoiceNumber = sale.InvoiceNumber,
                        SaleDate = sale.SaleDate,
                        CustomerName = sale.CustomerName ?? "Walk-in Customer",
                        ProductName = saleItem.ProductName,
                        Quantity = saleItem.Quantity,
                        UnitPrice = saleItem.UnitPrice,
                        SubTotal = itemSubTotal,
                        DiscountAmount = discountShare,
                        TaxAmount = taxSharePerItem,
                        TotalAmount = lineTotal,
                        PaymentMethod = sale.PaymentMethod,
                        PaidAmount = paidShare,
                        BalanceAmount = balanceShare
                    };
                    
                    _salesReportItems.Add(reportItem);
                }
            }
        }

        private void ApplyFilters()
        {
            try
            {
                // Use original unfiltered data as base
                var filteredItems = _originalSalesReportItems?.AsEnumerable() ?? _salesReportItems.AsEnumerable();
                
                // Customer filter
                if (cmbCustomer.SelectedItem != null)
                {
                    var selectedCustomer = (Customer)cmbCustomer.SelectedItem;
                    if (selectedCustomer != null && selectedCustomer.CustomerName != "All Customers")
                    {
                        filteredItems = filteredItems.Where(item => item.CustomerName == selectedCustomer.CustomerName);
                    }
                }
                
                // Payment method filter
                if (!string.IsNullOrWhiteSpace(cmbPaymentMethod.SelectedItem?.ToString()) && cmbPaymentMethod.SelectedItem.ToString() != "All Payment Methods")
                {
                    filteredItems = filteredItems.Where(item => item.PaymentMethod == cmbPaymentMethod.SelectedItem.ToString());
                }
                
                // Search filter - search through multiple fields
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        (item.InvoiceNumber?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.CustomerName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ProductName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.PaymentMethod?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.SaleDate.ToString("yyyy-MM-dd").Contains(searchTerm)) ||
                        (item.SaleDate.ToString("MM/dd/yyyy").Contains(searchTerm)) ||
                        (item.TotalAmount.ToString("F2").Contains(searchTerm)) ||
                        (item.DiscountAmount.ToString("F2").Contains(searchTerm)) ||
                        (item.Quantity.ToString().Contains(searchTerm)));
                }
                
                // Update filtered list (preserve original)
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
                bool hasSearchFilter = !string.IsNullOrWhiteSpace(txtSearch.Text);
                bool hasDataButNoResults = hasSearchFilter && _originalSalesReportItems != null && 
                                          _originalSalesReportItems.Count > 0 && _salesReportItems.Count == 0;
                
                dgvSalesReport.DataSource = null;
                
                if (hasDataButNoResults)
                {
                    // Show empty grid - message will be in summary labels
                    dgvSalesReport.DataSource = new List<SalesReportItem>();
                }
                else
                {
                dgvSalesReport.DataSource = _salesReportItems;
                }
                
                // Format decimal columns
                if (dgvSalesReport.Columns["UnitPrice"] != null)
                    dgvSalesReport.Columns["UnitPrice"].DefaultCellStyle.Format = "F2";
                if (dgvSalesReport.Columns["SubTotal"] != null)
                    dgvSalesReport.Columns["SubTotal"].DefaultCellStyle.Format = "F2";
                if (dgvSalesReport.Columns["DiscountAmount"] != null)
                    dgvSalesReport.Columns["DiscountAmount"].DefaultCellStyle.Format = "F2";
                if (dgvSalesReport.Columns["TaxAmount"] != null)
                    dgvSalesReport.Columns["TaxAmount"].DefaultCellStyle.Format = "F2";
                if (dgvSalesReport.Columns["TotalAmount"] != null)
                    dgvSalesReport.Columns["TotalAmount"].DefaultCellStyle.Format = "F2";
                if (dgvSalesReport.Columns["PaidAmount"] != null)
                    dgvSalesReport.Columns["PaidAmount"].DefaultCellStyle.Format = "F2";
                
                // Format date column
                if (dgvSalesReport.Columns["SaleDate"] != null)
                    dgvSalesReport.Columns["SaleDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
                
                // Add total row styling if there are items
                if (dgvSalesReport.Rows.Count > 0 && _salesReportItems.Count > 0)
                {
                    // Add a total row at the end
                    var totalRow = dgvSalesReport.Rows[dgvSalesReport.Rows.Count - 1];
                    totalRow.DefaultCellStyle.BackColor = Color.LightBlue;
                    totalRow.DefaultCellStyle.Font = new System.Drawing.Font(dgvSalesReport.DefaultCellStyle.Font, FontStyle.Bold);
                    
                    if (_isItemWiseMode)
                    {
                        // Item-wise mode totals
                        if (dgvSalesReport.Columns["ProductCode"] != null)
                            totalRow.Cells["ProductCode"].Value = "TOTAL";
                        if (dgvSalesReport.Columns["ProductName"] != null)
                            totalRow.Cells["ProductName"].Value = "";
                        if (dgvSalesReport.Columns["CategoryName"] != null)
                            totalRow.Cells["CategoryName"].Value = "";
                        if (dgvSalesReport.Columns["Quantity"] != null)
                            totalRow.Cells["Quantity"].Value = _salesReportItems.Sum(x => x.Quantity);
                        if (dgvSalesReport.Columns["UnitPrice"] != null)
                            totalRow.Cells["UnitPrice"].Value = "";
                        if (dgvSalesReport.Columns["SubTotal"] != null)
                            totalRow.Cells["SubTotal"].Value = _salesReportItems.Sum(x => x.SubTotal);
                        if (dgvSalesReport.Columns["CostPerUnit"] != null)
                            totalRow.Cells["CostPerUnit"].Value = "";
                        if (dgvSalesReport.Columns["TotalCost"] != null)
                            totalRow.Cells["TotalCost"].Value = _salesReportItems.Sum(x => x.TotalCost);
                        if (dgvSalesReport.Columns["GrossProfit"] != null)
                            totalRow.Cells["GrossProfit"].Value = _salesReportItems.Sum(x => x.GrossProfit);
                        if (dgvSalesReport.Columns["GrossProfitMargin"] != null)
                        {
                            var totalSales = _salesReportItems.Sum(x => x.SubTotal);
                            var totalProfit = _salesReportItems.Sum(x => x.GrossProfit);
                            var avgMargin = totalSales > 0 ? (totalProfit / totalSales) * 100 : 0;
                            totalRow.Cells["GrossProfitMargin"].Value = avgMargin.ToString("F2");
                        }
                        if (dgvSalesReport.Columns["DiscountAmount"] != null)
                            totalRow.Cells["DiscountAmount"].Value = _salesReportItems.Sum(x => x.DiscountAmount);
                        if (dgvSalesReport.Columns["TaxAmount"] != null)
                            totalRow.Cells["TaxAmount"].Value = _salesReportItems.Sum(x => x.TaxAmount);
                        if (dgvSalesReport.Columns["NetSalesAmount"] != null)
                            totalRow.Cells["NetSalesAmount"].Value = _salesReportItems.Sum(x => x.NetSalesAmount);
                        if (dgvSalesReport.Columns["CustomerName"] != null)
                            totalRow.Cells["CustomerName"].Value = "";
                    }
                    else
                    {
                        // Normal mode totals
                        if (dgvSalesReport.Columns["InvoiceNumber"] != null && dgvSalesReport.Columns["InvoiceNumber"].Visible)
                            totalRow.Cells["InvoiceNumber"].Value = "TOTAL";
                        if (dgvSalesReport.Columns["CustomerName"] != null && dgvSalesReport.Columns["CustomerName"].Visible)
                            totalRow.Cells["CustomerName"].Value = "";
                        if (dgvSalesReport.Columns["ProductName"] != null && dgvSalesReport.Columns["ProductName"].Visible)
                            totalRow.Cells["ProductName"].Value = "";
                        if (dgvSalesReport.Columns["Quantity"] != null)
                            totalRow.Cells["Quantity"].Value = _salesReportItems.Sum(x => x.Quantity);
                        if (dgvSalesReport.Columns["UnitPrice"] != null)
                            totalRow.Cells["UnitPrice"].Value = "";
                        if (dgvSalesReport.Columns["SubTotal"] != null)
                            totalRow.Cells["SubTotal"].Value = _salesReportItems.Sum(x => x.SubTotal);
                        if (dgvSalesReport.Columns["DiscountAmount"] != null)
                            totalRow.Cells["DiscountAmount"].Value = _salesReportItems.Sum(x => x.DiscountAmount);
                        if (dgvSalesReport.Columns["TaxAmount"] != null)
                            totalRow.Cells["TaxAmount"].Value = _salesReportItems.Sum(x => x.TaxAmount);
                        if (dgvSalesReport.Columns["TotalAmount"] != null)
                            totalRow.Cells["TotalAmount"].Value = _salesReportItems.Sum(x => x.TotalAmount);
                        if (dgvSalesReport.Columns["PaymentMethod"] != null && dgvSalesReport.Columns["PaymentMethod"].Visible)
                            totalRow.Cells["PaymentMethod"].Value = "";
                        if (dgvSalesReport.Columns["PaidAmount"] != null && dgvSalesReport.Columns["PaidAmount"].Visible)
                            totalRow.Cells["PaidAmount"].Value = _salesReportItems.Sum(x => x.PaidAmount);
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
                bool hasDataButNoResults = hasSearchFilter && _originalSalesReportItems != null && 
                                          _originalSalesReportItems.Count > 0 && _salesReportItems.Count == 0;
                
                if (hasDataButNoResults)
                {
                    // Show "No results" message in summary
                    lblTotalSales.Text = $"Total Sales: No results found for '{txtSearch.Text}'";
                    lblTotalQuantity.Text = "Total Quantity: No results found";
                    lblTotalTax.Text = "Total Tax: No results found";
                    lblTotalPaid.Text = "Total Paid: No results found";
                    lblTotalBalance.Text = "Total Balance: No results found";
                    lblUniqueCustomers.Text = "Unique Customers: 0";
                }
                else
                {
                    if (_isItemWiseMode)
                    {
                        // Item-wise mode summary with profit information
                        var totalSales = _salesReportItems.Sum(item => item.SubTotal);
                        var totalCost = _salesReportItems.Sum(item => item.TotalCost);
                        var totalProfit = _salesReportItems.Sum(item => item.GrossProfit);
                        var totalDiscount = _salesReportItems.Sum(item => item.DiscountAmount);
                        var totalTax = _salesReportItems.Sum(item => item.TaxAmount);
                        var totalNetSales = _salesReportItems.Sum(item => item.NetSalesAmount);
                        var totalQuantity = _salesReportItems.Sum(item => item.Quantity);
                        var avgProfitMargin = totalSales > 0 ? (totalProfit / totalSales) * 100 : 0;
                        var uniqueProducts = _salesReportItems.Count;
                        
                        lblTotalSales.Text = $"Total Sales: {totalSales:F2} | Net Sales: {totalNetSales:F2}";
                        lblTotalQuantity.Text = $"Total Quantity: {totalQuantity} | Products: {uniqueProducts}";
                        lblTotalTax.Text = $"Total Cost: {totalCost:F2} | Gross Profit: {totalProfit:F2}";
                        lblTotalPaid.Text = $"Profit Margin: {avgProfitMargin:F2}% | Discount: {totalDiscount:F2}";
                        lblTotalBalance.Text = $"Tax: {totalTax:F2}";
                        lblUniqueCustomers.Text = $"Total Items: {uniqueProducts}";
                    }
                    else
                    {
                        // Normal mode summary
                        var totalSales = _salesReportItems.Sum(item => item.TotalAmount);
                        var totalDiscount = _salesReportItems.Sum(item => item.DiscountAmount);
                        var totalQuantity = _salesReportItems.Sum(item => item.Quantity);
                        var totalTax = _salesReportItems.Sum(item => item.TaxAmount);
                        var totalPaid = _salesReportItems.Sum(item => item.PaidAmount);
                        var totalBalance = _salesReportItems.Sum(item => item.BalanceAmount);
                        var uniqueCustomers = _salesReportItems.Select(item => item.CustomerName).Distinct().Count();
                        
                        lblTotalSales.Text = $"Total Sales: {totalSales:F2} | Discount: {totalDiscount:F2}";
                        lblTotalQuantity.Text = $"Total Quantity: {totalQuantity}";
                        lblTotalTax.Text = $"Total Tax: {totalTax:F2}";
                        lblTotalPaid.Text = $"Total Paid: {totalPaid:F2}";
                        lblTotalBalance.Text = $"Total Balance: {totalBalance:F2}";
                        lblUniqueCustomers.Text = $"Unique Customers: {uniqueCustomers}";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating summary labels: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            try
            {
                // Set date range to include all sales data (from 2024 to now)
                dtpFromDate.Value = new DateTime(2024, 1, 1);
                dtpToDate.Value = DateTime.Now.AddDays(1);
                
                // Only set SelectedIndex if ComboBoxes have items
                if (cmbCustomer.Items.Count > 0)
                    cmbCustomer.SelectedIndex = 0;
                
                if (cmbPaymentMethod.Items.Count > 0)
                    cmbPaymentMethod.SelectedIndex = 0;
                
                txtSearch.Clear();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error setting initial state: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadSalesData();
        }

        private void BtnSeedDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "This will seed the database with test data. Continue?",
                    "Seed Database",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    SeedDatabaseWithTestData();
                    // Reload data after seeding
                    LoadSalesData();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error seeding database: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }


        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
                saveFileDialog.FileName = $"SalesReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToPDF(saveFileDialog.FileName);
                    ShowMessage($"PDF report exported successfully to: {saveFileDialog.FileName}", "Export Complete", MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting PDF: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnViewHTML_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure data is loaded before generating HTML report
                if (_salesReportItems.Count == 0)
                {
                    LoadSalesData();
                }
                
                var htmlContent = GenerateHTMLReport();
                var htmlViewer = new HTMLReportViewerForm();
                htmlViewer.LoadReport(htmlContent);
                htmlViewer.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error opening HTML report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnExportCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FileName = $"SalesReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveFileDialog.FileName);
                    ShowMessage($"CSV report exported successfully to: {saveFileDialog.FileName}", "Export Complete", MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting to CSV: {ex.Message}", "Error", MessageBoxIcon.Error);
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

        public void SetItemWiseMode()
        {
            // Set to show item-wise sales data (Industry Standard Format)
            this.Text = "Item-wise Sales Report";
            _isItemWiseMode = true;

            // Clear existing columns and add industry-standard columns for item-wise report
            dgvSalesReport.Columns.Clear();
            
            // Industry Standard Item-wise Sales Report Columns
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductCode",
                HeaderText = "Item Code",
                DataPropertyName = "ProductCode",
                Width = 100,
                MinimumWidth = 80
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductName",
                HeaderText = "Item Name",
                DataPropertyName = "ProductName",
                Width = 250,
                MinimumWidth = 200
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CategoryName",
                HeaderText = "Category",
                DataPropertyName = "CategoryName",
                Width = 120,
                MinimumWidth = 100
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Quantity",
                HeaderText = "Qty Sold",
                DataPropertyName = "Quantity",
                Width = 80,
                MinimumWidth = 70,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UnitPrice",
                HeaderText = "Avg Unit Price",
                DataPropertyName = "UnitPrice",
                Width = 110,
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SubTotal",
                HeaderText = "Total Sales",
                DataPropertyName = "SubTotal",
                Width = 120,
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CostPerUnit",
                HeaderText = "Cost/Unit",
                DataPropertyName = "CostPerUnit",
                Width = 100,
                MinimumWidth = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalCost",
                HeaderText = "Total Cost",
                DataPropertyName = "TotalCost",
                Width = 110,
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GrossProfit",
                HeaderText = "Gross Profit",
                DataPropertyName = "GrossProfit",
                Width = 120,
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GrossProfitMargin",
                HeaderText = "Profit Margin %",
                DataPropertyName = "GrossProfitMargin",
                Width = 110,
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DiscountAmount",
                HeaderText = "Discounts",
                DataPropertyName = "DiscountAmount",
                Width = 100,
                MinimumWidth = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TaxAmount",
                HeaderText = "Tax",
                DataPropertyName = "TaxAmount",
                Width = 100,
                MinimumWidth = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NetSalesAmount",
                HeaderText = "Net Sales",
                DataPropertyName = "NetSalesAmount",
                Width = 120,
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerName",
                HeaderText = "Customers",
                DataPropertyName = "CustomerName",
                Width = 120,
                MinimumWidth = 100
            });

            // Load data for this mode
            LoadSalesData();
        }

        private void SeedDatabaseWithTestData()
        {
            try
            {
                // Read and execute the test data SQL script
                string sqlScript = File.ReadAllText("Test_Data_Seed.sql");
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    
                    // Split the script into individual commands
                    string[] commands = sqlScript.Split(new string[] { "GO", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    
                    int successCount = 0;
                    int errorCount = 0;
                    
                    foreach (string command in commands)
                    {
                        if (!string.IsNullOrWhiteSpace(command.Trim()))
                        {
                            try
                            {
                                using (var sqlCommand = new SqlCommand(command.Trim(), connection))
                                {
                                    sqlCommand.ExecuteNonQuery();
                                    successCount++;
                                }
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                // Log error but continue with other commands
                                System.Diagnostics.Debug.WriteLine($"Error executing command: {ex.Message}");
                            }
                        }
                    }
                    
                    ShowMessage($"Database seeding completed!\n\nSuccessful commands: {successCount}\nErrors: {errorCount}", 
                        "Database Seeding", MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error seeding database: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DgvSalesReport_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Handle DataGridView data errors gracefully
            e.Cancel = true;
            // Optionally log the error or show a user-friendly message
            // For now, we'll just suppress the error dialog
        }

        private string GenerateHTMLReport()
        {
            try
            {
                var html = new StringBuilder();
                
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html>");
                html.AppendLine("<head>");
                html.AppendLine("<title>Sales Report</title>");
                html.AppendLine("<style>");
                html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; line-height: 1.4; }");
                html.AppendLine("h1 { color: #2c3e50; text-align: center; margin-bottom: 20px; }");
                html.AppendLine("h2 { color: #34495e; border-bottom: 2px solid #3498db; margin-top: 30px; margin-bottom: 15px; }");
                html.AppendLine("table { width: 100%; border-collapse: collapse; margin: 20px 0; font-size: 12px; }");
                html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; vertical-align: top; }");
                html.AppendLine("th { background-color: #2c3e50; color: white; font-weight: bold; text-align: center; }");
                html.AppendLine("tr:nth-child(even) { background-color: #f8f9fa; }");
                html.AppendLine(".summary { background-color: #ecf0f1; padding: 15px; border-radius: 5px; margin: 20px 0; }");
                html.AppendLine(".summary-item { margin: 8px 0; font-weight: bold; }");
                html.AppendLine(".total-row { background-color: #e8f4fd; font-weight: bold; }");
                html.AppendLine("td { word-wrap: break-word; max-width: 150px; }");
                html.AppendLine("</style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");
                
                // Header
                string reportTitle = _isItemWiseMode ? "ITEM-WISE SALES REPORT" : "SALES REPORT";
                html.AppendLine($"<h1>{reportTitle}</h1>");
                html.AppendLine($"<p><strong>Report Period:</strong> {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}</p>");
                html.AppendLine($"<p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
                
                // Summary
                html.AppendLine("<h2>Summary</h2>");
                html.AppendLine("<div class='summary'>");
                
                if (_isItemWiseMode)
                {
                    // Item-wise mode summary with profit information
                    var totalSales = _salesReportItems.Sum(x => x.SubTotal);
                    var totalCost = _salesReportItems.Sum(x => x.TotalCost);
                    var totalProfit = _salesReportItems.Sum(x => x.GrossProfit);
                    var totalDiscount = _salesReportItems.Sum(x => x.DiscountAmount);
                    var totalTax = _salesReportItems.Sum(x => x.TaxAmount);
                    var totalNetSales = _salesReportItems.Sum(x => x.NetSalesAmount);
                    var totalQuantity = _salesReportItems.Sum(x => x.Quantity);
                    var avgProfitMargin = totalSales > 0 ? (totalProfit / totalSales) * 100 : 0;
                    var uniqueProducts = _salesReportItems.Count;
                    
                    html.AppendLine($"<div class='summary-item'>Total Sales: {totalSales:F2}</div>");
                    html.AppendLine($"<div class='summary-item'>Total Cost: {totalCost:F2}</div>");
                    html.AppendLine($"<div class='summary-item'>Gross Profit: {totalProfit:F2}</div>");
                    html.AppendLine($"<div class='summary-item'>Profit Margin: {avgProfitMargin:F2}%</div>");
                    html.AppendLine($"<div class='summary-item'>Total Discount: {totalDiscount:F2}</div>");
                    html.AppendLine($"<div class='summary-item'>Total Tax: {totalTax:F2}</div>");
                    html.AppendLine($"<div class='summary-item'>Net Sales: {totalNetSales:F2}</div>");
                    html.AppendLine($"<div class='summary-item'>Total Quantity: {totalQuantity}</div>");
                    html.AppendLine($"<div class='summary-item'>Total Products: {uniqueProducts}</div>");
                }
                else
                {
                    // Normal mode summary
                    html.AppendLine($"<div class='summary-item'>Total Sales: {_salesReportItems.Sum(x => x.TotalAmount):F2}</div>");
                    html.AppendLine($"<div class='summary-item'>Total Quantity: {_salesReportItems.Sum(x => x.Quantity)}</div>");
                    html.AppendLine($"<div class='summary-item'>Total Discount: {_salesReportItems.Sum(x => x.DiscountAmount):F2}</div>");
                    html.AppendLine($"<div class='summary-item'>Total Tax: {_salesReportItems.Sum(x => x.TaxAmount):F2}</div>");
                    html.AppendLine($"<div class='summary-item'>Total Paid: {_salesReportItems.Sum(x => x.PaidAmount):F2}</div>");
                    html.AppendLine($"<div class='summary-item'>Total Balance: {_salesReportItems.Sum(x => x.BalanceAmount):F2}</div>");
                    html.AppendLine($"<div class='summary-item'>Unique Customers: {_salesReportItems.Select(x => x.CustomerName).Distinct().Count()}</div>");
                }
                
                html.AppendLine("</div>");
                
                // Data table
                html.AppendLine("<h2>Sales Details</h2>");
                html.AppendLine("<table>");
                html.AppendLine("<tr>");
                
                if (_isItemWiseMode)
                {
                    // Industry Standard Item-wise mode headers
                    html.AppendLine("<th>Item Code</th>");
                    html.AppendLine("<th>Item Name</th>");
                    html.AppendLine("<th>Category</th>");
                    html.AppendLine("<th>Qty Sold</th>");
                    html.AppendLine("<th>Unit Price</th>");
                    html.AppendLine("<th>Total Sales</th>");
                    html.AppendLine("<th>Cost/Unit</th>");
                    html.AppendLine("<th>Total Cost</th>");
                    html.AppendLine("<th>Gross Profit</th>");
                    html.AppendLine("<th>Profit Margin %</th>");
                    html.AppendLine("<th>Discounts</th>");
                    html.AppendLine("<th>Tax</th>");
                    html.AppendLine("<th>Net Sales</th>");
                    html.AppendLine("<th>Customers</th>");
                    html.AppendLine("</tr>");
                    
                    foreach (var item in _salesReportItems)
                    {
                        html.AppendLine("<tr>");
                        html.AppendLine($"<td>{item.ProductCode ?? ""}</td>");
                        html.AppendLine($"<td>{item.ProductName}</td>");
                        html.AppendLine($"<td>{item.CategoryName ?? ""}</td>");
                        html.AppendLine($"<td>{item.Quantity}</td>");
                        html.AppendLine($"<td>{item.UnitPrice:F2}</td>");
                        html.AppendLine($"<td>{item.SubTotal:F2}</td>");
                        html.AppendLine($"<td>{item.CostPerUnit:F2}</td>");
                        html.AppendLine($"<td>{item.TotalCost:F2}</td>");
                        html.AppendLine($"<td>{item.GrossProfit:F2}</td>");
                        html.AppendLine($"<td>{item.GrossProfitMargin:F2}</td>");
                        html.AppendLine($"<td>{item.DiscountAmount:F2}</td>");
                        html.AppendLine($"<td>{item.TaxAmount:F2}</td>");
                        html.AppendLine($"<td>{item.NetSalesAmount:F2}</td>");
                        html.AppendLine($"<td>{item.CustomerName}</td>");
                        html.AppendLine("</tr>");
                    }
                    
                    // Add total row
                    if (_salesReportItems.Count > 0)
                    {
                        var totalSales = _salesReportItems.Sum(x => x.SubTotal);
                        var totalCost = _salesReportItems.Sum(x => x.TotalCost);
                        var totalProfit = _salesReportItems.Sum(x => x.GrossProfit);
                        var totalNetSales = _salesReportItems.Sum(x => x.NetSalesAmount);
                        var totalQuantity = _salesReportItems.Sum(x => x.Quantity);
                        var totalDiscount = _salesReportItems.Sum(x => x.DiscountAmount);
                        var totalTax = _salesReportItems.Sum(x => x.TaxAmount);
                        var avgProfitMargin = totalSales > 0 ? (totalProfit / totalSales) * 100 : 0;
                        
                        html.AppendLine("<tr class='total-row'>");
                        html.AppendLine("<td><strong>TOTAL</strong></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine($"<td><strong>{totalQuantity}</strong></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine($"<td><strong>{totalSales:F2}</strong></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine($"<td><strong>{totalCost:F2}</strong></td>");
                        html.AppendLine($"<td><strong>{totalProfit:F2}</strong></td>");
                        html.AppendLine($"<td><strong>{avgProfitMargin:F2}</strong></td>");
                        html.AppendLine($"<td><strong>{totalDiscount:F2}</strong></td>");
                        html.AppendLine($"<td><strong>{totalTax:F2}</strong></td>");
                        html.AppendLine($"<td><strong>{totalNetSales:F2}</strong></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine("</tr>");
                    }
                }
                else
                {
                    // Normal mode headers
                    html.AppendLine("<th>Invoice</th>");
                    html.AppendLine("<th>Date</th>");
                    html.AppendLine("<th>Customer</th>");
                    html.AppendLine("<th>Product</th>");
                    html.AppendLine("<th>Qty</th>");
                    html.AppendLine("<th>Unit Price</th>");
                    html.AppendLine("<th>Sub Total</th>");
                    html.AppendLine("<th>Discount</th>");
                    html.AppendLine("<th>Tax</th>");
                    html.AppendLine("<th>Total</th>");
                    html.AppendLine("<th>Payment</th>");
                    html.AppendLine("<th>Paid</th>");
                    html.AppendLine("</tr>");
                    
                    foreach (var item in _salesReportItems)
                    {
                        html.AppendLine("<tr>");
                        html.AppendLine($"<td>{item.InvoiceNumber}</td>");
                        html.AppendLine($"<td>{item.SaleDate:yyyy-MM-dd}</td>");
                        html.AppendLine($"<td>{item.CustomerName}</td>");
                        html.AppendLine($"<td>{item.ProductName}</td>");
                        html.AppendLine($"<td>{item.Quantity}</td>");
                        html.AppendLine($"<td>{item.UnitPrice:F2}</td>");
                        html.AppendLine($"<td>{item.SubTotal:F2}</td>");
                        html.AppendLine($"<td>{item.DiscountAmount:F2}</td>");
                        html.AppendLine($"<td>{item.TaxAmount:F2}</td>");
                        html.AppendLine($"<td>{item.TotalAmount:F2}</td>");
                        html.AppendLine($"<td>{item.PaymentMethod}</td>");
                        html.AppendLine($"<td>{item.PaidAmount:F2}</td>");
                        html.AppendLine("</tr>");
                    }
                    
                    // Add total row
                    if (_salesReportItems.Count > 0)
                    {
                        var totalSales = _salesReportItems.Sum(x => x.TotalAmount);
                        var totalQuantity = _salesReportItems.Sum(x => x.Quantity);
                        var totalSubTotal = _salesReportItems.Sum(x => x.SubTotal);
                        var totalTax = _salesReportItems.Sum(x => x.TaxAmount);
                        var totalDiscount = _salesReportItems.Sum(x => x.DiscountAmount);
                        var totalPaid = _salesReportItems.Sum(x => x.PaidAmount);
                        
                        html.AppendLine("<tr class='total-row'>");
                        html.AppendLine("<td><strong>TOTAL</strong></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine($"<td><strong>{totalQuantity}</strong></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine($"<td><strong>{totalSubTotal:F2}</strong></td>");
                        html.AppendLine($"<td><strong>{totalDiscount:F2}</strong></td>");
                        html.AppendLine($"<td><strong>{totalTax:F2}</strong></td>");
                        html.AppendLine($"<td><strong>{totalSales:F2}</strong></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine($"<td><strong>{totalPaid:F2}</strong></td>");
                        html.AppendLine("</tr>");
                    }
                }
                
                html.AppendLine("</table>");
                html.AppendLine("</body>");
                html.AppendLine("</html>");
                
                return html.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating HTML report: {ex.Message}");
            }
        }

        private void SalesReportForm_Load(object sender, EventArgs e)
        {
            // Maximize the form
            this.WindowState = FormWindowState.Maximized;
            
            // Set initial state
            SetInitialState();
            
            // Load data automatically
            LoadSalesData();
        }

        private void ExportToCSV(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                if (_isItemWiseMode)
                {
                    // Industry Standard Item-wise Sales Report CSV Export
                    writer.WriteLine("Item Code,Item Name,Category,Quantity Sold,Avg Unit Price,Total Sales,Cost/Unit,Total Cost,Gross Profit,Profit Margin %,Discounts,Tax,Net Sales,Customers");
                    
                    // Write data
                    foreach (var item in _salesReportItems)
                    {
                        writer.WriteLine($"{item.ProductCode},{item.ProductName},{item.CategoryName},{item.Quantity},{item.UnitPrice:F2},{item.SubTotal:F2},{item.CostPerUnit:F2},{item.TotalCost:F2},{item.GrossProfit:F2},{item.GrossProfitMargin:F2},{item.DiscountAmount:F2},{item.TaxAmount:F2},{item.NetSalesAmount:F2},{item.CustomerName}");
                    }
                    
                    // Add total row if there are items
                    if (_salesReportItems.Count > 0)
                    {
                        var totalQuantity = _salesReportItems.Sum(x => x.Quantity);
                        var totalSales = _salesReportItems.Sum(x => x.SubTotal);
                        var totalCost = _salesReportItems.Sum(x => x.TotalCost);
                        var totalProfit = _salesReportItems.Sum(x => x.GrossProfit);
                        var totalDiscount = _salesReportItems.Sum(x => x.DiscountAmount);
                        var totalTax = _salesReportItems.Sum(x => x.TaxAmount);
                        var totalNetSales = _salesReportItems.Sum(x => x.NetSalesAmount);
                        var avgProfitMargin = totalSales > 0 ? (totalProfit / totalSales) * 100 : 0;
                        
                        writer.WriteLine($"TOTAL,,,{totalQuantity},,{totalSales:F2},,{totalCost:F2},{totalProfit:F2},{avgProfitMargin:F2},{totalDiscount:F2},{totalTax:F2},{totalNetSales:F2},");
                    }
                }
                else
                {
                    // Normal mode: Invoice-focused export
                    writer.WriteLine("Invoice Number,Sale Date,Customer Name,Product Name,Quantity,Unit Price,Sub Total,Discount Amount,Tax Amount,Total Amount,Payment Method,Paid Amount");
                    
                    // Write data
                    foreach (var item in _salesReportItems)
                    {
                        writer.WriteLine($"{item.InvoiceNumber},{item.SaleDate:yyyy-MM-dd},{item.CustomerName},{item.ProductName},{item.Quantity},{item.UnitPrice:F2},{item.SubTotal:F2},{item.DiscountAmount:F2},{item.TaxAmount:F2},{item.TotalAmount:F2},{item.PaymentMethod},{item.PaidAmount:F2}");
                    }
                    
                    // Add total row if there are items
                    if (_salesReportItems.Count > 0)
                    {
                        var totalQuantity = _salesReportItems.Sum(x => x.Quantity);
                        var totalSubTotal = _salesReportItems.Sum(x => x.SubTotal);
                        var totalDiscount = _salesReportItems.Sum(x => x.DiscountAmount);
                        var totalTax = _salesReportItems.Sum(x => x.TaxAmount);
                        var totalAmount = _salesReportItems.Sum(x => x.TotalAmount);
                        var totalPaid = _salesReportItems.Sum(x => x.PaidAmount);
                        
                        writer.WriteLine($"TOTAL,,,{totalQuantity},,{totalSubTotal:F2},{totalDiscount:F2},{totalTax:F2},{totalAmount:F2},,{totalPaid:F2}");
                    }
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

                // Title - adjust for item-wise mode
                string reportTitle = _isItemWiseMode 
                    ? "MADNI MOBILE & PHOTOSTATE - ITEM-WISE SALES REPORT" 
                    : "MADNI MOBILE & PHOTOSTATE - SALES REPORT";
                Paragraph title = new Paragraph(reportTitle, titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Report info
                Paragraph reportInfo = new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nDate Range: {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}", normalFont);
                reportInfo.SpacingAfter = 15f;
                document.Add(reportInfo);

                // Summary section
                Paragraph summaryTitle = new Paragraph("SUMMARY", headerFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                // Create summary table
                PdfPTable summaryTable = new PdfPTable(2);
                summaryTable.WidthPercentage = 50;
                summaryTable.SetWidths(new float[] { 1, 1 });

                if (_isItemWiseMode)
                {
                    // Item-wise mode summary with profit information
                    var totalSales = _salesReportItems.Sum(item => item.SubTotal);
                    var totalCost = _salesReportItems.Sum(item => item.TotalCost);
                    var totalProfit = _salesReportItems.Sum(item => item.GrossProfit);
                    var totalDiscount = _salesReportItems.Sum(item => item.DiscountAmount);
                    var totalTax = _salesReportItems.Sum(item => item.TaxAmount);
                    var totalNetSales = _salesReportItems.Sum(item => item.NetSalesAmount);
                    var totalQuantity = _salesReportItems.Sum(item => item.Quantity);
                    var avgProfitMargin = totalSales > 0 ? (totalProfit / totalSales) * 100 : 0;
                    var uniqueProducts = _salesReportItems.Count;

                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Sales:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{totalSales:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Cost:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{totalCost:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Gross Profit:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{totalProfit:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Profit Margin %:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{avgProfitMargin:F2}%", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Discount:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{totalDiscount:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Tax:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{totalTax:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Net Sales:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{totalNetSales:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Quantity:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase(totalQuantity.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Products:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase(uniqueProducts.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                }
                else
                {
                    // Normal mode summary
                    var totalSales = _salesReportItems.Sum(item => item.TotalAmount);
                    var totalQuantity = _salesReportItems.Sum(item => item.Quantity);
                    var totalDiscount = _salesReportItems.Sum(item => item.DiscountAmount);
                    var totalTax = _salesReportItems.Sum(item => item.TaxAmount);
                    var totalPaid = _salesReportItems.Sum(item => item.PaidAmount);
                    var totalBalance = _salesReportItems.Sum(item => item.BalanceAmount);
                    var uniqueCustomers = _salesReportItems.Select(item => item.CustomerName).Distinct().Count();

                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Sales:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{totalSales:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Quantity:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase(totalQuantity.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Discount:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{totalDiscount:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Tax:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{totalTax:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Paid:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{totalPaid:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Total Balance:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"{totalBalance:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    summaryTable.AddCell(new PdfPCell(new Phrase("Unique Customers:", normalFont)) { Border = 0 });
                    summaryTable.AddCell(new PdfPCell(new Phrase(uniqueCustomers.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                summaryTable.SpacingAfter = 20f;
                document.Add(summaryTable);

                // Sales details section
                Paragraph detailsTitle = new Paragraph(_isItemWiseMode ? "ITEM-WISE SALES DETAILS" : "SALES DETAILS", headerFont);
                detailsTitle.SpacingAfter = 10f;
                document.Add(detailsTitle);

                PdfPTable salesTable;
                if (_isItemWiseMode)
                {
                    // Industry Standard Item-wise Sales Report PDF Table
                    salesTable = new PdfPTable(13);
                    salesTable.WidthPercentage = 100;
                    salesTable.SetWidths(new float[] { 0.8f, 1.8f, 1f, 0.6f, 0.8f, 0.9f, 0.8f, 0.9f, 0.9f, 0.8f, 0.7f, 0.7f, 0.9f });

                    // Add headers for item-wise mode (Industry Standard)
                    string[] headers = { "Code", "Item Name", "Category", "Qty", "Unit Price", "Total Sales", "Cost/Unit", "Total Cost", "Profit", "Margin %", "Discount", "Tax", "Net Sales" };
                    foreach (string header in headers)
                    {
                        PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                        headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        headerCell.Padding = 4f;
                        salesTable.AddCell(headerCell);
                    }

                    // Add data rows
                    foreach (var item in _salesReportItems)
                    {
                        salesTable.AddCell(new PdfPCell(new Phrase(item.ProductCode ?? "", smallFont)) { Padding = 2f });
                        salesTable.AddCell(new PdfPCell(new Phrase(item.ProductName.Length > 20 ? item.ProductName.Substring(0, 20) + "..." : item.ProductName, smallFont)) { Padding = 2f });
                        salesTable.AddCell(new PdfPCell(new Phrase(item.CategoryName ?? "", smallFont)) { Padding = 2f });
                        salesTable.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), smallFont)) { Padding = 2f, HorizontalAlignment = Element.ALIGN_CENTER });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.UnitPrice:F2}", smallFont)) { Padding = 2f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.SubTotal:F2}", smallFont)) { Padding = 2f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.CostPerUnit:F2}", smallFont)) { Padding = 2f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.TotalCost:F2}", smallFont)) { Padding = 2f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.GrossProfit:F2}", smallFont)) { Padding = 2f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.GrossProfitMargin:F2}", smallFont)) { Padding = 2f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.DiscountAmount:F2}", smallFont)) { Padding = 2f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.TaxAmount:F2}", smallFont)) { Padding = 2f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.NetSalesAmount:F2}", smallFont)) { Padding = 2f, HorizontalAlignment = Element.ALIGN_RIGHT });
                    }

                    // Add total row
                    if (_salesReportItems.Count > 0)
                    {
                        var totalSales = _salesReportItems.Sum(x => x.SubTotal);
                        var totalQuantity = _salesReportItems.Sum(x => x.Quantity);
                        var totalCost = _salesReportItems.Sum(x => x.TotalCost);
                        var totalProfit = _salesReportItems.Sum(x => x.GrossProfit);
                        var totalNetSales = _salesReportItems.Sum(x => x.NetSalesAmount);
                        var totalDiscount = _salesReportItems.Sum(x => x.DiscountAmount);
                        var totalTax = _salesReportItems.Sum(x => x.TaxAmount);
                        var avgProfitMargin = totalSales > 0 ? (totalProfit / totalSales) * 100 : 0;
                        
                        salesTable.AddCell(new PdfPCell(new Phrase("TOTAL", headerFont)) { Padding = 3f, Colspan = 3, HorizontalAlignment = Element.ALIGN_CENTER });
                        salesTable.AddCell(new PdfPCell(new Phrase(totalQuantity.ToString(), headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER });
                        salesTable.AddCell(new PdfPCell(new Phrase("", headerFont)) { Padding = 3f });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{totalSales:F2}", headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase("", headerFont)) { Padding = 3f });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{totalCost:F2}", headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{totalProfit:F2}", headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{avgProfitMargin:F2}", headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{totalDiscount:F2}", headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{totalTax:F2}", headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{totalNetSales:F2}", headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }
                else
                {
                    // Normal mode: Invoice-focused table
                    salesTable = new PdfPTable(8);
                    salesTable.WidthPercentage = 100;
                    salesTable.SetWidths(new float[] { 1.4f, 1.1f, 1.8f, 0.8f, 0.9f, 1f, 0.9f, 1f });

                    // Add headers
                    string[] headers = { "Invoice #", "Date", "Customer", "Qty", "Unit Price", "Sub Total", "Discount", "Total" };
                    foreach (string header in headers)
                    {
                        PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                        headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        headerCell.Padding = 5f;
                        salesTable.AddCell(headerCell);
                    }

                    // Add data rows
                    foreach (var item in _salesReportItems)
                    {
                        salesTable.AddCell(new PdfPCell(new Phrase(item.InvoiceNumber, smallFont)) { Padding = 3f });
                        salesTable.AddCell(new PdfPCell(new Phrase(item.SaleDate.ToString("yyyy-MM-dd"), smallFont)) { Padding = 3f });
                        salesTable.AddCell(new PdfPCell(new Phrase(item.CustomerName.Length > 20 ? item.CustomerName.Substring(0, 20) + "..." : item.CustomerName, smallFont)) { Padding = 3f });
                        salesTable.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.UnitPrice:F2}", smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.SubTotal:F2}", smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.DiscountAmount:F2}", smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{item.TotalAmount:F2}", smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                    }

                    // Add total row
                    if (_salesReportItems.Count > 0)
                    {
                        var totalQuantity = _salesReportItems.Sum(x => x.Quantity);
                        var totalSubTotal = _salesReportItems.Sum(x => x.SubTotal);
                        var totalDiscount = _salesReportItems.Sum(x => x.DiscountAmount);
                        var totalSales = _salesReportItems.Sum(x => x.TotalAmount);
                        
                        salesTable.AddCell(new PdfPCell(new Phrase("TOTAL", headerFont)) { Padding = 3f, Colspan = 3, HorizontalAlignment = Element.ALIGN_CENTER });
                        salesTable.AddCell(new PdfPCell(new Phrase(totalQuantity.ToString(), headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER });
                        salesTable.AddCell(new PdfPCell(new Phrase("", headerFont)) { Padding = 3f });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{totalSubTotal:F2}", headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{totalDiscount:F2}", headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                        salesTable.AddCell(new PdfPCell(new Phrase($"{totalSales:F2}", headerFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }

                document.Add(salesTable);

                // Footer
                Paragraph footer = new Paragraph($"\n\nReport generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}", smallFont);
                footer.Alignment = Element.ALIGN_CENTER;
                document.Add(footer);

                document.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating PDF: {ex.Message}");
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
        public string ProductCode { get; set; } // Added for item-wise report
        public string CategoryName { get; set; } // Added for item-wise report
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        
        // Item-wise report specific fields
        public decimal CostPerUnit { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal GrossProfitMargin { get; set; } // Percentage
        public decimal NetSalesAmount { get; set; } // After discounts and tax
    }
}
