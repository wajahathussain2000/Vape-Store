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
            LoadReceiptPreview();
        }


        private void LoadReceiptPreview()
        {
            // Create a preview of the receipt - same as sales form
            var previewPanel = this.Controls[0] as Panel;
            if (previewPanel != null)
            {
                previewPanel.Paint += PreviewPanel_Paint;
            }
        }

        private void PreviewPanel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                float yPosition = 20;
                float leftMargin = 25;
                float rightMargin = 400;
                float centerX = 225;

                // Store header with better formatting
                yPosition = DrawCenteredText(g, "VAPE STORE", new Font("Arial", 18, FontStyle.Bold), centerX, yPosition);
                yPosition += 5;
                yPosition = DrawCenteredText(g, "123 Main Street, City, State 12345", new Font("Arial", 9), centerX, yPosition);
                yPosition = DrawCenteredText(g, "Phone: (555) 123-4567", new Font("Arial", 9), centerX, yPosition);
                yPosition = DrawCenteredText(g, "info@vapestore.com", new Font("Arial", 9), centerX, yPosition);
                
                yPosition += 15;
                DrawDashedLine(g, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 15;

                // Purchase invoice details with better alignment
                yPosition = DrawText(g, "PURCHASE INVOICE #: " + _purchase.InvoiceNumber, new Font("Arial", 11, FontStyle.Bold), leftMargin, yPosition);
                yPosition = DrawText(g, "DATE: " + _purchase.PurchaseDate.ToString("MM/dd/yyyy hh:mm tt"), new Font("Arial", 9), leftMargin, yPosition);
                
                // Add user information
                if (!string.IsNullOrEmpty(_purchase.UserName))
                {
                    yPosition = DrawText(g, "ENTERED BY: " + _purchase.UserName, new Font("Arial", 9), leftMargin, yPosition);
                }
                
                // Add supplier information
                if (!string.IsNullOrEmpty(_purchase.SupplierName))
                {
                    yPosition = DrawText(g, "SUPPLIER: " + _purchase.SupplierName, new Font("Arial", 9), leftMargin, yPosition);
                }
                
                yPosition += 10;
                DrawDashedLine(g, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 15;

                // Items header with better alignment - all on same line
                float itemCol = leftMargin;
                float qtyCol = leftMargin + 130;
                float priceCol = leftMargin + 200;
                float totalCol = leftMargin + 300;
                
                // Draw all headers at the same Y position
                DrawText(g, "ITEM", new Font("Arial", 9, FontStyle.Bold), itemCol, yPosition);
                DrawText(g, "QTY", new Font("Arial", 9, FontStyle.Bold), qtyCol, yPosition);
                DrawText(g, "PRICE", new Font("Arial", 9, FontStyle.Bold), priceCol, yPosition);
                DrawText(g, "TOTAL", new Font("Arial", 9, FontStyle.Bold), totalCol, yPosition);
                
                // Move to next line after drawing all headers
                yPosition += new Font("Arial", 9, FontStyle.Bold).Height;
                
                yPosition += 5;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 10;

                // Items with better formatting - all columns on same line
                foreach (var item in _purchaseItems)
                {
                    string productName = item.ProductName;
                    if (productName.Length > 18)
                    {
                        productName = productName.Substring(0, 15) + "...";
                    }
                    
                    // Draw all item details at the same Y position
                    DrawText(g, productName, new Font("Arial", 8), itemCol, yPosition);
                    DrawText(g, item.Quantity.ToString(), new Font("Arial", 8), qtyCol, yPosition);
                    DrawText(g, item.UnitPrice.ToString("F2"), new Font("Arial", 8), priceCol, yPosition);
                    DrawText(g, item.SubTotal.ToString("F2"), new Font("Arial", 8), totalCol, yPosition);
                    
                    // Move to next line for next item
                    yPosition += new Font("Arial", 8).Height + 3;
                }

                yPosition += 10;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 15;

                // Financial summary with better alignment
                float labelCol = leftMargin + 220;
                float valueCol = leftMargin + 300;
                
                // Subtotal
                DrawText(g, "SUBTOTAL:", new Font("Arial", 9, FontStyle.Bold), labelCol, yPosition);
                DrawText(g, _purchase.SubTotal.ToString("F2"), new Font("Arial", 9), valueCol, yPosition);
                yPosition += new Font("Arial", 9).Height;
                
                // Tax
                if (_purchase.TaxAmount > 0)
                {
                    DrawText(g, "TAX (" + _purchase.TaxPercent.ToString("F1") + "%):", new Font("Arial", 9, FontStyle.Bold), labelCol, yPosition);
                    DrawText(g, _purchase.TaxAmount.ToString("F2"), new Font("Arial", 9), valueCol, yPosition);
                    yPosition += new Font("Arial", 9).Height;
                }
                
                // Discount (if applicable)
                decimal discountAmount = _purchase.SubTotal + _purchase.TaxAmount - _purchase.TotalAmount;
                if (discountAmount > 0)
                {
                    DrawText(g, "DISCOUNT:", new Font("Arial", 9, FontStyle.Bold), labelCol, yPosition);
                    DrawText(g, "-" + discountAmount.ToString("F2"), new Font("Arial", 9), valueCol, yPosition);
                    yPosition += new Font("Arial", 9).Height;
                }
                
                yPosition += 10;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 15;

                // Total with emphasis
                DrawText(g, "TOTAL:", new Font("Arial", 12, FontStyle.Bold), labelCol, yPosition);
                DrawText(g, _purchase.TotalAmount.ToString("F2"), new Font("Arial", 12, FontStyle.Bold), valueCol, yPosition);
                yPosition += new Font("Arial", 12, FontStyle.Bold).Height;
                
                yPosition += 15;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 15;

                // Payment details with better formatting
                yPosition = DrawText(g, "PAYMENT METHOD: " + _purchase.PaymentMethod.ToUpper(), new Font("Arial", 9, FontStyle.Bold), leftMargin, yPosition);
                yPosition = DrawText(g, "AMOUNT PAID: $" + _purchase.PaidAmount.ToString("F2"), new Font("Arial", 9), leftMargin, yPosition);
                
                // Calculate balance amount
                decimal balanceAmount = _purchase.TotalAmount - _purchase.PaidAmount;
                if (balanceAmount > 0)
                {
                    yPosition = DrawText(g, "BALANCE DUE: $" + balanceAmount.ToString("F2"), new Font("Arial", 9, FontStyle.Bold), leftMargin, yPosition);
                }
                else if (balanceAmount < 0)
                {
                    yPosition = DrawText(g, "OVERPAID: $" + Math.Abs(balanceAmount).ToString("F2"), new Font("Arial", 9, FontStyle.Bold), leftMargin, yPosition);
                }

                yPosition += 20;

                // Additional receipt information
                yPosition = DrawCenteredText(g, "Purchase Receipt #" + _purchase.InvoiceNumber, new Font("Arial", 8), centerX, yPosition);
                yPosition = DrawCenteredText(g, "Generated: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), new Font("Arial", 8), centerX, yPosition);
                yPosition += 10;

                // Footer with better styling
                yPosition = DrawCenteredText(g, "Thank you for your business!", new Font("Arial", 9, FontStyle.Bold), centerX, yPosition);
                yPosition = DrawCenteredText(g, "Please come again!", new Font("Arial", 8), centerX, yPosition);
                yPosition += 10;
                yPosition = DrawCenteredText(g, "---", new Font("Arial", 8), centerX, yPosition);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating preview: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void BtnPrintDirect_Click(object sender, EventArgs e)
        {
            try
            {
                // Convert purchase to sale format for printing
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
                
                _receiptService.PrintReceiptDirect(sale);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing receipt: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}