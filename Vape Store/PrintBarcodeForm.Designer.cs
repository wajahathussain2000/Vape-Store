using System.Windows.Forms;
using System.Drawing;

namespace Vape_Store
{
    partial class PrintBarcodeForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtCode;
        private NumericUpDown numWidth;
        private NumericUpDown numHeight;
        private PictureBox pictureBox;
        private Button btnPreview;
        private Button btnPrint;
        private Button btnSave;
        private Panel panelActions;
        private NumericUpDown numCount;
        private TextBox txtLabel;
        private NumericUpDown numCols;
        private ComboBox cmbProduct;
        private Label lblProduct;
        private Label lblBarcodeValue;
        private CheckBox chkThermal;
        private ComboBox cmbSizePreset;
        private Label lblSizeInfo;
        private NumericUpDown numGap;
        private NumericUpDown numMarginLeft;
        private NumericUpDown numMarginRight;
        private NumericUpDown numMarginTop;
        private NumericUpDown numMarginBottom;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.Text = "Print Barcode";
            this.Size = new Size(720, 430);
            this.StartPosition = FormStartPosition.CenterParent;

            lblProduct = new Label { Text = "Product:", Left = 20, Top = 20, Width = 120 };
            cmbProduct = new ComboBox { Left = 150, Top = 16, Width = 250, DropDownStyle = ComboBoxStyle.DropDown, AutoCompleteMode = AutoCompleteMode.SuggestAppend, AutoCompleteSource = AutoCompleteSource.ListItems };

            var lblCode = new Label { Text = "Barcode Data:", Left = 20, Top = 55, Width = 120 };
            txtCode = new TextBox { Left = 150, Top = 51, Width = 250 };
            lblBarcodeValue = new Label { Left = 410, Top = 55, Width = 260, ForeColor = Color.DimGray };

            var lblW = new Label { Text = "Width:", Left = 20, Top = 90, Width = 120 };
            numWidth = new NumericUpDown { Left = 150, Top = 85, Width = 100, Minimum = 50, Maximum = 2000, Value = 300 };

            var lblH = new Label { Text = "Height:", Left = 270, Top = 90, Width = 60 };
            numHeight = new NumericUpDown { Left = 330, Top = 85, Width = 100, Minimum = 30, Maximum = 1000, Value = 100 };

            var lblCount = new Label { Text = "Quantity:", Left = 20, Top = 125, Width = 120 };
            numCount = new NumericUpDown { Left = 150, Top = 120, Width = 100, Minimum = 1, Maximum = 1000, Value = 1 };

            var lblCols = new Label { Text = "Columns:", Left = 270, Top = 125, Width = 70 };
            numCols = new NumericUpDown { Left = 340, Top = 120, Width = 90, Minimum = 1, Maximum = 6, Value = 3 };

            var lblText = new Label { Text = "Label (optional):", Left = 20, Top = 160, Width = 120 };
            txtLabel = new TextBox { Left = 150, Top = 156, Width = 280 };
            
            // Sticker gap control (for thermal printing)
            var lblGap = new Label { Text = "Gap (mm):", Left = 450, Top = 160, Width = 70 };
            numGap = new NumericUpDown { Left = 520, Top = 156, Width = 80, Minimum = 0, Maximum = 50, Value = 3, DecimalPlaces = 0 };
            
            // Margin controls - for fine-tuning barcode position
            var lblMargins = new Label { Text = "Margins (mm):", Left = 20, Top = 195, Width = 120, Font = new System.Drawing.Font("Segoe UI", 9, FontStyle.Bold) };
            
            var lblMarginLeft = new Label { Text = "Left:", Left = 150, Top = 195, Width = 40 };
            numMarginLeft = new NumericUpDown { Left = 190, Top = 191, Width = 60, Minimum = 0, Maximum = 50, Value = 0, DecimalPlaces = 0 };
            
            var lblMarginRight = new Label { Text = "Right:", Left = 260, Top = 195, Width = 45 };
            numMarginRight = new NumericUpDown { Left = 305, Top = 191, Width = 60, Minimum = 0, Maximum = 50, Value = 0, DecimalPlaces = 0 };
            
            var lblMarginTop = new Label { Text = "Top:", Left = 375, Top = 195, Width = 35 };
            numMarginTop = new NumericUpDown { Left = 410, Top = 191, Width = 60, Minimum = 0, Maximum = 50, Value = 0, DecimalPlaces = 0 };
            
            var lblMarginBottom = new Label { Text = "Bottom:", Left = 480, Top = 195, Width = 55 };
            numMarginBottom = new NumericUpDown { Left = 535, Top = 191, Width = 60, Minimum = 0, Maximum = 50, Value = 0, DecimalPlaces = 0 };

            // Action bar at bottom
            panelActions = new Panel { Dock = DockStyle.Bottom, Height = 56, Padding = new Padding(10), BackColor = SystemColors.Control };
            btnSave = new Button { Text = "Download", Width = 110, Height = 32, Anchor = AnchorStyles.Right | AnchorStyles.Top };
            btnPreview = new Button { Text = "Preview", Width = 110, Height = 32, Anchor = AnchorStyles.Right | AnchorStyles.Top };
            btnPrint = new Button { Text = "Print", Width = 110, Height = 32, Anchor = AnchorStyles.Right | AnchorStyles.Top };
            // Place buttons right-aligned inside action bar
            btnPrint.Left = panelActions.Width - btnPrint.Width - 10; btnPrint.Top = 12; btnPrint.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnPreview.Left = btnPrint.Left - btnPreview.Width - 10; btnPreview.Top = 12; btnPreview.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnSave.Left = btnPreview.Left - btnSave.Width - 10; btnSave.Top = 12; btnSave.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            panelActions.Controls.Add(btnSave);
            panelActions.Controls.Add(btnPreview);
            panelActions.Controls.Add(btnPrint);

            pictureBox = new PictureBox { Left = 20, Top = 225, Width = 650, Height = 135, BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };

            chkThermal = new CheckBox { Text = "Thermal Size (Roll)", Left = 450, Top = 125, Width = 150 };
            
            var lblPreset = new Label { Text = "Size Preset:", Left = 440, Top = 90, Width = 80 };
            cmbSizePreset = new ComboBox { Left = 520, Top = 86, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            
            lblSizeInfo = new Label { Text = "", Left = 440, Top = 60, Width = 250, Font = new Font("Segoe UI", 8, FontStyle.Italic), ForeColor = Color.Gray };
            

            this.Controls.Add(lblProduct);
            this.Controls.Add(cmbProduct);
            this.Controls.Add(lblCode);
            this.Controls.Add(txtCode);
            this.Controls.Add(lblBarcodeValue);
            this.Controls.Add(lblW);
            this.Controls.Add(numWidth);
            this.Controls.Add(lblH);
            this.Controls.Add(numHeight);
            this.Controls.Add(lblPreset);
            this.Controls.Add(cmbSizePreset);
            this.Controls.Add(lblSizeInfo);
            this.Controls.Add(lblGap);
            this.Controls.Add(numGap);
            this.Controls.Add(lblMargins);
            this.Controls.Add(lblMarginLeft);
            this.Controls.Add(numMarginLeft);
            this.Controls.Add(lblMarginRight);
            this.Controls.Add(numMarginRight);
            this.Controls.Add(lblMarginTop);
            this.Controls.Add(numMarginTop);
            this.Controls.Add(lblMarginBottom);
            this.Controls.Add(numMarginBottom);
            this.Controls.Add(lblCount);
            this.Controls.Add(numCount);
            this.Controls.Add(lblCols);
            this.Controls.Add(numCols);
            this.Controls.Add(lblText);
            this.Controls.Add(txtLabel);
            this.Controls.Add(chkThermal);
            this.Controls.Add(panelActions);
            this.Controls.Add(pictureBox);
        }
    }
}


