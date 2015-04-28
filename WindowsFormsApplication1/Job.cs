using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    class Job
    {
        string _header;
        string _price;
        string _article;
        public string header
        {
            get { return _header; }
        }
        public string price
        {
            get { return _price; }
        }
        public string article
        {
            get { return _article; }
        }
        public Job(string header,string price, string article)
        {
            _header = header;
            _price = price;
            _article = article;
        }
        public static bool operator ==(Job job1, Job job2)
        {
            if (job1.header == job2.header && job1.price == job2.price && job1.article == job2.article)
                return true;
            else 
                return false;
        }
        public static bool operator !=(Job job1, Job job2)
        {
            if (job1.header != job2.header || job1.price != job2.price || job1.article != job2.article)
                return true;
            else
                return false;
        }
    }
}
