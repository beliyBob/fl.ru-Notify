using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Thread thread;
        CookieContainer cookieContainer = new CookieContainer();
        public Form1(CookieContainer cookieContainer)
        {
            InitializeComponent();
            this.cookieContainer = cookieContainer;
            thread = new Thread(tick);
            thread.IsBackground = true;
            thread.Start();
            notifyIcon1.ContextMenu = new System.Windows.Forms.ContextMenu();
            notifyIcon1.ContextMenu.MenuItems.Add(new MenuItem("Перейти на сайт", goToSite));
            notifyIcon1.ContextMenu.MenuItems.Add(new MenuItem("Частота опроса", changeOptions));
            notifyIcon1.ContextMenu.MenuItems.Add(new MenuItem("Прервать парсинг", pause));
            notifyIcon1.ContextMenu.MenuItems.Add(new MenuItem("Выход", exit));
        }

        delegate void Del(string header, string price, string article);

        void tick()
        {
            Job tempJob = parse();
            while (true)
            {
                Thread.Sleep(Options.interval);
                Job job = parse();
                if (job != tempJob) 
                {
                    tempJob = job;
                    Del del = new Del(showWindow);
                    this.Invoke(del, new object[] { job.header, job.price, job.article });
                }
            }
        }

        Job parse()
        {
            string header;
            string article;
            string price;
            try
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.fl.ru/projects/");
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
              /*  req.CookieContainer = new CookieContainer();
                req.CookieContainer.Add(new Cookie("pwd", "325f93cdffd9fea9a3467f41b7ea5759", "/", "fl.ru"));
                req.CookieContainer.Add(new Cookie("name", "freeDot", "/", "fl.ru"));
                req.CookieContainer.Add(new Cookie("id", "1897595", "/", "fl.ru"));*/
                req.CookieContainer = this.cookieContainer;
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                htmlDoc.Load(resp.GetResponseStream());

                HtmlNode parentNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='b-post  b-post_padbot_15 b-post_margbot_20 b-post_bordbot_eee b-post_relative ']");
                HtmlNode node = parentNode.SelectSingleNode("//h2[@class='b-post__title b-post__title_inline ']");
                header = node.InnerText;
                reg(ref header);
                node = parentNode.SelectNodes("script")[0];
                node = HtmlNode.CreateNode(extractScript(node.InnerHtml));
                price = node.InnerText;
                reg(ref price);
                node = parentNode.SelectNodes("script")[1];
                node = HtmlNode.CreateNode(extractScript(node.InnerHtml));
                node = node.SelectSingleNode("//div[@class='b-post__txt ']");
                article = node.InnerText;
                reg(ref article);
                return new Job(header, price, article);
            }
            catch
            {
                MessageBox.Show("disconnect");
                return new Job("", "", "");
            }
        }

        void reg(ref string s)
        {
            s = Regex.Replace(s, "[\\t,\\r,\\n]", string.Empty);
            s = Regex.Replace(s, "[' ']{2,}", string.Empty);
            s = Regex.Replace(s, "&nbsp;", string.Empty);
        }

        string extractScript(string s)
        {
            s = Regex.Replace(s, "document.write\\('", string.Empty);
            s = Regex.Replace(s, "'\\);", string.Empty);
            return s;
        }

        void compare()
        {
        }

        void showWindow(string header, string price, string article)
        {
            notifyIcon1.BalloonTipText = header + "\r\n           "+price+"\r\n\r\n"+article;
            notifyIcon1.ShowBalloonTip(10000);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        private void exit(object sender, EventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }
        private void changeOptions(object sender, EventArgs e)
        {
            new ChangeOptionsForm().Show();
        }
        private void goToSite(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://fl.ru/projects");
        }
        private void pause(object sender, EventArgs e)
        { 
            MenuItem menu = (MenuItem)sender;
            if (thread.ThreadState.HasFlag(ThreadState.Suspended) || thread.ThreadState.HasFlag(ThreadState.SuspendRequested))
            {
                menu.Text = "Прервать парсинг";
                thread.Resume();
                return;
            }
            if (!thread.ThreadState.HasFlag(ThreadState.Suspended))
            {
                menu.Text = "Возобновить парсинг";
                thread.Suspend();
                return;
            }
        }

        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                System.Reflection.MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                mi.Invoke(notifyIcon1, null);
            }
        }
    }
}
