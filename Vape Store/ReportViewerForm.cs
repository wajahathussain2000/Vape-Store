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

namespace Vape_Store
{
    public partial class ReportViewerForm : Form
    {
        private WebBrowser webBrowser1;
        private Button btnClose;
        private Button btnExport;
        private Panel panel1;

        public ReportViewerForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.webBrowser1 = new WebBrowser();
            this.btnClose = new Button();
            this.btnExport = new Button();
            this.panel1 = new Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = DockStyle.Fill;
            this.webBrowser1.Location = new Point(0, 0);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new Size(800, 600);
            this.webBrowser1.TabIndex = 0;
            
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.btnClose.BackColor = Color.FromArgb(220, 53, 69);
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnClose.ForeColor = Color.White;
            this.btnClose.Location = new Point(720, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(70, 30);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new EventHandler(this.BtnClose_Click);
            
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.btnExport.BackColor = Color.FromArgb(40, 167, 69);
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.FlatStyle = FlatStyle.Flat;
            this.btnExport.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnExport.ForeColor = Color.White;
            this.btnExport.Location = new Point(640, 10);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new Size(70, 30);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new EventHandler(this.BtnExport_Click);
            
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = DockStyle.Bottom;
            this.panel1.Location = new Point(0, 570);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(800, 50);
            this.panel1.TabIndex = 2;
            
            // 
            // ReportViewerForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 620);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.panel1);
            this.Name = "ReportViewerForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Report Viewer";
            this.WindowState = FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        public void LoadReport(string htmlContent)
        {
            try
            {
                webBrowser1.DocumentText = htmlContent;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadSalesReport(DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Create a simple sales report data
                var salesData = new List<SalesReportItem>
                {
                    new SalesReportItem
                    {
                        SaleID = 1,
                        InvoiceNumber = "INV-001",
                        SaleDate = DateTime.Now.AddDays(-1),
                        CustomerName = "John Doe",
                        ProductName = "Vape Pen",
                        Quantity = 2,
                        UnitPrice = 25.50m,
                        SubTotal = 51.00m,
                        TaxAmount = 5.10m,
                        TotalAmount = 56.10m,
                        PaymentMethod = "Cash",
                        PaidAmount = 56.10m,
                        BalanceAmount = 0.00m
                    },
                    new SalesReportItem
                    {
                        SaleID = 2,
                        InvoiceNumber = "INV-002",
                        SaleDate = DateTime.Now.AddDays(-2),
                        CustomerName = "Jane Smith",
                        ProductName = "E-Liquid",
                        Quantity = 3,
                        UnitPrice = 15.00m,
                        SubTotal = 45.00m,
                        TaxAmount = 4.50m,
                        TotalAmount = 49.50m,
                        PaymentMethod = "Card",
                        PaidAmount = 49.50m,
                        BalanceAmount = 0.00m
                    }
                };

                // Generate HTML report
                string htmlContent = GenerateSalesReportHTML(salesData, fromDate, toDate);
                webBrowser1.DocumentText = htmlContent;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sales report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateSalesReportHTML(List<SalesReportItem> salesData, DateTime fromDate, DateTime toDate)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<title>Sales Report</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine("h1 { color: #333; text-align: center; }");
            html.AppendLine("h2 { color: #666; }");
            html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            html.AppendLine("th { background-color: #f2f2f2; font-weight: bold; }");
            html.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
            html.AppendLine(".total { font-weight: bold; background-color: #e6f3ff; }");
            html.AppendLine(".header { background-color: #4CAF50; color: white; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            html.AppendLine("<h1>Attock Mobiles Rwp - Sales Report</h1>");
            html.AppendLine($"<h2>Report Period: {fromDate:dd/MM/yyyy} to {toDate:dd/MM/yyyy}</h2>");
            html.AppendLine($"<p>Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            
            html.AppendLine("<table>");
            html.AppendLine("<thead>");
            html.AppendLine("<tr class='header'>");
            html.AppendLine("<th>Invoice #</th>");
            html.AppendLine("<th>Date</th>");
            html.AppendLine("<th>Customer</th>");
            html.AppendLine("<th>Product</th>");
            html.AppendLine("<th>Qty</th>");
            html.AppendLine("<th>Unit Price</th>");
            html.AppendLine("<th>Sub Total</th>");
            html.AppendLine("<th>Tax</th>");
            html.AppendLine("<th>Total</th>");
            html.AppendLine("<th>Paid</th>");
            html.AppendLine("</tr>");
            html.AppendLine("</thead>");
            html.AppendLine("<tbody>");
            
            decimal totalSales = 0;
            decimal totalTax = 0;
            decimal totalPaid = 0;
            
            foreach (var item in salesData)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{item.InvoiceNumber}</td>");
                html.AppendLine($"<td>{item.SaleDate:dd/MM/yyyy}</td>");
                html.AppendLine($"<td>{item.CustomerName}</td>");
                html.AppendLine($"<td>{item.ProductName}</td>");
                html.AppendLine($"<td>{item.Quantity}</td>");
                html.AppendLine($"<td>{item.UnitPrice:F2}</td>");
                html.AppendLine($"<td>{item.SubTotal:F2}</td>");
                html.AppendLine($"<td>{item.TaxAmount:F2}</td>");
                html.AppendLine($"<td>{item.TotalAmount:F2}</td>");
                html.AppendLine($"<td>{item.PaidAmount:F2}</td>");
                html.AppendLine("</tr>");
                
                totalSales += item.TotalAmount;
                totalTax += item.TaxAmount;
                totalPaid += item.PaidAmount;
            }
            
            html.AppendLine("</tbody>");
            html.AppendLine("<tfoot>");
            html.AppendLine("<tr class='total'>");
            html.AppendLine("<td colspan='6'><strong>Total:</strong></td>");
            html.AppendLine($"<td><strong>{totalSales - totalTax:F2}</strong></td>");
            html.AppendLine($"<td><strong>{totalTax:F2}</strong></td>");
            html.AppendLine($"<td><strong>{totalSales:F2}</strong></td>");
            html.AppendLine($"<td><strong>{totalPaid:F2}</strong></td>");
            html.AppendLine("</tr>");
            html.AppendLine("</tfoot>");
            html.AppendLine("</table>");
            
            html.AppendLine("<div style='margin-top: 30px;'>");
            html.AppendLine("<h3>Summary</h3>");
            html.AppendLine($"<p><strong>Total Sales:</strong> {totalSales:F2}</p>");
            html.AppendLine($"<p><strong>Total Tax:</strong> {totalTax:F2}</p>");
            html.AppendLine($"<p><strong>Total Paid:</strong> {totalPaid:F2}</p>");
            html.AppendLine($"<p><strong>Outstanding:</strong> {totalSales - totalPaid:F2}</p>");
            html.AppendLine("</div>");
            
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return html.ToString();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*";
                saveDialog.FileName = $"SalesReport_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string htmlContent = webBrowser1.DocumentText;
                    System.IO.File.WriteAllText(saveDialog.FileName, htmlContent);
                    MessageBox.Show("Report exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
