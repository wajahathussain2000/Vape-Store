using System;
using System.Drawing;
using System.Windows.Forms;

namespace Vape_Store
{
    public class LoadingIndicator : Form
    {
        private Label lblMessage;
        private ProgressBar progressBar;
        private Timer animationTimer;
        private int animationStep = 0;
        private string baseMessage = "";
        private bool isDisposed = false;

        public LoadingIndicator(string message = "Loading...")
        {
            baseMessage = message;
            InitializeComponent();
            lblMessage.Text = message;
            StartAnimation();
        }

        private void InitializeComponent()
        {
            this.lblMessage = new Label();
            this.progressBar = new ProgressBar();
            this.animationTimer = new Timer();
            
            this.SuspendLayout();
            
            // Form properties
            this.Text = "Loading";
            this.Size = new Size(300, 120);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            
            // Message label
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new Point(20, 20);
            this.lblMessage.Size = new Size(260, 20);
            this.lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            this.lblMessage.Font = new Font("Segoe UI", 10F);
            
            // Progress bar
            this.progressBar.Location = new Point(20, 50);
            this.progressBar.Size = new Size(260, 20);
            this.progressBar.Style = ProgressBarStyle.Marquee;
            this.progressBar.MarqueeAnimationSpeed = 30;
            
            // Timer
            this.animationTimer.Interval = 500;
            this.animationTimer.Tick += AnimationTimer_Tick;
            
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.progressBar);
            
            this.ResumeLayout(false);
        }

        private void StartAnimation()
        {
            if (animationTimer != null && !isDisposed)
            {
                animationTimer.Start();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (isDisposed || lblMessage == null || lblMessage.IsDisposed)
            {
                animationTimer?.Stop();
                return;
            }

            try
            {
                if (InvokeRequired)
                {
                    this.BeginInvoke(new Action(() => UpdateAnimation()));
                }
                else
                {
                    UpdateAnimation();
                }
            }
            catch (ObjectDisposedException)
            {
                animationTimer?.Stop();
            }
            catch (InvalidOperationException)
            {
                animationTimer?.Stop();
            }
        }

        private void UpdateAnimation()
        {
            if (isDisposed || lblMessage == null || lblMessage.IsDisposed)
                return;

            try
            {
                animationStep++;
                string dots = new string('.', (animationStep % 4));
                
                // Safely update the message - keep base message and add dots
                string messageWithoutDots = baseMessage;
                if (string.IsNullOrEmpty(messageWithoutDots))
                {
                    messageWithoutDots = "Loading";
                }
                
                lblMessage.Text = messageWithoutDots + dots;
            }
            catch (ObjectDisposedException)
            {
                animationTimer?.Stop();
            }
            catch (InvalidOperationException)
            {
                animationTimer?.Stop();
            }
        }

        public void UpdateMessage(string message)
        {
            if (isDisposed || lblMessage == null || lblMessage.IsDisposed)
                return;

            try
            {
                if (InvokeRequired)
                {
                    this.BeginInvoke(new Action(() => UpdateMessage(message)));
                    return;
                }

                baseMessage = message ?? "Loading";
                lblMessage.Text = baseMessage;
                animationStep = 0; // Reset animation
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        public void SetProgress(int percentage)
        {
            if (isDisposed || progressBar == null || progressBar.IsDisposed)
                return;

            try
            {
                if (InvokeRequired)
                {
                    this.BeginInvoke(new Action(() => SetProgress(percentage)));
                    return;
                }

                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = Math.Min(100, Math.Max(0, percentage));
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isDisposed = true;
            animationTimer?.Stop();
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            isDisposed = true;
            animationTimer?.Stop();
            animationTimer?.Dispose();
            animationTimer = null;
            base.OnFormClosed(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                isDisposed = true;
                animationTimer?.Stop();
                animationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public static class LoadingHelper
    {
        private static LoadingIndicator currentIndicator;
        private static readonly object lockObject = new object();

        public static void ShowLoading(string message = "Loading...")
        {
            lock (lockObject)
            {
                if (currentIndicator == null || currentIndicator.IsDisposed)
                {
                    if (Application.OpenForms.Count > 0)
                    {
                        var parentForm = Application.OpenForms[0];
                        currentIndicator = new LoadingIndicator(message);
                        currentIndicator.StartPosition = FormStartPosition.CenterParent;
                        currentIndicator.Owner = parentForm;
                    }
                    else
                    {
                        currentIndicator = new LoadingIndicator(message);
                        currentIndicator.StartPosition = FormStartPosition.CenterScreen;
                    }
                    
                    currentIndicator.Show();
                }
                else
                {
                    currentIndicator.UpdateMessage(message);
                }
            }
        }

        public static void UpdateLoading(string message)
        {
            lock (lockObject)
            {
                if (currentIndicator != null && !currentIndicator.IsDisposed)
                {
                    currentIndicator.UpdateMessage(message);
                }
            }
        }

        public static void SetProgress(int percentage)
        {
            lock (lockObject)
            {
                if (currentIndicator != null && !currentIndicator.IsDisposed)
                {
                    currentIndicator.SetProgress(percentage);
                }
            }
        }

        public static void HideLoading()
        {
            lock (lockObject)
            {
                if (currentIndicator != null && !currentIndicator.IsDisposed)
                {
                    try
                    {
                        if (currentIndicator.InvokeRequired)
                        {
                            currentIndicator.Invoke(new Action(() => {
                                currentIndicator.Close();
                                currentIndicator.Dispose();
                            }));
                        }
                        else
                        {
                            currentIndicator.Close();
                            currentIndicator.Dispose();
                        }
                    }
                    catch (ObjectDisposedException) { }
                    catch (InvalidOperationException) { }
                    finally
                    {
                        currentIndicator = null;
                    }
                }
            }
        }
    }
}
