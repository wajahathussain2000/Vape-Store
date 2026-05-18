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
            _headerFont = new Font("Arial", 9f, FontStyle.Bold);
            _bodyFont = new Font("Arial", 8f, FontStyle.Regular);
            _footerFont = new Font("Arial", 7f, FontStyle.Regular);
        }

        public void PrintReceipt(Sale sale)
        {
            try
            {
                _currentSale = sale;
                
                var config = ConfigurationService.Instance;
                
                // Set Paper Size and Margins
                int receiptHeight = EstimateReceiptHeight();
                int paperWidth = config.ThermalPaperWidth;
                var paperSize = new PaperSize("Thermal Receipt", paperWidth, Math.Max(600, receiptHeight)); 
                _printDocument.DefaultPageSettings.PaperSize = paperSize;
                _printDocument.DefaultPageSettings.Margins = new Margins(15, 15, 10, 10);

                // Set Printer
                if (!string.IsNullOrEmpty(config.ThermalPrinterName))
                {
                    _printDocument.PrinterSettings.PrinterName = config.ThermalPrinterName;
                }

                if (config.DirectPrintReceipt)
                {
                    _printDocument.Print();
                }
                else
                {
                    var printDialog = new PrintDialog();
                    printDialog.Document = _printDocument;
                    
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        _printDocument.Print();
                    }
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
                
                int receiptHeight = EstimateReceiptHeight();
                int paperWidth = ConfigurationService.Instance.ThermalPaperWidth;
                var paperSize = new PaperSize("Thermal Receipt", paperWidth, Math.Max(600, receiptHeight));
                _printDocument.DefaultPageSettings.PaperSize = paperSize;
                _printDocument.DefaultPageSettings.Margins = new Margins(15, 15, 10, 10);

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
                float y = 15;
                float margin = 15;
                float width = e.PageBounds.Width;
                float printableWidth = width - (margin * 2);
                float centerX = width / 2f;

                var centerFormat = new StringFormat { Alignment = StringAlignment.Center };
                var rightFormat = new StringFormat { Alignment = StringAlignment.Far };
                var leftFormat = new StringFormat { Alignment = StringAlignment.Near };

                // 1. Store Header
                var config = ConfigurationService.Instance;
                string storeName = (config.ApplicationName ?? "VAPE STORE").ToUpper();
                g.DrawString(storeName, _titleFont, Brushes.Black, new RectangleF(margin, y, printableWidth, 25), centerFormat);
                y += 22;

                g.DrawString("Contact: " + config.StoreContact, _bodyFont, Brushes.Black, new RectangleF(margin, y, printableWidth, 18), centerFormat);
                y += 16;

                string address = config.StoreAddress ?? "";
                var addressRect = new RectangleF(margin, y, printableWidth, 40);
                g.DrawString(address, _bodyFont, Brushes.Black, addressRect, centerFormat);
                y += g.MeasureString(address, _bodyFont, (int)printableWidth).Height + 5;

                // Separator
                g.DrawLine(Pens.Black, margin, y, width - margin, y);
                y += 8;

                // 2. Transaction Info
                g.DrawString("INVOICE: " + _currentSale.InvoiceNumber, _headerFont, Brushes.Black, margin, y);
                y += 18;
                g.DrawString("DATE: " + _currentSale.SaleDate.ToString("MM/dd/yyyy HH:mm"), _bodyFont, Brushes.Black, margin, y);
                y += 16;
                g.DrawString("CASHIER: " + (_currentSale.UserName ?? "Admin"), _bodyFont, Brushes.Black, margin, y);
                y += 20;

                string customerName = (_currentSale.CustomerName ?? "Walk-in").ToUpper();
                g.DrawString("CUSTOMER: " + customerName, _bodyFont, Brushes.Black, margin, y);
                y += 25;

                // 3. Items Table Header
                g.DrawLine(new Pen(Color.Black, 1f), margin, y, width - margin, y);
                y += 5;

                float colQty = 40;
                float colTotal = 80;
                float colProduct = printableWidth - colQty - colTotal;

                g.DrawString("ITEM", _headerFont, Brushes.Black, margin, y);
                g.DrawString("QTY", _headerFont, Brushes.Black, margin + colProduct, y);
                g.DrawString("TOTAL", _headerFont, Brushes.Black, margin + colProduct + colQty, y);
                y += 18;
                g.DrawLine(Pens.Black, margin, y, width - margin, y);
                y += 8;

                // 4. Item Rows
                foreach (var item in _currentSale.SaleItems)
                {
                    string name = item.ProductName;
                    var nameSize = g.MeasureString(name, _bodyFont, (int)colProduct);
                    g.DrawString(name, _bodyFont, Brushes.Black, new RectangleF(margin, y, colProduct, nameSize.Height), leftFormat);
                    
                    g.DrawString(item.Quantity.ToString(), _bodyFont, Brushes.Black, margin + colProduct, y);
                    g.DrawString(item.SubTotal.ToString("N2"), _bodyFont, Brushes.Black, new RectangleF(margin + colProduct + colQty, y, colTotal, 20), rightFormat);
                    
                    y += Math.Max(20, nameSize.Height + 5);
                }

                // 5. Totals Section
                y += 5;
                g.DrawLine(Pens.Black, margin, y, width - margin, y);
                y += 10;

                DrawTotalRow(g, "SUBTOTAL:", _currentSale.SubTotal, _bodyFont, margin, y, printableWidth);
                y += 18;

                if (_currentSale.DiscountAmount > 0)
                {
                    DrawTotalRow(g, $"DISCOUNT ({_currentSale.DiscountPercent}%):", _currentSale.DiscountAmount, _bodyFont, margin, y, printableWidth);
                    y += 18;
                }

                y += 5;
                g.DrawString("GRAND TOTAL:", _headerFont, Brushes.Black, margin, y);
                g.DrawString(_currentSale.TotalAmount.ToString("N2"), _headerFont, Brushes.Black, new RectangleF(margin, y, printableWidth, 20), rightFormat);
                y += 22;
                g.DrawLine(new Pen(Color.Black, 1.5f), margin, y, width - margin, y);
                y += 8;

                DrawTotalRow(g, "CASH PAID:", _currentSale.PaidAmount, _bodyFont, margin, y, printableWidth);
                y += 16;
                DrawTotalRow(g, "CHANGE:", _currentSale.ChangeAmount, _bodyFont, margin, y, printableWidth);
                y += 25;

                // 6. Barcode
                if (!string.IsNullOrEmpty(_currentSale.BarcodeData))
                {
                    try {
                        var bs = new BarcodeService();
                        var img = bs.GenerateBarcodeImageObject(_currentSale.BarcodeData, (int)printableWidth, 40);
                        if (img != null) {
                            g.DrawImage(img, margin, y, printableWidth, 40);
                            y += 45;
                            g.DrawString(_currentSale.BarcodeData, _footerFont, Brushes.Black, new RectangleF(margin, y, printableWidth, 15), centerFormat);
                            y += 20;
                        }
                    } catch { }
                }

                // 7. Footer
                string footerText = config.ReceiptFooter ?? "Thank you for your business!";
                var footerSize = g.MeasureString(footerText, _footerFont, (int)printableWidth);
                var footerRect = new RectangleF(margin, y, printableWidth, footerSize.Height + 5);
                g.DrawString(footerText, _footerFont, Brushes.Black, footerRect, centerFormat);
                y += footerSize.Height + 10;

                g.DrawString("Powered By: DevFleet Technologies | +923225347757", _footerFont, Brushes.DimGray, new RectangleF(margin, y, printableWidth, 15), centerFormat);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating receipt: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DrawTotalRow(Graphics g, string label, decimal val, Font font, float margin, float y, float width)
        {
            g.DrawString(label, font, Brushes.Black, margin, y);
            g.DrawString(val.ToString("N2"), font, Brushes.Black, new RectangleF(margin, y, width, 20), new StringFormat { Alignment = StringAlignment.Far });
        }

        private int EstimateReceiptHeight()
        {
            if (_currentSale == null) return 600;

            int itemCount = _currentSale.SaleItems?.Count ?? 0;
            return 450 + (itemCount * 30);
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



