using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teachers.Models
{
    public class AssessmentResult
    {
        public int AssessmentResultID { set; get; }
        public int AssessmentID { set; get; }
        public int StudentID { set; get; }
        public double result { set; get; }
    }
}