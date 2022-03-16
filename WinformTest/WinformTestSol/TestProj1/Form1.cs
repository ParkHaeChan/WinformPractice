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
        const int REPEAT_INTERVAL = 5000;
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

            // infinite work until get cancel event
            while(true)
            {
                // Actual Work
                for (int i = 0; i <= 100; ++i)
                {
                    if (bgw.CancellationPending)
                    {
                        e.Cancel = true;    // Will get e.Cancelled == true at line: 65
                        return;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(50);
                        worker.ReportProgress(i);
                    }
                }

                // Repeat Timer(Cool down)
                int time = 0;
                while(time < REPEAT_INTERVAL)
                {
                    if(bgw.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        // 그냥 Sleep(5000)으로 쓰면 그 사이 발생한 Cancel 이벤트를 탐지할 수 없어
                        // 그만큼 Cancel이 느려져 UX가 저하되는 문제가 생김
                        System.Threading.Thread.Sleep(500); // Sleep 0.5 seconds
                        time += 500;
                    }
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
                resultLabel.Text = "Cancelled";
            }
            else
            {
                // succeeded.
                resultLabel.Text = e.Result.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            resultLabel.Text = "Start!";
            if (!bgw.IsBusy)
                bgw.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            resultLabel.Text = "wait for Cancel...";
            if (bgw.WorkerSupportsCancellation)
                bgw.CancelAsync();
        }
    }
}
