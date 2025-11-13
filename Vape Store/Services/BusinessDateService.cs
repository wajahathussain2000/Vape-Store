using System;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Repositories;

namespace Vape_Store.Services
{
    /// <summary>
    /// Service to manage business dates - tracks the current business date (when work starts)
    /// which may differ from the calendar date if shop is closed
    /// </summary>
    public class BusinessDateService
    {
        private DayClosingRepository _dayClosingRepository;

        public BusinessDateService()
        {
            _dayClosingRepository = new DayClosingRepository();
        }

        /// <summary>
        /// Gets the current business date - the date when work should start
        /// This is the last closed date + 1, or today if no dates are closed
        /// Never returns a date in the past - always returns today or a future date
        /// </summary>
        public DateTime GetCurrentBusinessDate()
        {
            DateTime today = DateTime.Now.Date;
            DateTime businessDate = today;
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Get the most recent closed date
                    string query = @"
                        SELECT TOP 1 ClosingDate 
                        FROM DayClosings 
                        WHERE Status = 'Closed'
                        ORDER BY ClosingDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        var result = command.ExecuteScalar();
                        
                        if (result != null && result != DBNull.Value)
                        {
                            DateTime lastClosedDate = Convert.ToDateTime(result).Date;
                            // Return the day after the last closed date
                            businessDate = lastClosedDate.AddDays(1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If table doesn't exist or error, return today's date
                System.Diagnostics.Debug.WriteLine($"Error getting business date: {ex.Message}");
                return today;
            }

            // Never return a date in the past - always use today if business date is older
            // This ensures new purchases always get today's date
            if (businessDate < today)
            {
                return today;
            }

            return businessDate;
        }

        /// <summary>
        /// Checks if a date is closed (day end has been performed)
        /// </summary>
        public bool IsDateClosed(DateTime date)
        {
            return _dayClosingRepository.IsDayClosed(date);
        }

        /// <summary>
        /// Validates if a transaction can be created for the given date
        /// Returns true if allowed, false if date is closed
        /// </summary>
        public bool CanCreateTransaction(DateTime transactionDate)
        {
            DateTime dateOnly = transactionDate.Date;
            return !IsDateClosed(dateOnly);
        }

        /// <summary>
        /// Gets the validation message if transaction cannot be created
        /// </summary>
        public string GetValidationMessage(DateTime transactionDate)
        {
            if (IsDateClosed(transactionDate.Date))
            {
                return $"Cannot create transaction for {transactionDate:yyyy-MM-dd}. This date has been closed (Day End performed).\n\nPlease use the current business date: {GetCurrentBusinessDate():yyyy-MM-dd}";
            }
            return string.Empty;
        }
    }
}

