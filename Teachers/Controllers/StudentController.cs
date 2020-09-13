using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teachers.Models;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace Teachers.Controllers
{
    public class StudentController : Controller
    {
        public EducatorsContext db = new EducatorsContext();

        public ActionResult ViewMyMarks()
        {
            return RedirectToAction("MySubjects");
        }

        public ActionResult ViewProfile()
        {
            return View("ViewProfile");
        }

        public ActionResult MySubjects()
        {
            int stu_id = int.Parse(Session["StudentID"].ToString());


            var search = from student in db.Students
                         join studentClass in db.StudentClasses on student.StudentID equals studentClass.StudentID
                         join cls in db.Classes on studentClass.ClassID equals cls.ClassID
                         join teacher in db.Teachers on cls.employee_number equals teacher.employee_number
                         where student.StudentID == stu_id
                         select new { cls.subject_name, teacher.title, teacher.name, teacher.surname, cls.ClassID };

            List<Subs> s = new List<Subs>();

            foreach(var item in search)
            {
                Subs subs = new Subs();

                subs.subject = item.subject_name;

                subs.teacher = item.title + " " + item.name + " " + item.surname;

                subs.class_id = item.ClassID;

                s.Add(subs);

            }


            return View("MySubjects",s);
        }

        public ActionResult Assessments(int class_id)
        {
            int studentID = int.Parse(Session["StudentID"].ToString());

            var search = from a in db.Assessments
                         join ar in db.AssessmentResults on a.AssessmentID equals ar.AssessmentID
                         where a.ClassID == class_id && ar.StudentID == studentID
                         select new { a.assessment_name, a.assessment_date, ar.result };

            var sub = from cls in db.Classes
                      where cls.ClassID == class_id
                      select cls;

            ViewBag.Subject = sub.FirstOrDefault<Class>();

            //Past Due and marked
            List < Marked > marked = new List<Marked>();

            List<string> notMarked = new List<string>();

            List<Upcoming> upcoming = new List<Upcoming>();

            foreach(var item in search)
            {
                if(DateTime.Parse(item.assessment_date) < DateTime.Now)
                {
                    if (item.result != -1)  //Marked
                    {
                        Marked m = new Marked();

                        m.name = item.assessment_name;
                        m.mark = item.result;

                        marked.Add(m);
                    }
                    else
                    {
                        notMarked.Add(item.assessment_name);
                    }
                }
                else
                {
                    Upcoming u = new Upcoming();
                    u.date = item.assessment_date;
                    u.name = item.assessment_name;

                    upcoming.Add(u);
                }
            }

            ViewBag.Upcoming = upcoming;

            ViewBag.Marked = marked;

            ViewBag.NotMarked = notMarked;
            


            //To be Written

            return View("Assessments");
        }

        public ActionResult AllMarks()
        {
            int studentID = int.Parse(Session["StudentID"].ToString());


            var query = from s in db.Students
                        join ar in db.AssessmentResults on s.StudentID equals ar.StudentID
                        join a in db.Assessments on ar.AssessmentID equals a.AssessmentID
                        join c in db.Classes on a.ClassID equals c.ClassID
                        where s.StudentID == studentID && ar.result != -1
                        select new { a.assessment_name, a.assessment_date, ar.result, c.subject_name };

            List<Results> results = new List<Results>();

            foreach(var item in query)
            {
                Results r = new Results();

                r.date = item.assessment_date;
                r.name = item.assessment_name;
                r.mark = item.result;
                r.subject = item.subject_name;

                results.Add(r);

            }

            return View("AllMarks",results);
        }


        public ActionResult Terms()
        {
            return View("Terms");
        }

        public ActionResult FinalReport(string term)
        {
            ViewBag.Term = term;
            int t = int.Parse(term);


            int studentID = int.Parse(Session["StudentID"].ToString());


            var query = from s in db.Students
                        join ar in db.AssessmentResults on s.StudentID equals ar.StudentID
                        join a in db.Assessments on ar.AssessmentID equals a.AssessmentID
                        join c in db.Classes on a.ClassID equals c.ClassID
                        where s.StudentID == studentID && ar.result != -1 && a.term == t
                        select new { a.assessment_name, a.assessment_date, ar.result, c.subject_name, a.assessment_constribution };

            List<Results> results = new List<Results>();

            foreach (var item in query)
            {
                Results r = new Results();

                r.date = item.assessment_date;
                r.name = item.assessment_name;
                r.mark = (item.result/100) * item.assessment_constribution;
                r.subject = item.subject_name;

                results.Add(r);

            }

            var search = from r in results
                         group r by new { r.subject } into g
                         select new
                         {
                             subject = g.Key.subject,
                             mark = g.Select(x => x.mark).Sum()
                         };


            List<Results2> results2 = new List<Results2>();

            double accum = 0;
            double count = 0;

            foreach (var item in search)
            {
                Results2 r = new Results2();

                r.subject = item.subject;
                r.mark = Math.Round(item.mark);

                accum += r.mark;
                count += 1;

                results2.Add(r);
            }

            if (count > 0)
            {
                ViewBag.Average = Math.Round(accum / count);
            }
            else
            {
                ViewBag.Average = "";
            }


            return View("FinalReport", results2);
        }

        public class Results2
        {
            public string subject;
            public double mark;
        }

        public class Results
        {
            public string subject;
            //public string type;
            public string name;
            public string date;
            public double mark;
        }

        public class Upcoming
        {
            public string name;
            public string date;
        }

        public class Marked
        {
            public string name;
            public double mark;
        }


        public class Subs
        {
            public string subject;
            public string teacher;
            public int class_id;
        }
    }
}