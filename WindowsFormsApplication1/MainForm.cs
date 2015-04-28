using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.IO;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WindowsFormsApplication1
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //просим u_token_key для включения в POST запрос авторизации
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.fl.ru/");
            req.CookieContainer = new CookieContainer();
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader respStream = new StreamReader(resp.GetResponseStream());
            string html = respStream.ReadToEnd();
            string U_TOKEN_KEY = Regex.Match(html,"<head>[\\s\\S]+?var _TOKEN_KEY = '(.*)';").Result("$1");
            //POST запрос авторизации
            string loginPass=string.Format("action=login&autologin=1&login={0}&passwd={1}&u_token_key={2}",this.textBoxLogin.Text,this.textBoxPassword.Text,U_TOKEN_KEY);
            req = (HttpWebRequest)WebRequest.Create("https://www.fl.ru/");
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
            req.ContentLength = loginPass.Length;
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Referer = "https://www.fl.ru/";
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.Add(resp.Cookies);
            req.Method = "POST";
            byte[] postMsg = Encoding.UTF8.GetBytes(loginPass);
            req.GetRequestStream().Write(postMsg, 0, postMsg.Length);
            resp = (HttpWebResponse)req.GetResponse();

            Form1 form = new Form1(req.CookieContainer);
            form.Show();
            form.Hide();
            this.Hide();
        }
    }
}
