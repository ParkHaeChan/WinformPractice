using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestProj2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // textBox에 입력된 url을 bgw로 보냄
            string url = textBox1.Text;
            if (url == null || url.Length == 0)
            {
                label1.Text = "textbox에 url을 입력하세요";
                return;
            }
            label1.Text = "input url: " + url;
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync(url);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string url = e.Argument as string;  // e.Argument.ToString()은 class 명을 print하는게 기본동작임을 주의

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //request.Method = "GET";
                request.Timeout = 30 * 1000;    // 30초

                // using문 사용: Scope 벗어나면 리소스 자동 해제
                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        e.Result = sr.ReadToEnd();
                    }
                }     
            }
            catch(Exception ex)
            {
                if(ex is WebException)
                {
                    e.Result = ex.Message;
                    return;
                }
                else
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        // 이벤트 함수 직접 추가해 주어야 함 (Form1.Designer.cs 71줄)
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // CancelAsync was called.
                label1.Text = "Failed: " + e.Result.ToString();
            }
            else
            {
                // succeeded.
                label1.Text = e.Result.ToString();
            }
        }
    }
}
