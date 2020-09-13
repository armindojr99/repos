using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teachers.Models
{
    public class Assessment
    {
        public int AssessmentID { set; get; }
        public string assessment_type { set; get; }
        public string assessment_name { set; get; }
        public string assessment_date { set; get; }
        public int assessment_constribution { set; get; }
        public int ClassID { set; get; }
        public int term { set; get; }
    }
}