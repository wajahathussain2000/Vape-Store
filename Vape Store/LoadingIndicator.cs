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

        public LoadingIndicator(string message = "Loading...")
        {
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
            this.StartPosition = FormStartPosition.CenterParent;
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
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationStep++;
            string dots = new string('.', (animationStep % 4));
            lblMessage.Text = lblMessage.Text.Split('.')[0] + dots;
        }

        public void UpdateMessage(string message)
        {
            lblMessage.Text = message;
        }

        public void SetProgress(int percentage)
        {
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = Math.Min(100, Math.Max(0, percentage));
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            animationTimer?.Stop();
            animationTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }

    public static class LoadingHelper
    {
        private static LoadingIndicator currentIndicator;

        public static void ShowLoading(string message = "Loading...")
        {
            if (currentIndicator == null || currentIndicator.IsDisposed)
            {
                currentIndicator = new LoadingIndicator(message);
                currentIndicator.Show();
            }
            else
            {
                currentIndicator.UpdateMessage(message);
            }
        }

        public static void UpdateLoading(string message)
        {
            if (currentIndicator != null && !currentIndicator.IsDisposed)
            {
                currentIndicator.UpdateMessage(message);
            }
        }

        public static void SetProgress(int percentage)
        {
            if (currentIndicator != null && !currentIndicator.IsDisposed)
            {
                currentIndicator.SetProgress(percentage);
            }
        }

        public static void HideLoading()
        {
            if (currentIndicator != null && !currentIndicator.IsDisposed)
            {
                currentIndicator.Close();
                currentIndicator.Dispose();
                currentIndicator = null;
            }
        }
    }
}
