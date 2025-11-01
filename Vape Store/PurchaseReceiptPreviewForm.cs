using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Services;

namespace Vape_Store
{
    public partial class PurchaseReceiptPreviewForm : Form
    {
        private Purchase _purchase;
        private List<PurchaseItem> _purchaseItems;
        private ThermalReceiptService _receiptService;

        public PurchaseReceiptPreviewForm(Purchase purchase, List<PurchaseItem> purchaseItems)
        {
            InitializeComponent();
            _purchase = purchase;
            _purchaseItems = purchaseItems ?? new List<PurchaseItem>();
            _receiptService = new ThermalReceiptService();
            
            // Wire up event handlers after InitializeComponent
            WireUpEventHandlers();
            
            LoadReceiptPreview();
        }

        private void WireUpEventHandlers()
        {
            // Find buttons and wire up events - more robust approach
            try
            {
                // Find the main panel first
                Panel mainPanel = null;
                foreach (Control control in this.Controls)
                {
                    if (control is Panel panel)
                    {
                        mainPanel = panel;
                        break;
                    }
                }

                if (mainPanel != null)
                {
                    // Find the button panel
                    Panel buttonPanel = null;
                    foreach (Control control in mainPanel.Controls)
                    {
                        if (control is Panel panel && panel.BackColor == System.Drawing.Color.LightGray)
                        {
                            buttonPanel = panel;
                            break;
                        }
                    }

                    if (buttonPanel != null)
                    {
                        // Wire up button events
                        foreach (Control control in buttonPanel.Controls)
                        {
                            if (control is Button button)
                            {
                                switch (button.Text)
                                {
                                    case "Print Receipt":
                                        button.Click += BtnPrint_Click;
                                        break;
                                    case "Save PDF":
                                        button.Click += BtnSavePDF_Click;
                                        break;
                                    case "Close":
                                        button.Click += BtnClose_Click;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error wiring up event handlers: {ex.Message}");
            }
        }


        private void LoadReceiptPreview()
        {
            // Create a preview of the receipt with simple A4 design
            try
            {
                // Find the receipt panel more robustly
                Panel receiptPanel = null;
                
                // Find the main panel first
                foreach (Control control in this.Controls)
                {
                    if (control is Panel mainPanel)
                    {
                        // Find the receipt panel within main panel
                        foreach (Control subControl in mainPanel.Controls)
                        {
                            if (subControl is Panel panel && panel.AutoScroll == true)
                            {
                                receiptPanel = panel;
                                break;
                            }
                        }
                        break;
                    }
                }

                if (receiptPanel != null)
                {
                    receiptPanel.Paint += ReceiptPanel_Paint;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading receipt preview: {ex.Message}");
            }
        }

        private void ReceiptPanel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // A4 dimensions and margins
                float pageWidth = 720; // A4 width in pixels at 96 DPI
                float leftMargin = 50;
                float rightMargin = pageWidth - 50;
                float centerX = pageWidth / 2;
                float yPosition = 30;

                // Simple header
                DrawSimpleHeader(g, leftMargin, rightMargin, ref yPosition);

                // Invoice details section
                DrawSimpleInvoiceDetails(g, leftMargin, rightMargin, ref yPosition);

                // Supplier information section
                DrawSimpleSupplierInfo(g, leftMargin, rightMargin, ref yPosition);

                // Items table with simple styling
                DrawSimpleItemsTable(g, leftMargin, rightMargin, ref yPosition);

                // Financial summary with simple design
                DrawSimpleFinancialSummary(g, leftMargin, rightMargin, ref yPosition);

                // Payment details
                DrawSimplePaymentDetails(g, leftMargin, rightMargin, ref yPosition);

                // Footer with simple branding
                DrawSimpleFooter(g, leftMargin, rightMargin, ref yPosition);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating receipt: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DrawSimpleHeader(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            // Simple header - no background colors
            float centerX = (leftMargin + rightMargin) / 2;
            
            // Company name centered
            using (var blackBrush = new SolidBrush(Color.Black))
            {
                g.DrawString("VAPE STORE", new Font("Arial", 24, FontStyle.Bold), blackBrush, centerX - 100, yPosition);
                g.DrawString("123 Main Street, City, State 12345", new Font("Arial", 10), blackBrush, centerX - 120, yPosition + 30);
                g.DrawString("Phone: (555) 123-4567 | Email: info@vapestore.com", new Font("Arial", 9), blackBrush, centerX - 150, yPosition + 45);
            }

            yPosition += 80;
            
            // Simple line separator
            g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
            yPosition += 20;
        }

        private void DrawSimpleInvoiceDetails(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            // Simple invoice details - no background colors
            using (var blackBrush = new SolidBrush(Color.Black))
            {
                g.DrawString("PURCHASE INVOICE", new Font("Arial", 14, FontStyle.Bold), blackBrush, leftMargin, yPosition);
                g.DrawString($"Invoice #: {_purchase.InvoiceNumber}", new Font("Arial", 11, FontStyle.Bold), blackBrush, leftMargin, yPosition + 25);
                g.DrawString($"Date: {_purchase.PurchaseDate:MMM dd, yyyy 'at' hh:mm tt}", new Font("Arial", 10), blackBrush, leftMargin, yPosition + 45);
                
                if (!string.IsNullOrEmpty(_purchase.UserName))
                {
                    g.DrawString($"Entered by: {_purchase.UserName}", new Font("Arial", 10), blackBrush, rightMargin - 200, yPosition + 25);
                }
            }

            yPosition += 80;
            
            // Simple line separator
            g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
            yPosition += 20;
        }

        private void DrawSimpleSupplierInfo(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            if (!string.IsNullOrEmpty(_purchase.SupplierName))
            {
                // Simple supplier info - no background colors
                using (var blackBrush = new SolidBrush(Color.Black))
                {
                    g.DrawString("SUPPLIER INFORMATION", new Font("Arial", 12, FontStyle.Bold), blackBrush, leftMargin, yPosition);
                    g.DrawString($"Supplier: {_purchase.SupplierName}", new Font("Arial", 11), blackBrush, leftMargin, yPosition + 25);
                }

                yPosition += 60;
                
                // Simple line separator
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 20;
            }
        }

        private void DrawSimpleItemsTable(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            // Simple table headers - no background colors
            float[] columnPositions = { leftMargin, leftMargin + 300, leftMargin + 400, leftMargin + 500 };
            string[] headers = { "PRODUCT", "QTY", "UNIT PRICE", "TOTAL" };

            using (var blackBrush = new SolidBrush(Color.Black))
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    g.DrawString(headers[i], new Font("Arial", 10, FontStyle.Bold), blackBrush, columnPositions[i], yPosition);
                }
            }

            yPosition += 25;
            
            // Simple line separator
            g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
            yPosition += 15;

            // Items rows - simple alternating background
            bool isEven = false;
            foreach (var item in _purchaseItems)
            {
                // Simple alternating row background
                if (isEven)
                {
                    using (var lightBrush = new SolidBrush(Color.FromArgb(248, 248, 248)))
                    {
                        g.FillRectangle(lightBrush, leftMargin, yPosition - 5, rightMargin - leftMargin, 25);
                    }
                }

                // Row content
                using (var blackBrush = new SolidBrush(Color.Black))
                {
                    g.DrawString(item.ProductName, new Font("Arial", 10), blackBrush, columnPositions[0], yPosition);
                    g.DrawString(item.Quantity.ToString(), new Font("Arial", 10), blackBrush, columnPositions[1], yPosition);
                    g.DrawString(item.UnitPrice.ToString("F2"), new Font("Arial", 10), blackBrush, columnPositions[2], yPosition);
                    g.DrawString(item.SubTotal.ToString("F2"), new Font("Arial", 10, FontStyle.Bold), blackBrush, columnPositions[3], yPosition);
                }

                yPosition += 25;
                isEven = !isEven;
            }

            yPosition += 20;
        }

        private void DrawSimpleFinancialSummary(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            // Simple financial summary - no background colors
            float summaryX = rightMargin - 300;
            float summaryY = yPosition;

            using (var blackBrush = new SolidBrush(Color.Black))
            {
                // Subtotal
                g.DrawString("Subtotal:", new Font("Arial", 12), blackBrush, summaryX, summaryY);
                g.DrawString(_purchase.SubTotal.ToString("F2"), new Font("Arial", 12, FontStyle.Bold), blackBrush, summaryX + 150, summaryY);
                summaryY += 25;

                // Tax
                if (_purchase.TaxAmount > 0)
                {
                    g.DrawString($"Tax ({_purchase.TaxPercent:F1}%):", new Font("Arial", 12), blackBrush, summaryX, summaryY);
                    g.DrawString(_purchase.TaxAmount.ToString("F2"), new Font("Arial", 12, FontStyle.Bold), blackBrush, summaryX + 150, summaryY);
                    summaryY += 25;
                }

                // Discount
                decimal discountAmount = _purchase.SubTotal + _purchase.TaxAmount - _purchase.TotalAmount;
                if (discountAmount > 0)
                {
                    g.DrawString("Discount:", new Font("Arial", 12), blackBrush, summaryX, summaryY);
                    g.DrawString($"-{discountAmount:F2}", new Font("Arial", 12, FontStyle.Bold), blackBrush, summaryX + 150, summaryY);
                    summaryY += 25;
                }

                // Total with emphasis
                summaryY += 10;
                g.DrawString("TOTAL:", new Font("Arial", 16, FontStyle.Bold), blackBrush, summaryX, summaryY);
                g.DrawString(_purchase.TotalAmount.ToString("F2"), new Font("Arial", 16, FontStyle.Bold), blackBrush, summaryX + 150, summaryY);
            }

            yPosition += 120;
        }

        private void DrawSimplePaymentDetails(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            // Simple payment details - no background colors
            using (var blackBrush = new SolidBrush(Color.Black))
            {
                g.DrawString("PAYMENT DETAILS", new Font("Arial", 12, FontStyle.Bold), blackBrush, leftMargin, yPosition);
                g.DrawString($"Payment Method: {_purchase.PaymentMethod.ToUpper()}", new Font("Arial", 11), blackBrush, leftMargin, yPosition + 25);
                g.DrawString($"Amount Paid: {_purchase.PaidAmount:F2}", new Font("Arial", 11), blackBrush, leftMargin, yPosition + 45);

                decimal balanceAmount = _purchase.TotalAmount - _purchase.PaidAmount;
                if (balanceAmount > 0)
                {
                    g.DrawString($"Balance Due: {balanceAmount:F2}", new Font("Arial", 11, FontStyle.Bold), blackBrush, leftMargin, yPosition + 65);
                }
                else if (balanceAmount < 0)
                {
                    g.DrawString($"Overpaid: {Math.Abs(balanceAmount):F2}", new Font("Arial", 11, FontStyle.Bold), blackBrush, leftMargin, yPosition + 65);
                }
            }

            yPosition += 100;
        }

        private void DrawSimpleFooter(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            // Simple footer - no background colors
            yPosition += 20;
            
            // Simple line separator
            g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
            yPosition += 20;

            using (var blackBrush = new SolidBrush(Color.Black))
            {
                g.DrawString("Thank you for your business!", new Font("Arial", 12, FontStyle.Bold), blackBrush, leftMargin, yPosition);
                g.DrawString("Generated on " + DateTime.Now.ToString("MMM dd, yyyy 'at' hh:mm tt"), new Font("Arial", 9), blackBrush, leftMargin, yPosition + 20);
                g.DrawString("Purchase Receipt #" + _purchase.InvoiceNumber, new Font("Arial", 9), blackBrush, leftMargin, yPosition + 35);
            }
        }

        private float DrawText(Graphics g, string text, Font font, float x, float y)
        {
            g.DrawString(text, font, Brushes.Black, x, y);
            return y + font.Height;
        }

        private float DrawCenteredText(Graphics g, string text, Font font, float centerX, float y)
        {
            SizeF textSize = g.MeasureString(text, font);
            float x = centerX - (textSize.Width / 2);
            g.DrawString(text, font, Brushes.Black, x, y);
            return y + font.Height;
        }

        private void DrawDashedLine(Graphics g, float x1, float y1, float x2, float y2)
        {
            using (Pen dashedPen = new Pen(Color.Black, 1))
            {
                dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawLine(dashedPen, x1, y1, x2, y2);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // Convert purchase to sale format for printing (since ThermalReceiptService expects Sale)
                var sale = new Sale
                {
                    InvoiceNumber = _purchase.InvoiceNumber,
                    SaleDate = _purchase.PurchaseDate,
                    SubTotal = _purchase.SubTotal,
                    TaxAmount = _purchase.TaxAmount,
                    TaxPercent = _purchase.TaxPercent,
                    TotalAmount = _purchase.TotalAmount,
                    PaymentMethod = _purchase.PaymentMethod,
                    PaidAmount = _purchase.PaidAmount,
                    ChangeAmount = _purchase.TotalAmount - _purchase.PaidAmount,
                    UserName = _purchase.UserName,
                    SaleItems = _purchaseItems.Select(pi => new SaleItem
                    {
                        ProductName = pi.ProductName,
                        Quantity = pi.Quantity,
                        UnitPrice = pi.UnitPrice,
                        SubTotal = pi.SubTotal
                    }).ToList()
                };
                
                _receiptService.PrintReceipt(sale);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing receipt: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSavePDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"Purchase_Receipt_{_purchase.InvoiceNumber}_{DateTime.Now:yyyyMMdd}.pdf",
                    Title = "Save Purchase Receipt as PDF"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // TODO: Implement PDF generation
                    MessageBox.Show("PDF generation feature will be implemented soon!", "Feature Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving PDF: {ex.Message}", "PDF Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Helper method to draw rounded rectangles
        private void FillRoundedRectangle(Graphics g, Brush brush, float x, float y, float width, float height, float radius)
        {
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddArc(x, y, radius * 2, radius * 2, 180, 90);
                path.AddArc(x + width - radius * 2, y, radius * 2, radius * 2, 270, 90);
                path.AddArc(x + width - radius * 2, y + height - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(x, y + height - radius * 2, radius * 2, radius * 2, 90, 90);
                path.CloseFigure();
                
                g.FillPath(brush, path);
            }
        }
    }
}