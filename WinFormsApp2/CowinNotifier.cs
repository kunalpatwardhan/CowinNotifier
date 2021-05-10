using Newtonsoft.Json;
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

                string[] splitJSON = json.Split(",");

                for(int i=0;i<splitJSON.Count();i++)
                {
                    dataGridView1.RowCount++;
                    string[] split2 = splitJSON[i].Split(":");
                    if (split2.Length == 2)
                    {
                        dataGridView1[0, i].Value = split2[0];
                        dataGridView1[1, i].Value = split2[1];
                    }
                }
                
            }
        }
    }
}
