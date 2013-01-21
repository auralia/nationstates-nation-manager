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
using System.Web;
using System.Text.RegularExpressions;

namespace Puppet_AutoLogin
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (startButton.Text.Equals("Start"))
            {
                if (!(Regex.IsMatch(emailTextBox.Text, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase)))
                { // Regex borrowed from Microsoft <http://msdn.microsoft.com/en-us/library/01escwtf.aspx>
                    MessageBox.Show("A valid email address must be provided.", "");
                    return;
                }

                try
                {
                    List<String> puppetLogins = textBox.Text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                
                    for (int i = 0; i < puppetLogins.Count; i++)
                    {
                        String username = puppetLogins[i].Substring(0, puppetLogins[i].IndexOf(","));
                        String password = puppetLogins[i].Substring(puppetLogins[i].IndexOf(",") + 1);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Puppet account information formatted incorrectly.");
                    return;
                }

                progressBar.Value = 0;
                startButton.Text = "Cancel";

                backgroundWorker.RunWorkerAsync();
            }
            else if (startButton.Text.Equals("Cancel"))
            {
                startButton.Enabled = false;

                backgroundWorker.CancelAsync();
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = "";

            List<String> puppetLogins = textBox.Text.Split(new String[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < puppetLogins.Count; i++)
            {
                String username = puppetLogins[i].Substring(0, puppetLogins[i].IndexOf(","));
                String password = puppetLogins[i].Substring(puppetLogins[i].IndexOf(",") + 1);

                if (!login(username, password, e, i + 1, puppetLogins.Count))
                {
                    e.Result = "WARNING";
                }
            }

            if (!e.Cancel && !e.Result.Equals("WARNING"))
                e.Result = "SUCCESS";
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != -1)
                progressBar.Value = e.ProgressPercentage;

            listView.Items.Add(((Log)e.UserState).entry, ((Log)e.UserState).type);
            listView.EnsureVisible(listView.Items.Count - 1);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                listView.Items.Add(DateTime.Now.ToLongTimeString() + ": Process cancelled by user.", Int32.Parse(Log.Error.ToString()));
            else if (((String)e.Result).Equals("WARNING"))
                listView.Items.Add(DateTime.Now.ToLongTimeString() + ": Process completed, but with errors.", Int32.Parse(Log.Error.ToString()));
            else
                listView.Items.Add(DateTime.Now.ToLongTimeString() + ": Process completed.", Int32.Parse(Log.Information.ToString()));

            listView.EnsureVisible(listView.Items.Count - 1);
            progressBar.Value = 100;

            startButton.Text = "Start";
            startButton.Enabled = true;
        }

        private Boolean login(String username, String password, DoWorkEventArgs e, int curr, int total)
        {
            try
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;

                    throw new Exception("Process cancelled.");
                }

                CookieContainer c = new CookieContainer();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.nationstates.net/");
                request.Method = "POST";
                request.UserAgent = "Puppet AutoLogin " + Application.ProductVersion + " - " +
                        "Author (not responsible for use): Auralia (federal.republic.of.auralia@gmail.com) - " +
                        "Current user (responsible for use): " + username + " (" + emailTextBox.Text + ")"; ;
                request.ContentType = "application/x-www-form-urlencoded";
                request.CookieContainer = c;

                Stream requestStream = request.GetRequestStream();
                StreamWriter requestStreamWriter = new StreamWriter(requestStream);
                requestStreamWriter.Write("logging_in=1&nation=" + HttpUtility.UrlEncode(username.Trim().Replace(" ", "_")) + "&password=" + HttpUtility.UrlEncode(password) + "&autologin=no&submit=Login");
                requestStreamWriter.Close();
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();

                if (c.GetCookies(new Uri("http://www.nationstates.net")).Count == 0)
                    throw new Exception("Cookies missing.");

                backgroundWorker.ReportProgress((int) (((double) curr / (double) total) * 100), new Log(DateTime.Now.ToLongTimeString() + ": Successfully logged into " + username, Log.Information));

                try
                {
                    request = (HttpWebRequest)WebRequest.Create("http://www.nationstates.net/logout");
                    response = (HttpWebResponse)request.GetResponse();
                    response.Close();
                }
                catch (Exception)
                {
                    // Do nothing
                }

                return true;
            }
            catch (Exception ex)
            {
                if (!e.Cancel)
                {
                    backgroundWorker.ReportProgress(100, new Log(DateTime.Now.ToLongTimeString() + ": Failed to login to " + username + "! (" + ex.GetType() + ": " + ex.Message + " - " + ex.StackTrace + ")", Log.Warning));
                }
                    
                return false;
            }
        }

        private void listView_Resize(object sender, EventArgs e)
        {
            // Resize listview column to full width of window
            listView.Columns[0].Width = -2;
        }
    }

    class Log
    {
        public const int Information = 0;
        public const int Warning = 1;
        public const int Error = 2;

        public const int NoChange = -1;

        public String entry;
        public int type;

        public Log(String entry, int type)
        {
            this.entry = entry;
            this.type = type;
        }
    }
}
