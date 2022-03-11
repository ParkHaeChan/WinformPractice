using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestProj1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            bgw.WorkerReportsProgress = true;
            bgw.WorkerSupportsCancellation = true;

            bgw.DoWork +=
                new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            bgw_RunWorkerCompleted);
            bgw.ProgressChanged +=
                new ProgressChangedEventHandler(
            bgw_ProgressChanged);
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            for (int i = 0; i <= 100; ++i)
            {
                if (bgw.CancellationPending)
                {
                    e.Cancel = true;    // Will get e.Cancelled == true at line: 65
                }
                else
                {
                    System.Threading.Thread.Sleep(50);
                    worker.ReportProgress(i);
                }
            }
        }

        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // CancelAsync was called.
                resultLabel.Text = "Canceled";
            }
            else
            {
                // succeeded.
                resultLabel.Text = e.Result.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!bgw.IsBusy)
                bgw.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(bgw.WorkerSupportsCancellation)
                bgw.CancelAsync();
        }
    }
}
