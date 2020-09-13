using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teachers.Models
{
    public class Class
    {
        public int ClassID { set;get;}
        public string subject_name { set; get; }
        public string grade_number { set; get; }
        public string employee_number { set; get; }
    }
}