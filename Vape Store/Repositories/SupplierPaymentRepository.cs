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

                                command.ExecuteNonQuery();
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
                                command.ExecuteNonQuery();
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
                    var query = @"
                        SELECT ISNULL(MAX(CAST(SUBSTRING(VoucherNumber, 3, LEN(VoucherNumber)) AS INT)), 0) + 1
                        FROM SupplierPayments 
                        WHERE VoucherNumber LIKE 'SP%'";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        var nextNumber = Convert.ToInt32(command.ExecuteScalar());
                        return $"SP{nextNumber:D6}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating voucher number: {ex.Message}", ex);
            }
        }

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

                    return totalPurchases - totalPayments;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating supplier total payable: {ex.Message}", ex);
            }
        }

        public decimal GetSupplierPreviousBalance(int supplierId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT ISNULL(SUM(RemainingAmount), 0) 
                        FROM SupplierPayments 
                        WHERE SupplierID = @SupplierID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SupplierID", supplierId);
                        connection.Open();
                        return Convert.ToDecimal(command.ExecuteScalar());
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
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    RemainingBalance = Convert.ToDecimal(reader["RemainingBalance"]),
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
