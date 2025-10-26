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

        public string GenerateCustomBarcode(string customText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customText))
                {
                    throw new Exception("Custom barcode text cannot be empty.");
                }

                // Clean the input text
                customText = customText.Trim();

                // Validate the custom barcode
                if (!ValidateBarcode(customText))
                {
                    throw new Exception("Invalid characters in barcode. Only letters, numbers, hyphens, underscores, and dots are allowed.");
                }

                // Check length (Code 128 supports up to 80 characters)
                if (customText.Length > 80)
                {
                    throw new Exception("Barcode text is too long. Maximum 80 characters allowed.");
                }

                if (customText.Length < 1)
                {
                    throw new Exception("Barcode text is too short. Minimum 1 character required.");
                }

                return customText;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating custom barcode: {ex.Message}", ex);
            }
        }

        public bool TestBarcodeGeneration(string barcodeData)
        {
            try
            {
                if (!ValidateBarcode(barcodeData))
                    return false;

                // Try to generate the barcode to test if it's valid
                var testImage = GenerateBarcodeImageObject(barcodeData, 100, 50);
                return testImage != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
