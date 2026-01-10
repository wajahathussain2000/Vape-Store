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
            _titleFont = new Font("Arial", 12f, FontStyle.Bold);
            _headerFont = new Font("Arial", 8.75f, FontStyle.Bold);
            _bodyFont = new Font("Arial", 7.75f, FontStyle.Regular);
            _footerFont = new Font("Arial", 7.25f, FontStyle.Regular);
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
                int receiptHeight = EstimateReceiptHeight();
                var paperSize = new PaperSize("Thermal Receipt", 315, Math.Max(600, receiptHeight)); // 80mm = 315 hundredths of an inch
                _printDocument.DefaultPageSettings.PaperSize = paperSize;
                _printDocument.DefaultPageSettings.Margins = new Margins(24, 26, 10, 10);
                
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
                float leftMargin = 24;
                float rightMargin = e.PageBounds.Width - 26;
                float printableWidth = rightMargin - leftMargin;
                float centerX = leftMargin + (printableWidth / 2);
                float itemWidth = printableWidth * 0.46f;
                float qtyWidth = printableWidth * 0.115f;
                float priceWidth = printableWidth * 0.215f;
                float totalWidth = printableWidth - itemWidth - qtyWidth - priceWidth;
                var leftFormat = new StringFormat { Alignment = StringAlignment.Near, FormatFlags = StringFormatFlags.NoWrap };
                var centerFormat = new StringFormat { Alignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap };
                var rightFormat = new StringFormat { Alignment = StringAlignment.Far, FormatFlags = StringFormatFlags.NoWrap };

                // Store name and header
                yPosition = DrawCenteredText(g, "Attock Mobiles Rwp", _titleFont, centerX, yPosition);
                yPosition = DrawCenteredText(g, "Address : V5 G Mall Ground Floor", _titleFont, centerX, yPosition);
                yPosition = DrawCenteredText(g, "Shop no 5 Attock Mobiles Rwp", _bodyFont, centerX, yPosition);
                yPosition = DrawCenteredText(g, "Bahria Phase7 Food Street", _bodyFont, centerX, yPosition);
                
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
                float rowHeight = Math.Max(_bodyFont.GetHeight(g) + 4, 16f);
                RectangleF itemHeaderRect = new RectangleF(leftMargin, yPosition, itemWidth, rowHeight);
                RectangleF qtyHeaderRect = new RectangleF(itemHeaderRect.Right - 3, yPosition, qtyWidth, rowHeight);
                RectangleF priceHeaderRect = new RectangleF(qtyHeaderRect.Right, yPosition, priceWidth, rowHeight);
                RectangleF totalHeaderRect = new RectangleF(priceHeaderRect.Right, yPosition, totalWidth, rowHeight);

                DrawText(g, "ITEM", _headerFont, itemHeaderRect, leftFormat);
                DrawText(g, "QTY", _headerFont, qtyHeaderRect, centerFormat);
                DrawText(g, "PRICE", _headerFont, priceHeaderRect, rightFormat);
                DrawText(g, "TOTAL", _headerFont, totalHeaderRect, rightFormat);
                
                yPosition += rowHeight;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 6;

                // Items
                foreach (var item in _currentSale.SaleItems)
                {
                    // Product name (wrap if too long)
                    string productName = item.ProductName;
                    if (productName.Length > 20)
                    {
                        productName = productName.Substring(0, 17) + "...";
                    }
                    
                    string fittedName = FitTextToWidth(g, productName, _bodyFont, itemWidth);
                    RectangleF itemRect = new RectangleF(leftMargin, yPosition, itemWidth, rowHeight);
                    RectangleF qtyRect = new RectangleF(itemRect.Right, yPosition, qtyWidth, rowHeight);
                    RectangleF priceRect = new RectangleF(qtyRect.Right, yPosition, priceWidth, rowHeight);
                    RectangleF totalRect = new RectangleF(priceRect.Right, yPosition, totalWidth, rowHeight);

                    DrawText(g, fittedName, _bodyFont, itemRect, leftFormat);
                    DrawText(g, item.Quantity.ToString(), _bodyFont, qtyRect, centerFormat);
                    DrawText(g, item.UnitPrice.ToString("F2"), _bodyFont, priceRect, rightFormat);
                    DrawText(g, item.SubTotal.ToString("F2"), _bodyFont, totalRect, rightFormat);
                    yPosition += rowHeight;
                }

                yPosition += 5;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 10;

                // Totals
                float labelWidth = printableWidth * 0.55f;
                float amountWidth = printableWidth - labelWidth;
                RectangleF labelRect = new RectangleF(leftMargin, yPosition, labelWidth, rowHeight);
                RectangleF valueRect = new RectangleF(labelRect.Right, yPosition, amountWidth, rowHeight);
                DrawText(g, "SUBTOTAL:", _headerFont, labelRect, leftFormat);
                DrawText(g, _currentSale.SubTotal.ToString("F2"), _bodyFont, valueRect, rightFormat);
                yPosition += rowHeight;
                // Discount
                if (_currentSale.DiscountAmount > 0)
                {
                    labelRect = new RectangleF(leftMargin, yPosition, labelWidth, rowHeight);
                    valueRect = new RectangleF(labelRect.Right, yPosition, amountWidth, rowHeight);
                    string discountText = _currentSale.DiscountPercent > 0
                        ? $"Discount ({_currentSale.DiscountPercent:F1}%):"
                        : "Discount:";
                    DrawText(g, discountText, _headerFont, labelRect, leftFormat);
                    DrawText(g, _currentSale.DiscountAmount.ToString("F2"), _bodyFont, valueRect, rightFormat);
                    yPosition += rowHeight;
                }
                
                if (_currentSale.TaxAmount > 0)
                {
                    labelRect = new RectangleF(leftMargin, yPosition, labelWidth, rowHeight);
                    valueRect = new RectangleF(labelRect.Right, yPosition, amountWidth, rowHeight);
                    DrawText(g, "TAX (" + _currentSale.TaxPercent.ToString("F1") + "%):", _headerFont, labelRect, leftFormat);
                    DrawText(g, _currentSale.TaxAmount.ToString("F2"), _bodyFont, valueRect, rightFormat);
                    yPosition += rowHeight;
                }
                
                yPosition += 5;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 10;

                // Total
                labelRect = new RectangleF(leftMargin, yPosition, labelWidth, rowHeight);
                valueRect = new RectangleF(labelRect.Right, yPosition, amountWidth, rowHeight);
                DrawText(g, "TOTAL:", _titleFont, labelRect, leftFormat);
                DrawText(g, _currentSale.TotalAmount.ToString("F2"), _titleFont, valueRect, rightFormat);
                yPosition += rowHeight + 4;
                g.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
                yPosition += 8;

                // Payment details
                yPosition = DrawText(g, "PAYMENT METHOD: " + _currentSale.PaymentMethod, _bodyFont, leftMargin, yPosition);
                yPosition = DrawText(g, "PAID: " + _currentSale.PaidAmount.ToString("F2"), _bodyFont, leftMargin, yPosition);
                
                if (_currentSale.ChangeAmount > 0)
                {
                    yPosition = DrawText(g, "CHANGE: " + _currentSale.ChangeAmount.ToString("F2"), _bodyFont, leftMargin, yPosition);
                }

                yPosition += 20;

                // Footer
                yPosition = DrawCenteredText(g, "Note:", _footerFont, centerX, yPosition);
                yPosition = DrawCenteredText(g, "1. Goods once sold are only exchangeable within 3 days", _footerFont, centerX, yPosition);
                yPosition = DrawCenteredText(g, "2. No return policy", _footerFont, centerX, yPosition);
                yPosition = DrawCenteredText(g, "3. Attock Mobiles Rwp is not responsible", _footerFont, centerX, yPosition);
                yPosition = DrawCenteredText(g, "   for any warranty claims", _footerFont, centerX, yPosition);
                yPosition += 10;
                yPosition = DrawCenteredText(g, "---", _footerFont, centerX, yPosition);
                yPosition += 5;
                yPosition = DrawCenteredText(g, "Developed By: DevFleet Technologies", _footerFont, centerX, yPosition);
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

        private float DrawText(Graphics g, string text, Font font, RectangleF bounds, StringFormat format)
        {
            g.DrawString(text, font, Brushes.Black, bounds, format);
            return bounds.Bottom;
        }

        private float DrawCenteredText(Graphics g, string text, Font font, float centerX, float y)
        {
            SizeF textSize = g.MeasureString(text, font);
            float x = centerX - (textSize.Width / 2);
            g.DrawString(text, font, Brushes.Black, x, y);
            return y + font.Height;
        }

        private string FitTextToWidth(Graphics g, string text, Font font, float maxWidth)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (g.MeasureString(text, font).Width <= maxWidth)
            {
                return text;
            }

            string working = text.Trim();
            while (working.Length > 0)
            {
                working = working.Substring(0, working.Length - 1);
                string candidate = working.TrimEnd() + "...";
                if (g.MeasureString(candidate, font).Width <= maxWidth)
                {
                    return candidate;
                }
            }

            return "...";
        }

        private string GetCustomerName(int customerId)
        {
            // This would typically query the database
            // For now, return a placeholder
            return "Customer #" + customerId;
        }

        private int EstimateReceiptHeight()
        {
            if (_currentSale == null)
            {
                return 600;
            }

            int lineHeight = (int)Math.Ceiling(_bodyFont.GetHeight()) + 6;
            int headerHeight = lineHeight * 6;
            int businessHeight = lineHeight * 6;
            int customerHeight = (_currentSale.CustomerID > 0 ? 2 : 1) * lineHeight;
            int itemCount = _currentSale.SaleItems?.Count ?? 0;
            int itemsHeight = lineHeight * (Math.Max(itemCount, 1) + 4); // header + divider + rows + closing line
            int totalsHeight = lineHeight * (3 + (_currentSale.TaxAmount > 0 ? 1 : 0) + (_currentSale.DiscountAmount > 0 ? 1 : 0));
            int paymentsHeight = lineHeight * (2 + (_currentSale.ChangeAmount > 0 ? 1 : 0));
            int footerHeight = lineHeight * 7;
            int padding = lineHeight * 2;

            return headerHeight + businessHeight + customerHeight + itemsHeight + totalsHeight + paymentsHeight + footerHeight + padding;
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



