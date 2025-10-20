using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;

namespace Vape_Store
{
    public partial class SupplierDueReportForm : Form
    {
        private SupplierRepository _supplierRepository;
        private PurchaseRepository _purchaseRepository;
        private SupplierPaymentRepository _supplierPaymentRepository;
        private ReportingService _reportingService;
        
        private List<SupplierDueReportItem> _supplierDueItems;
        private List<Supplier> _suppliers;

        public SupplierDueReportForm()
        {
            InitializeComponent();
            _supplierRepository = new SupplierRepository();
            _purchaseRepository = new PurchaseRepository();
            _supplierPaymentRepository = new SupplierPaymentRepository();
            _reportingService = new ReportingService();
            
            _supplierDueItems = new List<SupplierDueReportItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadSuppliers();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnGenerateReport.Click += BtnGenerateReport_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnExportPDF.Click += BtnExportPDF_Click;
            btnPrint.Click += BtnPrint_Click;
            btnClear.Click += BtnClear_Click;
            btnClose.Click += BtnClose_Click;
            
            // Date range event handlers
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpToDate.ValueChanged += DtpToDate_ValueChanged;
            
            // Filter event handlers
            cmbSupplier.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += SupplierDueReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvSupplierDueReport.AutoGenerateColumns = false;
                dgvSupplierDueReport.AllowUserToAddRows = false;
                dgvSupplierDueReport.AllowUserToDeleteRows = false;
                dgvSupplierDueReport.ReadOnly = true;
                dgvSupplierDueReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvSupplierDueReport.MultiSelect = false;

                // Define columns
                dgvSupplierDueReport.Columns.Clear();
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SupplierCode",
                    HeaderText = "Code",
                    DataPropertyName = "SupplierCode",
                    Width = 80
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SupplierName",
                    HeaderText = "Supplier Name",
                    DataPropertyName = "SupplierName",
                    Width = 200
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Phone",
                    HeaderText = "Phone",
                    DataPropertyName = "Phone",
                    Width = 120
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalPurchases",
                    HeaderText = "Total Purchases",
                    DataPropertyName = "TotalPurchases",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalPaid",
                    HeaderText = "Total Paid",
                    DataPropertyName = "TotalPaid",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalDue",
                    HeaderText = "Total Due",
                    DataPropertyName = "TotalDue",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LastPurchaseDate",
                    HeaderText = "Last Purchase",
                    DataPropertyName = "LastPurchaseDate",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvSupplierDueReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LastPaymentDate",
                    HeaderText = "Last Payment",
                    DataPropertyName = "LastPaymentDate",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadSuppliers()
        {
            try
            {
                _suppliers = _supplierRepository.GetAllSuppliers();
                cmbSupplier.DataSource = new List<Supplier> { new Supplier { SupplierID = 0, SupplierName = "All Suppliers" } }.Concat(_suppliers).ToList();
                cmbSupplier.DisplayMember = "SupplierName";
                cmbSupplier.ValueMember = "SupplierID";
                cmbSupplier.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading suppliers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadSupplierDueData()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                
                _supplierDueItems.Clear();
                
                foreach (var supplier in _suppliers)
                {
                    // Get purchases for this supplier
                    var purchases = _purchaseRepository.GetPurchasesBySupplierAndDateRange(supplier.SupplierID, fromDate, toDate);
                    var totalPurchases = purchases.Sum(p => p.TotalAmount);
                    
                    // Get payments for this supplier
                    var payments = _supplierPaymentRepository.GetPaymentsBySupplierAndDateRange(supplier.SupplierID, fromDate, toDate);
                    var totalPaid = payments.Sum(p => p.PaidAmount);
                    
                    var totalDue = totalPurchases - totalPaid;
                    var lastPurchaseDate = purchases.Any() ? purchases.Max(p => p.PurchaseDate) : (DateTime?)null;
                    var lastPaymentDate = payments.Any() ? payments.Max(p => p.PaymentDate) : (DateTime?)null;
                    
                    var reportItem = new SupplierDueReportItem
                    {
                        SupplierID = supplier.SupplierID,
                        SupplierCode = supplier.SupplierCode,
                        SupplierName = supplier.SupplierName,
                        Phone = supplier.Phone,
                        TotalPurchases = totalPurchases,
                        TotalPaid = totalPaid,
                        TotalDue = totalDue,
                        LastPurchaseDate = lastPurchaseDate,
                        LastPaymentDate = lastPaymentDate
                    };
                    
                    _supplierDueItems.Add(reportItem);
                }
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading supplier due data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = _supplierDueItems.AsEnumerable();
                
                // Supplier filter
                if (cmbSupplier.SelectedItem != null)
                {
                    var selectedSupplier = (Supplier)cmbSupplier.SelectedItem;
                    filteredItems = filteredItems.Where(item => item.SupplierID == selectedSupplier.SupplierID);
                }
                
                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        item.SupplierName.ToLower().Contains(searchTerm) ||
                        item.SupplierCode.ToLower().Contains(searchTerm) ||
                        item.Phone.ToLower().Contains(searchTerm));
                }
                
                dgvSupplierDueReport.DataSource = filteredItems.ToList();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error applying filters: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                dgvSupplierDueReport.Refresh();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateSummaryLabels()
        {
            try
            {
                var totalPurchases = _supplierDueItems.Sum(item => item.TotalPurchases);
                var totalPaid = _supplierDueItems.Sum(item => item.TotalPaid);
                var totalDue = _supplierDueItems.Sum(item => item.TotalDue);
                var suppliersWithDue = _supplierDueItems.Count(item => item.TotalDue > 0);
                var totalSuppliers = _supplierDueItems.Count;
                
                lblTotalPurchases.Text = $"Total Purchases: ${totalPurchases:F2}";
                lblTotalPaid.Text = $"Total Paid: ${totalPaid:F2}";
                lblTotalDue.Text = $"Total Due: ${totalDue:F2}";
                lblSuppliersWithDue.Text = $"Suppliers with Due: {suppliersWithDue}";
                lblTotalSuppliers.Text = $"Total Suppliers: {totalSuppliers}";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating summary labels: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;
            cmbSupplier.SelectedIndex = 0;
            txtSearch.Clear();
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadSupplierDueData();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_supplierDueItems == null || _supplierDueItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FileName = $"SupplierDueReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveFileDialog.FileName);
                    ShowMessage("Report exported successfully!", "Export Complete", MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting report: {ex.Message}", "Export Error", MessageBoxIcon.Error);
            }
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (_supplierDueItems == null || _supplierDueItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Implement PDF export functionality
                ShowMessage("PDF export functionality will be implemented in the next version.", "Feature Coming Soon", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting to PDF: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (_supplierDueItems == null || _supplierDueItems.Count == 0)
                {
                    ShowMessage("No data to print. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Implement print functionality
                ShowMessage("Print functionality will be implemented in the next version.", "Feature Coming Soon", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            SetInitialState();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFromDate.Value > dtpToDate.Value)
                dtpToDate.Value = dtpFromDate.Value;
        }

        private void DtpToDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpToDate.Value < dtpFromDate.Value)
                dtpFromDate.Value = dtpToDate.Value;
        }

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void ExportToCSV(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Supplier Code,Supplier Name,Phone,Total Purchases,Total Paid,Total Due,Last Purchase,Last Payment");
                
                // Write data
                foreach (var item in _supplierDueItems)
                {
                    writer.WriteLine($"{item.SupplierCode},{item.SupplierName},{item.Phone},{item.TotalPurchases:F2},{item.TotalPaid:F2},{item.TotalDue:F2},{item.LastPurchaseDate?.ToString("yyyy-MM-dd")},{item.LastPaymentDate?.ToString("yyyy-MM-dd")}");
                }
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void SupplierDueReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
        }
    }

    public class SupplierDueReportItem
    {
        public int SupplierID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Phone { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
    }
}

