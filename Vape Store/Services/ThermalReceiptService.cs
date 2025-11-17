using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using Vape_Store.Models;

namespace Vape_Store.Services
{
    public class ThermalReceiptService
    {
        private Sale _currentSale;
        private PrintDocument _printDocument;
        private Font _headerFont;
        private Font _bodyFont;
        private Font _footerFont;
        private Font _titleFont;

        public ThermalReceiptService()
        {
            InitializeFonts();
            _printDocument = new PrintDocument();
            _printDocument.PrintPage += PrintDocument_PrintPage;
        }

        private void InitializeFonts()
        {
            _titleFont = new Font("Arial", 14, FontStyle.Bold);
            _headerFont = new Font("Arial", 10, FontStyle.Bold);
            _bodyFont = new Font("Arial", 9, FontStyle.Regular);
            _footerFont = new Font("Arial", 8, FontStyle.Regular);
        }

        public void PrintReceipt(Sale sale)
        {
            try
            {
                _currentSale = sale;
                
                // Configure printer settings for thermal printer
                var printDialog = new PrintDialog();
                printDialog.Document = _printDocument;
                
                // Set paper size for thermal receipt (typically 80mm width)
                var paperSize = new PaperSize("Thermal Receipt", 315, 0); // 80mm = 315 hundredths of an inch
                _printDocument.DefaultPageSettings.PaperSize = paperSize;
                _printDocument.DefaultPageSettings.Margins = new Margins(20, 10, 10, 10);
                
                // Show print dialog
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    _printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing receipt: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void PrintReceiptDirect(Sale sale)
        {
            try
            {
                _currentSale = sale;
                
                // Print directly to default printer
                _printDocument.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing receipt: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                float yPosition = 10;
                float leftMargin = 20;
                float rightMargin = e.PageBounds.Width - 10;
                float centerX = e.PageBounds.Width / 2;

                // Store name and header
                string storeName = "MADNI MOBILE & PHOTOSTATE";
                string storeAddress = "Shop #3, opp Save Mart, main Tulsa road, lalazar, RWP";
                string storePhone = "Ph: 0345-5518744";
                //string storeEmail = "info@vapestore.com";

                // Draw store header
                yPosition = DrawCenteredText(g, storeName, _titleFont, centerX, yPosition);
                yPosition += 5;
                yPosition = DrawCenteredText(g, storeAddress, _bodyFont, centerX, yPosition);
                yPosition = DrawCenteredText(g, storePhone, _bodyFont, centerX, yPosition);
                //yPosition = DrawCenteredText(g, storeEmail, _bodyFont, centerX, yPosition);
                
                // Draw separator line
                yPosition += 10;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 10;

                // Invoice details
                yPosition = DrawText(g, "INVOICE #: " + _currentSale.InvoiceNumber, _headerFont, leftMargin, yPosition);
                yPosition = DrawText(g, "DATE: " + _currentSale.SaleDate.ToString("MM/dd/yyyy hh:mm tt"), _bodyFont, leftMargin, yPosition);
                
                if (_currentSale.CustomerID > 0)
                {
                    yPosition = DrawText(g, "CUSTOMER: " + GetCustomerName(_currentSale.CustomerID), _bodyFont, leftMargin, yPosition);
                }
                
                yPosition += 5;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 10;

                // Items header
                yPosition = DrawText(g, "ITEM", _headerFont, leftMargin, yPosition);
                yPosition = DrawText(g, "QTY", _headerFont, leftMargin + 200, yPosition);
                yPosition = DrawText(g, "PRICE", _headerFont, leftMargin + 250, yPosition);
                yPosition = DrawText(g, "TOTAL", _headerFont, leftMargin + 300, yPosition);
                
                yPosition += 5;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 10;

                // Items
                foreach (var item in _currentSale.SaleItems)
                {
                    // Product name (wrap if too long)
                    string productName = item.ProductName;
                    if (productName.Length > 20)
                    {
                        productName = productName.Substring(0, 17) + "...";
                    }
                    
                    yPosition = DrawText(g, productName, _bodyFont, leftMargin, yPosition);
                    yPosition = DrawText(g, item.Quantity.ToString(), _bodyFont, leftMargin + 200, yPosition);
                    yPosition = DrawText(g, "$" + item.UnitPrice.ToString("F2"), _bodyFont, leftMargin + 250, yPosition);
                    yPosition = DrawText(g, "$" + item.SubTotal.ToString("F2"), _bodyFont, leftMargin + 300, yPosition);
                    yPosition += 5;
                }

                yPosition += 5;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 10;

                // Totals
                yPosition = DrawText(g, "SUBTOTAL:", _headerFont, leftMargin + 200, yPosition);
                yPosition = DrawText(g, "$" + _currentSale.SubTotal.ToString("F2"), _bodyFont, leftMargin + 300, yPosition);
                
                if (_currentSale.TaxAmount > 0)
                {
                    yPosition = DrawText(g, "TAX (" + _currentSale.TaxPercent.ToString("F1") + "%):", _headerFont, leftMargin + 200, yPosition);
                    yPosition = DrawText(g, "$" + _currentSale.TaxAmount.ToString("F2"), _bodyFont, leftMargin + 300, yPosition);
                }
                
                yPosition += 5;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 10;

                // Total
                yPosition = DrawText(g, "TOTAL:", _titleFont, leftMargin + 200, yPosition);
                yPosition = DrawText(g, "$" + _currentSale.TotalAmount.ToString("F2"), _titleFont, leftMargin + 300, yPosition);
                
                yPosition += 10;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 10;

                // Payment details
                yPosition = DrawText(g, "PAYMENT METHOD: " + _currentSale.PaymentMethod, _bodyFont, leftMargin, yPosition);
                yPosition = DrawText(g, "PAID: $" + _currentSale.PaidAmount.ToString("F2"), _bodyFont, leftMargin, yPosition);
                
                if (_currentSale.ChangeAmount > 0)
                {
                    yPosition = DrawText(g, "CHANGE: $" + _currentSale.ChangeAmount.ToString("F2"), _bodyFont, leftMargin, yPosition);
                }

                yPosition += 20;

                // Footer
                yPosition = DrawCenteredText(g, "Note:", _footerFont, centerX, yPosition);
                yPosition = DrawCenteredText(g, "1. Goods once sold are only exchangeable within 3 days", _footerFont, centerX, yPosition);
                yPosition = DrawCenteredText(g, "2. No return policy", _footerFont, centerX, yPosition);
                yPosition = DrawCenteredText(g, "3. MADNI MOBILE & PHOTOSTATE shop is not responsible for any warranty claims", _footerFont, centerX, yPosition);
                yPosition += 10;
                yPosition = DrawCenteredText(g, "---", _footerFont, centerX, yPosition);
                yPosition += 5;
                yPosition = DrawCenteredText(g, "Developed By: DevFleet Technologies | +923225347757", _footerFont, centerX, yPosition);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating receipt: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private string GetCustomerName(int customerId)
        {
            // This would typically query the database
            // For now, return a placeholder
            return "Customer #" + customerId;
        }

        public void Dispose()
        {
            _titleFont?.Dispose();
            _headerFont?.Dispose();
            _bodyFont?.Dispose();
            _footerFont?.Dispose();
            _printDocument?.Dispose();
        }
    }
}



