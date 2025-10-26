using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;

namespace Vape_Store.Services
{
    /// <summary>
    /// Service class for barcode generation and validation
    /// Provides functionality to create, validate, and manage barcodes using Code 128 format
    /// </summary>
    public class BarcodeService
    {
        #region Constants

        /// <summary>
        /// Default barcode width in pixels
        /// </summary>
        private const int DEFAULT_WIDTH = 300;

        /// <summary>
        /// Default barcode height in pixels
        /// </summary>
        private const int DEFAULT_HEIGHT = 100;

        /// <summary>
        /// Maximum length for Code 128 barcode text
        /// </summary>
        private const int MAX_BARCODE_LENGTH = 80;

        /// <summary>
        /// Minimum length for barcode text
        /// </summary>
        private const int MIN_BARCODE_LENGTH = 1;

        #endregion

        #region Barcode Generation Methods

        /// <summary>
        /// Generates a barcode image as a byte array
        /// </summary>
        /// <param name="data">The data to encode in the barcode</param>
        /// <param name="width">Width of the barcode image in pixels</param>
        /// <param name="height">Height of the barcode image in pixels</param>
        /// <returns>Byte array containing the PNG image data</returns>
        /// <exception cref="Exception">Thrown when barcode generation fails</exception>
        public byte[] GenerateBarcodeImage(string data, int width = DEFAULT_WIDTH, int height = DEFAULT_HEIGHT)
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

        /// <summary>
        /// Generates a barcode image as a System.Drawing.Image object
        /// </summary>
        /// <param name="data">The data to encode in the barcode</param>
        /// <param name="width">Width of the barcode image in pixels</param>
        /// <param name="height">Height of the barcode image in pixels</param>
        /// <returns>System.Drawing.Image object containing the barcode</returns>
        /// <exception cref="Exception">Thrown when barcode generation fails</exception>
        public Image GenerateBarcodeImageObject(string data, int width = DEFAULT_WIDTH, int height = DEFAULT_HEIGHT)
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

        /// <summary>
        /// Generates a barcode image and returns it as a Base64 encoded string
        /// </summary>
        /// <param name="data">The data to encode in the barcode</param>
        /// <param name="width">Width of the barcode image in pixels</param>
        /// <param name="height">Height of the barcode image in pixels</param>
        /// <returns>Base64 encoded string containing the PNG image data</returns>
        /// <exception cref="Exception">Thrown when barcode generation fails</exception>
        public string GenerateBarcodeBase64(string data, int width = DEFAULT_WIDTH, int height = DEFAULT_HEIGHT)
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

        #endregion

        #region Barcode Validation Methods

        /// <summary>
        /// Validates barcode data for Code 128 format compatibility
        /// </summary>
        /// <param name="barcodeData">The barcode data to validate</param>
        /// <returns>True if the barcode data is valid, false otherwise</returns>
        public bool ValidateBarcode(string barcodeData)
        {
            try
            {
                // Basic validation for Code 128 format
                if (string.IsNullOrWhiteSpace(barcodeData))
                    return false;

                // Code 128 can contain alphanumeric characters and specific symbols
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

        /// <summary>
        /// Tests if barcode data can be successfully generated into an image
        /// </summary>
        /// <param name="barcodeData">The barcode data to test</param>
        /// <returns>True if barcode can be generated successfully, false otherwise</returns>
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

        #endregion

        #region Custom Barcode Methods

        /// <summary>
        /// Generates a custom barcode text with validation and formatting
        /// </summary>
        /// <param name="customText">The custom text to use for the barcode</param>
        /// <returns>Validated and formatted barcode text</returns>
        /// <exception cref="Exception">Thrown when custom text is invalid or generation fails</exception>
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

                // Check length constraints
                if (customText.Length > MAX_BARCODE_LENGTH)
                {
                    throw new Exception($"Barcode text is too long. Maximum {MAX_BARCODE_LENGTH} characters allowed.");
                }

                if (customText.Length < MIN_BARCODE_LENGTH)
                {
                    throw new Exception($"Barcode text is too short. Minimum {MIN_BARCODE_LENGTH} character required.");
                }

                return customText;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating custom barcode: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Generates a unique barcode using timestamp and random number
        /// </summary>
        /// <returns>Unique barcode string</returns>
        public string GenerateUniqueBarcode()
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string random = new Random().Next(1000, 9999).ToString();
                return $"AUTO_{timestamp}_{random}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating unique barcode: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the maximum allowed length for barcode text
        /// </summary>
        /// <returns>Maximum barcode length</returns>
        public int GetMaxBarcodeLength()
        {
            return MAX_BARCODE_LENGTH;
        }

        /// <summary>
        /// Gets the minimum required length for barcode text
        /// </summary>
        /// <returns>Minimum barcode length</returns>
        public int GetMinBarcodeLength()
        {
            return MIN_BARCODE_LENGTH;
        }

        #endregion
    }
}
