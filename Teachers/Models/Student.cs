using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teachers.Models
{
    public class Student
    {
        public int StudentID { set; get; }
        public string first_name { set; get; }
        public string last_name { set; get; }
        public string email_address { set; get; }
        public string Gender { set; get; }
        public string date_of_birth { set; get; }
        public string Grade { set; get; }
        public string student_number { set; get; }
        public string password { set; get; }
    }
}