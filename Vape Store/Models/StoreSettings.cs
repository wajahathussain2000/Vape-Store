using System;

namespace Vape_Store.Models
{
    public class StoreSettings
    {
        public int SettingID { get; set; }
        public string StoreName { get; set; }
        public string StoreContact { get; set; }
        public string StoreAddress { get; set; }
        public string StoreEmail { get; set; }
        public string ReceiptFooter { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Barcode printing settings
        public string BarcodeDefaultLabel { get; set; }
        public int BarcodeWidth { get; set; }
        public int BarcodeHeight { get; set; }
        public decimal BarcodeGap { get; set; }
        public decimal BarcodeMarginLeft { get; set; }
        public decimal BarcodeMarginRight { get; set; }
        public decimal BarcodeMarginTop { get; set; }
        public decimal BarcodeMarginBottom { get; set; }
        public bool BarcodeIsThermal { get; set; }
        
        // Receipt settings
        public int ThermalPaperWidth { get; set; }

        // Printer selection settings
        public string ThermalPrinterName { get; set; }
        public string BarcodePrinterName { get; set; }
        public bool DirectPrintReceipt { get; set; }
    }
}
