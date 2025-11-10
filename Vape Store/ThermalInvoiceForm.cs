using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;
using Vape_Store.Helpers;

namespace Vape_Store
{
    public partial class ThermalInvoiceForm : Form
    {
        private SaleRepository _saleRepository;
        private Sale _currentSale;
        private List<SaleItem> _saleItems;
        private Purchase _currentPurchase;
        private List<PurchaseItem> _purchaseItems;
        private Font _headerFont;
        private Font _bodyFont;
        private Font _footerFont;
        private int _paperWidth = 300; // 3 inch thermal printer width
        private int _currentY = 0;
        private int _lineHeight = 20;
        private Panel _thermalReceiptPanel;
        private Label _lblEnterInvoice;

        public ThermalInvoiceForm()
        {
            InitializeComponent();
            _saleRepository = new SaleRepository();
            _saleItems = new List<SaleItem>();
            
            // Set up fonts for thermal printing
            _headerFont = new Font("Courier New", 12, FontStyle.Bold);
            _bodyFont = new Font("Courier New", 9, FontStyle.Regular);
            _footerFont = new Font("Courier New", 10, FontStyle.Bold);
            
            SetupThermalReceiptPanel();
            SetupEventHandlers();
            LoadInvoiceNumbers();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            btnLoadSale.Click += BtnLoadSale_Click;
            btnPrintInvoice.Click += BtnPrintInvoice_Click;
            btnPreviewInvoice.Click += BtnPreviewInvoice_Click;
            btnClose.Click += BtnClose_Click;
            cmbInvoiceNumber.SelectedIndexChanged += CmbInvoiceNumber_SelectedIndexChanged;
            cmbInvoiceNumber.TextChanged += CmbInvoiceNumber_TextChanged;
            cmbInvoiceNumber.KeyDown += CmbInvoiceNumber_KeyDown;
        }

        private void LoadInvoiceNumbers()
        {
            try
            {
                var sales = _saleRepository.GetAllSales();
                var invoiceNumbers = sales.Select(s => s.InvoiceNumber).OrderByDescending(inv => inv).Distinct().ToList();
                
                // Make the ComboBox searchable
                if (invoiceNumbers.Count > 0)
                {
                    SearchableComboBoxHelper.MakeSearchable(cmbInvoiceNumber, invoiceNumbers);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading invoice numbers: {ex.Message}");
                ShowMessage($"Error loading invoice numbers: {ex.Message}", "Error", MessageBoxIcon.Warning);
            }
        }

        private void CmbInvoiceNumber_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cmbInvoiceNumber.Text))
            {
                btnLoadSale.Enabled = true;
            }
            else
            {
                btnLoadSale.Enabled = false;
                btnPrintInvoice.Enabled = false;
                btnPreviewInvoice.Enabled = false;
            }
        }

        private void CmbInvoiceNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInvoiceNumber.SelectedIndex >= 0 && !string.IsNullOrWhiteSpace(cmbInvoiceNumber.Text))
            {
                btnLoadSale.Enabled = true;
            }
        }

        private void CmbInvoiceNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnLoadSale.Enabled)
            {
                BtnLoadSale_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void SetInitialState()
        {
            cmbInvoiceNumber.Text = "";
            cmbInvoiceNumber.SelectedIndex = -1;
            lblSaleInfo.Text = "Enter Invoice Number to Load Sale";
            btnLoadSale.Enabled = false;
            btnPrintInvoice.Enabled = false;
            btnPreviewInvoice.Enabled = false;
            ClearSaleDisplay();
            ClearThermalReceipt();
        }

        private void BtnLoadSale_Click(object sender, EventArgs e)
        {
            try
            {
                string invoiceNumber = cmbInvoiceNumber.Text.Trim();
                if (string.IsNullOrWhiteSpace(invoiceNumber))
                {
                    ShowMessage("Please enter an invoice number.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                _currentSale = _saleRepository.GetSaleByInvoiceNumber(invoiceNumber);
                if (_currentSale == null)
                {
                    ShowMessage("Sale not found with the given invoice number.", "Not Found", MessageBoxIcon.Warning);
                    lblSaleInfo.Text = "Sale not found";
                    ClearSaleDisplay();
                    ClearThermalReceipt();
                    btnPrintInvoice.Enabled = false;
                    btnPreviewInvoice.Enabled = false;
                    return;
                }

                _saleItems = _saleRepository.GetSaleItems(_currentSale.SaleID);
                _currentSale.SaleItems = _saleItems ?? new List<SaleItem>();

                // Validate that we have items
                if (_saleItems == null || _saleItems.Count == 0)
                {
                    ShowMessage("Warning: No items found for this sale.", "No Items", MessageBoxIcon.Warning);
                    _saleItems = new List<SaleItem>();
                    _currentSale.SaleItems = _saleItems;
                }

                // Update display labels
                UpdateSaleDisplay();

                // Update thermal receipt display
                UpdateThermalReceipt();

                // Update info label
                lblSaleInfo.Text = $"Sale Found: {_currentSale.InvoiceNumber} - {_currentSale.SaleDate:MM/dd/yyyy HH:mm} - ${_currentSale.TotalAmount:F2}";
                
                // Enable buttons only if we have valid sale data AND items
                bool hasValidData = _currentSale != null && _saleItems != null && _saleItems.Count > 0;
                btnPrintInvoice.Enabled = hasValidData;
                btnPreviewInvoice.Enabled = hasValidData;
                
                // Debug output
                System.Diagnostics.Debug.WriteLine($"[ThermalInvoice] Sale loaded: {_currentSale.InvoiceNumber}, Items count: {(_saleItems?.Count ?? 0)}");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading sale: {ex.Message}", "Error", MessageBoxIcon.Error);
                ClearSaleDisplay();
                ClearThermalReceipt();
                btnPrintInvoice.Enabled = false;
                btnPreviewInvoice.Enabled = false;
                _currentSale = null;
                _saleItems = null;
            }
        }

        private void UpdateSaleDisplay()
        {
            if (_currentSale == null)
            {
                ClearSaleDisplay();
                return;
            }

            // Update labels with sale information
            lblInvoiceNumber.Text = $"Invoice Number: {_currentSale.InvoiceNumber}";
            lblDate.Text = $"Date: {_currentSale.SaleDate:MM/dd/yyyy HH:mm}";
            
            // Change "Supplier" to "Customer" for sales
            lblSupplier.Text = $"Customer: {(_currentSale.CustomerID > 0 ? (_currentSale.CustomerName ?? "N/A") : "Walk-in Customer")}";
            
            // Show total with currency formatting
            lblTotal.Text = $"Total: ${_currentSale.TotalAmount:F2}";
        }

        private void ClearSaleDisplay()
        {
            lblInvoiceNumber.Text = "Invoice Number:";
            lblDate.Text = "Date:";
            lblSupplier.Text = "Customer:";
            lblTotal.Text = "Total:";
        }

        private void SetupThermalReceiptPanel()
        {
            // Create thermal receipt panel
            _thermalReceiptPanel = new Panel();
            _thermalReceiptPanel.Location = new Point(230, 60);
            _thermalReceiptPanel.Size = new Size(350, 600);
            _thermalReceiptPanel.BackColor = Color.White;
            _thermalReceiptPanel.BorderStyle = BorderStyle.FixedSingle;
            _thermalReceiptPanel.AutoScroll = true;
            _thermalReceiptPanel.Paint += ThermalReceiptPanel_Paint;
            
            // Add label for instruction
            _lblEnterInvoice = new Label();
            _lblEnterInvoice.Location = new Point(12, 90);
            _lblEnterInvoice.Size = new Size(200, 13);
            _lblEnterInvoice.Text = "Enter Invoice Number to Load Sale";
            _lblEnterInvoice.ForeColor = Color.Gray;
            
            // Update form size to accommodate thermal receipt
            this.Size = new Size(600, 700);
            this.MinimumSize = new Size(600, 700);
            
            // Add controls to form
            this.Controls.Add(_thermalReceiptPanel);
            this.Controls.Add(_lblEnterInvoice);
            
            // Bring thermal receipt panel to front
            _thermalReceiptPanel.BringToFront();
        }

        private void ThermalReceiptPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_currentSale == null)
            {
                // Draw placeholder message
                StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("Enter Invoice Number\nto Load Sale", 
                    new Font("Courier New", 12, FontStyle.Regular), 
                    Brushes.Gray, 
                    new Rectangle(0, 0, _thermalReceiptPanel.Width, _thermalReceiptPanel.Height), 
                    centerFormat);
                return;
            }

            // Use _currentSale.SaleItems if _saleItems is null or empty
            if ((_saleItems == null || _saleItems.Count == 0) && 
                (_currentSale.SaleItems != null && _currentSale.SaleItems.Count > 0))
            {
                _saleItems = _currentSale.SaleItems;
            }

            DrawThermalReceipt(e.Graphics);
        }

        private void DrawThermalReceipt(Graphics g)
        {
            int paperWidth = _thermalReceiptPanel.Width - 20;
            int currentY = 10;
            int lineHeight = 18;
            int leftMargin = 10;
            int rightMargin = paperWidth - 10;

            // Use monospace font for thermal receipt look
            Font headerFont = new Font("Courier New", 11, FontStyle.Bold);
            Font bodyFont = new Font("Courier New", 9, FontStyle.Regular);
            Font footerFont = new Font("Courier New", 10, FontStyle.Bold);
            StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };

            // Header - Store Info
            g.DrawString("VAPE STORE", headerFont, Brushes.Black, 
                new Rectangle(leftMargin, currentY, paperWidth - leftMargin * 2, lineHeight), centerFormat);
            currentY += lineHeight;

            g.DrawString("Electronic Cigarettes & Accessories", bodyFont, Brushes.Black, 
                new Rectangle(leftMargin, currentY, paperWidth - leftMargin * 2, lineHeight), centerFormat);
            currentY += lineHeight;

            g.DrawString("123 Main Street, City, State 12345", bodyFont, Brushes.Black, 
                new Rectangle(leftMargin, currentY, paperWidth - leftMargin * 2, lineHeight), centerFormat);
            currentY += lineHeight;

            g.DrawString("Phone: (555) 123-4567", bodyFont, Brushes.Black, 
                new Rectangle(leftMargin, currentY, paperWidth - leftMargin * 2, lineHeight), centerFormat);
            currentY += lineHeight * 2;

            // Separator line
            g.DrawLine(Pens.Black, leftMargin, currentY, rightMargin, currentY);
            currentY += lineHeight;

            // Invoice Info
            g.DrawString($"Invoice: {_currentSale.InvoiceNumber}", bodyFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight;

            g.DrawString($"Date: {_currentSale.SaleDate:MM/dd/yyyy HH:mm}", bodyFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight;

            g.DrawString($"Cashier: {_currentSale.UserName ?? "System"}", bodyFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight * 2;

            // Separator line
            g.DrawLine(Pens.Black, leftMargin, currentY, rightMargin, currentY);
            currentY += lineHeight;

            // Customer Info
            string customerName = _currentSale.CustomerID > 0 
                ? (_currentSale.CustomerName ?? "Walk-in Customer") 
                : "Walk-in Customer";
            g.DrawString($"Customer: {customerName}", bodyFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight * 2;

            // Items Header
            g.DrawString("Items:", bodyFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight;

            g.DrawString(new string('-', 40), bodyFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight;

            // Items - use _currentSale.SaleItems if _saleItems is null or empty
            var itemsToDisplay = (_saleItems != null && _saleItems.Count > 0) ? _saleItems : 
                                 (_currentSale.SaleItems != null && _currentSale.SaleItems.Count > 0) ? _currentSale.SaleItems : 
                                 new List<SaleItem>();

            if (itemsToDisplay.Count == 0)
            {
                g.DrawString("No items found", bodyFont, Brushes.Red, leftMargin, currentY);
                currentY += lineHeight;
            }
            else
            {
                foreach (var item in itemsToDisplay)
                {
                    // Product name (truncate if too long)
                    string productName = item.ProductName ?? "Unknown Product";
                    if (productName.Length > 30)
                    {
                        productName = productName.Substring(0, 27) + "...";
                    }
                    g.DrawString(productName, bodyFont, Brushes.Black, leftMargin, currentY);
                    currentY += lineHeight;

                    // Quantity and price
                    string itemLine = $"  Qty: {item.Quantity} x ${item.UnitPrice:F2} = ${item.SubTotal:F2}";
                    g.DrawString(itemLine, bodyFont, Brushes.Black, leftMargin, currentY);
                    currentY += lineHeight;
                }
            }

            g.DrawString(new string('-', 40), bodyFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight;

            // Totals
            g.DrawString($"Subtotal: ${_currentSale.SubTotal:F2}", bodyFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight;

            // Discount
            if (_currentSale.DiscountAmount > 0)
            {
                string discountText = _currentSale.DiscountPercent > 0 
                    ? $"Discount ({_currentSale.DiscountPercent:F1}%): ${_currentSale.DiscountAmount:F2}"
                    : $"Discount: ${_currentSale.DiscountAmount:F2}";
                g.DrawString(discountText, bodyFont, Brushes.Black, leftMargin, currentY);
                currentY += lineHeight;
            }

            // Tax
            if (_currentSale.TaxAmount > 0)
            {
                g.DrawString($"Tax ({_currentSale.TaxPercent:F1}%): ${_currentSale.TaxAmount:F2}", bodyFont, Brushes.Black, leftMargin, currentY);
                currentY += lineHeight;
            }

            // Total
            g.DrawString($"TOTAL: ${_currentSale.TotalAmount:F2}", footerFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight * 2;

            // Payment Info
            g.DrawString($"Payment Method: {_currentSale.PaymentMethod}", bodyFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight;

            g.DrawString($"Amount Paid: ${_currentSale.PaidAmount:F2}", bodyFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight;

            if (_currentSale.ChangeAmount > 0)
            {
                g.DrawString($"Change: ${_currentSale.ChangeAmount:F2}", bodyFont, Brushes.Black, leftMargin, currentY);
                currentY += lineHeight;
            }

            currentY += lineHeight;
            g.DrawString(new string('-', 40), bodyFont, Brushes.Black, leftMargin, currentY);
            currentY += lineHeight;

            // Footer
            g.DrawString("Thank you for your business!", bodyFont, Brushes.Black, 
                new Rectangle(leftMargin, currentY, paperWidth - leftMargin * 2, lineHeight), centerFormat);
            currentY += lineHeight;

            g.DrawString("Please come again!", bodyFont, Brushes.Black, 
                new Rectangle(leftMargin, currentY, paperWidth - leftMargin * 2, lineHeight), centerFormat);

            // Clean up fonts
            headerFont.Dispose();
            bodyFont.Dispose();
            footerFont.Dispose();
        }

        private void UpdateThermalReceipt()
        {
            if (_thermalReceiptPanel != null)
            {
                _thermalReceiptPanel.Invalidate();
                _thermalReceiptPanel.Update();
            }
        }

        private void ClearThermalReceipt()
        {
            if (_thermalReceiptPanel != null)
            {
                _thermalReceiptPanel.Invalidate();
                _thermalReceiptPanel.Update();
            }
        }

        private void BtnPreviewInvoice_Click(object sender, EventArgs e)
        {
            if (_currentSale == null)
            {
                ShowMessage("Please load a sale first.", "No Sale", MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Create preview form
                InvoicePreviewForm previewForm = new InvoicePreviewForm(_currentSale);
                previewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error showing preview: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnPrintInvoice_Click(object sender, EventArgs e)
        {
            // Check if we have purchase data or sale data
            if (_currentPurchase == null && _currentSale == null)
            {
                ShowMessage("Please load data first.", "No Data", MessageBoxIcon.Warning);
                return;
            }

            // Ensure we have items for sale
            if (_currentSale != null)
            {
                // Ensure items are loaded
                if (_saleItems == null || _saleItems.Count == 0)
                {
                    if (_currentSale.SaleItems != null && _currentSale.SaleItems.Count > 0)
                    {
                        _saleItems = _currentSale.SaleItems;
                    }
                    else
                    {
                        // Try to reload items
                        try
                        {
                            _saleItems = _saleRepository.GetSaleItems(_currentSale.SaleID);
                            _currentSale.SaleItems = _saleItems;
                        }
                        catch (Exception ex)
                        {
                            ShowMessage($"Error loading items: {ex.Message}", "Error", MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                if (_saleItems == null || _saleItems.Count == 0)
                {
                    ShowMessage("No items found for this sale. Cannot print.", "No Items", MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrintPage += PrintInvoicePage;
                printDoc.DefaultPageSettings.PaperSize = new PaperSize("Thermal 3x5", _paperWidth, 600); // 3 inch width, 5 inch height
                printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDoc;
                printDialog.AllowSomePages = true;
                printDialog.AllowSelection = false;
                printDialog.AllowCurrentPage = false;
                printDialog.AllowPrintToFile = false;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDoc.Print();
                    // Success message removed to avoid annoying popups
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing invoice: {ex.Message}", "Print Error", MessageBoxIcon.Error);
            }
        }

        private void PrintInvoicePage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                _currentY = 0;

                // Print header
                PrintHeader(g);
                
                // Print business info
                PrintBusinessInfo(g);
                
                // Print customer info
                PrintCustomerInfo(g);
                
                // Print items
                PrintItems(g);
                
                // Print totals
                PrintTotals(g);
                
                // Print footer
                PrintFooter(g);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing: {ex.Message}", "Print Error", MessageBoxIcon.Error);
            }
        }

        private void PrintHeader(Graphics g)
        {
            string header = "VAPE STORE";
            string subHeader = "Electronic Cigarettes & Accessories";
            string address = "123 Main Street, City, State 12345";
            string phone = "Phone: (555) 123-4567";
            string email = "Email: info@vapestore.com";

            // Center align header
            StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };
            
            g.DrawString(header, _headerFont, Brushes.Black, new Rectangle(0, _currentY, _paperWidth, _lineHeight), centerFormat);
            _currentY += _lineHeight;
            
            g.DrawString(subHeader, _bodyFont, Brushes.Black, new Rectangle(0, _currentY, _paperWidth, _lineHeight), centerFormat);
            _currentY += _lineHeight;
            
            g.DrawString(address, _bodyFont, Brushes.Black, new Rectangle(0, _currentY, _paperWidth, _lineHeight), centerFormat);
            _currentY += _lineHeight;
            
            g.DrawString(phone, _bodyFont, Brushes.Black, new Rectangle(0, _currentY, _paperWidth, _lineHeight), centerFormat);
            _currentY += _lineHeight;
            
            g.DrawString(email, _bodyFont, Brushes.Black, new Rectangle(0, _currentY, _paperWidth, _lineHeight), centerFormat);
            _currentY += _lineHeight * 2;

            // Draw line
            g.DrawLine(Pens.Black, 0, _currentY, _paperWidth, _currentY);
            _currentY += _lineHeight;
        }

        private void PrintBusinessInfo(Graphics g)
        {
            string invoiceNumber, date, cashier;
            
            if (_currentPurchase != null)
            {
                // Purchase invoice
                invoiceNumber = $"Purchase Invoice: {_currentPurchase.InvoiceNumber}";
                date = $"Date: {_currentPurchase.PurchaseDate:MM/dd/yyyy HH:mm}";
                cashier = $"Entered By: {_currentPurchase.UserName ?? "System"}";
            }
            else
            {
                // Sale invoice
                invoiceNumber = $"Invoice: {_currentSale.InvoiceNumber}";
                date = $"Date: {_currentSale.SaleDate:MM/dd/yyyy HH:mm}";
                cashier = $"Cashier: {_currentSale.UserName ?? "System"}";
            }

            g.DrawString(invoiceNumber, _bodyFont, Brushes.Black, 0, _currentY);
            _currentY += _lineHeight;
            
            g.DrawString(date, _bodyFont, Brushes.Black, 0, _currentY);
            _currentY += _lineHeight;
            
            g.DrawString(cashier, _bodyFont, Brushes.Black, 0, _currentY);
            _currentY += _lineHeight * 2;

            // Draw line
            g.DrawLine(Pens.Black, 0, _currentY, _paperWidth, _currentY);
            _currentY += _lineHeight;
        }

        private void PrintCustomerInfo(Graphics g)
        {
            if (_currentPurchase != null)
            {
                // Purchase invoice - show supplier info
                string supplierName = $"Supplier: {_currentPurchase.SupplierName ?? "N/A"}";
                g.DrawString(supplierName, _bodyFont, Brushes.Black, 0, _currentY);
                _currentY += _lineHeight;
            }
            else if (_currentSale != null)
            {
                // Sale invoice - show customer info
                if (_currentSale.CustomerID > 0)
                {
                    string customerName = $"Customer: {_currentSale.CustomerName ?? "Walk-in Customer"}";
                    g.DrawString(customerName, _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;
                }
                else
                {
                    g.DrawString("Customer: Walk-in Customer", _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;
                }
            }
            
            _currentY += _lineHeight;
        }

        private void PrintItems(Graphics g)
        {
            // Header
            g.DrawString("Items:", _bodyFont, Brushes.Black, 0, _currentY);
            _currentY += _lineHeight;
            
            g.DrawString("----------------------------------------", _bodyFont, Brushes.Black, 0, _currentY);
            _currentY += _lineHeight;

            // Items - handle both sales and purchases
            if (_currentPurchase != null && _purchaseItems != null && _purchaseItems.Count > 0)
            {
                // Purchase items
                foreach (var item in _purchaseItems)
                {
                    // Product name (truncate if too long)
                    string productName = (item.ProductName ?? "Unknown").Length > 25 ? item.ProductName.Substring(0, 22) + "..." : item.ProductName;
                    g.DrawString(productName, _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;

                    // Quantity and price
                    string itemLine = $"Qty: {item.Quantity} x ${item.UnitPrice:F2} = ${item.SubTotal:F2}";
                    g.DrawString(itemLine, _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;
                }
            }
            else if (_currentSale != null)
            {
                // Use _currentSale.SaleItems if _saleItems is null or empty
                var itemsToPrint = (_saleItems != null && _saleItems.Count > 0) ? _saleItems : 
                                  (_currentSale.SaleItems != null && _currentSale.SaleItems.Count > 0) ? _currentSale.SaleItems : 
                                  new List<SaleItem>();

                if (itemsToPrint.Count == 0)
                {
                    g.DrawString("No items found", _bodyFont, Brushes.Red, 0, _currentY);
                    _currentY += _lineHeight;
                }
                else
                {
                    // Sale items
                    foreach (var item in itemsToPrint)
                    {
                        // Product name (truncate if too long)
                        string productName = (item.ProductName ?? "Unknown Product").Length > 25 ? item.ProductName.Substring(0, 22) + "..." : item.ProductName;
                        g.DrawString(productName, _bodyFont, Brushes.Black, 0, _currentY);
                        _currentY += _lineHeight;

                        // Quantity and price
                        string itemLine = $"Qty: {item.Quantity} x ${item.UnitPrice:F2} = ${item.SubTotal:F2}";
                        g.DrawString(itemLine, _bodyFont, Brushes.Black, 0, _currentY);
                        _currentY += _lineHeight;
                    }
                }
            }

            g.DrawString("----------------------------------------", _bodyFont, Brushes.Black, 0, _currentY);
            _currentY += _lineHeight;
        }

        private void PrintTotals(Graphics g)
        {
            if (_currentPurchase != null)
            {
                // Purchase totals
                g.DrawString($"Subtotal: ${_currentPurchase.SubTotal:F2}", _bodyFont, Brushes.Black, 0, _currentY);
                _currentY += _lineHeight;

                // Discount
                if (_currentPurchase.DiscountAmount > 0)
                {
                    g.DrawString($"Discount: ${_currentPurchase.DiscountAmount:F2}", _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;
                }

                // Tax
                if (_currentPurchase.TaxAmount > 0)
                {
                    g.DrawString($"Tax ({_currentPurchase.TaxPercent:F1}%): ${_currentPurchase.TaxAmount:F2}", _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;
                }

                // Total
                g.DrawString($"TOTAL: ${_currentPurchase.TotalAmount:F2}", _footerFont, Brushes.Black, 0, _currentY);
                _currentY += _lineHeight * 2;

                // Payment info
                g.DrawString($"Payment Method: {_currentPurchase.PaymentMethod}", _bodyFont, Brushes.Black, 0, _currentY);
                _currentY += _lineHeight;
                
                g.DrawString($"Amount Paid: ${_currentPurchase.PaidAmount:F2}", _bodyFont, Brushes.Black, 0, _currentY);
                _currentY += _lineHeight;
                
                // Calculate balance amount (Total - Paid)
                decimal balanceAmount = _currentPurchase.TotalAmount - _currentPurchase.PaidAmount;
                if (balanceAmount > 0)
                {
                    g.DrawString($"Balance: ${balanceAmount:F2}", _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;
                }
            }
            else if (_currentSale != null)
            {
                // Sale totals
                g.DrawString($"Subtotal: ${_currentSale.SubTotal:F2}", _bodyFont, Brushes.Black, 0, _currentY);
                _currentY += _lineHeight;

                // Discount
                if (_currentSale.DiscountAmount > 0)
                {
                    string discountText = _currentSale.DiscountPercent > 0 
                        ? $"Discount ({_currentSale.DiscountPercent:F1}%): ${_currentSale.DiscountAmount:F2}"
                        : $"Discount: ${_currentSale.DiscountAmount:F2}";
                    g.DrawString(discountText, _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;
                }

                // Tax
                if (_currentSale.TaxAmount > 0)
                {
                    g.DrawString($"Tax ({_currentSale.TaxPercent:F1}%): ${_currentSale.TaxAmount:F2}", _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;
                }

                // Total
                g.DrawString($"TOTAL: ${_currentSale.TotalAmount:F2}", _footerFont, Brushes.Black, 0, _currentY);
                _currentY += _lineHeight * 2;

                // Payment info
                g.DrawString($"Payment Method: {_currentSale.PaymentMethod}", _bodyFont, Brushes.Black, 0, _currentY);
                _currentY += _lineHeight;
                
                g.DrawString($"Amount Paid: ${_currentSale.PaidAmount:F2}", _bodyFont, Brushes.Black, 0, _currentY);
                _currentY += _lineHeight;
                
                if (_currentSale.ChangeAmount > 0)
                {
                    g.DrawString($"Change: ${_currentSale.ChangeAmount:F2}", _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;
                }
            }
        }

        private void PrintFooter(Graphics g)
        {
            _currentY += _lineHeight;
            
            g.DrawString("----------------------------------------", _bodyFont, Brushes.Black, 0, _currentY);
            _currentY += _lineHeight;
            
            g.DrawString("Thank you for your business!", _bodyFont, Brushes.Black, 0, _currentY);
            _currentY += _lineHeight;
            
            g.DrawString("Please come again!", _bodyFont, Brushes.Black, 0, _currentY);
            _currentY += _lineHeight;
            
            g.DrawString("", _bodyFont, Brushes.Black, 0, _currentY);
            _currentY += _lineHeight;
            
            g.DrawString("", _bodyFont, Brushes.Black, 0, _currentY);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        public void SetPurchaseData(Purchase purchase, List<PurchaseItem> purchaseItems)
        {
            try
            {
                _currentPurchase = purchase;
                _purchaseItems = purchaseItems;
                
                // Update form title
                this.Text = $"Purchase Invoice - {purchase.InvoiceNumber}";
                
                // Update invoice number field
                cmbInvoiceNumber.Text = purchase.InvoiceNumber;
                
                // Load purchase data
                LoadPurchaseData();
                
                // Don't show the form here - let the caller decide when to show it
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting purchase data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPurchaseData()
        {
            try
            {
                if (_currentPurchase == null || _purchaseItems == null)
                    return;
                
                // Update form fields with purchase data (matching format of UpdateSaleDisplay)
                lblInvoiceNumber.Text = $"Invoice Number: {_currentPurchase.InvoiceNumber}";
                lblDate.Text = $"Date: {_currentPurchase.PurchaseDate:MM/dd/yyyy HH:mm}";
                lblSupplier.Text = $"Supplier: {_currentPurchase.SupplierName ?? "N/A"}";
                lblTotal.Text = $"Total: ${_currentPurchase.TotalAmount:F2}";
                
                // Enable print and preview buttons
                btnPrintInvoice.Enabled = true;
                btnPreviewInvoice.Enabled = true;
                
                // Update info label
                lblSaleInfo.Text = $"Purchase Found: {_currentPurchase.InvoiceNumber} - {_currentPurchase.PurchaseDate:MM/dd/yyyy HH:mm} - ${_currentPurchase.TotalAmount:F2}";
                
                // Update thermal receipt for purchase
                UpdateThermalReceipt();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading purchase data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // Preview form for thermal invoice
    public partial class InvoicePreviewForm : Form
    {
        private Sale _sale;
        private List<SaleItem> _saleItems;

        public InvoicePreviewForm(Sale sale)
        {
            _sale = sale;
            _saleItems = sale?.SaleItems ?? new List<SaleItem>();
            
            SetupPreview();
        }

        private void SetupPreview()
        {
            this.Text = $"Invoice Preview - {_sale.InvoiceNumber}";
            this.Size = new Size(400, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            
            // Create preview panel
            Panel previewPanel = new Panel();
            previewPanel.Dock = DockStyle.Fill;
            previewPanel.AutoScroll = true;
            previewPanel.BackColor = Color.White;
            previewPanel.Paint += PreviewPanel_Paint;
            
            this.Controls.Add(previewPanel);
        }

        private void PreviewPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int currentY = 10;
            int lineHeight = 20;
            int paperWidth = 350;

            // Header
            StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };
            Font headerFont = new Font("Courier New", 12, FontStyle.Bold);
            Font bodyFont = new Font("Courier New", 9, FontStyle.Regular);
            Font footerFont = new Font("Courier New", 10, FontStyle.Bold);

            g.DrawString("VAPE STORE", headerFont, Brushes.Black, new Rectangle(0, currentY, paperWidth, lineHeight), centerFormat);
            currentY += lineHeight;
            
            g.DrawString("Electronic Cigarettes & Accessories", bodyFont, Brushes.Black, new Rectangle(0, currentY, paperWidth, lineHeight), centerFormat);
            currentY += lineHeight;
            
            g.DrawString("123 Main Street, City, State 12345", bodyFont, Brushes.Black, new Rectangle(0, currentY, paperWidth, lineHeight), centerFormat);
            currentY += lineHeight;
            
            g.DrawString("Phone: (555) 123-4567", bodyFont, Brushes.Black, new Rectangle(0, currentY, paperWidth, lineHeight), centerFormat);
            currentY += lineHeight * 2;

            g.DrawLine(Pens.Black, 0, currentY, paperWidth, currentY);
            currentY += lineHeight;

            // Invoice info
            g.DrawString($"Invoice: {_sale.InvoiceNumber}", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight;
            
            g.DrawString($"Date: {_sale.SaleDate:MM/dd/yyyy HH:mm}", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight;
            
            g.DrawString($"Cashier: {_sale.UserName ?? "System"}", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight * 2;

            g.DrawLine(Pens.Black, 0, currentY, paperWidth, currentY);
            currentY += lineHeight;

            // Customer
            g.DrawString($"Customer: {_sale.CustomerName ?? "Walk-in Customer"}", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight * 2;

            // Items
            g.DrawString("Items:", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight;
            
            g.DrawString("----------------------------------------", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight;

            foreach (var item in _saleItems)
            {
                string productName = item.ProductName.Length > 25 ? item.ProductName.Substring(0, 22) + "..." : item.ProductName;
                g.DrawString(productName, bodyFont, Brushes.Black, 0, currentY);
                currentY += lineHeight;

                string itemLine = $"Qty: {item.Quantity} x ${item.UnitPrice:F2} = ${item.SubTotal:F2}";
                g.DrawString(itemLine, bodyFont, Brushes.Black, 0, currentY);
                currentY += lineHeight;
            }

            g.DrawString("----------------------------------------", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight;

            // Totals
            g.DrawString($"Subtotal: ${_sale.SubTotal:F2}", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight;

            // Discount
            if (_sale.DiscountAmount > 0)
            {
                string discountText = _sale.DiscountPercent > 0 
                    ? $"Discount ({_sale.DiscountPercent:F1}%): ${_sale.DiscountAmount:F2}"
                    : $"Discount: ${_sale.DiscountAmount:F2}";
                g.DrawString(discountText, bodyFont, Brushes.Black, 0, currentY);
                currentY += lineHeight;
            }

            if (_sale.TaxAmount > 0)
            {
                g.DrawString($"Tax ({_sale.TaxPercent:F1}%): ${_sale.TaxAmount:F2}", bodyFont, Brushes.Black, 0, currentY);
                currentY += lineHeight;
            }

            g.DrawString($"TOTAL: ${_sale.TotalAmount:F2}", footerFont, Brushes.Black, 0, currentY);
            currentY += lineHeight * 2;

            g.DrawString($"Payment Method: {_sale.PaymentMethod}", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight;
            
            g.DrawString($"Amount Paid: ${_sale.PaidAmount:F2}", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight;
            
            if (_sale.ChangeAmount > 0)
            {
                g.DrawString($"Change: ${_sale.ChangeAmount:F2}", bodyFont, Brushes.Black, 0, currentY);
                currentY += lineHeight;
            }

            currentY += lineHeight;
            g.DrawString("----------------------------------------", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight;
            
            g.DrawString("Thank you for your business!", bodyFont, Brushes.Black, 0, currentY);
            currentY += lineHeight;
            
            g.DrawString("Please come again!", bodyFont, Brushes.Black, 0, currentY);
        }

    }
}

