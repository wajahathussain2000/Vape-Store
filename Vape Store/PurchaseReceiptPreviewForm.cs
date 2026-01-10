using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfFont = iTextSharp.text.Font;
using PdfRectangle = iTextSharp.text.Rectangle;
using DrawFont = System.Drawing.Font;

namespace Vape_Store
{
    public partial class PurchaseReceiptPreviewForm : Form
    {
        private Purchase _purchase;
        private List<PurchaseItem> _purchaseItems;
        private ThermalReceiptService _receiptService;
        private Panel _receiptPanel;

        public PurchaseReceiptPreviewForm(Purchase purchase, List<PurchaseItem> purchaseItems)
        {
            InitializeComponent();
            _purchase = purchase;
            _purchaseItems = purchaseItems ?? new List<PurchaseItem>();
            _receiptService = new ThermalReceiptService();
            
            SetupReceiptPanel();
            SetupButtons();
        }

        private void SetupReceiptPanel()
        {
            // Create receipt panel with proper A4 dimensions
            _receiptPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(60, 50, 60, 50)
            };
            _receiptPanel.Paint += ReceiptPanel_Paint;
            this.Controls.Add(_receiptPanel);
        }

        private void SetupButtons()
        {
            // Create button panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            this.Controls.Add(buttonPanel);

            // Print Receipt button
            Button btnPrint = new Button
            {
                Text = "Print Receipt",
                Size = new Size(130, 35),
                Location = new Point(50, 12),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new DrawFont("Arial", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Click += BtnPrint_Click;
            buttonPanel.Controls.Add(btnPrint);

            // Save PDF button
            Button btnSavePDF = new Button
            {
                Text = "Save PDF",
                Size = new Size(130, 35),
                Location = new Point(200, 12),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new DrawFont("Arial", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnSavePDF.FlatAppearance.BorderSize = 0;
            btnSavePDF.Click += BtnSavePDF_Click;
            buttonPanel.Controls.Add(btnSavePDF);

            // Close button
            Button btnClose = new Button
            {
                Text = "Close",
                Size = new Size(100, 35),
                Location = new Point(650, 12),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new DrawFont("Arial", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += BtnClose_Click;
            buttonPanel.Controls.Add(btnClose);
        }

        private void ReceiptPanel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // A4 dimensions: 794 x 1123 pixels at 96 DPI (210mm x 297mm)
                float pageWidth = 794f;
                float leftMargin = 50f;
                float rightMargin = pageWidth - 50f;
                float centerX = pageWidth / 2f;
                float yPosition = 0f;

                // Draw top blue header bar
                DrawTopBlueBar(g, leftMargin, rightMargin, ref yPosition);
                
                // Draw main title "INVOICE"
                DrawInvoiceTitle(g, centerX, ref yPosition);
                
                // Draw three-column layout: Company | Invoice To | Invoice Details
                DrawThreeColumnLayout(g, leftMargin, rightMargin, ref yPosition);
                
                // Draw items table with blue header
                DrawItemsTable(g, leftMargin, rightMargin, ref yPosition);
                
                // Draw financial summary (right aligned)
                DrawFinancialSummary(g, leftMargin, rightMargin, ref yPosition);
                
                // Draw notes and terms sections
                DrawNotesAndTerms(g, leftMargin, rightMargin, ref yPosition);
                
                // Draw bottom blue footer bar
                DrawBottomBlueBar(g, leftMargin, rightMargin, ref yPosition);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating receipt: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DrawTopBlueBar(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            // Blue header bar - similar to template
            Color blueColor = Color.FromArgb(41, 128, 185); // Professional blue
            using (var blueBrush = new SolidBrush(blueColor))
            {
                g.FillRectangle(blueBrush, leftMargin, yPosition, rightMargin - leftMargin, 40f);
            }
            yPosition += 50f; // Space after blue bar
        }

        private void DrawInvoiceTitle(Graphics g, float centerX, ref float yPosition)
        {
            using (var blackBrush = new SolidBrush(Color.Black))
            {
                // Large "INVOICE" title centered
                DrawFont invoiceTitleFont = new DrawFont("Arial", 36, FontStyle.Bold);
                SizeF titleSize = g.MeasureString("INVOICE", invoiceTitleFont);
                g.DrawString("INVOICE", invoiceTitleFont, blackBrush, centerX - (titleSize.Width / 2), yPosition);
                yPosition += titleSize.Height + 30f;
            }
        }

        private void DrawThreeColumnLayout(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            float columnWidth = (rightMargin - leftMargin) / 3f;
            float col1X = leftMargin;
            float col2X = leftMargin + columnWidth;
            float col3X = leftMargin + (columnWidth * 2f);
            
            DrawFont labelFont = new DrawFont("Arial", 9, FontStyle.Bold);
            DrawFont valueFont = new DrawFont("Arial", 9, FontStyle.Regular);
            DrawFont headerFont = new DrawFont("Arial", 10, FontStyle.Bold);
            
            using (var blackBrush = new SolidBrush(Color.Black))
            {
                // Column 1: Company Information (Left)
                g.DrawString("Attock Mobiles Rwp", headerFont, blackBrush, col1X, yPosition);
                yPosition += headerFont.Height + 5f;
                g.DrawString("Address : V5 G Mall Ground Floor", valueFont, blackBrush, col1X, yPosition);
                yPosition += valueFont.Height + 3f;
                g.DrawString("Shop no 5 Attock Mobiles Rwp", valueFont, blackBrush, col1X, yPosition);
                yPosition += valueFont.Height + 3f;
                g.DrawString("Bahria Phase7 Food Street", valueFont, blackBrush, col1X, yPosition);
                
                float col1EndY = yPosition + valueFont.Height;
                yPosition = col1EndY - (valueFont.Height * 4f) - 5f; // Reset to start
                
                // Column 2: Invoice To (Supplier) - Middle
                g.DrawString("INVOICE TO:", headerFont, blackBrush, col2X, yPosition);
                yPosition += headerFont.Height + 5f;
                if (!string.IsNullOrEmpty(_purchase.SupplierName))
                {
                    g.DrawString(_purchase.SupplierName, valueFont, blackBrush, col2X, yPosition);
                }
                else
                {
                    g.DrawString("Supplier Name", valueFont, blackBrush, col2X, yPosition);
                }
                
                float col2EndY = yPosition + valueFont.Height;
                yPosition = col2EndY - valueFont.Height - 5f; // Reset to start
                
                // Column 3: Invoice Details (Right)
                g.DrawString("Invoice Number", labelFont, blackBrush, col3X, yPosition);
                yPosition += labelFont.Height + 2f;
                g.DrawString($"#{_purchase.InvoiceNumber}", valueFont, blackBrush, col3X, yPosition);
                yPosition += valueFont.Height + 8f;
                
                g.DrawString("Date of Invoice", labelFont, blackBrush, col3X, yPosition);
                yPosition += labelFont.Height + 2f;
                g.DrawString(_purchase.PurchaseDate.ToString("yyyy-MM-dd"), valueFont, blackBrush, col3X, yPosition);
                yPosition += valueFont.Height + 8f;
                
                g.DrawString("Due Date", labelFont, blackBrush, col3X, yPosition);
                yPosition += labelFont.Height + 2f;
                g.DrawString(_purchase.PurchaseDate.ToString("yyyy-MM-dd"), valueFont, blackBrush, col3X, yPosition);
                
                // Set yPosition to the maximum of all three columns
                float maxY = Math.Max(Math.Max(col1EndY, col2EndY), yPosition);
                yPosition = maxY + 25f;
            }
        }

        private void DrawItemsTable(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            float tableWidth = rightMargin - leftMargin;
            // Column widths matching template: DESCRIPTION (wide), QTY, UNIT PRICE, TOTAL
            float col1Width = tableWidth * 0.55f;  // DESCRIPTION - 55%
            float col2Width = tableWidth * 0.12f;  // QTY - 12%
            float col3Width = tableWidth * 0.16f;  // UNIT PRICE - 16%
            float col4Width = tableWidth * 0.17f;  // TOTAL - 17%

            float col1X = leftMargin;
            float col2X = col1X + col1Width;
            float col3X = col2X + col2Width;
            float col4X = col3X + col3Width;

            // Blue header background (matching template)
            Color blueColor = Color.FromArgb(41, 128, 185);
            using (var headerBrush = new SolidBrush(blueColor))
            {
                g.FillRectangle(headerBrush, leftMargin, yPosition, tableWidth, 35f);
            }

            // Header text in white
            using (var whiteBrush = new SolidBrush(Color.White))
            {
                DrawFont headerFont = new DrawFont("Arial", 11, FontStyle.Bold);
                
                RectangleF descHeader = new RectangleF(col1X + 10, yPosition + 8, col1Width - 20, 20);
                g.DrawString("DESCRIPTION", headerFont, whiteBrush, descHeader);

                RectangleF qtyHeader = new RectangleF(col2X, yPosition + 8, col2Width, 20);
                StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString("QTY", headerFont, whiteBrush, qtyHeader, centerFormat);

                RectangleF priceHeader = new RectangleF(col3X, yPosition + 8, col3Width, 20);
                StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far };
                g.DrawString("UNIT PRICE", headerFont, whiteBrush, priceHeader, rightFormat);

                RectangleF totalHeader = new RectangleF(col4X, yPosition + 8, col4Width - 10, 20);
                g.DrawString("TOTAL", headerFont, whiteBrush, totalHeader, rightFormat);
            }

            yPosition += 35f;

            // Draw items with alternating row colors
            DrawFont itemFont = new DrawFont("Arial", 10, FontStyle.Regular);
            DrawFont itemBoldFont = new DrawFont("Arial", 10, FontStyle.Bold);
            bool isEven = false;

            foreach (var item in _purchaseItems)
            {
                // Alternating row background (light grey and white)
                Color rowColor = isEven ? Color.FromArgb(245, 245, 245) : Color.White;
                using (var rowBrush = new SolidBrush(rowColor))
                {
                    g.FillRectangle(rowBrush, leftMargin, yPosition, tableWidth, 28f);
                }

                // Product name (DESCRIPTION)
                string productName = item.ProductName;
                RectangleF productRect = new RectangleF(col1X + 10, yPosition + 6, col1Width - 20, 20);
                g.DrawString(productName, itemFont, Brushes.Black, productRect);

                // Quantity - center aligned
                RectangleF qtyRect = new RectangleF(col2X, yPosition + 6, col2Width, 20);
                StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(item.Quantity.ToString(), itemFont, Brushes.Black, qtyRect, centerFormat);

                // Unit Price - right aligned
                RectangleF priceRect = new RectangleF(col3X, yPosition + 6, col3Width, 20);
                StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far };
                g.DrawString(item.UnitPrice.ToString("F2"), itemFont, Brushes.Black, priceRect, rightFormat);

                // Total - right aligned, bold
                RectangleF totalRect = new RectangleF(col4X, yPosition + 6, col4Width - 10, 20);
                g.DrawString(item.SubTotal.ToString("F2"), itemBoldFont, Brushes.Black, totalRect, rightFormat);

                yPosition += 28f;
                isEven = !isEven;
            }

            // Bottom border
            using (Pen borderPen = new Pen(Color.Black, 1f))
            {
                g.DrawLine(borderPen, leftMargin, yPosition, rightMargin, yPosition);
            }
            yPosition += 20f;
        }

        private void DrawFinancialSummary(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            // Right-aligned summary section (matching template)
            float summaryWidth = 280f;
            float summaryX = rightMargin - summaryWidth;
            float labelWidth = 180f;
            float valueX = summaryX + labelWidth;

            using (var blackBrush = new SolidBrush(Color.Black))
            {
                DrawFont labelFont = new DrawFont("Arial", 10, FontStyle.Regular);
                DrawFont valueFont = new DrawFont("Arial", 10, FontStyle.Bold);
                StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far };

                // SUBTOTAL
                g.DrawString("SUBTOTAL", labelFont, blackBrush, summaryX, yPosition);
                RectangleF subtotalValueRect = new RectangleF(valueX, yPosition, summaryWidth - labelWidth, labelFont.Height);
                g.DrawString(_purchase.SubTotal.ToString("F2"), valueFont, blackBrush, subtotalValueRect, rightFormat);
                yPosition += 20f;

                // DISCOUNT
                decimal discountAmount = _purchase.SubTotal + _purchase.TaxAmount - _purchase.TotalAmount;
                if (discountAmount > 0)
                {
                    g.DrawString("DISCOUNT", labelFont, blackBrush, summaryX, yPosition);
                    RectangleF discountValueRect = new RectangleF(valueX, yPosition, summaryWidth - labelWidth, labelFont.Height);
                    g.DrawString($"-{discountAmount:F2}", valueFont, blackBrush, discountValueRect, rightFormat);
                    yPosition += 20f;

                    // SUBTOTAL LESS DISCOUNT
                    decimal subtotalLessDiscount = _purchase.SubTotal - discountAmount;
                    g.DrawString("SUBTOTAL LESS DISCOUNT", labelFont, blackBrush, summaryX, yPosition);
                    RectangleF lessDiscountValueRect = new RectangleF(valueX, yPosition, summaryWidth - labelWidth, labelFont.Height);
                    g.DrawString(subtotalLessDiscount.ToString("F2"), valueFont, blackBrush, lessDiscountValueRect, rightFormat);
                    yPosition += 20f;
                }

                // TAX RATE
                if (_purchase.TaxAmount > 0)
                {
                    g.DrawString("TAX RATE", labelFont, blackBrush, summaryX, yPosition);
                    RectangleF taxRateValueRect = new RectangleF(valueX, yPosition, summaryWidth - labelWidth, labelFont.Height);
                    g.DrawString($"{_purchase.TaxPercent:F1}%", valueFont, blackBrush, taxRateValueRect, rightFormat);
                    yPosition += 20f;

                    // TAX TOTAL
                    g.DrawString("TAX TOTAL", labelFont, blackBrush, summaryX, yPosition);
                    RectangleF taxValueRect = new RectangleF(valueX, yPosition, summaryWidth - labelWidth, labelFont.Height);
                    g.DrawString(_purchase.TaxAmount.ToString("F2"), valueFont, blackBrush, taxValueRect, rightFormat);
                    yPosition += 20f;
                }

                // BALANCE DUE (highlighted)
                yPosition += 5f;
                DrawFont balanceFont = new DrawFont("Arial", 12, FontStyle.Bold);
                g.DrawString("BALANCE DUE", balanceFont, blackBrush, summaryX, yPosition);
                RectangleF balanceValueRect = new RectangleF(valueX, yPosition, summaryWidth - labelWidth, balanceFont.Height);
                g.DrawString(_purchase.TotalAmount.ToString("F2"), balanceFont, blackBrush, balanceValueRect, rightFormat);
                yPosition += balanceFont.Height + 15f;
            }
        }

        private void DrawNotesAndTerms(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            float sectionWidth = (rightMargin - leftMargin) / 2f - 20f;
            float notesX = leftMargin;
            float termsX = leftMargin + sectionWidth + 40f;

            using (var blackBrush = new SolidBrush(Color.Black))
            {
                DrawFont headerFont = new DrawFont("Arial", 10, FontStyle.Bold);
                DrawFont valueFont = new DrawFont("Arial", 9, FontStyle.Regular);

                // NOTES section (left side)
                g.DrawString("NOTES:", headerFont, blackBrush, notesX, yPosition);
                yPosition += headerFont.Height + 8f;
                
                // Payment details in notes
                string notesText = $"Payment Method: {_purchase.PaymentMethod.ToUpper()}\n";
                notesText += $"Amount Paid: {_purchase.PaidAmount:F2}\n";
                decimal balanceAmount = _purchase.TotalAmount - _purchase.PaidAmount;
                if (balanceAmount > 0)
                {
                    notesText += $"Balance Due: {balanceAmount:F2}";
                }
                else if (balanceAmount < 0)
                {
                    notesText += $"Overpaid: {Math.Abs(balanceAmount):F2}";
                }
                
                RectangleF notesRect = new RectangleF(notesX, yPosition, sectionWidth, 80f);
                g.DrawString(notesText, valueFont, blackBrush, notesRect);

                // TERMS AND CONDITIONS section (right side)
                float termsY = yPosition - headerFont.Height - 8f; // Align with NOTES header
                g.DrawString("TERMS AND CONDITIONS:", headerFont, blackBrush, termsX, termsY);
                termsY += headerFont.Height + 8f;
                
                string termsText = "Payment due upon receipt.\n";
                termsText += "Thank you for your business.";
                
                RectangleF termsRect = new RectangleF(termsX, termsY, sectionWidth, 80f);
                g.DrawString(termsText, valueFont, blackBrush, termsRect);

                yPosition = Math.Max(yPosition + 80f, termsY + 80f) + 20f;
            }
        }

        private void DrawBottomBlueBar(Graphics g, float leftMargin, float rightMargin, ref float yPosition)
        {
            // Blue footer bar (matching template)
            Color blueColor = Color.FromArgb(41, 128, 185);
            using (var blueBrush = new SolidBrush(blueColor))
            {
                g.FillRectangle(blueBrush, leftMargin, yPosition, rightMargin - leftMargin, 30f);
            }
            
            // Developer credit on blue bar (white text)
            using (var whiteBrush = new SolidBrush(Color.White))
            {
                DrawFont devFont = new DrawFont("Arial", 9, FontStyle.Italic);
                string devText = "Developed By: DevFleet Technologies";
                SizeF devSize = g.MeasureString(devText, devFont);
                float centerX = (leftMargin + rightMargin) / 2f;
                g.DrawString(devText, devFont, whiteBrush, centerX - (devSize.Width / 2), yPosition + 8f);
            }
            
            yPosition += 40f;
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
                    ExportToPDF(saveDialog.FileName);
                    MessageBox.Show($"PDF saved successfully to:\n{saveDialog.FileName}", "PDF Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving PDF: {ex.Message}", "PDF Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToPDF(string filePath)
        {
            try
            {
                // Create PDF document with A4 size
                Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                // Set up fonts
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                PdfFont titleFont = new PdfFont(baseFont, 20, PdfFont.BOLD);
                PdfFont headerFont = new PdfFont(baseFont, 14, PdfFont.BOLD);
                PdfFont normalFont = new PdfFont(baseFont, 11, PdfFont.NORMAL);
                PdfFont smallFont = new PdfFont(baseFont, 9, PdfFont.NORMAL);
                PdfFont italicFont = new PdfFont(baseFont, 9, PdfFont.ITALIC);

                // Header - Company Info
                Paragraph companyName = new Paragraph("Attock Mobiles Rwp", titleFont);
                companyName.Alignment = Element.ALIGN_CENTER;
                companyName.SpacingAfter = 8f;
                document.Add(companyName);

                Paragraph address1 = new Paragraph("Address : V5 G Mall Ground Floor", normalFont);
                address1.Alignment = Element.ALIGN_CENTER;
                address1.SpacingAfter = 5f;
                document.Add(address1);

                Paragraph address2 = new Paragraph("Shop no 5 Attock Mobiles Rwp", normalFont);
                address2.Alignment = Element.ALIGN_CENTER;
                address2.SpacingAfter = 5f;
                document.Add(address2);

                Paragraph address3 = new Paragraph("Bahria Phase7 Food Street", normalFont);
                address3.Alignment = Element.ALIGN_CENTER;
                address3.SpacingAfter = 15f;
                document.Add(address3);

                // Separator line
                document.Add(new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -1))));
                document.Add(new Paragraph(" "));

                // Invoice Details
                Paragraph invoiceTitle = new Paragraph("PURCHASE INVOICE", headerFont);
                invoiceTitle.SpacingAfter = 10f;
                document.Add(invoiceTitle);

                Paragraph invoiceNumber = new Paragraph($"Invoice #: {_purchase.InvoiceNumber}", normalFont);
                invoiceNumber.SpacingAfter = 5f;
                document.Add(invoiceNumber);

                Paragraph invoiceDate = new Paragraph($"Date: {_purchase.PurchaseDate:MMM dd, yyyy 'at' hh:mm tt}", normalFont);
                if (!string.IsNullOrEmpty(_purchase.UserName))
                {
                    invoiceDate.Add(new Chunk($"                    Entered by: {_purchase.UserName}", normalFont));
                }
                invoiceDate.SpacingAfter = 15f;
                document.Add(invoiceDate);

                // Separator
                document.Add(new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, -1))));
                document.Add(new Paragraph(" "));

                // Supplier Information
                if (!string.IsNullOrEmpty(_purchase.SupplierName))
                {
                    Paragraph supplierTitle = new Paragraph("SUPPLIER INFORMATION", headerFont);
                    supplierTitle.SpacingAfter = 5f;
                    document.Add(supplierTitle);

                    Paragraph supplierName = new Paragraph($"Supplier: {_purchase.SupplierName}", normalFont);
                    supplierName.SpacingAfter = 15f;
                    document.Add(supplierName);

                    // Separator
                    document.Add(new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, -1))));
                    document.Add(new Paragraph(" "));
                }

                // Items Table
                PdfPTable itemsTable = new PdfPTable(4);
                itemsTable.WidthPercentage = 100;
                itemsTable.SetWidths(new float[] { 3f, 1f, 1.5f, 1.5f });

                // Table Headers
                string[] headers = { "PRODUCT", "QTY", "UNIT PRICE", "TOTAL" };
                foreach (string header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                    headerCell.BackgroundColor = new BaseColor(240, 240, 240);
                    headerCell.Padding = 8f;
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    itemsTable.AddCell(headerCell);
                }

                // Table Rows
                foreach (var item in _purchaseItems)
                {
                    itemsTable.AddCell(new PdfPCell(new Phrase(item.ProductName.Length > 40 ? item.ProductName.Substring(0, 37) + "..." : item.ProductName, normalFont)) { Padding = 6f });
                    itemsTable.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), normalFont)) { Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER });
                    itemsTable.AddCell(new PdfPCell(new Phrase(item.UnitPrice.ToString("F2"), normalFont)) { Padding = 6f, HorizontalAlignment = Element.ALIGN_RIGHT });
                    itemsTable.AddCell(new PdfPCell(new Phrase(item.SubTotal.ToString("F2"), normalFont)) { Padding = 6f, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                document.Add(itemsTable);
                document.Add(new Paragraph(" "));

                // Financial Summary - Right Aligned
                PdfPTable summaryTable = new PdfPTable(2);
                summaryTable.WidthPercentage = 40;
                summaryTable.HorizontalAlignment = Element.ALIGN_RIGHT;
                summaryTable.SetWidths(new float[] { 1.5f, 1f });

                summaryTable.AddCell(new PdfPCell(new Phrase("Subtotal:", normalFont)) { Border = 0, Padding = 5f });
                summaryTable.AddCell(new PdfPCell(new Phrase(_purchase.SubTotal.ToString("F2"), normalFont)) { Border = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_RIGHT });

                if (_purchase.TaxAmount > 0)
                {
                    summaryTable.AddCell(new PdfPCell(new Phrase($"Tax ({_purchase.TaxPercent:F1}%):", normalFont)) { Border = 0, Padding = 5f });
                    summaryTable.AddCell(new PdfPCell(new Phrase(_purchase.TaxAmount.ToString("F2"), normalFont)) { Border = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                decimal discountAmount = _purchase.SubTotal + _purchase.TaxAmount - _purchase.TotalAmount;
                if (discountAmount > 0)
                {
                    summaryTable.AddCell(new PdfPCell(new Phrase("Discount:", normalFont)) { Border = 0, Padding = 5f });
                    summaryTable.AddCell(new PdfPCell(new Phrase($"-{discountAmount:F2}", normalFont)) { Border = 0, Padding = 5f, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                // Total with emphasis
                summaryTable.AddCell(new PdfPCell(new Phrase("TOTAL:", headerFont)) { Border = PdfRectangle.TOP_BORDER | PdfRectangle.BOTTOM_BORDER, Padding = 8f });
                summaryTable.AddCell(new PdfPCell(new Phrase(_purchase.TotalAmount.ToString("F2"), headerFont)) { Border = PdfRectangle.TOP_BORDER | PdfRectangle.BOTTOM_BORDER, Padding = 8f, HorizontalAlignment = Element.ALIGN_RIGHT });

                document.Add(summaryTable);
                document.Add(new Paragraph(" "));

                // Payment Details
                Paragraph paymentTitle = new Paragraph("PAYMENT DETAILS", headerFont);
                paymentTitle.SpacingAfter = 8f;
                document.Add(paymentTitle);

                Paragraph paymentMethod = new Paragraph($"Payment Method: {_purchase.PaymentMethod.ToUpper()}", normalFont);
                paymentMethod.SpacingAfter = 5f;
                document.Add(paymentMethod);

                Paragraph amountPaid = new Paragraph($"Amount Paid: {_purchase.PaidAmount:F2}", normalFont);
                amountPaid.SpacingAfter = 5f;
                document.Add(amountPaid);

                decimal balanceAmount = _purchase.TotalAmount - _purchase.PaidAmount;
                if (balanceAmount > 0)
                {
                    Paragraph balance = new Paragraph($"Balance Due: {balanceAmount:F2}", headerFont);
                    balance.SpacingAfter = 15f;
                    document.Add(balance);
                }
                else if (balanceAmount < 0)
                {
                    Paragraph overpaid = new Paragraph($"Overpaid: {Math.Abs(balanceAmount):F2}", headerFont);
                    overpaid.SpacingAfter = 15f;
                    document.Add(overpaid);
                }

                // Separator
                document.Add(new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -1))));
                document.Add(new Paragraph(" "));

                // Footer
                Paragraph receiptInfo = new Paragraph($"Generated on {DateTime.Now:MMM dd, yyyy 'at' hh:mm tt}\nPurchase Receipt #{_purchase.InvoiceNumber}", smallFont);
                receiptInfo.SpacingAfter = 10f;
                document.Add(receiptInfo);

                Paragraph developer = new Paragraph("Developed By: DevFleet Technologies", italicFont);
                developer.Alignment = Element.ALIGN_CENTER;
                document.Add(developer);

                document.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating PDF: {ex.Message}", ex);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _receiptService?.Dispose();
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
