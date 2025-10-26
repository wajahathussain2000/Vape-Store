using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;

namespace Vape_Store.Services
{
    public class BarcodeService
    {
        public byte[] GenerateBarcodeImage(string data, int width = 300, int height = 100)
        {
            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Height = height,
                        Width = width,
                        Margin = 10,
                        PureBarcode = false
                    }
                };

                var bitmap = writer.Write(data);
                
                // Convert bitmap to byte array
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating barcode: {ex.Message}", ex);
            }
        }

        public Image GenerateBarcodeImageObject(string data, int width = 300, int height = 100)
        {
            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Height = height,
                        Width = width,
                        Margin = 10,
                        PureBarcode = false
                    }
                };

                return writer.Write(data);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating barcode: {ex.Message}", ex);
            }
        }

        public string GenerateBarcodeBase64(string data, int width = 300, int height = 100)
        {
            try
            {
                var barcodeBytes = GenerateBarcodeImage(data, width, height);
                return Convert.ToBase64String(barcodeBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating barcode base64: {ex.Message}", ex);
            }
        }

        public bool ValidateBarcode(string barcodeData)
        {
            try
            {
                // Basic validation for Code 128 format
                if (string.IsNullOrWhiteSpace(barcodeData))
                    return false;

                // Code 128 can contain alphanumeric characters
                foreach (char c in barcodeData)
                {
                    if (!char.IsLetterOrDigit(c) && c != '-' && c != '_' && c != '.')
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
