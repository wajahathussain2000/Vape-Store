using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Vape_Store.Services;
using Vape_Store.Repositories;
using Vape_Store.Models;

namespace Vape_Store
{
    public partial class PrintBarcodeForm : Form
    {
        private readonly BarcodeService _barcodeService;
        private Image _previewImage;
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
            }
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
                    txtLabel.Text = string.IsNullOrWhiteSpace(txtLabel.Text) ? p.ProductName : txtLabel.Text;
                    lblBarcodeValue.Text = $"DB Code: {p.Barcode}";
                }
            }
            catch { }
        }

        private Image GenerateCompositePreview()
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
            using (var font = new Font("Segoe UI", 9))
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
                        g.DrawImage(single, new Rectangle(x, y, cellW, (int)numHeight.Value));
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
                using (var sfd = new SaveFileDialog { Filter = "PNG Image|*.png", FileName = $"barcode_{DateTime.Now:yyyyMMdd_HHmmss}.png" })
                {
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        _previewImage.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        MessageBox.Show("Saved.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save error: {ex.Message}", "Error");
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
            using (var font = new Font("Segoe UI", 9))
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
                        e.Graphics.DrawImage(img, new Rectangle(x, y, drawW, drawH));
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


