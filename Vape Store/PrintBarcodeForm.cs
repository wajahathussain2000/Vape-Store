using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;
using Vape_Store.Services;
using Vape_Store.Repositories;
using Vape_Store.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Vape_Store
{
    public partial class PrintBarcodeForm : Form
    {
        private readonly BarcodeService _barcodeService;
        private System.Drawing.Image _previewImage;
        private PrintDocument _printDocument;
        private readonly ProductRepository _productRepository = new ProductRepository();
        private System.Collections.Generic.List<Product> _products;

        // Snapshot fields for printing to ensure thread safety and consistent state
        private string _jobCode;
        private string _jobLabel;
        private int _jobWidth;
        private int _jobHeight;
        private int _jobCount;
        private int _jobCols;
        private bool _jobIsThermal;
        private double _jobGap;
        private double _jobMarginLeft;
        private double _jobMarginRight;
        private double _jobMarginTop;
        private double _jobMarginBottom;

        public PrintBarcodeForm()
        {
            InitializeComponent();
            
            _barcodeService = new BarcodeService();
            _printDocument = new PrintDocument();
            _printDocument.PrintPage += PrintDocumentOnPrintPage;

            // Wire events
            btnPreview.Click += (s, e) => Preview();
            btnPrint.Click += (s, e) => {
                try
                {
                    // Validate barcode data
                    if (string.IsNullOrWhiteSpace(txtCode.Text))
                    {
                        MessageBox.Show("Please enter barcode data.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Ensure we have a preview image (needed for validation)
                    if (_previewImage == null)
                    {
                        _previewImage = GenerateCompositePreview();
                    }

                    // CAPTURE SETTINGS SNAPSHOT
                    _jobCode = txtCode.Text.Trim();
                    _jobLabel = txtLabel.Text;
                    _jobWidth = (int)numWidth.Value;
                    _jobHeight = (int)numHeight.Value;
                    _jobCount = (int)numCount.Value;
                    _jobCols = (int)numCols.Value;
                    _jobIsThermal = chkThermal.Checked;
                    _jobGap = (double)numGap.Value;
                    _jobMarginLeft = (double)numMarginLeft.Value;
                    _jobMarginRight = (double)numMarginRight.Value;
                    _jobMarginTop = (double)numMarginTop.Value;
                    _jobMarginBottom = (double)numMarginBottom.Value;

                    // Reset print counter before printing
                    _printedCount = 0;

                    // Re-initialize PrintDocument to prevent state issues (fixes single-page print bug)
                    if (_printDocument != null) _printDocument.Dispose();
                    _printDocument = new PrintDocument();
                    _printDocument.PrintPage += PrintDocumentOnPrintPage;

                    SetupThermalPageSettings();
                    _printDocument.Print();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Print Error"); }
            };
            btnSave.Click += (s, e) => SaveComposite();
            cmbProduct.SelectedIndexChanged += (s, e) => OnProductSelected();
            chkThermal.CheckedChanged += (s, e) => {
                numCols.Enabled = !chkThermal.Checked;
                if (chkThermal.Checked) numCols.Value = 1;
            };
            InitializePresets();
            cmbSizePreset.SelectedIndexChanged += (s, e) => OnPresetChanged();
            
            // Update size info when user manually changes width/height
            numWidth.ValueChanged += (s, e) => UpdateSizeInfo();
            numHeight.ValueChanged += (s, e) => UpdateSizeInfo();

            // Skip database calls during design-time to let the Designer load
            if (!IsDesignMode())
            {
                LoadProducts();
                InitializeForm();
            }
        }

        private void InitializeForm()
        {
            try
            {
                var config = ConfigurationService.Instance;
                
                // Set Default Settings from configuration
                txtLabel.Text = config.BarcodeDefaultLabel;
                txtCode.Clear();
                lblBarcodeValue.Text = "";
                cmbProduct.SelectedIndex = -1;
                pictureBox.Image = null;

                // Apply Default Layout Settings from configuration
                chkThermal.Checked = config.BarcodeIsThermal;
                numWidth.Value = config.BarcodeWidth;
                numHeight.Value = config.BarcodeHeight;
                numGap.Value = config.BarcodeGap;
                
                // Margins
                numMarginLeft.Value = config.BarcodeMarginLeft;
                numMarginRight.Value = config.BarcodeMarginRight;
                numMarginTop.Value = config.BarcodeMarginTop;
                numMarginBottom.Value = config.BarcodeMarginBottom;
            }
            catch { }
        }

        private bool IsDesignMode()
        {
            return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime || DesignMode;
        }

        private void Preview()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCode.Text))
                {
                    MessageBox.Show("Please enter barcode data.", "Validation");
                    return;
                }
                _previewImage = GenerateCompositePreview();
                pictureBox.Image = _previewImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Preview error: {ex.Message}", "Error");
            }
        }

        private void LoadProducts()
        {
            try
            {
                _products = _productRepository.GetAllProducts();
                cmbProduct.DataSource = _products;
                cmbProduct.DisplayMember = nameof(Product.ProductName);
                cmbProduct.ValueMember = nameof(Product.ProductID);
            }
            catch
            {
                // ignore load issues; user can still type custom code
            }
        }

        private void OnProductSelected()
        {
            try
            {
                if (cmbProduct.SelectedItem is Product p)
                {
                    txtCode.Text = p.Barcode ?? string.Empty;
                    // Don't auto-fill label - keep it optional as the field name suggests
                    // User can manually enter label if needed
                    lblBarcodeValue.Text = $"DB Code: {p.Barcode ?? "N/A"}";
                }
                else
                {
                    // Clear fields if no product selected
                    txtCode.Clear();
                    lblBarcodeValue.Text = "";
                }
            }
            catch { }
        }

        private void InitializePresets()
        {
            cmbSizePreset.Items.Add("Custom (Adjust Manually)");
            
            // Common thermal sticker sizes
            cmbSizePreset.Items.Add("40mm x 30mm (Small Sticker)");
            cmbSizePreset.Items.Add("50mm x 25mm (Standard Label)");
            cmbSizePreset.Items.Add("50mm x 30mm (Medium Label)");
            cmbSizePreset.Items.Add("60mm x 40mm (Large Label)");
            cmbSizePreset.Items.Add("70mm x 30mm (Wide Label)");
            cmbSizePreset.Items.Add("80mm x 40mm (Extra Large)");
            
            // Standard paper sizes
            cmbSizePreset.Items.Add("100mm x 50mm (Shipping Label)");
            cmbSizePreset.Items.Add("100mm x 150mm (4x6\" Label)");
            
            cmbSizePreset.SelectedIndex = 0; // Default to Custom
        }

        private void OnPresetChanged()
        {
            switch (cmbSizePreset.SelectedIndex)
            {
                case 0: // Custom - do nothing
                    break;
                case 1: // 40x30mm
                    numWidth.Value = 150;
                    numHeight.Value = 115;
                    break;
                case 2: // 50x25mm (Standard)
                    numWidth.Value = 190;
                    numHeight.Value = 95;
                    break;
                case 3: // 50x30mm
                    numWidth.Value = 190;
                    numHeight.Value = 115;
                    break;
                case 4: // 60x40mm
                    numWidth.Value = 230;
                    numHeight.Value = 150;
                    break;
                case 5: // 70x30mm
                    numWidth.Value = 265;
                    numHeight.Value = 115;
                    break;
                case 6: // 80x40mm
                    numWidth.Value = 300;
                    numHeight.Value = 150;
                    break;
                case 7: // 100x50mm
                    numWidth.Value = 380;
                    numHeight.Value = 190;
                    break;
                case 8: // 100x150mm (4x6")
                    numWidth.Value = 380;
                    numHeight.Value = 570;
                    break;
            }
            
            // Update size info label when preset changes
            UpdateSizeInfo();
        }

        private void UpdateSizeInfo()
        {
            // Calculate actual size in mm and inches
            // Assuming 96 DPI (standard screen resolution)
            // 1 inch = 96 pixels, 1 inch = 25.4mm
            
            double widthMm = ((double)numWidth.Value / 96.0) * 25.4;
            double heightMm = ((double)numHeight.Value / 96.0) * 25.4;
            
            double widthInch = (double)numWidth.Value / 96.0;
            double heightInch = (double)numHeight.Value / 96.0;
            
            // Update label if it exists
            if (lblSizeInfo != null)
            {
                lblSizeInfo.Text = $"≈ {widthMm:F0}mm × {heightMm:F0}mm  ({widthInch:F1}\" × {heightInch:F1}\")";
            }
        }

        private System.Drawing.Image GenerateCompositePreview()
        {
            // Create one barcode image
            var single = _barcodeService.GenerateBarcodeImageObject(txtCode.Text.Trim(), (int)numWidth.Value, (int)numHeight.Value);

            int count = (int)numCount.Value;
            int cols = (int)numCols.Value;
            if (cols <= 0) cols = 1;

            // Thermal mode: Show vertical roll format with separators
            if (chkThermal.Checked)
            {
                return GenerateThermalPreview(single, count);
            }

            // Standard mode: Grid layout
            int rows = (int)Math.Ceiling(count / (double)cols);

            int gutter = 10;
            int cellW = (int)numWidth.Value;
            int cellH = (int)numHeight.Value + 30; // label area
            int bmpW = cols * cellW + (cols - 1) * gutter;
            int bmpH = rows * cellH + (rows - 1) * gutter;

            var bmp = new Bitmap(bmpW, bmpH);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new System.Drawing.Font("Segoe UI", 9))
            using (var brush = new SolidBrush(Color.Black))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.Clear(Color.White);
                int printed = 0;
                for (int r = 0; r < rows && printed < count; r++)
                {
                    for (int c = 0; c < cols && printed < count; c++)
                    {
                        int x = c * (cellW + gutter);
                        int y = r * (cellH + gutter);
                        g.DrawImage(single, new System.Drawing.Rectangle(x, y, cellW, (int)numHeight.Value));
                        if (!string.IsNullOrWhiteSpace(txtLabel.Text))
                        {
                            g.DrawString(txtLabel.Text, font, brush, new RectangleF(x, y + (int)numHeight.Value + 5, cellW, 20), sf);
                        }
                        printed++;
                    }
                }
            }
            single.Dispose();
            return bmp;
        }

        private System.Drawing.Image GenerateThermalPreview(System.Drawing.Image barcodeImage, int count)
        {
            // Thermal roll preview: Show stickers vertically with dashed separators
            int stickerWidth = (int)numWidth.Value;
            int stickerHeight = (int)numHeight.Value;
            int labelHeight = 25; // Space for text label
            
            // Convert gap from mm to pixels (1mm ≈ 3.78 pixels at 96 DPI)
            // Formula: pixels = (mm / 25.4) * 96
            double gapMm = (double)numGap.Value;
            int gapPixels = (int)Math.Round((gapMm / 25.4) * 96);
            int separatorHeight = Math.Max(15, gapPixels); // Minimum 15px for visual separator
            
            // Convert margins from mm to pixels
            double marginLeftMm = (double)numMarginLeft.Value;
            double marginTopMm = (double)numMarginTop.Value;
            int marginLeftPx = (int)Math.Round((marginLeftMm / 25.4) * 96);
            int marginTopPx = (int)Math.Round((marginTopMm / 25.4) * 96);
            
            // Total height for one sticker unit (barcode + label + gap)
            int unitHeight = stickerHeight + labelHeight + separatorHeight + marginTopPx;
            
            // Canvas size (include margins)
            int canvasWidth = stickerWidth + 40 + marginLeftPx; // Add padding + left margin
            int canvasHeight = (unitHeight * count) + 20; // Add top/bottom padding
            
            var bmp = new Bitmap(canvasWidth, canvasHeight);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new System.Drawing.Font("Segoe UI", 8))
            using (var brush = new SolidBrush(Color.Black))
            using (var grayBrush = new SolidBrush(Color.Gray))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            using (var dashedPen = new Pen(Color.Gray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
            using (var marginPen = new Pen(Color.LightBlue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
            {
                g.Clear(Color.White);
                
                for (int i = 0; i < count; i++)
                {
                    int yOffset = 10 + (i * unitHeight) + marginTopPx; // Apply top margin
                    int xOffset = 20 + marginLeftPx; // Apply left margin
                    
                    // Draw margin guides (light blue dotted lines) if margins are set
                    if (marginLeftMm > 0 || marginTopMm > 0)
                    {
                        // Left margin line
                        if (marginLeftMm > 0)
                            g.DrawLine(marginPen, xOffset, yOffset - marginTopPx, xOffset, yOffset + stickerHeight + labelHeight);
                        
                        // Top margin line
                        if (marginTopMm > 0)
                            g.DrawLine(marginPen, xOffset, yOffset, xOffset + stickerWidth, yOffset);
                    }
                    
                    // Draw barcode
                    g.DrawImage(barcodeImage, xOffset, yOffset, stickerWidth, stickerHeight);
                    
                    // Draw label text below barcode
                    if (!string.IsNullOrWhiteSpace(txtLabel.Text))
                    {
                        g.DrawString(txtLabel.Text, font, brush, 
                            new RectangleF(xOffset, yOffset + stickerHeight + 3, stickerWidth, 20), sf);
                    }
                    
                    // Draw dashed separator line (except after last sticker)
                    if (i < count - 1)
                    {
                        int separatorY = yOffset + stickerHeight + labelHeight + (separatorHeight / 2);
                        
                        // Draw dashed line across the width
                        g.DrawLine(dashedPen, 5, separatorY, canvasWidth - 5, separatorY);
                        
                        // Optional: Add small "CUT HERE" text or gap indicator
                        using (var smallFont = new System.Drawing.Font("Segoe UI", 6, FontStyle.Italic))
                        {
                            if (gapMm > 0)
                            {
                                g.DrawString($"✂ {gapMm}mm gap", smallFont, grayBrush, 
                                    new RectangleF(0, separatorY - 8, canvasWidth, 16), sf);
                            }
                            else
                            {
                                g.DrawString("✂", smallFont, grayBrush, 
                                    new RectangleF(0, separatorY - 8, canvasWidth, 16), sf);
                            }
                        }
                    }
                }
            }
            
            barcodeImage.Dispose();
            return bmp;
        }

        private void SaveComposite()
        {
            try
            {
                if (_previewImage == null)
                {
                    _previewImage = GenerateCompositePreview();
                }
                using (var sfd = new SaveFileDialog
                {
                    Filter = "PNG Image|*.png|PDF Document|*.pdf",
                    FilterIndex = 1,
                    FileName = $"barcode_{DateTime.Now:yyyyMMdd_HHmmss}"
                })
                {
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        string extension = Path.GetExtension(sfd.FileName).ToLower();

                        if (extension == ".pdf")
                        {
                            SaveAsPdf(sfd.FileName);
                        }
                        else
                        {
                            // Default to PNG
                            _previewImage.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        MessageBox.Show("Saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveAsPdf(string filePath)
        {
            try
            {
                // Create PDF document
                Document document = new Document(PageSize.A4, 20, 20, 20, 20);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                // Convert Image to iTextSharp Image
                iTextSharp.text.Image pdfImage;
                using (MemoryStream ms = new MemoryStream())
                {
                    _previewImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    pdfImage = iTextSharp.text.Image.GetInstance(ms.ToArray());
                }

                // Scale image to fit page width while maintaining aspect ratio
                float pageWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                float pageHeight = document.PageSize.Height - document.TopMargin - document.BottomMargin;

                if (pdfImage.Width > pageWidth)
                {
                    float ratio = pageWidth / pdfImage.Width;
                    pdfImage.ScaleAbsoluteWidth(pageWidth);
                    pdfImage.ScaleAbsoluteHeight(pdfImage.Height * ratio);
                }

                // Center the image on the page
                pdfImage.SetAbsolutePosition(
                    document.LeftMargin + (pageWidth - pdfImage.ScaledWidth) / 2,
                    document.PageSize.Height - document.TopMargin - pdfImage.ScaledHeight - 20
                );

                document.Add(pdfImage);
                document.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating PDF: {ex.Message}", ex);
            }
        }

        private void SetupThermalPageSettings()
        {
            if (_jobIsThermal)
            {
                // For thermal sticker printing, we rely on the Printer Driver to handle copies
                // This is much more robust than sending multiple pages manually
                _printDocument.PrinterSettings.Copies = (short)_jobCount;

                // IMPORTANT: Set page size to the EXACT user-defined physical label size
                // Do NOT add extra height for text/gap here, or the printer will see a size mismatch
                // and stop after the first label (the "1 print" bug).
                
                int widthInHundredths = (int)((double)_jobWidth / 96.0 * 100);
                int heightInHundredths = (int)((double)_jobHeight / 96.0 * 100);
                
                // Minimum safety constraint (e.g., 0.2 inches)
                if (widthInHundredths < 20) widthInHundredths = 20; 
                if (heightInHundredths < 20) heightInHundredths = 20;
                
                // Create custom paper size matching the PHYSICAL sticker
                PaperSize customSize = new PaperSize("Sticker", widthInHundredths, heightInHundredths);
                _printDocument.DefaultPageSettings.PaperSize = customSize;
                _printDocument.DefaultPageSettings.Landscape = false;
                
                // Minimal margins
                _printDocument.DefaultPageSettings.Margins = new Margins(2, 2, 2, 2);
            }
            else
            {
                // Standard printing: Reset copies to 1 (we handle tiling manually)
                _printDocument.PrinterSettings.Copies = 1;
                _printDocument.DefaultPageSettings.PaperSize = null;
                _printDocument.DefaultPageSettings.Margins = new Margins(100, 100, 100, 100);
            }
        }

        private void PrintDocumentOnPrintPage(object sender, PrintPageEventArgs e)
        {
            if (_previewImage == null)
            {
                e.Cancel = true;
                return;
            }

            if (_jobIsThermal)
            {
                PrintThermal(e);
            }
            else
            {
                PrintStandard(e);
            }
        }

        private int _printedCount = 0;

        private void PrintThermal(PrintPageEventArgs e)
        {
            // For Thermal, we use PrinterSettings.Copies, so we only render ONE page here.
            // No loops, no HasMorePages logic.

            // Available space depends on the configured Page Size (which matches _jobHeight)
            int pageW = (int)e.PageBounds.Width;
            int pageH = (int)e.PageBounds.Height;
            
            // Convert margins
            int marginLeftPx = (int)Math.Round((_jobMarginLeft / 25.4) * 96);
            int marginTopPx = (int)Math.Round((_jobMarginTop / 25.4) * 96);
            int marginBottomPx = (int)Math.Round((_jobMarginBottom / 25.4) * 96);
            
            // Determine required space for text
            int textHeight = !string.IsNullOrWhiteSpace(_jobLabel) ? 25 : 0; 
            
            // Safety Buffer: Add 5px padding at the bottom to prevent "last label cutoff"
            // This lifts the text away from the tear-off line/gap
            int safetyPad = 5;
            
            // Calculate strictly available height for the BARCODE
            // Total - Top Margin - Bottom Margin - Text - Safety Pad
            int availableHeightForBarcode = pageH - marginTopPx - marginBottomPx - textHeight - safetyPad;
            
            // Safety: Ensure barcode has at least minimal visibility (e.g., 20px)
            if (availableHeightForBarcode < 20) availableHeightForBarcode = 20;

            using (var font = new System.Drawing.Font("Segoe UI", 8))
            using (var brush = new SolidBrush(Color.Black))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                float x = marginLeftPx;
                float y = marginTopPx;
                
                // Draw barcode
                // It will stretch/shrink to fit exactly in the calculated available space
                var img = _barcodeService.GenerateBarcodeImageObject(_jobCode, pageW, availableHeightForBarcode);
                e.Graphics.DrawImage(img, x, y, pageW, availableHeightForBarcode);
                img.Dispose();
                
                // Draw label text below barcode
                if (textHeight > 0)
                {
                    // Center the text in the designated text area
                    // Position: Top Margin + Barcode Height
                    float labelY = y + availableHeightForBarcode;
                    var textRect = new RectangleF(0, labelY, pageW, textHeight);
                    e.Graphics.DrawString(_jobLabel, font, brush, textRect, sf);
                }
            }
            
            // Strictly ONE page. The Driver handles the 'Copies'.
            e.HasMorePages = false;
        }





        private void PrintStandard(PrintPageEventArgs e)
        {
            // Tiled printing of N barcodes (numCount) in numCols columns
            int count = _jobCount;
            int cols = _jobCols;
            if (cols <= 0) cols = 1;

            var margin = e.MarginBounds;
            int gutter = 10;
            int cellWidth = (margin.Width - (cols - 1) * gutter) / cols;
            // Use snapshot height
            int cellHeight = (int)Math.Max(_jobHeight + 30, 60); // barcode + label area

            int row = 0;
            using (var font = new System.Drawing.Font("Segoe UI", 9))
            using (var brush = new SolidBrush(Color.Black))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                while (_printedCount < count)
                {
                    for (int col = 0; col < cols && _printedCount < count; col++)
                    {
                        int x = margin.Left + col * (cellWidth + gutter);
                        int y = margin.Top + row * (cellHeight + gutter);

                        var img = _barcodeService.GenerateBarcodeImageObject(_jobCode, _jobWidth, _jobHeight);
                        int drawW = cellWidth;
                        int drawH = _jobHeight;
                        e.Graphics.DrawImage(img, new System.Drawing.Rectangle(x, y, drawW, drawH));
                        img.Dispose();

                        if (!string.IsNullOrWhiteSpace(_jobLabel))
                        {
                            e.Graphics.DrawString(_jobLabel, font, brush, new RectangleF(x, y + drawH + 5, drawW, 20), sf);
                        }

                        _printedCount++;
                    }

                    row++;

                    if (margin.Top + (row + 1) * (cellHeight + gutter) > margin.Bottom)
                    {
                        e.HasMorePages = _printedCount < count;
                        if (!e.HasMorePages) _printedCount = 0;
                        return;
                    }
                }
                _printedCount = 0;
            }
        }
    }
}
