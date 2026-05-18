using System;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;

namespace Vape_Store
{
    public partial class SettingsForm : Form
    {
        private StoreSettingsRepository _repo;

        public SettingsForm()
        {
            InitializeComponent();
            
            _repo = new StoreSettingsRepository();
            guna2ShadowForm1.SetShadowForm(this);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            PopulatePrinters();
            LoadSettings();
        }

        private void PopulatePrinters()
        {
            try
            {
                cmbThermalPrinter.Items.Clear();
                cmbBarcodePrinter.Items.Clear();
                cmbThermalPrinter.Items.Add("System Default");
                cmbBarcodePrinter.Items.Add("System Default");

                foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    cmbThermalPrinter.Items.Add(printer);
                    cmbBarcodePrinter.Items.Add(printer);
                }

                cmbThermalPrinter.SelectedIndex = 0;
                cmbBarcodePrinter.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadSettings()
        {
            var settings = _repo.GetSettings();
            if (settings != null)
            {
                // Store Info
                txtStoreName.Text = settings.StoreName;
                txtStoreContact.Text = settings.StoreContact;
                txtStoreAddress.Text = settings.StoreAddress;
                txtStoreEmail.Text = settings.StoreEmail;
                txtReceiptFooter.Text = settings.ReceiptFooter;

                // Printing Defaults
                txtDefaultLabel.Text = settings.BarcodeDefaultLabel;
                numWidth.Value = settings.BarcodeWidth > 0 ? settings.BarcodeWidth : 130;
                numHeight.Value = settings.BarcodeHeight > 0 ? settings.BarcodeHeight : 90;
                numGap.Value = settings.BarcodeGap;
                numMarginLeft.Value = settings.BarcodeMarginLeft;
                numMarginTop.Value = settings.BarcodeMarginTop;
                numPaperWidth.Value = settings.ThermalPaperWidth > 0 ? settings.ThermalPaperWidth : 300;
                chkIsThermal.Checked = settings.BarcodeIsThermal;

                // Printer Selection
                if (!string.IsNullOrEmpty(settings.ThermalPrinterName) && cmbThermalPrinter.Items.Contains(settings.ThermalPrinterName))
                    cmbThermalPrinter.SelectedItem = settings.ThermalPrinterName;
                
                if (!string.IsNullOrEmpty(settings.BarcodePrinterName) && cmbBarcodePrinter.Items.Contains(settings.BarcodePrinterName))
                    cmbBarcodePrinter.SelectedItem = settings.BarcodePrinterName;

                chkDirectPrint.Checked = settings.DirectPrintReceipt;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStoreName.Text))
            {
                MessageBox.Show("Store Name is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var settings = new StoreSettings
            {
                StoreName = txtStoreName.Text.Trim(),
                StoreContact = txtStoreContact.Text.Trim(),
                StoreAddress = txtStoreAddress.Text.Trim(),
                StoreEmail = txtStoreEmail.Text.Trim(),
                ReceiptFooter = txtReceiptFooter.Text.Trim(),
                
                BarcodeDefaultLabel = txtDefaultLabel.Text.Trim(),
                BarcodeWidth = (int)numWidth.Value,
                BarcodeHeight = (int)numHeight.Value,
                BarcodeGap = numGap.Value,
                BarcodeMarginLeft = numMarginLeft.Value,
                BarcodeMarginRight = 12m,
                BarcodeMarginTop = numMarginTop.Value,
                BarcodeMarginBottom = 0m,
                BarcodeIsThermal = chkIsThermal.Checked,
                ThermalPaperWidth = (int)numPaperWidth.Value,

                ThermalPrinterName = cmbThermalPrinter.SelectedIndex > 0 ? cmbThermalPrinter.SelectedItem.ToString() : "",
                BarcodePrinterName = cmbBarcodePrinter.SelectedIndex > 0 ? cmbBarcodePrinter.SelectedItem.ToString() : "",
                DirectPrintReceipt = chkDirectPrint.Checked
            };

            if (_repo.UpdateSettings(settings))
            {
                ConfigurationService.Instance.RefreshSettings();
                MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to save settings. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
