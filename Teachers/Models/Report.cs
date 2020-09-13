using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teachers.Models
{
    public class Report
    {
        public int ReportID { set; get; }
        public int StudentID { set; get; }
        public int ClassID { set; get; }
    }
}