using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vape_Store
{
    /// <summary>
    /// Main entry point for the Attock Mobiles Rwp POS System
    /// Handles application startup, global exception handling, and initialization
    /// </summary>
    internal static class Program
    {
        #region Application Entry Point

        /// <summary>
        /// The main entry point for the application.
        /// Initializes the Windows Forms application and sets up global exception handling.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Configure global exception handling
                SetupGlobalExceptionHandling();

                // Initialize Database (Create missing tables)
                Vape_Store.DataAccess.DatabaseInitializer.Initialize();

                // Initialize Windows Forms application
                InitializeApplication();

                // Start the main application form
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                ShowCriticalError($"Critical error starting the application: {ex.Message}", ex);
            }
        }

        #endregion

        #region Application Initialization

        private static void SetupGlobalExceptionHandling()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void InitializeApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        #endregion

        #region Exception Handling

        /// <summary>
        /// Handles unhandled exceptions from the main UI thread
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event arguments containing exception details</param>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ShowError($"An unexpected error occurred: {e.Exception.Message}", e.Exception);
        }

        /// <summary>
        /// Handles unhandled exceptions from any thread
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event arguments containing exception details</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowCriticalError($"A critical error occurred: {e.ExceptionObject}", e.ExceptionObject as Exception);
        }

        #endregion

        #region Error Display Methods

        /// <summary>
        /// Displays a critical error message to the user
        /// </summary>
        /// <param name="message">Error message to display</param>
        /// <param name="exception">Exception that caused the error</param>
        private static void ShowCriticalError(string message, Exception exception = null)
        {
            MessageBox.Show(message, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            // Log the exception if logging is available
            if (exception != null)
            {
                // TODO: Implement proper logging mechanism
                System.Diagnostics.Debug.WriteLine($"Critical Error: {message}\nException: {exception}");
            }
        }

        /// <summary>
        /// Displays a general error message to the user
        /// </summary>
        /// <param name="message">Error message to display</param>
        /// <param name="exception">Exception that caused the error</param>
        private static void ShowError(string message, Exception exception = null)
        {
            MessageBox.Show(message, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            // Log the exception if logging is available
            if (exception != null)
            {
                // TODO: Implement proper logging mechanism
                System.Diagnostics.Debug.WriteLine($"Error: {message}\nException: {exception}");
            }
        }

        #endregion
    }
}
