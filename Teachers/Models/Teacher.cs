using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teachers.Models
{
    public class Teacher
    {
        public int ID { set; get; }
        public string title { set; get; }
        public string name { set; get; }
        public string surname { set; get; }
        public string employee_number { set; get; }
        public string email { set; get; }
        public string password { set; get; }
    }
}