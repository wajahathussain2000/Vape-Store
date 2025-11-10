using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vape_Store.Models;
using Vape_Store.DataAccess;

namespace Vape_Store.Repositories
{
    public class SupplierPaymentRepository
    {
        public SupplierPaymentRepository()
        {
        }

        public List<SupplierPayment> GetAllSupplierPayments()
        {
            var payments = new List<SupplierPayment>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT sp.*, s.SupplierName, s.Phone as SupplierContact, u.FullName as UserName
                        FROM SupplierPayments sp
                        LEFT JOIN Suppliers s ON sp.SupplierID = s.SupplierID
                        LEFT JOIN Users u ON sp.UserID = u.UserID
                        ORDER BY sp.PaymentDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                payments.Add(new SupplierPayment
                                {
                                    PaymentID = Convert.ToInt32(reader["PaymentID"]),
                                    VoucherNumber = reader["VoucherNumber"].ToString(),
                                    SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                    PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                                    PreviousBalance = Convert.ToDecimal(reader["PreviousBalance"]),
                                    TotalPayable = Convert.ToDecimal(reader["TotalPayable"]),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    RemainingAmount = Convert.ToDecimal(reader["RemainingAmount"]),
                                    Description = reader["Description"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    SupplierName = reader["SupplierName"]?.ToString(),
                                    SupplierContact = reader["SupplierContact"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving supplier payments: {ex.Message}", ex);
            }

            return payments;
        }

        public SupplierPayment GetSupplierPaymentById(int paymentId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT sp.*, s.SupplierName, s.Phone as SupplierContact, u.FullName as UserName
                        FROM SupplierPayments sp
                        LEFT JOIN Suppliers s ON sp.SupplierID = s.SupplierID
                        LEFT JOIN Users u ON sp.UserID = u.UserID
                        WHERE sp.PaymentID = @PaymentID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PaymentID", paymentId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SupplierPayment
                                {
                                    PaymentID = Convert.ToInt32(reader["PaymentID"]),
                                    VoucherNumber = reader["VoucherNumber"].ToString(),
                                    SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                    PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                                    PreviousBalance = Convert.ToDecimal(reader["PreviousBalance"]),
                                    TotalPayable = Convert.ToDecimal(reader["TotalPayable"]),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    RemainingAmount = Convert.ToDecimal(reader["RemainingAmount"]),
                                    Description = reader["Description"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    SupplierName = reader["SupplierName"]?.ToString(),
                                    SupplierContact = reader["SupplierContact"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving supplier payment: {ex.Message}", ex);
            }

            return null;
        }

        public bool AddSupplierPayment(SupplierPayment payment)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var query = @"
                                INSERT INTO SupplierPayments (VoucherNumber, SupplierID, PaymentDate, PreviousBalance, 
                                                             TotalPayable, PaidAmount, RemainingAmount, Description, UserID, CreatedDate)
                                VALUES (@VoucherNumber, @SupplierID, @PaymentDate, @PreviousBalance, 
                                        @TotalPayable, @PaidAmount, @RemainingAmount, @Description, @UserID, @CreatedDate);
                                SELECT SCOPE_IDENTITY();";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@VoucherNumber", payment.VoucherNumber);
                                command.Parameters.AddWithValue("@SupplierID", payment.SupplierID);
                                command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                                command.Parameters.AddWithValue("@PreviousBalance", payment.PreviousBalance);
                                command.Parameters.AddWithValue("@TotalPayable", payment.TotalPayable);
                                command.Parameters.AddWithValue("@PaidAmount", payment.PaidAmount);
                                command.Parameters.AddWithValue("@RemainingAmount", payment.RemainingAmount);
                                command.Parameters.AddWithValue("@Description", payment.Description ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@UserID", payment.UserID);
                                command.Parameters.AddWithValue("@CreatedDate", payment.CreatedDate);

                                payment.PaymentID = Convert.ToInt32(command.ExecuteScalar());
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding supplier payment: {ex.Message}", ex);
            }
        }

        public bool UpdateSupplierPayment(SupplierPayment payment)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var query = @"
                                UPDATE SupplierPayments 
                                SET SupplierID = @SupplierID, PaymentDate = @PaymentDate, PreviousBalance = @PreviousBalance,
                                    TotalPayable = @TotalPayable, PaidAmount = @PaidAmount, RemainingAmount = @RemainingAmount,
                                    Description = @Description, UserID = @UserID
                                WHERE PaymentID = @PaymentID";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@PaymentID", payment.PaymentID);
                                command.Parameters.AddWithValue("@SupplierID", payment.SupplierID);
                                command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                                command.Parameters.AddWithValue("@PreviousBalance", payment.PreviousBalance);
                                command.Parameters.AddWithValue("@TotalPayable", payment.TotalPayable);
                                command.Parameters.AddWithValue("@PaidAmount", payment.PaidAmount);
                                command.Parameters.AddWithValue("@RemainingAmount", payment.RemainingAmount);
                                command.Parameters.AddWithValue("@Description", payment.Description ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@UserID", payment.UserID);

                                int rowsAffected = command.ExecuteNonQuery();
                                
                                if (rowsAffected == 0)
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating supplier payment: {ex.Message}", ex);
            }
        }

        public bool DeleteSupplierPayment(int paymentId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var query = "DELETE FROM SupplierPayments WHERE PaymentID = @PaymentID";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@PaymentID", paymentId);
                                int rowsAffected = command.ExecuteNonQuery();
                                
                                if (rowsAffected == 0)
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting supplier payment: {ex.Message}", ex);
            }
        }

        public string GetNextVoucherNumber()
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // More robust query that safely extracts numeric part from SP voucher numbers
                    // Only processes records that start with 'SP' and have numeric suffix
                    var query = @"
                        SELECT ISNULL(MAX(
                            CASE 
                                WHEN VoucherNumber LIKE 'SP%' 
                                     AND LEN(VoucherNumber) >= 3 
                                     AND ISNUMERIC(SUBSTRING(VoucherNumber, 3, LEN(VoucherNumber))) = 1
                                THEN CAST(SUBSTRING(VoucherNumber, 3, LEN(VoucherNumber)) AS INT)
                                ELSE NULL
                            END
                        ), 0) + 1
                        FROM SupplierPayments 
                        WHERE VoucherNumber IS NOT NULL 
                        AND VoucherNumber LIKE 'SP%'";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        var result = command.ExecuteScalar();
                        var nextNumber = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 1;
                        return $"SP{nextNumber:D6}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating voucher number: {ex.Message}", ex);
            }
        }

        public bool IsVoucherNumberExists(string voucherNumber, int? excludePaymentId = null)
        {
            if (string.IsNullOrWhiteSpace(voucherNumber))
                return false;

            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = "SELECT COUNT(*) FROM SupplierPayments WHERE VoucherNumber = @VoucherNumber";
                    
                    if (excludePaymentId.HasValue)
                    {
                        query += " AND PaymentID != @ExcludePaymentId";
                    }

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VoucherNumber", voucherNumber.Trim());
                        
                        if (excludePaymentId.HasValue)
                        {
                            command.Parameters.AddWithValue("@ExcludePaymentId", excludePaymentId.Value);
                        }
                        
                        connection.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking voucher number existence: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the total outstanding amount owed to supplier
        /// Formula: Total Purchases - Total Payments Made
        /// </summary>
        public decimal GetSupplierTotalPayable(int supplierId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Calculate total purchase amount for supplier
                    var purchasesQuery = @"
                        SELECT ISNULL(SUM(TotalAmount), 0) 
                        FROM Purchases 
                        WHERE SupplierID = @SupplierID";

                    decimal totalPurchases = 0;
                    using (var purchasesCommand = new SqlCommand(purchasesQuery, connection))
                    {
                        purchasesCommand.Parameters.AddWithValue("@SupplierID", supplierId);
                        connection.Open();
                        totalPurchases = Convert.ToDecimal(purchasesCommand.ExecuteScalar());
                    }

                    // Calculate total payments made to supplier
                    var paymentsQuery = @"
                        SELECT ISNULL(SUM(PaidAmount), 0) 
                        FROM SupplierPayments 
                        WHERE SupplierID = @SupplierID";

                    decimal totalPayments = 0;
                    using (var paymentsCommand = new SqlCommand(paymentsQuery, connection))
                    {
                        paymentsCommand.Parameters.AddWithValue("@SupplierID", supplierId);
                        totalPayments = Convert.ToDecimal(paymentsCommand.ExecuteScalar());
                    }

                    // Total Outstanding = Total Purchases - Total Payments
                    return totalPurchases - totalPayments;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating supplier total payable: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the remaining balance from the most recent payment transaction
        /// This represents the balance carried forward from the last payment
        /// </summary>
        public decimal GetSupplierPreviousBalance(int supplierId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Get the most recent payment's remaining amount
                    var query = @"
                        SELECT TOP 1 ISNULL(RemainingAmount, 0) 
                        FROM SupplierPayments 
                        WHERE SupplierID = @SupplierID
                        ORDER BY PaymentDate DESC, PaymentID DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SupplierID", supplierId);
                        connection.Open();
                        var result = command.ExecuteScalar();
                        return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating supplier previous balance: {ex.Message}", ex);
            }
        }

        public List<SupplierPayment> GetPaymentsBySupplierAndDateRange(int supplierId, DateTime fromDate, DateTime toDate)
        {
            var payments = new List<SupplierPayment>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT sp.*, s.SupplierName, u.FullName as UserName
                        FROM SupplierPayments sp
                        LEFT JOIN Suppliers s ON sp.SupplierID = s.SupplierID
                        LEFT JOIN Users u ON sp.UserID = u.UserID
                        WHERE sp.SupplierID = @SupplierID AND sp.PaymentDate BETWEEN @FromDate AND @ToDate
                        ORDER BY sp.PaymentDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SupplierID", supplierId);
                        command.Parameters.AddWithValue("@FromDate", fromDate);
                        command.Parameters.AddWithValue("@ToDate", toDate);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var payment = new SupplierPayment
                                {
                                    PaymentID = Convert.ToInt32(reader["PaymentID"]),
                                    VoucherNumber = reader["VoucherNumber"].ToString(),
                                    SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                    PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                                    PreviousBalance = Convert.ToDecimal(reader["PreviousBalance"]),
                                    TotalPayable = Convert.ToDecimal(reader["TotalPayable"]),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    RemainingAmount = Convert.ToDecimal(reader["RemainingAmount"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    SupplierName = reader["SupplierName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                };
                                
                                payments.Add(payment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving supplier payments by supplier and date range: {ex.Message}", ex);
            }

            return payments;
        }
    }
}
