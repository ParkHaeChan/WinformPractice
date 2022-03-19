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
            if (!backgroundWorker1.IsBusy)  // 안막으면 버튼 연타하면 예외발생함
                backgroundWorker1.RunWorkerAsync(url);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string url = e.Argument as string;  // e.Argument.ToString()으로 써도 된다.

            try
            {
                e.Result = Do_HTTP_GET(url);
                // e.Result = Do_HTTP_POST();
                // e.Result = "DownLoaded file at: " + DownLoad();
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

        string Do_HTTP_GET(string url)
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
                    return sr.ReadToEnd();
                }
            }
        }

        string Do_HTTP_POST(string url = "https://httpbin.org/post")
        {
            // POST를 지원하는 url에 써야하므로 위의 테스트 url외에는 확인이 어려움

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 30 * 1000;
            //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

            // POST할 데이터를 Request Stream에 쓴다
            string data = "{ \"id\": \"101\", \"name\" : \"Alex\" }";
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            request.ContentLength = bytes.Length;

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }

            // Response 처리
            string responseText = string.Empty;
            using (WebResponse resp = request.GetResponse())
            {
                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
            }

            return responseText;
        }

        string DownLoad(string url= "http://httpbin.org/image/png")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            // Response 바이너리 데이터 처리                        
            using (WebResponse resp = request.GetResponse())
            {
                var buff = new byte[1024];
                int pos = 0;
                int count;

                using (Stream stream = resp.GetResponseStream())
                {
                    // 이미지 파일로 저장
                    string filename = "image.png";
                    using (var fs = new FileStream(filename, FileMode.Create))
                    {
                        do
                        {
                            count = stream.Read(buff, pos, buff.Length);
                            fs.Write(buff, 0, count);
                        } while (count > 0);
                        return Directory.GetCurrentDirectory() + "/" + filename;
                    }
                }
            }
        }
    }
}
