using FormSerialisation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class CowinNotifier : Form
    {
        public CowinNotifier()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //WebRequest request = WebRequest.Create("https://www.cowin.gov.in/api/v2/appointment/sessions/public/calendarByDistrict?district_id=363&date=11-05-2021");
            //WebResponse response = request.GetResponse();
            timer1.Start();

            try
            {
                string urlToCheck = "";
                HttpWebRequest request = HttpWebRequest.Create("https://www.cowin.gov.in/api/v2/appointment/sessions/public/calendarByDistrict?district_id=363&date=11-05-2021") as HttpWebRequest;
                request.Method = "GET";
                /* Sart browser signature */
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us,en;q=0.5");
                /* Sart browser signature */

                Console.WriteLine(request.RequestUri.AbsoluteUri);
                WebResponse response = request.GetResponse();



                using (StreamReader r = new StreamReader(response.GetResponseStream()))
                {
                    string json = r.ReadToEnd();
                    textBox1.Text = json;
                    textBox1.Text = textBox1.Text.Replace("center_id", "center_id" + Environment.NewLine);

                    string[] splitJSON = json.Split(",");

                    string[] strArrKey = txtKey.Text.Split(Environment.NewLine);
                    string[] strArrValue = txtValue.Text.Split(Environment.NewLine);

                    int centerMatchedCount = 0;
                    int propertyMatchedCount = 0;
                    string lastMatched = "";

                    dataGridView1.Rows.Clear();

                    if (splitJSON.Count() > 0)
                    {
                        if (WindowState != FormWindowState.Minimized)
                        {
                            dataGridView1.RowCount = splitJSON.Count();
                            lstIndex.Clear();
                        }
                        for (int i = 0; i < splitJSON.Count(); i++)
                        {
                            //dataGridView1.RowCount++;
                            string[] split2 = splitJSON[i].Split(":");
                            if (split2.Length == 2)
                            {
                                if (WindowState != FormWindowState.Minimized)
                                {
                                    dataGridView1[0, i].Value = split2[0];
                                    dataGridView1[1, i].Value = split2[1];
                                }
                                if (split2[0] == "\"name\"")
                                {
                                    propertyMatchedCount = 0;
                                    lastMatched = "";
                                }

                                if (split2[0] == "{\"session_id\"")
                                {
                                    propertyMatchedCount = 0;
                                    lastMatched = "";
                                }

                                for (int j = 0; j < strArrKey.Length; j++)
                                {
                                    string keyStr = strArrKey[j];
                                    string ValueStr = strArrValue[j];
                                    if (ValueStr.Contains('!'))
                                    {
                                        if ((keyStr == split2[0]) && (ValueStr != ('!' + split2[1])))
                                        {
                                            if (lastMatched != keyStr)
                                                propertyMatchedCount++;

                                            lastMatched = keyStr;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if ((keyStr == split2[0]) && (ValueStr == split2[1]))
                                        {
                                            if (lastMatched != keyStr)
                                                propertyMatchedCount++;

                                            lastMatched = keyStr;
                                            break;
                                        }
                                    }
                                }

                                if (propertyMatchedCount == strArrKey.Length)
                                {
                                    centerMatchedCount++;
                                    propertyMatchedCount = 0;
                                    lastMatched = "";
                                    if (WindowState != FormWindowState.Minimized)
                                    {
                                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                        lstIndex.Items.Add(i.ToString());
                                    }
                                }

                            }

                        }
                        txtCenterMatched.Text = centerMatchedCount.ToString();
                    }
                }
            }
            catch(Exception ex)
            {
                textBox1.Text = ex.Message;
            }
        }

        private void CowinNotifier_Load(object sender, EventArgs e)
        {
            FormSerialisor.Deserialise(this, Application.StartupPath + @"\serialise.xml");
        }

        private void CowinNotifier_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormSerialisor.Serialise(this, Application.StartupPath + @"\serialise.xml");
        }

        private void lstIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer1.Stop();
            dataGridView1.FirstDisplayedScrollingRowIndex = Convert.ToInt32( lstIndex.FocusedItem.Text);
        }

        bool playBeep = false;
        private void txtCenterMatched_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(txtCenterMatched.Text) > 0)
                {
                    if(WindowState == FormWindowState.Minimized)
                        timer1.Interval = 1000;

                    WindowState = FormWindowState.Normal;
                    playBeep = true;
                }
                else
                    playBeep = false;
            }
            catch(Exception ex)
            { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 10000;
            //if(WindowState != FormWindowState.Normal)
            if (playBeep)    
                SystemSounds.Beep.Play();

            //if (WindowState==FormWindowState.Minimized)
                button1_Click(null, null);
        }

        private void txtKey_TextChanged(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Start();
        }
    }
}
