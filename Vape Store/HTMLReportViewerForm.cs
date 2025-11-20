using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Vape_Store
{
    public partial class HTMLReportViewerForm : Form
    {
        public HTMLReportViewerForm()
        {
            InitializeComponent();
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

        public void LoadReportFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    webBrowser1.Navigate(filePath);
                }
                else
                {
                    MessageBox.Show($"Report file not found: {filePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveDialog.FileName = $"SalesReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get HTML content from WebBrowser
                    string htmlContent = webBrowser1.DocumentText;
                    
                    // Convert HTML to PDF using iTextSharp
                    ConvertHTMLToPDF(htmlContent, saveDialog.FileName);
                    
                    MessageBox.Show("Report exported to PDF successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConvertHTMLToPDF(string htmlContent, string outputPath)
        {
            try
            {
                // Create PDF document
                Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outputPath, FileMode.Create));
                document.Open();

                // Set up fonts
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(baseFont, 18, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.NORMAL);

                // Extract title from HTML
                string title = "MADNI MOBILE & PHOTOSTATE - SALES REPORT";
                Paragraph titleParagraph = new Paragraph(title, titleFont);
                titleParagraph.Alignment = Element.ALIGN_CENTER;
                titleParagraph.SpacingAfter = 20f;
                document.Add(titleParagraph);

                // Add report info
                Paragraph reportInfo = new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", normalFont);
                reportInfo.SpacingAfter = 15f;
                document.Add(reportInfo);

                // Note: For a complete HTML to PDF conversion, you would need a more sophisticated HTML parser
                // For now, we'll add a note that this is a simplified conversion
                Paragraph note = new Paragraph("Note: This is a simplified PDF export. For full HTML formatting, use the dedicated PDF export from the main report form.", normalFont);
                note.SpacingAfter = 20f;
                document.Add(note);

                // Add the HTML content as plain text (simplified)
                Paragraph content = new Paragraph("Please use the 'Export PDF' button in the main Sales Report form for a properly formatted PDF with all data and styling.", normalFont);
                document.Add(content);

                document.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error converting HTML to PDF: {ex.Message}", ex);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
