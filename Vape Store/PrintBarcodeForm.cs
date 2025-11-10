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

        public PrintBarcodeForm()
        {
            InitializeComponent();
            _barcodeService = new BarcodeService();
            _printDocument = new PrintDocument();
            _printDocument.PrintPage += PrintDocumentOnPrintPage;

            // Wire events
            btnPreview.Click += (s, e) => Preview();
            btnPrint.Click += (s, e) => { if (_previewImage != null) try { _printDocument.Print(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Print Error"); } };
            btnSave.Click += (s, e) => SaveComposite();
            cmbProduct.SelectedIndexChanged += (s, e) => OnProductSelected();

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
                // Clear all fields on form load
                txtLabel.Clear();
                txtCode.Clear();
                lblBarcodeValue.Text = "";
                cmbProduct.SelectedIndex = -1;
                pictureBox.Image = null;
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

        private System.Drawing.Image GenerateCompositePreview()
        {
            // Create one barcode image
            var single = _barcodeService.GenerateBarcodeImageObject(txtCode.Text.Trim(), (int)numWidth.Value, (int)numHeight.Value);
            int count = (int)numCount.Value;
            int cols = (int)numCols.Value;
            if (cols <= 0) cols = 1;
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

        private void PrintDocumentOnPrintPage(object sender, PrintPageEventArgs e)
        {
            if (_previewImage == null)
            {
                e.Cancel = true;
                return;
            }

            // Tiled printing of N barcodes (numCount) in numCols columns
            int count = (int)numCount.Value;
            int cols = (int)numCols.Value;
            if (cols <= 0) cols = 1;

            var margin = e.MarginBounds;
            int gutter = 10;
            int cellWidth = (margin.Width - (cols - 1) * gutter) / cols;
            int cellHeight = (int)Math.Max(numHeight.Value + 30, 60); // barcode + label area

            int printed = 0;
            int row = 0;
            using (var font = new System.Drawing.Font("Segoe UI", 9))
            using (var brush = new SolidBrush(Color.Black))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                while (printed < count)
                {
                    for (int col = 0; col < cols && printed < count; col++)
                    {
                        int x = margin.Left + col * (cellWidth + gutter);
                        int y = margin.Top + row * (cellHeight + gutter);

                        // Generate image for each (so scaling matches current settings)
                        var img = _barcodeService.GenerateBarcodeImageObject(txtCode.Text.Trim(), (int)numWidth.Value, (int)numHeight.Value);
                        int drawW = cellWidth;
                        int drawH = (int)numHeight.Value;
                        e.Graphics.DrawImage(img, new System.Drawing.Rectangle(x, y, drawW, drawH));
                        img.Dispose();

                        // Draw label text if provided
                        if (!string.IsNullOrWhiteSpace(txtLabel.Text))
                        {
                            e.Graphics.DrawString(txtLabel.Text, font, brush, new RectangleF(x, y + drawH + 5, drawW, 20), sf);
                        }

                        printed++;
                    }

                    row++;

                    // Check if another row fits; if not, continue on next page
                    if (margin.Top + (row + 1) * (cellHeight + gutter) > margin.Bottom)
                    {
                        e.HasMorePages = printed < count;
                        return;
                    }
                }
            }
        }
    }
}


