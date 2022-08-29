using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class AboutUs
    {
        public int ID { get; set; }
        public string TITLE { get; set; }
        public int STATUS { get; set; }
    }

    public class User
    {
        public int Id { get;set;}
        public string UserName { get;set;}
        public string LoginDate { get;set;}
        public bool Status { get;set;}
    }
}
