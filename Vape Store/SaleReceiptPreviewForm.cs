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
    public partial class SaleReceiptPreviewForm : Form
    {
        private Sale _sale;
        private List<SaleItem> _saleItems;
        private ThermalReceiptService _receiptService;
        private Panel _receiptPanel;

        public SaleReceiptPreviewForm(Sale sale, List<SaleItem> saleItems = null)
        {
            _sale = sale;
            _saleItems = saleItems ?? sale.SaleItems ?? new List<SaleItem>();
            _receiptService = new ThermalReceiptService();
            
            this.Text = $"Sale Invoice Progress - {_sale.InvoiceNumber}";
            this.Size = new Size(900, 950);
            this.StartPosition = FormStartPosition.CenterParent;

            SetupReceiptPanel();
            SetupButtons();
        }

        private void SetupReceiptPanel()
        {
            _receiptPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(220, 220, 220),
                Padding = new Padding(40)
            };
            
            // Inner white A4 paper
            var paperPanel = new Panel
            {
                Size = new Size(800, 1100),
                BackColor = Color.White,
                Location = new Point(40, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            paperPanel.Paint += ReceiptPanel_Paint;
            _receiptPanel.Controls.Add(paperPanel);
            
            this.Controls.Add(_receiptPanel);
        }

        private void SetupButtons()
        {
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            this.Controls.Add(buttonPanel);

            Button btnPrint = new Button
            {
                Text = "Print Thermal",
                Size = new Size(130, 35),
                Location = new Point(50, 12),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new DrawFont("Arial", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnPrint.Click += (s, e) => _receiptService.PrintReceipt(_sale);
            buttonPanel.Controls.Add(btnPrint);

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
            btnSavePDF.Click += BtnSavePDF_Click;
            buttonPanel.Controls.Add(btnSavePDF);

            Button btnClose = new Button
            {
                Text = "Close",
                Size = new Size(100, 35),
                Location = new Point(750, 12),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new DrawFont("Arial", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClose.Click += (s, e) => this.Close();
            buttonPanel.Controls.Add(btnClose);
        }

        private void ReceiptPanel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                float pageWidth = 800f;
                float leftMargin = 50f;
                float rightMargin = pageWidth - 50f;
                float centerX = pageWidth / 2f;
                float y = 0f;

                // 1. Blue Top Bar
                using (var brush = new SolidBrush(Color.FromArgb(41, 128, 185)))
                    g.FillRectangle(brush, 0, y, pageWidth, 40f);
                y += 60f;

                // 2. Title
                using (var font = new DrawFont("Arial", 36, FontStyle.Bold))
                {
                    string title = "INVOICE";
                    var size = g.MeasureString(title, font);
                    g.DrawString(title, font, Brushes.Black, centerX - (size.Width / 2), y);
                    y += size.Height + 30f;
                }

                // 3. Info Grid
                var config = ConfigurationService.Instance;
                float colWidth = (rightMargin - leftMargin) / 3f;
                
                // Col 1: Store info
                float tempY = y;
                g.DrawString(config.ApplicationName?.ToUpper() ?? "VAPE STORE", new DrawFont("Arial", 10, FontStyle.Bold), Brushes.Black, leftMargin, tempY);
                tempY += 18;
                g.DrawString("PH: " + config.StoreContact, new DrawFont("Arial", 9), Brushes.Black, leftMargin, tempY);
                tempY += 16;
                g.DrawString(config.StoreAddress, new DrawFont("Arial", 9), Brushes.Black, new RectangleF(leftMargin, tempY, colWidth, 60));
                
                // Col 2: Customer
                tempY = y;
                g.DrawString("BILL TO:", new DrawFont("Arial", 10, FontStyle.Bold), Brushes.Black, leftMargin + colWidth, tempY);
                tempY += 18;
                g.DrawString(_sale.CustomerName?.ToUpper() ?? "WALK-IN CUSTOMER", new DrawFont("Arial", 9), Brushes.Black, leftMargin + colWidth, tempY);
                
                // Col 3: Invoice Info
                tempY = y;
                g.DrawString("INVOICE #:", new DrawFont("Arial", 9, FontStyle.Bold), Brushes.Black, leftMargin + colWidth * 2, tempY);
                g.DrawString(_sale.InvoiceNumber, new DrawFont("Arial", 9), Brushes.Black, leftMargin + colWidth * 2 + 80, tempY);
                tempY += 18;
                g.DrawString("DATE:", new DrawFont("Arial", 9, FontStyle.Bold), Brushes.Black, leftMargin + colWidth * 2, tempY);
                g.DrawString(_sale.SaleDate.ToString("yyyy-MM-dd"), new DrawFont("Arial", 9), Brushes.Black, leftMargin + colWidth * 2 + 80, tempY);
                tempY += 18;
                g.DrawString("CASHIER:", new DrawFont("Arial", 9, FontStyle.Bold), Brushes.Black, leftMargin + colWidth * 2, tempY);
                g.DrawString(_sale.UserName ?? "Admin", new DrawFont("Arial", 9), Brushes.Black, leftMargin + colWidth * 2 + 80, tempY);
                
                y += 100f;

                // 4. Items Table
                using (var brush = new SolidBrush(Color.FromArgb(41, 128, 185)))
                    g.FillRectangle(brush, leftMargin, y, rightMargin - leftMargin, 35f);
                
                g.DrawString("DESCRIPTION", new DrawFont("Arial", 11, FontStyle.Bold), Brushes.White, leftMargin + 10, y + 8);
                g.DrawString("QTY", new DrawFont("Arial", 11, FontStyle.Bold), Brushes.White, leftMargin + 400, y + 8);
                g.DrawString("UNIT PRICE", new DrawFont("Arial", 11, FontStyle.Bold), Brushes.White, leftMargin + 500, y + 8);
                g.DrawString("TOTAL", new DrawFont("Arial", 11, FontStyle.Bold), Brushes.White, rightMargin - 80, y + 8);
                y += 35f;

                bool alt = false;
                foreach (var item in _saleItems)
                {
                    if (alt) g.FillRectangle(new SolidBrush(Color.FromArgb(245, 245, 245)), leftMargin, y, rightMargin - leftMargin, 28f);
                    
                    g.DrawString(item.ProductName, new DrawFont("Arial", 10), Brushes.Black, leftMargin + 10, y + 6);
                    g.DrawString(item.Quantity.ToString(), new DrawFont("Arial", 10), Brushes.Black, leftMargin + 400, y + 6);
                    g.DrawString(item.UnitPrice.ToString("N2"), new DrawFont("Arial", 10), Brushes.Black, leftMargin + 500, y + 6);
                    g.DrawString(item.SubTotal.ToString("N2"), new DrawFont("Arial", 10, FontStyle.Bold), Brushes.Black, new RectangleF(rightMargin - 100, y + 6, 90, 20), new StringFormat { Alignment = StringAlignment.Far });
                    
                    y += 28f;
                    alt = !alt;
                }
                y += 20f;

                // 5. Totals
                float totalX = rightMargin - 250;
                g.DrawString("SUBTOTAL:", new DrawFont("Arial", 10), Brushes.Black, totalX, y);
                g.DrawString(_sale.SubTotal.ToString("N2"), new DrawFont("Arial", 10, FontStyle.Bold), Brushes.Black, new RectangleF(totalX + 120, y, 130, 20), new StringFormat { Alignment = StringAlignment.Far });
                y += 22;

                if (_sale.DiscountAmount > 0)
                {
                    g.DrawString($"DISCOUNT ({_sale.DiscountPercent}%):", new DrawFont("Arial", 10), Brushes.Black, totalX, y);
                    g.DrawString("-" + _sale.DiscountAmount.ToString("N2"), new DrawFont("Arial", 10, FontStyle.Bold), Brushes.Black, new RectangleF(totalX + 120, y, 130, 20), new StringFormat { Alignment = StringAlignment.Far });
                    y += 22;
                }

                if (_sale.TaxAmount > 0)
                {
                    g.DrawString($"TAX ({_sale.TaxPercent}%):", new DrawFont("Arial", 10), Brushes.Black, totalX, y);
                    g.DrawString("+" + _sale.TaxAmount.ToString("N2"), new DrawFont("Arial", 10, FontStyle.Bold), Brushes.Black, new RectangleF(totalX + 120, y, 130, 20), new StringFormat { Alignment = StringAlignment.Far });
                    y += 22;
                }

                y += 10;
                g.DrawString("GRAND TOTAL:", new DrawFont("Arial", 12, FontStyle.Bold), Brushes.Black, totalX, y);
                g.DrawString(_sale.TotalAmount.ToString("N2"), new DrawFont("Arial", 12, FontStyle.Bold), Brushes.Black, new RectangleF(totalX + 120, y, 130, 25), new StringFormat { Alignment = StringAlignment.Far });
                y += 40;

                // 6. Footer
                using (var brush = new SolidBrush(Color.FromArgb(41, 128, 185)))
                    g.FillRectangle(brush, leftMargin, 1050, rightMargin - leftMargin, 30f);
                
                g.DrawString("Developed By: DevFleet Technologies | +923225347757", new DrawFont("Arial", 9, FontStyle.Italic), Brushes.White, new RectangleF(leftMargin, 1057, rightMargin - leftMargin, 20), new StringFormat { Alignment = StringAlignment.Center });
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnSavePDF_Click(object sender, EventArgs e)
        {
            var sd = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = $"Invoice_{_sale.InvoiceNumber}.pdf" };
            if (sd.ShowDialog() == DialogResult.OK)
            {
                ExportToProfessionalPDF(sd.FileName);
                MessageBox.Show("PDF Saved Successfully!");
            }
        }

        private void ExportToProfessionalPDF(string filePath)
        {
            Document document = new Document(PageSize.A4, 50, 50, 50, 50);
            PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            document.Open();

            var bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            var titleFont = new PdfFont(bf, 24, PdfFont.BOLD);
            var headerFont = new PdfFont(bf, 12, PdfFont.BOLD, new BaseColor(41, 128, 185));
            var normalFont = new PdfFont(bf, 10, PdfFont.NORMAL);

            string storeName = ConfigurationService.Instance.ApplicationName.ToUpper();
            document.Add(new Paragraph(storeName, titleFont) { Alignment = Element.ALIGN_CENTER });
            document.Add(new Paragraph("Professional Sale Invoice", headerFont) { Alignment = Element.ALIGN_CENTER });
            document.Add(new Paragraph(" "));

            PdfPTable table = new PdfPTable(4) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 3f, 1f, 1.5f, 1.5f });
            
            table.AddCell(new PdfPCell(new Phrase("Product", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase("Qty", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase("Price", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase("Total", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

            foreach (var item in _saleItems)
            {
                table.AddCell(new Phrase(item.ProductName, normalFont));
                table.AddCell(new Phrase(item.Quantity.ToString(), normalFont));
                table.AddCell(new Phrase(item.UnitPrice.ToString("N2"), normalFont));
                table.AddCell(new Phrase(item.SubTotal.ToString("N2"), normalFont));
            }
            document.Add(table);
            
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph($"Subtotal: {_sale.SubTotal:N2}", normalFont) { Alignment = Element.ALIGN_RIGHT });
            if (_sale.DiscountAmount > 0)
                document.Add(new Paragraph($"Discount ({_sale.DiscountPercent}%): -{_sale.DiscountAmount:N2}", normalFont) { Alignment = Element.ALIGN_RIGHT });
            if (_sale.TaxAmount > 0)
                document.Add(new Paragraph($"Tax ({_sale.TaxPercent}%): +{_sale.TaxAmount:N2}", normalFont) { Alignment = Element.ALIGN_RIGHT });
            document.Add(new Paragraph($"Grand Total: {_sale.TotalAmount:N2}", titleFont) { Alignment = Element.ALIGN_RIGHT });

            document.Close();
        }
    }
}
