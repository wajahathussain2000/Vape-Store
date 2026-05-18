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
        private const int _printLeftMargin = 24;
        private const int _printRightMargin = 26;
        private const int _printTopMargin = 10;
        private const int _printBottomMargin = 20;
        private Panel _thermalReceiptPanel;

        public ThermalInvoiceForm()
        {
            InitializeComponent();
            
            _saleRepository = new SaleRepository();
            _saleItems = new List<SaleItem>();
            
            // Set up fonts for thermal printing
            _headerFont = new Font("Arial", 10f, FontStyle.Bold);
            _bodyFont = new Font("Arial", 8f, FontStyle.Regular);
            _footerFont = new Font("Arial", 8f, FontStyle.Italic);
            
            SetupThermalReceiptPanel();
            SetupEventHandlers();
            LoadInvoiceNumbers();
            
            // Apply theme
            ThemeManager.ApplyTheme(this);
            pnlButtonContainer.BackColor = Color.FromArgb(240, 240, 240);
            pnlReceiptContainer.BackColor = Color.FromArgb(100, 100, 100);
            
            SetInitialState();
        }

        public ThermalInvoiceForm(Purchase purchase, List<PurchaseItem> purchaseItems) : this()
        {
            _currentPurchase = purchase;
            _purchaseItems = purchaseItems;
            
            // Hide selection controls since we are in direct preview mode
            pnlSelection.Visible = false;
            
            UpdateThermalReceipt();
            
            btnPrintInvoice.Enabled = true;
            btnPreviewInvoice.Enabled = true;
            btnDownloadPDF.Enabled = true;
        }

        public ThermalInvoiceForm(Sale sale) : this()
        {
            _currentSale = sale;
            _saleItems = sale.SaleItems ?? new List<SaleItem>();
            
            // Hide selection controls since we are in direct preview mode
            pnlSelection.Visible = false;
            
            UpdateThermalReceipt();
            
            btnPrintInvoice.Enabled = true;
            btnPreviewInvoice.Enabled = true;
            btnDownloadPDF.Enabled = true;
        }

        private void SetupEventHandlers()
        {
            btnLoadSale.Click += BtnLoadSale_Click;
            btnPrintInvoice.Click += BtnPrintInvoice_Click;
            btnPreviewInvoice.Click += BtnPreviewInvoice_Click;
            btnDownloadPDF.Click += BtnDownloadPDF_Click;
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
            }
        }

        private void CmbInvoiceNumber_TextChanged(object sender, EventArgs e)
        {
            btnLoadSale.Enabled = !string.IsNullOrWhiteSpace(cmbInvoiceNumber.Text);
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
            lblSaleInfo.Text = "Select an Invoice Number to Load";
            btnLoadSale.Enabled = false;
            btnPrintInvoice.Enabled = false;
            btnPreviewInvoice.Enabled = false;
            ClearThermalReceipt();
        }

        private void BtnLoadSale_Click(object sender, EventArgs e)
        {
            try
            {
                string invoiceNumber = cmbInvoiceNumber.Text.Trim();
                if (string.IsNullOrWhiteSpace(invoiceNumber)) return;

                _currentSale = _saleRepository.GetSaleByInvoiceNumber(invoiceNumber);
                if (_currentSale == null)
                {
                    lblSaleInfo.Text = "Sale not found";
                    ClearThermalReceipt();
                    btnPrintInvoice.Enabled = false;
                    btnPreviewInvoice.Enabled = false;
                    return;
                }

                _saleItems = _saleRepository.GetSaleItems(_currentSale.SaleID);
                _currentSale.SaleItems = _saleItems ?? new List<SaleItem>();

                // Update thermal receipt display
                UpdateThermalReceipt();

                // Update info label
                lblSaleInfo.Text = $"Sale Found: {_currentSale.InvoiceNumber} - {_currentSale.SaleDate:MM/dd/yyyy}";
                
                // Enable buttons only if we have valid sale data AND items
                bool hasValidData = _currentSale != null && _saleItems != null && _saleItems.Count > 0;
                btnPrintInvoice.Enabled = hasValidData;
                btnPreviewInvoice.Enabled = hasValidData;
                btnDownloadPDF.Enabled = hasValidData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sale: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearThermalReceipt();
                btnPrintInvoice.Enabled = false;
                btnPreviewInvoice.Enabled = false;
            }
        }

        private void SetupThermalReceiptPanel()
        {
            // Create thermal receipt panel to look like paper
            _thermalReceiptPanel = new Panel();
            _thermalReceiptPanel.Size = new Size(320, 1000); // 80mm width roughly
            _thermalReceiptPanel.BackColor = Color.White;
            _thermalReceiptPanel.BorderStyle = BorderStyle.None;
            _thermalReceiptPanel.Paint += ThermalReceiptPanel_Paint;
            
            // Center the paper in its container
            _thermalReceiptPanel.Location = new Point((pnlReceiptContainer.Width - _thermalReceiptPanel.Width) / 2, 20);
            
            pnlReceiptContainer.AutoScroll = true;
            pnlReceiptContainer.Controls.Add(_thermalReceiptPanel);
            
            pnlReceiptContainer.SizeChanged += (s, e) => {
                _thermalReceiptPanel.Left = Math.Max(20, (pnlReceiptContainer.Width - _thermalReceiptPanel.Width - 20) / 2);
            };
        }

        private void ThermalReceiptPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_currentSale == null && _currentPurchase == null)
            {
                StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("Please load a transaction\nto preview the receipt.", 
                    new Font("Segoe UI", 10, FontStyle.Italic), 
                    Brushes.LightGray, 
                    new Rectangle(0, 0, _thermalReceiptPanel.Width, 300), 
                    centerFormat);
                return;
            }

            DrawThermalInvoice(e.Graphics, _thermalReceiptPanel.Width);
        }

        private void DrawThermalInvoice(Graphics g, int width)
        {
            g.Clear(Color.White);
            float y = 15;
            float margin = 15;
            float printableWidth = width - (margin * 2);
            float centerX = width / 2f;

            var titleFont = new Font("Arial", 10, FontStyle.Bold);
            var headerFont = new Font("Arial", 9, FontStyle.Bold);
            var bodyFont = new Font("Arial", 8, FontStyle.Regular);
            var footerFont = new Font("Arial", 7, FontStyle.Regular);
            
            var centerFormat = new StringFormat { Alignment = StringAlignment.Center };
            var rightFormat = new StringFormat { Alignment = StringAlignment.Far };
            var leftFormat = new StringFormat { Alignment = StringAlignment.Near };

            // 1. Store Header
            string storeName = (ConfigurationService.Instance.ApplicationName ?? "VAPE STORE").ToUpper();
            g.DrawString(storeName, titleFont, Brushes.Black, new RectangleF(margin, y, printableWidth, 25), centerFormat);
            y += 22;

            g.DrawString("Contact: " + ConfigurationService.Instance.StoreContact, bodyFont, Brushes.Black, new RectangleF(margin, y, printableWidth, 18), centerFormat);
            y += 16;

            string address = ConfigurationService.Instance.StoreAddress ?? "";
            var addressRect = new RectangleF(margin, y, printableWidth, 40);
            g.DrawString(address, bodyFont, Brushes.Black, addressRect, centerFormat);
            y += g.MeasureString(address, bodyFont, (int)printableWidth).Height + 5;

            // Separator
            g.DrawLine(Pens.Black, margin, y, width - margin, y);
            y += 8;

            // 2. Transaction Info
            string invNo = _currentSale?.InvoiceNumber ?? _currentPurchase?.InvoiceNumber ?? "N/A";
            DateTime date = _currentSale?.SaleDate ?? _currentPurchase?.PurchaseDate ?? DateTime.Now;
            string user = (_currentSale?.UserName ?? _currentPurchase?.UserName) ?? "Admin";

            g.DrawString("INVOICE: " + invNo, headerFont, Brushes.Black, margin, y);
            y += 18;
            g.DrawString("DATE: " + date.ToString("MM/dd/yyyy HH:mm"), bodyFont, Brushes.Black, margin, y);
            y += 16;
            g.DrawString("CASHIER: " + user, bodyFont, Brushes.Black, margin, y);
            y += 20;

            string partyType = _currentSale != null ? "CUSTOMER: " : "SUPPLIER: ";
            string partyName = (_currentSale != null ? (_currentSale.CustomerName ?? "Walk-in") : (_currentPurchase?.SupplierName ?? "N/A")).ToUpper();
            g.DrawString(partyType + partyName, bodyFont, Brushes.Black, margin, y);
            y += 25;

            // 3. Items Table Header
            g.DrawLine(new Pen(Color.Black, 1f), margin, y, width - margin, y);
            y += 5;

            float colQty = 40;
            float colTotal = 80;
            float colProduct = printableWidth - colQty - colTotal;

            g.DrawString("ITEM", headerFont, Brushes.Black, margin, y);
            g.DrawString("QTY", headerFont, Brushes.Black, margin + colProduct, y);
            g.DrawString("TOTAL", headerFont, Brushes.Black, margin + colProduct + colQty, y);
            y += 18;
            g.DrawLine(Pens.Black, margin, y, width - margin, y);
            y += 8;

            // 4. Item Rows
            if (_currentSale != null)
            {
                foreach (var item in _saleItems)
                {
                    string name = item.ProductName;
                    var nameSize = g.MeasureString(name, bodyFont, (int)colProduct);
                    g.DrawString(name, bodyFont, Brushes.Black, new RectangleF(margin, y, colProduct, nameSize.Height), leftFormat);
                    
                    g.DrawString(item.Quantity.ToString(), bodyFont, Brushes.Black, margin + colProduct, y);
                    g.DrawString(item.SubTotal.ToString("N2"), bodyFont, Brushes.Black, new RectangleF(margin + colProduct + colQty, y, colTotal, 20), rightFormat);
                    
                    y += Math.Max(20, nameSize.Height + 5);
                }
            }
            else if (_currentPurchase != null)
            {
                foreach (var item in _purchaseItems)
                {
                    string name = item.ProductName;
                    var nameSize = g.MeasureString(name, bodyFont, (int)colProduct);
                    g.DrawString(name, bodyFont, Brushes.Black, new RectangleF(margin, y, colProduct, nameSize.Height), leftFormat);
                    
                    g.DrawString(item.Quantity.ToString(), bodyFont, Brushes.Black, margin + colProduct, y);
                    g.DrawString(item.SubTotal.ToString("N2"), bodyFont, Brushes.Black, new RectangleF(margin + colProduct + colQty, y, colTotal, 20), rightFormat);
                    
                    y += Math.Max(20, nameSize.Height + 5);
                }
            }

            // 5. Totals Section
            y += 5;
            g.DrawLine(Pens.Black, margin, y, width - margin, y);
            y += 10;

            decimal subTotal = _currentSale?.SubTotal ?? _currentPurchase?.SubTotal ?? 0;
            decimal total = _currentSale?.TotalAmount ?? _currentPurchase?.TotalAmount ?? 0;
            decimal paid = _currentSale?.PaidAmount ?? _currentPurchase?.PaidAmount ?? 0;
            decimal change = _currentSale != null ? _currentSale.ChangeAmount : 0;

            DrawTotalRow(g, "SUBTOTAL:", subTotal, bodyFont, margin, y, printableWidth);
            y += 18;

            if ((_currentSale?.DiscountAmount ?? 0) > 0)
            {
                DrawTotalRow(g, $"DISCOUNT ({_currentSale.DiscountPercent}%):", _currentSale.DiscountAmount, bodyFont, margin, y, printableWidth);
                y += 18;
            }

            if ((_currentSale?.TaxAmount ?? 0) > 0)
            {
                DrawTotalRow(g, $"TAX ({_currentSale.TaxPercent}%):", _currentSale.TaxAmount, bodyFont, margin, y, printableWidth);
                y += 18;
            }

            y += 5;
            g.DrawString("GRAND TOTAL:", headerFont, Brushes.Black, margin, y);
            g.DrawString(total.ToString("N2"), headerFont, Brushes.Black, new RectangleF(margin, y, printableWidth, 20), rightFormat);
            y += 22;
            g.DrawLine(new Pen(Color.Black, 1.5f), margin, y, width - margin, y);
            y += 8;

            DrawTotalRow(g, "CASH PAID:", paid, bodyFont, margin, y, printableWidth);
            y += 16;
            DrawTotalRow(g, "CHANGE:", change, bodyFont, margin, y, printableWidth);
            y += 25;

            // 6. Barcode
            string bc = _currentSale?.BarcodeData ?? _currentPurchase?.BarcodeData;
            if (!string.IsNullOrEmpty(bc))
            {
                try {
                    var bs = new BarcodeService();
                    var img = bs.GenerateBarcodeImageObject(bc, (int)printableWidth, 40);
                    if (img != null) {
                        g.DrawImage(img, margin, y, printableWidth, 40);
                        y += 45;
                        g.DrawString(bc, footerFont, Brushes.Black, new RectangleF(margin, y, printableWidth, 15), centerFormat);
                        y += 20;
                    }
                } catch { }
            }

            // 7. Footer
            string footerText = ConfigurationService.Instance.ReceiptFooter ?? "Thank you for your business!";
            var footerSize = g.MeasureString(footerText, footerFont, (int)printableWidth);
            var footerRect = new RectangleF(margin, y, printableWidth, footerSize.Height + 5);
            g.DrawString(footerText, footerFont, Brushes.Black, footerRect, centerFormat);
            y += footerSize.Height + 10;

            g.DrawString("Powered By: DevFleet Technologies | +923225347757", footerFont, Brushes.DimGray, new RectangleF(margin, y, printableWidth, 15), centerFormat);

            // Update Panel height based on content
            if (y + 50 > _thermalReceiptPanel.Height) {
                _thermalReceiptPanel.Height = (int)y + 100;
            }
        }

        private void DrawTotalRow(Graphics g, string label, decimal val, Font font, float margin, float y, float width)
        {
            g.DrawString(label, font, Brushes.Black, margin, y);
            g.DrawString(val.ToString("N2"), font, Brushes.Black, new RectangleF(margin, y, width, 20), new StringFormat { Alignment = StringAlignment.Far });
        }

        private void UpdateThermalReceipt()
        {
            if (_thermalReceiptPanel != null)
            {
                _thermalReceiptPanel.Invalidate();
            }
        }

        private void ClearThermalReceipt()
        {
            _currentSale = null;
            _currentPurchase = null;
            UpdateThermalReceipt();
        }

        private void BtnPreviewInvoice_Click(object sender, EventArgs e)
        {
            if (_currentSale == null)
            {
                MessageBox.Show("Please load a sale first.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // The current form already provides a high-quality thermal preview.
                // If they want the A4 Full Preview, we can redirect to the new SaleReceiptPreviewForm.
                var a4Preview = new SaleReceiptPreviewForm(_currentSale, _saleItems);
                a4Preview.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing A4 preview: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrintInvoice_Click(object sender, EventArgs e)
        {
            if (_currentPurchase == null && _currentSale == null)
            {
                MessageBox.Show("Please load data first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                PrintDocument printDoc = new PrintDocument();
                
                // Get default printer from settings
                var settingsRepo = new Repositories.StoreSettingsRepository();
                var settings = settingsRepo.GetSettings();
                if (settings != null && !string.IsNullOrEmpty(settings.ThermalPrinterName) && settings.ThermalPrinterName != "System Default")
                {
                    printDoc.PrinterSettings.PrinterName = settings.ThermalPrinterName;
                }

                printDoc.PrintPage += (s, ppe) => {
                    // Use the same professional drawing logic for the physical printer
                    DrawThermalInvoice(ppe.Graphics, (int)ppe.PageSettings.PrintableArea.Width);
                };

                // Dynamic height calculation
                int dynamicHeight = EstimateContentHeight();
                var paperSize = new PaperSize("Thermal80", 300, Math.Max(dynamicHeight, 600));
                printDoc.DefaultPageSettings.PaperSize = paperSize;
                printDoc.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDoc;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDoc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int EstimateContentHeight()
        {
            // Simple estimation for paper height
            int itemCount = (_currentSale?.SaleItems?.Count ?? _purchaseItems?.Count ?? 0);
            return 350 + (itemCount * 25);
        }

        private void BtnDownloadPDF_Click(object sender, EventArgs e)
        {
            if (_currentSale == null && _currentPurchase == null) return;

            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"Thermal_Receipt_{(_currentSale?.InvoiceNumber ?? _currentPurchase?.InvoiceNumber)}.pdf",
                    Title = "Export Professional Thermal Receipt"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToProfessionalPDF(saveDialog.FileName);
                    MessageBox.Show("Professional Receipt exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToProfessionalPDF(string filePath)
        {
            // Set 80mm width in points
            float widthPoints = 226f; 
            float heightPoints = EstimateContentHeight() * 0.75f; 
            
            iTextSharp.text.Document doc = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(widthPoints, Math.Max(450, heightPoints)), 15, 15, 15, 15);
            
            using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                iTextSharp.text.pdf.PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var baseFont = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                var boldFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.BOLD);
                var normalFont = new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL);
                var titleFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);

                // Store Info
                var pTitle = new iTextSharp.text.Paragraph((ConfigurationService.Instance.ApplicationName ?? "VAPE STORE").ToUpper(), titleFont);
                pTitle.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                doc.Add(pTitle);

                var pAddr = new iTextSharp.text.Paragraph(ConfigurationService.Instance.StoreAddress, normalFont);
                pAddr.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                doc.Add(pAddr);
                
                doc.Add(new iTextSharp.text.Paragraph(" ", normalFont));
                doc.Add(new iTextSharp.text.Paragraph($"INV: {(_currentSale?.InvoiceNumber ?? _currentPurchase?.InvoiceNumber)}", boldFont));
                doc.Add(new iTextSharp.text.Paragraph($"DATE: {DateTime.Now:MM/dd/yyyy HH:mm}", normalFont));
                
                doc.Add(new iTextSharp.text.Paragraph("--------------------------------------------------", normalFont));

                // Items Table
                iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 3f, 1f, 1.5f });

                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ITEM", boldFont)) { Border = 0 });
                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("QTY", boldFont)) { Border = 0 });
                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("TOTAL", boldFont)) { Border = 0, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT });

                if (_currentSale != null) {
                    foreach (var item in _saleItems) {
                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.ProductName, normalFont)) { Border = 0 });
                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.Quantity.ToString(), normalFont)) { Border = 0 });
                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.SubTotal.ToString("N2"), normalFont)) { Border = 0, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT });
                    }
                }

                doc.Add(table);
                doc.Add(new iTextSharp.text.Paragraph("--------------------------------------------------", normalFont));

                // Totals
                decimal subTotal  = _currentSale?.SubTotal  ?? _currentPurchase?.SubTotal  ?? 0;
                decimal total     = _currentSale?.TotalAmount ?? _currentPurchase?.TotalAmount ?? 0;
                decimal paid      = _currentSale?.PaidAmount  ?? _currentPurchase?.PaidAmount  ?? 0;
                decimal change    = _currentSale != null ? _currentSale.ChangeAmount : 0;

                var pSubTotal = new iTextSharp.text.Paragraph($"SUBTOTAL: {subTotal:N2}", normalFont);
                pSubTotal.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                doc.Add(pSubTotal);

                if ((_currentSale?.DiscountAmount ?? 0) > 0)
                {
                    var pDiscount = new iTextSharp.text.Paragraph($"DISCOUNT ({_currentSale.DiscountPercent}%): -{_currentSale.DiscountAmount:N2}", normalFont);
                    pDiscount.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                    doc.Add(pDiscount);
                }

                if ((_currentSale?.TaxAmount ?? 0) > 0)
                {
                    var pTax = new iTextSharp.text.Paragraph($"TAX ({_currentSale.TaxPercent}%): +{_currentSale.TaxAmount:N2}", normalFont);
                    pTax.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                    doc.Add(pTax);
                }

                doc.Add(new iTextSharp.text.Paragraph("--------------------------------------------------", normalFont));
                var pTotal = new iTextSharp.text.Paragraph($"GRAND TOTAL: {total:N2}", boldFont);
                pTotal.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                doc.Add(pTotal);

                var pPaid = new iTextSharp.text.Paragraph($"CASH PAID: {paid:N2}", normalFont);
                pPaid.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                doc.Add(pPaid);

                var pChange = new iTextSharp.text.Paragraph($"CHANGE: {change:N2}", normalFont);
                pChange.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                doc.Add(pChange);

                doc.Add(new iTextSharp.text.Paragraph(" ", normalFont));
                var pFooter = new iTextSharp.text.Paragraph(ConfigurationService.Instance.ReceiptFooter, normalFont);
                pFooter.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                doc.Add(pFooter);

                doc.Close();
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SetPurchaseData(Purchase purchase, List<PurchaseItem> purchaseItems)
        {
            _currentPurchase = purchase;
            _purchaseItems = purchaseItems;
            pnlSelection.Visible = false;
            UpdateThermalReceipt();
        }

        private void btnLoadSale_Click_1(object sender, EventArgs e)
        {

        }
    }
}

