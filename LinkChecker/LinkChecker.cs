using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using mshtml;
using System.Threading;

namespace LinkChecker
{
    public partial class LinkChecker : Form
    {
        int p_current = 0;
        int p_all = 0;
        BackgroundWorker p_worker = new BackgroundWorker();
        Semaphore sem = new Semaphore(1, 1);

        public LinkChecker()
        {
            InitializeComponent();
            p_worker.DoWork += new DoWorkEventHandler(WorkerDoWork);
            p_worker.ProgressChanged += new ProgressChangedEventHandler(WorkerProgressChanged);
            p_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
            p_worker.WorkerReportsProgress = true;
            p_worker.WorkerSupportsCancellation = true;
            toolStripStatusLabel1.Text = "Initialized";
        }        

        private void button3_Click(object a_sender, EventArgs a_e)
        {
            p_all = listBox1.Items.Count;
            double step = 100.0 / (double)p_all;
            if (step < 1)
            {
                progressBar1.Step = 1;
            }
            else
            {
                progressBar1.Step = (int)Math.Floor(step);
            }
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = true;            
            listBox2.Items.Clear();
            progressBar1.Value = 0;
            p_current = 0;
            toolStripStatusLabel1.Text = "Running";
            p_worker.RunWorkerAsync();
        }

        private void button4_Click(object a_sender, EventArgs a_e)
        {
            p_worker.CancelAsync();
            toolStripStatusLabel1.Text = "Stopped";
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = false;
            toolStripStatusLabel3.Text = "";
        }

        private void button1_Click(object a_sender, EventArgs a_e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string filename = dlg.FileName;

                StreamReader reader = new StreamReader(filename);

                listBox1.Items.Clear();
                listBox2.Items.Clear();

                while(reader.EndOfStream == false)
                {
                    string item = reader.ReadLine();
                    item = LinkChecker.Utilities.WsDomain.ToIdn(item);
                    listBox1.Items.Add(item);
                    if (item.Contains("www.") == false)
                    {
                        listBox1.Items.Add("www." + item);
                    }
                }
                
                reader.Close();
                button3.Enabled = true;

            }
        }

        private void button2_Click(object a_sender, EventArgs a_e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string filename = dlg.FileName;

                StreamWriter writer = new StreamWriter(filename);

                foreach (string item in listBox2.Items)
                {
                    writer.WriteLine(item);
                }
                writer.Flush();
                writer.Close();

            }
        }

        void WorkerCompleted(object a_sender, RunWorkerCompletedEventArgs a_e)
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = false;
            toolStripStatusLabel1.Text = "Completed";
            toolStripStatusLabel3.Text = "";
            progressBar1.Value = 100;
            label1.Text = "Done/All";
        }

        void WorkerProgressChanged(object a_sender, ProgressChangedEventArgs a_e)
        {
            switch (a_e.ProgressPercentage)
            {
                case 1:
                    progressBar1.PerformStep();
                    label1.Text = (string)a_e.UserState;
                    break;
                case 2:
                    toolStripStatusLabel3.Text = (string)a_e.UserState;
                    break;
                case 3:
                    listBox2.Items.Add((string)a_e.UserState);
                    break;
                case 4:
                    sem.WaitOne();
                    webBrowser1.Navigate((Uri)a_e.UserState);

                    while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                    {
                        System.Windows.Forms.Application.DoEvents();
                    }

                    listBox2.Items.Add(((Uri)a_e.UserState).AbsoluteUri.Replace("http://", "") + " | " + webBrowser1.Document.Window.Frames[0].Url);
                    sem.Release(1);
                    break;
                default:
                    break;
            }
        }

        private void WorkerDoWork(object a_sender, DoWorkEventArgs a_e)
        {
            foreach (string item in listBox1.Items)
            {
                Application.DoEvents();

                if (p_worker.CancellationPending == true)
                {
                    break;
                }

                p_worker.ReportProgress(2, "http://" + item);
                HttpWebRequest request = null;

                try
                {
                    Uri urlCheck = null;
                    try
                    {
                        urlCheck = new Uri("http://" + item);
                    }
                    catch
                    {
                        continue;
                    }                    

                    request = (HttpWebRequest)HttpWebRequest.Create(urlCheck);
                    request.AllowAutoRedirect = true;
                    request.MaximumAutomaticRedirections = 20;
                    request.Timeout = 10000;

                    ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                    HttpWebResponse response;

                    response = (HttpWebResponse)request.GetResponse();
                    Stream resStream = response.GetResponseStream();
                    StreamReader resreader = new StreamReader(resStream);

                    string content = resreader.ReadToEnd();
                    Dictionary<string, HtmlMeta> metatags = ParseHtml(content);
                    string[] sep = new string[1];
                    sep[0] = "=";

                    if (metatags.ContainsKey("refresh") == true)
                    {
                        string url = metatags["refresh"].Content.Split(sep, StringSplitOptions.RemoveEmptyEntries)[1];
                        url = url.Contains("http") == true ? url : response.ResponseUri.AbsoluteUri + url;
                        p_worker.ReportProgress(3, item + " | " + url);
                    }
                    else if (content.ToLower().Contains("<frame") == true)
                    {
                        p_worker.ReportProgress(4, urlCheck);
                        Thread.Sleep(500);
                        sem.WaitOne();
                        sem.Release(1);
                    }
                    else
                    {
                        p_worker.ReportProgress(3, item + " | " + response.ResponseUri.AbsoluteUri);
                    }
                }
                catch (WebException e_t)
                {
                    if (e_t.Message == "The operation has timed out")
                    {
                        p_worker.ReportProgress(3, item + " | " + ((request.Address.AbsolutePath == "/" || request.Address.AbsolutePath == "") ? "Invalid url (does not exist)" : request.Address.AbsolutePath));
                    }
                    else if (e_t.Message.Contains("The remote name could not be") == true)
                    {
                        p_worker.ReportProgress(3, item + " | " + "Invalid url (does not exist)");
                    }
                    else
                    {
                        if (e_t.Response == null)
                        {
                            p_worker.ReportProgress(3, item + " | " +  "empty");
                        }
                        else
                        {
                            p_worker.ReportProgress(3, item + " | " + e_t.Response.ResponseUri.AbsoluteUri);
                        }
                    }
                }
                finally
                {
                    p_current++;
                    double total = ((double)p_current * 100.0) / (double)p_all;
                    if (total >= progressBar1.Value)
                    {
                        p_worker.ReportProgress(1, p_current + "/" + p_all);
                    }
                }
            }
        }

        Dictionary<string,HtmlMeta> ParseHtml(string a_htmldata)
        {
            Regex metaregex =
                new Regex(@"<meta\s*(?:(?:\b(\w|-)+\b\s*(?:=\s*(?:""[^""]*""|'" +
                          @"[^']*'|[^""'<> ]+)\s*)?)*)/?\s*>",
                          RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

            Dictionary<string, HtmlMeta> MetaList = new Dictionary<string, HtmlMeta>();
            foreach (Match metamatch in metaregex.Matches(a_htmldata))
            {
                HtmlMeta mymeta = new HtmlMeta();

                Regex submetaregex =
                    new Regex(@"(?<name>\b(\w|-)+\b)\" +
                              @"s*=\s*(""(?<value>" +
                              @"[^""]*)""|'(?<value>[^']*)'" +
                              @"|(?<value>[^""'<> ]+)\s*)+",
                              RegexOptions.IgnoreCase |
                              RegexOptions.ExplicitCapture
                              | RegexOptions.Multiline);

                foreach (Match submetamatch in
                         submetaregex.Matches(metamatch.Value.ToString()))
                {
                    if ("http-equiv" ==
                          submetamatch.Groups["name"].ToString().ToLower())
                        mymeta.HttpEquiv =
                          submetamatch.Groups["value"].ToString();

                    if (("name" ==
                         submetamatch.Groups["name"].ToString().ToLower())
                         && (mymeta.HttpEquiv == String.Empty))
                        mymeta.Name = submetamatch.Groups["value"].ToString();

                    if ("scheme" ==
                        submetamatch.Groups["name"].ToString().ToLower())
                        mymeta.Scheme = submetamatch.Groups["value"].ToString();

                    if ("content" ==
                        submetamatch.Groups["name"].ToString().ToLower())
                    {
                        mymeta.Content = submetamatch.Groups["value"].ToString();
                        if (MetaList.ContainsKey(mymeta.HttpEquiv) == false)
                        {
                            MetaList.Add(mymeta.HttpEquiv, mymeta);
                        }
                    }
                }
            }
            return MetaList;
        }


    }
}
