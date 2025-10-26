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

        public ThermalInvoiceForm()
        {
            InitializeComponent();
            _saleRepository = new SaleRepository();
            _saleItems = new List<SaleItem>();
            
            // Set up fonts for thermal printing
            _headerFont = new Font("Courier New", 12, FontStyle.Bold);
            _bodyFont = new Font("Courier New", 9, FontStyle.Regular);
            _footerFont = new Font("Courier New", 10, FontStyle.Bold);
            
            SetupEventHandlers();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            btnLoadSale.Click += BtnLoadSale_Click;
            btnPrintInvoice.Click += BtnPrintInvoice_Click;
            btnPreviewInvoice.Click += BtnPreviewInvoice_Click;
            btnClose.Click += BtnClose_Click;
            txtInvoiceNumber.TextChanged += TxtInvoiceNumber_TextChanged;
        }

        private void SetInitialState()
        {
            txtInvoiceNumber.Clear();
            lblSaleInfo.Text = "Enter Invoice Number to Load Sale";
            btnPrintInvoice.Enabled = false;
            btnPreviewInvoice.Enabled = false;
        }

        private void TxtInvoiceNumber_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtInvoiceNumber.Text))
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

        private void BtnLoadSale_Click(object sender, EventArgs e)
        {
            try
            {
                string invoiceNumber = txtInvoiceNumber.Text.Trim();
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
                    btnPrintInvoice.Enabled = false;
                    btnPreviewInvoice.Enabled = false;
                    return;
                }

                _saleItems = _saleRepository.GetSaleItems(_currentSale.SaleID);
                _currentSale.SaleItems = _saleItems;

                // Update display
                lblSaleInfo.Text = $"Sale Found: {_currentSale.InvoiceNumber} - {_currentSale.SaleDate:MM/dd/yyyy HH:mm} - ${_currentSale.TotalAmount:F2}";
                btnPrintInvoice.Enabled = true;
                btnPreviewInvoice.Enabled = true;

                ShowMessage("Sale loaded successfully!", "Success", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading sale: {ex.Message}", "Error", MessageBoxIcon.Error);
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
            if (_currentPurchase != null && _purchaseItems != null)
            {
                // Purchase items
                foreach (var item in _purchaseItems)
                {
                    // Product name (truncate if too long)
                    string productName = item.ProductName.Length > 25 ? item.ProductName.Substring(0, 22) + "..." : item.ProductName;
                    g.DrawString(productName, _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;

                    // Quantity and price
                    string itemLine = $"Qty: {item.Quantity} x ${item.UnitPrice:F2} = ${item.SubTotal:F2}";
                    g.DrawString(itemLine, _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;
                }
            }
            else if (_currentSale != null && _saleItems != null)
            {
                // Sale items
                foreach (var item in _saleItems)
                {
                    // Product name (truncate if too long)
                    string productName = item.ProductName.Length > 25 ? item.ProductName.Substring(0, 22) + "..." : item.ProductName;
                    g.DrawString(productName, _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;

                    // Quantity and price
                    string itemLine = $"Qty: {item.Quantity} x ${item.UnitPrice:F2} = ${item.SubTotal:F2}";
                    g.DrawString(itemLine, _bodyFont, Brushes.Black, 0, _currentY);
                    _currentY += _lineHeight;
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
                txtInvoiceNumber.Text = purchase.InvoiceNumber;
                
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
                
                // Update form fields with purchase data
                lblInvoiceNumber.Text = _currentPurchase.InvoiceNumber;
                lblDate.Text = _currentPurchase.PurchaseDate.ToString("dd/MM/yyyy");
                lblSupplier.Text = _currentPurchase.SupplierName ?? "N/A";
                lblTotal.Text = _currentPurchase.TotalAmount.ToString("F2");
                
                // Success message removed to avoid annoying popups
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
            InitializeComponent();
            _sale = sale;
            _saleItems = sale.SaleItems;
            
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

