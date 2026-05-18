using System;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Repositories
{
    public class StoreSettingsRepository
    {
        public StoreSettings GetSettings()
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    string query = "SELECT TOP 1 * FROM StoreSettings ORDER BY SettingID DESC";
                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new StoreSettings
                                {
                                    SettingID = Convert.ToInt32(reader["SettingID"]),
                                    StoreName = reader["StoreName"].ToString(),
                                    StoreContact = reader["StoreContact"].ToString(),
                                    StoreAddress = reader["StoreAddress"].ToString(),
                                    StoreEmail = reader["StoreEmail"]?.ToString(),
                                    ReceiptFooter = reader["ReceiptFooter"]?.ToString(),
                                    UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"]),
                                    BarcodeDefaultLabel = reader["BarcodeDefaultLabel"]?.ToString(),
                                    BarcodeWidth = Convert.ToInt32(reader["BarcodeWidth"]),
                                    BarcodeHeight = Convert.ToInt32(reader["BarcodeHeight"]),
                                    BarcodeGap = Convert.ToDecimal(reader["BarcodeGap"]),
                                    BarcodeMarginLeft = Convert.ToDecimal(reader["BarcodeMarginLeft"]),
                                    BarcodeMarginRight = Convert.ToDecimal(reader["BarcodeMarginRight"]),
                                    BarcodeMarginTop = Convert.ToDecimal(reader["BarcodeMarginTop"]),
                                    BarcodeMarginBottom = Convert.ToDecimal(reader["BarcodeMarginBottom"]),
                                    BarcodeIsThermal = Convert.ToBoolean(reader["BarcodeIsThermal"]),
                                    ThermalPaperWidth = Convert.ToInt32(reader["ThermalPaperWidth"]),
                                    ThermalPrinterName = reader["ThermalPrinterName"]?.ToString(),
                                    BarcodePrinterName = reader["BarcodePrinterName"]?.ToString(),
                                    DirectPrintReceipt = reader["DirectPrintReceipt"] != DBNull.Value && Convert.ToBoolean(reader["DirectPrintReceipt"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting settings: {ex.Message}");
            }
            return null;
        }

        public bool UpdateSettings(StoreSettings settings)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    string query = @"
                        IF EXISTS (SELECT 1 FROM StoreSettings)
                        BEGIN
                            UPDATE StoreSettings SET 
                                StoreName = @StoreName,
                                StoreContact = @StoreContact,
                                StoreAddress = @StoreAddress,
                                StoreEmail = @StoreEmail,
                                ReceiptFooter = @ReceiptFooter,
                                BarcodeDefaultLabel = @BarcodeDefaultLabel,
                                BarcodeWidth = @BarcodeWidth,
                                BarcodeHeight = @BarcodeHeight,
                                BarcodeGap = @BarcodeGap,
                                BarcodeMarginLeft = @BarcodeMarginLeft,
                                BarcodeMarginRight = @BarcodeMarginRight,
                                BarcodeMarginTop = @BarcodeMarginTop,
                                BarcodeMarginBottom = @BarcodeMarginBottom,
                                BarcodeIsThermal = @BarcodeIsThermal,
                                ThermalPaperWidth = @ThermalPaperWidth,
                                ThermalPrinterName = @ThermalPrinterName,
                                BarcodePrinterName = @BarcodePrinterName,
                                DirectPrintReceipt = @DirectPrintReceipt,
                                UpdatedDate = GETDATE()
                        END
                        ELSE
                        BEGIN
                            INSERT INTO StoreSettings (StoreName, StoreContact, StoreAddress, StoreEmail, ReceiptFooter, 
                                BarcodeDefaultLabel, BarcodeWidth, BarcodeHeight, BarcodeGap, 
                                BarcodeMarginLeft, BarcodeMarginRight, BarcodeMarginTop, BarcodeMarginBottom, 
                                BarcodeIsThermal, ThermalPaperWidth, ThermalPrinterName, BarcodePrinterName, DirectPrintReceipt)
                            VALUES (@StoreName, @StoreContact, @StoreAddress, @StoreEmail, @ReceiptFooter, 
                                @BarcodeDefaultLabel, @BarcodeWidth, @BarcodeHeight, @BarcodeGap, 
                                @BarcodeMarginLeft, @BarcodeMarginRight, @BarcodeMarginTop, @BarcodeMarginBottom, 
                                @BarcodeIsThermal, @ThermalPaperWidth, @ThermalPrinterName, @BarcodePrinterName, @DirectPrintReceipt)
                        END";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StoreName", settings.StoreName);
                        command.Parameters.AddWithValue("@StoreContact", settings.StoreContact ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@StoreAddress", settings.StoreAddress ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@StoreEmail", settings.StoreEmail ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ReceiptFooter", settings.ReceiptFooter ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@BarcodeDefaultLabel", settings.BarcodeDefaultLabel ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@BarcodeWidth", settings.BarcodeWidth);
                        command.Parameters.AddWithValue("@BarcodeHeight", settings.BarcodeHeight);
                        command.Parameters.AddWithValue("@BarcodeGap", settings.BarcodeGap);
                        command.Parameters.AddWithValue("@BarcodeMarginLeft", settings.BarcodeMarginLeft);
                        command.Parameters.AddWithValue("@BarcodeMarginRight", settings.BarcodeMarginRight);
                        command.Parameters.AddWithValue("@BarcodeMarginTop", settings.BarcodeMarginTop);
                        command.Parameters.AddWithValue("@BarcodeMarginBottom", settings.BarcodeMarginBottom);
                        command.Parameters.AddWithValue("@BarcodeIsThermal", settings.BarcodeIsThermal);
                        command.Parameters.AddWithValue("@ThermalPaperWidth", settings.ThermalPaperWidth);
                        command.Parameters.AddWithValue("@ThermalPrinterName", settings.ThermalPrinterName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@BarcodePrinterName", settings.BarcodePrinterName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DirectPrintReceipt", settings.DirectPrintReceipt);

                        connection.Open();
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating settings: {ex.Message}");
                return false;
            }
        }
    }
}
