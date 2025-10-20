using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vape_Store.Models;
using Vape_Store.DataAccess;

namespace Vape_Store.Repositories
{
    public class CustomerPaymentRepository
    {
        public CustomerPaymentRepository()
        {
        }

        public List<CustomerPayment> GetAllCustomerPayments()
        {
            var payments = new List<CustomerPayment>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT cp.*, c.CustomerName, c.Phone as CustomerPhone, u.FullName as UserName
                        FROM CustomerPayments cp
                        LEFT JOIN Customers c ON cp.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON cp.UserID = u.UserID
                        ORDER BY cp.PaymentDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                payments.Add(new CustomerPayment
                                {
                                    PaymentID = Convert.ToInt32(reader["PaymentID"]),
                                    VoucherNumber = reader["VoucherNumber"].ToString(),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                                    PreviousBalance = Convert.ToDecimal(reader["PreviousBalance"]),
                                    TotalDue = Convert.ToDecimal(reader["TotalDue"]),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    RemainingBalance = Convert.ToDecimal(reader["RemainingBalance"]),
                                    Description = reader["Description"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString(),
                                    CustomerPhone = reader["CustomerPhone"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving customer payments: {ex.Message}", ex);
            }

            return payments;
        }

        public CustomerPayment GetCustomerPaymentById(int paymentId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT cp.*, c.CustomerName, c.Phone as CustomerPhone, u.FullName as UserName
                        FROM CustomerPayments cp
                        LEFT JOIN Customers c ON cp.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON cp.UserID = u.UserID
                        WHERE cp.PaymentID = @PaymentID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PaymentID", paymentId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new CustomerPayment
                                {
                                    PaymentID = Convert.ToInt32(reader["PaymentID"]),
                                    VoucherNumber = reader["VoucherNumber"].ToString(),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                                    PreviousBalance = Convert.ToDecimal(reader["PreviousBalance"]),
                                    TotalDue = Convert.ToDecimal(reader["TotalDue"]),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    RemainingBalance = Convert.ToDecimal(reader["RemainingBalance"]),
                                    Description = reader["Description"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString(),
                                    CustomerPhone = reader["CustomerPhone"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving customer payment: {ex.Message}", ex);
            }

            return null;
        }

        public bool AddCustomerPayment(CustomerPayment payment)
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
                                INSERT INTO CustomerPayments (VoucherNumber, CustomerID, PaymentDate, PreviousBalance, 
                                                             TotalDue, PaidAmount, RemainingBalance, Description, UserID, CreatedDate)
                                VALUES (@VoucherNumber, @CustomerID, @PaymentDate, @PreviousBalance, 
                                        @TotalDue, @PaidAmount, @RemainingBalance, @Description, @UserID, @CreatedDate);
                                SELECT SCOPE_IDENTITY();";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@VoucherNumber", payment.VoucherNumber);
                                command.Parameters.AddWithValue("@CustomerID", payment.CustomerID);
                                command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                                command.Parameters.AddWithValue("@PreviousBalance", payment.PreviousBalance);
                                command.Parameters.AddWithValue("@TotalDue", payment.TotalDue);
                                command.Parameters.AddWithValue("@PaidAmount", payment.PaidAmount);
                                command.Parameters.AddWithValue("@RemainingBalance", payment.RemainingBalance);
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
                throw new Exception($"Error adding customer payment: {ex.Message}", ex);
            }
        }

        public bool UpdateCustomerPayment(CustomerPayment payment)
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
                                UPDATE CustomerPayments 
                                SET CustomerID = @CustomerID, PaymentDate = @PaymentDate, PreviousBalance = @PreviousBalance,
                                    TotalDue = @TotalDue, PaidAmount = @PaidAmount, RemainingBalance = @RemainingBalance,
                                    Description = @Description, UserID = @UserID
                                WHERE PaymentID = @PaymentID";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@PaymentID", payment.PaymentID);
                                command.Parameters.AddWithValue("@CustomerID", payment.CustomerID);
                                command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                                command.Parameters.AddWithValue("@PreviousBalance", payment.PreviousBalance);
                                command.Parameters.AddWithValue("@TotalDue", payment.TotalDue);
                                command.Parameters.AddWithValue("@PaidAmount", payment.PaidAmount);
                                command.Parameters.AddWithValue("@RemainingBalance", payment.RemainingBalance);
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
                throw new Exception($"Error updating customer payment: {ex.Message}", ex);
            }
        }

        public bool DeleteCustomerPayment(int paymentId)
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
                            var query = "DELETE FROM CustomerPayments WHERE PaymentID = @PaymentID";
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
                throw new Exception($"Error deleting customer payment: {ex.Message}", ex);
            }
        }

        public string GetNextVoucherNumber()
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT ISNULL(MAX(CAST(SUBSTRING(VoucherNumber, 4, LEN(VoucherNumber)) AS INT)), 0) + 1
                        FROM CustomerPayments 
                        WHERE VoucherNumber LIKE 'CP%'";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        var nextNumber = Convert.ToInt32(command.ExecuteScalar());
                        return $"CP{nextNumber:D6}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating voucher number: {ex.Message}", ex);
            }
        }

        public decimal GetCustomerTotalDue(int customerId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Calculate total sales amount for customer
                    var salesQuery = @"
                        SELECT ISNULL(SUM(TotalAmount), 0) 
                        FROM Sales 
                        WHERE CustomerID = @CustomerID";

                    decimal totalSales = 0;
                    using (var salesCommand = new SqlCommand(salesQuery, connection))
                    {
                        salesCommand.Parameters.AddWithValue("@CustomerID", customerId);
                        connection.Open();
                        totalSales = Convert.ToDecimal(salesCommand.ExecuteScalar());
                    }

                    // Calculate total payments made by customer
                    var paymentsQuery = @"
                        SELECT ISNULL(SUM(PaidAmount), 0) 
                        FROM CustomerPayments 
                        WHERE CustomerID = @CustomerID";

                    decimal totalPayments = 0;
                    using (var paymentsCommand = new SqlCommand(paymentsQuery, connection))
                    {
                        paymentsCommand.Parameters.AddWithValue("@CustomerID", customerId);
                        totalPayments = Convert.ToDecimal(paymentsCommand.ExecuteScalar());
                    }

                    return totalSales - totalPayments;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating customer total due: {ex.Message}", ex);
            }
        }

        public decimal GetCustomerPreviousBalance(int customerId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT ISNULL(SUM(RemainingBalance), 0) 
                        FROM CustomerPayments 
                        WHERE CustomerID = @CustomerID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customerId);
                        connection.Open();
                        return Convert.ToDecimal(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating customer previous balance: {ex.Message}", ex);
            }
        }

        public List<CustomerPayment> GetPaymentsByCustomerAndDateRange(int customerId, DateTime fromDate, DateTime toDate)
        {
            var payments = new List<CustomerPayment>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT cp.*, c.CustomerName, u.FullName as UserName
                        FROM CustomerPayments cp
                        LEFT JOIN Customers c ON cp.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON cp.UserID = u.UserID
                        WHERE cp.CustomerID = @CustomerID AND cp.PaymentDate BETWEEN @FromDate AND @ToDate
                        ORDER BY cp.PaymentDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customerId);
                        command.Parameters.AddWithValue("@FromDate", fromDate);
                        command.Parameters.AddWithValue("@ToDate", toDate);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var payment = new CustomerPayment
                                {
                                    PaymentID = Convert.ToInt32(reader["PaymentID"]),
                                    VoucherNumber = reader["VoucherNumber"].ToString(),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                                    PreviousBalance = Convert.ToDecimal(reader["PreviousBalance"]),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    RemainingBalance = Convert.ToDecimal(reader["RemainingBalance"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString(),
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
                throw new Exception($"Error retrieving customer payments by customer and date range: {ex.Message}", ex);
            }

            return payments;
        }
    }
}
