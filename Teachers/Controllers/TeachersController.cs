using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Teachers.Models;
using System.Net.Mail;

namespace Teachers
{
    public class TeachersController : Controller
    {
        public EducatorsContext db = new EducatorsContext();

        public ActionResult Login()
        {
            return View();
        }



        public ActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        public ActionResult ForgotPasswordConfirmation(string email)
        {
            return View("ForgotPasswordConfirmation");
        }




        public ActionResult Logout()
        {
            try
            {
                Session.RemoveAll();

                return View("Login");

            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }


        public ActionResult Homepage()
        {
            try
            {
                string emp = Session["employee_number"].ToString();

                var search = from a in db.Classes
                             where a.employee_number == emp
                             select a;

                return View("Homepage", search);

            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        //public ActionResult AddTeacher()
        //{
        //  return View("AddTeacher");
        //}
        [HttpPost]
        public ActionResult AddTeacher(string employee_number, string title, string first_name, string last_name, string email)
        {
            try
            {
                Teacher teacher = new Teacher();


                var id = from number in db.Teachers
                         select number;

                teacher.employee_number = "T" + (DateTime.Now.Year).ToString() + (id.Count() + 1).ToString();


                teacher.title = title;
                teacher.name = first_name;
                teacher.surname = last_name;
                teacher.email = email;
                teacher.password = "1234";

                db.Teachers.Add(teacher);
                db.SaveChanges();

                return RedirectToAction("AdministratorsHomepage");
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult AddStudent(string first_name, string last_name, string email_address, string date, string gender, string grade)
        {
            try
            {
                Student student = new Student();


                var id = from number in db.Students
                         select number;

                student.student_number = "S" + (DateTime.Now.Year).ToString() + (id.Count() + 1).ToString();


                student.first_name = first_name;
                student.last_name = last_name;
                student.email_address = email_address;
                student.date_of_birth = date;
                student.Gender = gender;
                student.Grade = grade;
                student.password = "1234";

                db.Students.Add(student);
                db.SaveChanges();

                return RedirectToAction("AllAdminStudents");
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult AddClass(string subject_name, string grade_number, string employee_number)
        {
            try
            {
                Class classs = new Class();


                classs.subject_name = subject_name;
                classs.grade_number = grade_number;
                classs.employee_number = employee_number;


                db.Classes.Add(classs);
                db.SaveChanges();
                return RedirectToAction("AllClassessAdmin");
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult AdministratorsHomepage()
        {
            try
            {
                // New Controller
                var search = from teachers in db.Teachers
                             where teachers.employee_number != "ADMIN"
                             select new
                             {
                                 teachers.employee_number,
                                 teachers.title,
                                 teachers.name,
                                 teachers.surname,
                                 teachers.email
                             };
                List<AllTeachers> list = new List<AllTeachers>();

                foreach (var item in search)
                {
                    AllTeachers i = new AllTeachers();

                    i.employee_number = item.employee_number;
                    i.title = item.title;
                    i.name = item.name;
                    i.surname = item.surname;
                    i.email = item.email;
                    list.Add(i);
                };
                return View("ViewTeachers", list);
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }


        public ActionResult AllAdminStudents()
        {
            try
            {
                // New Controller
                var search = from students in db.Students
                             orderby students.last_name ascending
                             select students;


                return View("ViewStudents", search);
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult AllClassessAdmin()
        {
            try
            {
                // New Controller
                var search = from classes in db.Classes
                             join teacher in db.Teachers on classes.employee_number equals teacher.employee_number
                             select new
                             {
                                 classes.ClassID,
                                 classes.subject_name,
                                 classes.grade_number,
                                 classes.employee_number,
                                 teacher.title,
                                 teacher.surname,
                                 teacher.name
                             };
                List<AllClasses> list = new List<AllClasses>();

                foreach (var item in search)
                {
                    AllClasses c = new AllClasses();

                    c.ClassID = item.ClassID;
                    c.subject_name = item.subject_name;
                    c.grade_number = item.grade_number;
                    c.employee_number = item.employee_number;
                    c.title = item.title;
                    c.surname = item.surname;
                    c.name = item.name;
                    list.Add(c);
                };
                return View("ViewClasses", list);
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult Login(string emp_num, string pass)
        {

            try
            {
                if (emp_num[0] == 'S')
                {
                    var student = (from a in db.Students
                                   where a.student_number == emp_num && a.password == pass
                                   select a).FirstOrDefault<Student>();

                    if (student == null)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (student.Gender == "Male")
                        {
                            Session["title"] = "Mr";
                        }
                        else
                        {
                            Session["title"] = "Miss";
                        }

                        Session["employee_number"] = student.student_number;
                        Session["surname"] = student.last_name;
                        Session["name"] = student.first_name;
                        Session["email"] = student.email_address;
                        Session["password"] = student.password;
                        Session["StudentID"] = student.StudentID;
                        Session["gender"] = student.Gender;
                        Session["date"] = student.date_of_birth;
                        Session["grade"] = student.Grade;

                        return RedirectToAction("ViewMyMarks", "Student");
                    }
                }


                var search = from a in db.Teachers
                             where a.employee_number == emp_num && a.password == pass
                             select a;


                if (search != null)
                {
                    var employee = search.FirstOrDefault<Teacher>();

                    Session["employee_number"] = employee.employee_number;
                    Session["surname"] = employee.surname;
                    Session["title"] = employee.title;
                    Session["name"] = employee.name;
                    Session["ID"] = employee.ID;
                    Session["email"] = employee.email;
                    Session["password"] = employee.password;


                    if (employee.employee_number[0] == 'T')
                    {
                        return RedirectToAction("Homepage");
                    }

                    return RedirectToAction("AdministratorsHomepage");

                }
                else
                {
                    return View("Login");
                }
            }
            catch (Exception e)
            {
                return View("Login");
            }
        }

        public ActionResult ViewAssessments(int class_id)
        {
            try
            {
                Session["class_id"] = class_id;

                int cls = int.Parse(Session["class_id"].ToString());

                var sub = from a in db.Classes
                          where a.ClassID == cls
                          select a.subject_name;

                string b = sub.FirstOrDefault<string>();

                ViewBag.head = b.ToString();


                var search = from assessment in db.Assessments
                             where assessment.ClassID == class_id
                             orderby assessment.assessment_date ascending
                             select assessment;


                return View("ViewAssessments", search);

            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult AddAssessment()
        {
            return View("AddAssessment");
        }

        [HttpPost]
        public ActionResult AddAssessment(string name, string type, string date, int contr, string term)
        {
            try
            {
                int num = int.Parse(Session["class_id"].ToString());

                Assessment assessment = new Assessment();

                assessment.assessment_name = name;
                assessment.assessment_type = type;
                assessment.assessment_date = date;
                assessment.assessment_constribution = contr;
                assessment.term = int.Parse(term);
                assessment.ClassID = num;
                //assessment.term = term;

                db.Assessments.Add(assessment);
                db.SaveChanges();

                var q = from a in db.Assessments
                        where a.assessment_name == name && a.assessment_type == type && a.assessment_date == date && a.assessment_constribution == contr
                        select a.AssessmentID;

                var students = from b in db.StudentClasses
                               where b.ClassID == num
                               select b;

                int id = q.FirstOrDefault<int>();

                foreach (var x in students)
                {
                    AssessmentResult ar = new AssessmentResult();

                    ar.AssessmentID = id;
                    ar.StudentID = x.StudentID;
                    ar.result = -1;

                    db.AssessmentResults.Add(ar);
                }
                db.SaveChanges();


                return RedirectToAction("ViewAssessments", new { class_id = int.Parse(Session["class_id"].ToString()) });
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }


        public ActionResult DeleteAssessment(int ass_id)
        {
            try
            {
                var search = from assessment in db.Assessments
                             where assessment.AssessmentID == ass_id
                             select assessment;

                db.Assessments.Remove(search.FirstOrDefault<Assessment>());
                db.SaveChanges();


                return RedirectToAction("ViewAssessments", new { class_id = int.Parse(Session["class_id"].ToString()) });
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult EditAssessment(int ass_id)
        {
            try
            {
                var search = from a in db.Assessments
                             where a.AssessmentID == ass_id
                             select a;

                return View("EditAssessment", search.FirstOrDefault<Assessment>());
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult UpdateAssessment(string name, string type, string date, int contr, int ass_id, string term)
        {
            try
            {
                int num = int.Parse(Session["class_id"].ToString());

                //To Update
                var delete = from a in db.Assessments
                             where a.AssessmentID == ass_id
                             select a;

                var b = delete.FirstOrDefault<Assessment>();

                b.assessment_constribution = contr;
                b.assessment_date = date;
                b.assessment_name = name;
                b.assessment_type = type;
                b.term = int.Parse(term);
                //b.term = term;
                db.SaveChanges();

                return RedirectToAction("ViewAssessments", new { class_id = int.Parse(Session["class_id"].ToString()) });
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult RecordMarks(int ass_id)
        {
            try
            {
                Session["ass_id"] = ass_id;


                var query = from results in db.AssessmentResults
                            join student in db.Students on results.StudentID equals student.StudentID
                            where results.AssessmentID == ass_id
                            orderby results.StudentID ascending
                            select new { student.first_name, student.last_name, results.result, results.StudentID };


                List<Results> list = new List<Results>();

                foreach (var item in query)
                {
                    Results a = new Results();

                    a.first_name = item.first_name;
                    a.last_name = item.last_name;
                    a.result = item.result;
                    a.StudentID = item.StudentID;

                    list.Add(a);
                }


                return View("RecordMarks", list);
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult SaveMark(int mark, int stud_id)
        {
            try
            {
                //Check if already saved

                int ass_id = int.Parse(Session["ass_id"].ToString());

                var check_existance = from a in db.AssessmentResults
                                      where a.AssessmentID == ass_id && a.StudentID == stud_id
                                      select a;

                if (check_existance.FirstOrDefault<AssessmentResult>() != null)
                {
                    db.AssessmentResults.Remove(check_existance.FirstOrDefault<AssessmentResult>());
                    db.SaveChanges();
                }

                //Save Mark

                AssessmentResult assessmentResult = new AssessmentResult();

                assessmentResult.AssessmentID = int.Parse(Session["ass_id"].ToString());
                assessmentResult.StudentID = stud_id;
                assessmentResult.result = mark;

                db.AssessmentResults.Add(assessmentResult);
                db.SaveChanges();

                //Get List Again


                var query = from results in db.AssessmentResults
                            join student in db.Students on results.StudentID equals student.StudentID
                            where results.AssessmentID == ass_id
                            orderby results.StudentID ascending
                            select new { student.first_name, student.last_name, results.result, results.StudentID };


                List<Results> list = new List<Results>();

                foreach (var item in query)
                {
                    Results a = new Results();

                    a.first_name = item.first_name;
                    a.last_name = item.last_name;
                    a.result = item.result;
                    a.StudentID = item.StudentID;

                    list.Add(a);
                }

                return View("RecordMarks", list);
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult ViewProfile()
        {
            return View("ViewProfile");
        }

        [HttpPost]
        public ActionResult ChangePassword(string Password, string New_Password, string New_Password_Confirm)
        {
            try
            {
                string identity = Session["employee_number"].ToString();

                if (identity[0] == 'S')
                {
                    if (New_Password == New_Password_Confirm && Password == Session["password"].ToString())
                    {
                        string stud = Session["employee_number"].ToString();

                        var s = from st in db.Students
                                where st.student_number == stud
                                select st;

                        var old = s.FirstOrDefault<Student>();

                        db.Students.Remove(old);
                        db.SaveChanges();


                        Student student = new Student();

                        student.first_name = Session["name"].ToString();
                        student.last_name = Session["surname"].ToString();
                        student.email_address = Session["email"].ToString();
                        student.Gender = Session["gender"].ToString();
                        student.date_of_birth = Session["date"].ToString();
                        student.Grade = Session["grade"].ToString();
                        student.student_number = Session["employee_number"].ToString();
                        student.password = New_Password;

                        db.Students.Add(student);
                        db.SaveChanges();

                        return RedirectToAction("Logout");
                    }
                    else
                    {
                        return RedirectToAction("ViewProfile");
                    }
                }
                else
                {

                    if (New_Password == New_Password_Confirm && Password == Session["password"].ToString())
                    {
                        string empl = Session["employee_number"].ToString();

                        var emp = from a in db.Teachers
                                  where a.employee_number == empl
                                  select a;

                        var old = emp.FirstOrDefault<Teacher>();

                        db.Teachers.Remove(old);
                        db.SaveChanges();


                        Teacher teacher = new Teacher();

                        teacher.employee_number = Session["employee_number"].ToString();
                        teacher.surname = Session["surname"].ToString();
                        teacher.title = Session["title"].ToString();
                        teacher.name = Session["name"].ToString();
                        teacher.ID = int.Parse(Session["ID"].ToString());
                        teacher.email = Session["email"].ToString();
                        teacher.password = New_Password;

                        db.Teachers.Add(teacher);
                        db.SaveChanges();

                        return RedirectToAction("Logout");

                    }
                    else
                    {
                        return RedirectToAction("ViewProfile");
                    }
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("ViewProfile");
            }
        }

        public ActionResult ViewAllAssessments()
        {
            try
            {
                string emp_num = Session["employee_number"].ToString();

                var search = from classes in db.Classes
                             join assessment in db.Assessments on classes.ClassID equals assessment.ClassID
                             where classes.employee_number == emp_num
                             select new { assessment.assessment_date, classes.subject_name, classes.grade_number, assessment.assessment_type, assessment.assessment_name, assessment.assessment_constribution, assessment.term };


                List<AllAssessments> list = new List<AllAssessments>();

                foreach (var item in search)
                {
                    AllAssessments a = new AllAssessments();

                    a.date = item.assessment_date;
                    a.subject = item.subject_name;
                    a.grade = item.grade_number;
                    a.type = item.assessment_type;
                    a.title = item.assessment_name;
                    a.contribution = item.assessment_constribution;
                    a.term = item.term;

                    list.Add(a);
                }


                return View("AllAssessments", list);

            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }



        //Teacher's Profile
        public ActionResult TeacherProfile(string employee_number)
        {
            try
            {
                var subjects = from module in db.Classes
                               where module.employee_number == employee_number
                               select module;

                ViewBag.Subjects = subjects;


                var query = from teacher in db.Teachers
                            where teacher.employee_number == employee_number
                            select teacher;


                return View("TeacherProfile", query.FirstOrDefault<Teacher>());
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }


        public ActionResult StudentProfile(int StudentID)
        {
            try
            {
                Session["StudentID"] = StudentID;

                return View("StudentProfile");
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }


        public ActionResult ClassProfile(int ClassID)
        {
            try
            {
                Session["ClassID"] = ClassID;

                var search = from cls in db.Classes
                             join t in db.Teachers on cls.employee_number equals t.employee_number
                             where cls.ClassID == ClassID
                             select new { t.title, t.surname, t.name, cls.subject_name, cls.grade_number };


                CProfile profile = new CProfile();

                foreach (var item in search)
                {
                    profile.title = item.title;
                    profile.surname = item.surname;
                    profile.name = item.name;
                    profile.subject_name = item.subject_name;
                    profile.grade_number = item.grade_number;
                }

                ViewBag.Name = profile.title + " " + profile.surname + " " + profile.name;

                ViewBag.Subject = profile.subject_name;

                ViewBag.Grade = profile.grade_number;


                var students = from a in db.Students
                               join sc in db.StudentClasses on a.StudentID equals sc.StudentID
                               where sc.ClassID == ClassID
                               select a;

                ViewBag.Students = students;


                return View("ClassProfile", search);
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Enrol(string student)
        {
            var Student = (from s in db.Students
                           where s.student_number == student
                           select s).FirstOrDefault<Student>();

            int studentID = Student.StudentID;

            StudentClass studentClass = new StudentClass();

            studentClass.ClassID = int.Parse(Session["ClassID"].ToString());
            studentClass.StudentID = studentID;

            db.StudentClasses.Add(studentClass);
            db.SaveChanges();


            return RedirectToAction("ClassProfile", new { ClassID = int.Parse(Session["ClassID"].ToString()) });
        }

        public class CProfile
        {
            public string title;
            public string surname;
            public string name;
            public string subject_name;
            public string grade_number;
        }

        public ActionResult ConfirmDelete(int ass_id)
        {
            try
            {
                var search = from a in db.Assessments
                             where a.AssessmentID == ass_id
                             select a;

                return View("ConfirmDelete", search.FirstOrDefault<Assessment>());
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Contribution()
        {
            try
            {
                int class_id = int.Parse(Session["class_id"].ToString());

                int cls = int.Parse(Session["class_id"].ToString());


                var search = from assessment in db.Assessments
                             where assessment.ClassID == class_id
                             orderby assessment.assessment_date ascending
                             select assessment;


                return View("Contribution", search);

            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Dashboard()
        {
            //Get Number Of Classes
            string emp_num = Session["employee_number"].ToString();

            var cls = from a in db.Classes
                      where a.employee_number == emp_num
                      select a;

            ViewBag.Classes = cls.Count().ToString();


            //Get Number Of Assessments
            var search = from classes in db.Classes
                         join assessment in db.Assessments on classes.ClassID equals assessment.ClassID
                         where classes.employee_number == emp_num
                         select new { assessment.assessment_date, classes.subject_name, classes.grade_number, assessment.assessment_type, assessment.assessment_name, assessment.assessment_constribution };

            ViewBag.Tests = search.Count().ToString();


            //Get Number Of students
            var students = from teacher in db.Teachers
                           join clas in db.Classes on teacher.employee_number equals clas.employee_number
                           join class_student in db.StudentClasses on clas.ClassID equals class_student.ClassID
                           where teacher.employee_number == emp_num
                           select class_student;

            List<int> lis = new List<int>();

            foreach (var item in students)
            {
                lis.Add(item.StudentID);
            }


            ViewBag.Students = (lis.Distinct()).Count();


            //get number of overdue

            var overdue = from teacher in db.Teachers
                          join classes in db.Classes on teacher.employee_number equals classes.employee_number
                          join assessments in db.Assessments on classes.ClassID equals assessments.ClassID
                          where teacher.employee_number == emp_num
                          select new { assessments.assessment_name, assessments.assessment_date };

            int count = 0;

            foreach (var item in overdue)
            {
                if (DateTime.Parse(item.assessment_date) < DateTime.Now)
                {
                    count++;
                }
            }

            ViewBag.Overdue = count;


            //get number of risk
            var risk = from r in db.AssessmentResults
                       join a in db.Assessments on r.AssessmentID equals a.AssessmentID
                       join c in db.Classes on a.ClassID equals c.ClassID
                       join t in db.Teachers on c.employee_number equals t.employee_number
                       where t.employee_number == emp_num && r.result < 50 && r.result > -1
                       select r;

            List<int> li = new List<int>();

            foreach (var item in risk)
            {
                li.Add(item.StudentID);
            }


            //get number of pass
            var pass = from r in db.AssessmentResults
                       join a in db.Assessments on r.AssessmentID equals a.AssessmentID
                       join c in db.Classes on a.ClassID equals c.ClassID
                       join t in db.Teachers on c.employee_number equals t.employee_number
                       where t.employee_number == emp_num && r.result >= 50
                       select r;

            List<int> pa = new List<int>();

            foreach (var item in pass)
            {
                if (!(li.Contains(item.StudentID)))
                {
                    pa.Add(item.StudentID);

                }
            }


            ViewBag.Pass = (pa.Distinct()).Count();


            ViewBag.Risk = (li.Distinct()).Count();

            //List of assessment
            var list = from a in db.Assessments
                       join ar in db.AssessmentResults on a.AssessmentID equals ar.AssessmentID
                       join c in db.Classes on a.ClassID equals c.ClassID
                       join t in db.Teachers on c.employee_number equals t.employee_number
                       where t.employee_number == emp_num
                       select new { a.assessment_name, a.assessment_date, ar.result };

            List<String> written = new List<String>();

            foreach (var item in list)
            {
                if (DateTime.Parse(item.assessment_date) < DateTime.Now && item.result != -1)
                {
                    written.Add(item.assessment_name);
                }
            }

            ViewBag.WrittenAndMarked = written.Distinct();


            //Due but not marked
            var list2 = from a in db.Assessments
                        join ar in db.AssessmentResults on a.AssessmentID equals ar.AssessmentID
                        join c in db.Classes on a.ClassID equals c.ClassID
                        join t in db.Teachers on c.employee_number equals t.employee_number
                        where t.employee_number == emp_num
                        select new { a.assessment_name, a.assessment_date, ar.result };

            List<String> writtenNotMarked = new List<String>();

            foreach (var item in list)
            {
                if (DateTime.Parse(item.assessment_date) < DateTime.Now && item.result == -1)
                {
                    writtenNotMarked.Add(item.assessment_name);
                }
            }

            ViewBag.WrittenNotMarked = writtenNotMarked.Distinct();


            //NOT YET WRITTEN
            var list3 = from a in db.Assessments
                        join ar in db.AssessmentResults on a.AssessmentID equals ar.AssessmentID
                        join c in db.Classes on a.ClassID equals c.ClassID
                        join t in db.Teachers on c.employee_number equals t.employee_number
                        where t.employee_number == emp_num
                        select new { a.assessment_name, a.assessment_date };

            List<String> notWritten = new List<String>();

            foreach (var item in list)
            {
                if (DateTime.Parse(item.assessment_date) >= DateTime.Now)
                {
                    notWritten.Add(item.assessment_name);
                }
            }

            ViewBag.NotWritten = notWritten.Distinct();




            //Upcoming assessments
            var upcoming = from t in db.Teachers
                           join c in db.Classes on t.employee_number equals c.employee_number
                           join a in db.Assessments on c.ClassID equals a.ClassID
                           where t.employee_number == emp_num
                           select a;

            ViewBag.Upcoming = upcoming;


            //TOP Performers
            var top = from t in db.Teachers
                      join c in db.Classes on t.employee_number equals c.employee_number
                      join a in db.Assessments on c.ClassID equals a.ClassID
                      join ar in db.AssessmentResults on a.AssessmentID equals ar.AssessmentID
                      join s in db.Students on ar.StudentID equals s.StudentID
                      where ar.result > 70 && t.employee_number == emp_num
                      orderby ar.result descending
                      select new { s.first_name, s.last_name, ar.result };

            List<String> student = new List<String>();

            List<double> marks = new List<double>();

            foreach (var item in top)
            {
                student.Add(item.last_name + " " + item.first_name);
                marks.Add(item.result);
            }



            ViewBag.TopPerformers = student.Distinct();
            ViewBag.TopMarks = marks.Distinct();

            //AT RISK
            var low = from t in db.Teachers
                      join c in db.Classes on t.employee_number equals c.employee_number
                      join a in db.Assessments on c.ClassID equals a.ClassID
                      join ar in db.AssessmentResults on a.AssessmentID equals ar.AssessmentID
                      join s in db.Students on ar.StudentID equals s.StudentID
                      where ar.result < 50 && t.employee_number == emp_num && ar.result > -1
                      orderby ar.result descending
                      select new { s.first_name, s.last_name, ar.result };

            List<String> student2 = new List<String>();

            foreach (var item in low)
            {
                if (!(student.Contains(item.last_name + " " + item.first_name)))
                {
                    student2.Add(item.last_name + " " + item.first_name);
                }
            }



            ViewBag.LowPerformers = student2.Distinct();

            return View("Dashboard");
        }

        public ActionResult ViewMyMarks()
        {
            return View("ViewMarks");
        }

        public ActionResult Term()
        {
            return View("Term");
        }


        public ActionResult FinalMark(int class_id)
        {

            Session["class_id"] = class_id;

            int cls = int.Parse(Session["class_id"].ToString());

            var sub = from a in db.Classes
                      where a.ClassID == cls
                      select a.subject_name;

            string b = sub.FirstOrDefault<string>();

            ViewBag.head2 = b.ToString();

            int trm = int.Parse(Session["term"].ToString());


            var query = from s in db.Students
                        join ar in db.AssessmentResults on s.StudentID equals ar.StudentID
                        join a in db.Assessments on ar.AssessmentID equals a.AssessmentID
                        join c in db.Classes on a.ClassID equals c.ClassID
                        where c.ClassID == class_id && ar.result != -1 && a.term == trm
                        select new { s.student_number, s.last_name, s.first_name, ar.result, a.assessment_constribution };

            List<Total> totals = new List<Total>();

            foreach (var item in query)
            {
                Total total = new Total();

                total.student_number = item.student_number;
                total.lname = item.last_name;
                total.fname = item.first_name;
                total.mark = ((item.result) / 100) * item.assessment_constribution;

                totals.Add(total);
            }

            var search = from t in totals
                         group t by new { t.student_number, t.fname, t.lname } into g
                         select new
                         {
                             student_number = g.Key.student_number,
                             lname = g.Key.lname,
                             fname = g.Key.fname,
                             mark = g.Select(x => x.mark).Sum()
                         };

            List<Total> totals2 = new List<Total>();

            foreach (var item in search)
            {
                Total total2 = new Total();

                total2.student_number = item.student_number;
                total2.lname = item.lname;
                total2.fname = item.fname;
                total2.mark = item.mark;

                totals2.Add(total2);
            }

            return View("FinalMark", totals2);
        }


        public ActionResult SubjectReport(string term)
        {
            Session["term"] = term;

            try
            {
                string emp = Session["employee_number"].ToString();


                var search = from a in db.Classes
                             where a.employee_number == emp
                             select a;


                return View("SubjectReport", search);

            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult AdminTerm()
        {
            return View("AdminTerm");
        }

        public ActionResult AdminSubjectReport(string term)
        {
            Session["term"] = term;

            try
            {
                string emp = Session["employee_number"].ToString();


                var search = from a in db.Classes
                             select a;


                return View("AdminSubjectReport", search);

            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult AdminFinalMarks(int class_id)
        {

            Session["class_id"] = class_id;

            int cls = int.Parse(Session["class_id"].ToString());

            var sub = from a in db.Classes
                      where a.ClassID == cls
                      select a.subject_name;

            string b = sub.FirstOrDefault<string>();

            ViewBag.head2 = b.ToString();

            int trm = int.Parse(Session["term"].ToString());


            var query = from s in db.Students
                        join ar in db.AssessmentResults on s.StudentID equals ar.StudentID
                        join a in db.Assessments on ar.AssessmentID equals a.AssessmentID
                        join c in db.Classes on a.ClassID equals c.ClassID
                        where c.ClassID == class_id && ar.result != -1 && a.term == trm
                        select new { s.student_number, s.last_name, s.first_name, ar.result, a.assessment_constribution };

            List<Total> totals = new List<Total>();

            foreach (var item in query)
            {
                Total total = new Total();

                total.student_number = item.student_number;
                total.lname = item.last_name;
                total.fname = item.first_name;
                total.mark = ((item.result) / 100) * item.assessment_constribution;

                totals.Add(total);
            }

            var search = from t in totals
                         group t by new { t.student_number, t.fname, t.lname } into g
                         select new
                         {
                             student_number = g.Key.student_number,
                             lname = g.Key.lname,
                             fname = g.Key.fname,
                             mark = g.Select(x => x.mark).Sum()
                         };

            List<Total> totals2 = new List<Total>();

            foreach (var item in search)
            {
                Total total2 = new Total();

                total2.student_number = item.student_number;
                total2.lname = item.lname;
                total2.fname = item.fname;
                total2.mark = item.mark;

                totals2.Add(total2);
            }


            return View("AdminFinalMarks", totals2);
        }

        public ActionResult AdminReport(string term)
        {
            Session["term"] = term;

            var students = from s in db.Students
                           select s;

            return View("AdminReport", students);
        }

        public ActionResult FinalReport(int studentID)
        {
            int t = int.Parse(Session["term"].ToString());

            var student = (from s in db.Students
                           where s.StudentID == studentID
                           select s).FirstOrDefault<Student>();

            ViewBag.Name = student.first_name;
            ViewBag.Surname = student.last_name;
            ViewBag.DOB = student.date_of_birth;
            ViewBag.Number = student.student_number;
            ViewBag.Grade = student.Grade;


            var query = from s in db.Students
                        join ar in db.AssessmentResults on s.StudentID equals ar.StudentID
                        join a in db.Assessments on ar.AssessmentID equals a.AssessmentID
                        join c in db.Classes on a.ClassID equals c.ClassID
                        where s.StudentID == studentID && ar.result != -1 && a.term == t
                        select new { a.assessment_name, a.assessment_date, ar.result, c.subject_name, a.assessment_constribution };

            List<Result> results = new List<Result>();

            foreach (var item in query)
            {
                Result r = new Result();

                r.date = item.assessment_date;
                r.name = item.assessment_name;
                r.mark = (item.result / 100) * item.assessment_constribution;
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


            return View("FinalReport");
        }


        public ActionResult FinalReportForParents(int studentID, int term)
        {
            int t = term;

            var student = (from s in db.Students
                           where s.StudentID == studentID
                           select s).FirstOrDefault<Student>();

            ViewBag.Name = student.first_name;
            ViewBag.Surname = student.last_name;
            ViewBag.DOB = student.date_of_birth;
            ViewBag.Number = student.student_number;
            ViewBag.Grade = student.Grade;


            var query = from s in db.Students
                        join ar in db.AssessmentResults on s.StudentID equals ar.StudentID
                        join a in db.Assessments on ar.AssessmentID equals a.AssessmentID
                        join c in db.Classes on a.ClassID equals c.ClassID
                        where s.StudentID == studentID && ar.result != -1 && a.term == t
                        select new { a.assessment_name, a.assessment_date, ar.result, c.subject_name, a.assessment_constribution };

            List<Result> results = new List<Result>();

            foreach (var item in query)
            {
                Result r = new Result();

                r.date = item.assessment_date;
                r.name = item.assessment_name;
                r.mark = (item.result / 100) * item.assessment_constribution;
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


            return View("FinalReport");
        }

        public ActionResult AdminStudentsReport(string class_id)
        {
            int id = int.Parse(class_id);

            var search = from s in db.Students
                         join sc in db.StudentClasses on s.StudentID equals sc.StudentID
                         join c in db.Classes on sc.ClassID equals c.ClassID
                         where c.ClassID == id
                         select new { s.first_name, s.last_name, s.student_number };


            var sub = (from c in db.Classes
                       where c.ClassID == id
                       select new { c.grade_number });

            ViewBag.Grade = null;

            foreach (var item in sub)
            {
                ViewBag.Grade = item.grade_number;
            }


            List<StudentMark> sm = new List<StudentMark>();

            foreach (var item in search)
            {
                StudentMark s = new StudentMark();
                s.fname = item.first_name;
                s.lname = item.last_name;
                s.number = item.student_number;

                sm.Add(s);
            }


            return View("AdminStudentsReport", sm);
        }

        public ActionResult SendReportToParents ()
        {
            int t = int.Parse(Session["term"].ToString());
            var allstudents = from students in db.Students
                         orderby students.last_name ascending
                         select students;

            foreach (var item in allstudents)
            {
                SendReportLinkEmail(item.email_address, item.StudentID, item.first_name, t);
            }
            return RedirectToAction("AdminTerm");
        }

        [NonAction]
        public void SendReportLinkEmail(string parents_emailID, int StudentID, string student_name, int TermNum)
        {
            var verifyUrl = "/Teachers/FinalReport?StudentID=" + StudentID + "&term=" + TermNum;
            var link = "https://nizamiyechecks.azurewebsites.net" + verifyUrl;

            var fromEmail = new MailAddress("prodigy5uj@gmail.com", "Nizamiye Schools Mayfair");
            var toEmail = new MailAddress(parents_emailID);
            var fromEmailPassword = "Prodigies@5"; // Prodigy5 actual pass
            string subject = "Nizamiye Schools Term Progress Report";

            var inlineLogo = new LinkedResource(Server.MapPath("~/Content/newassets/img/nizamiyesquare.png"), "image/png");
            inlineLogo.ContentId = Guid.NewGuid().ToString();


            string body = "Dear Parents and Guardians,<br/><br/>It has come to the end of another term for the year. In an effort to decrease paper use, " +
                "you may now access your child's term report through a link bellow.  "+
                "Please note that these results are a result of the effort your child has put in throughout the term and this may be affected by several factors. <br/><br/>" +
                " Please <a href='" + link + "'>" + "click here" + "</a> "+ "to view the report for" + student_name +
                "<br/> For questions related to your child's term report, please contact the school office. " +
                "<br/> Thank you." + string.Format(@"<img src=""cid:{0}""/>",inlineLogo.ContentId);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        public class Results2
        {
            public string subject;
            public double mark;
        }

        public class Result
        {
            public string subject;
            //public string type;
            public string name;
            public string date;
            public double mark;
        }

        public class StudentMark
        {
            public string fname;
            public string lname;
            public string number;
        }

        public class TeacherClass
        {
            public string subject;
            public string grade;

            public string teacher;
            public int ID;
        }

        public class Total
        {
            public string student_number;
            public string fname;
            public string lname;
            public double mark;
        }


        public class Results
        {
            public string first_name;
            public string last_name;
            public double result;
            public int StudentID;
        }

        public class AllAssessments
        {
            public string date;
            public string subject;
            public string grade;
            public string title;
            public string type;
            public int term;
            public int contribution;
        }

        public class AllTeachers
        {
            public string employee_number;
            public string title;
            public string name;
            public string surname;
            public string email;
        }

        public class AllClasses
        {
            public int ClassID;
            public string subject_name;
            public string grade_number;
            public string employee_number;
            public string title;
            public string surname;
            public string name;
        }

        public class AllStudents
        {
            public int StudentID;
            public string first_name;
            public string last_name;
            public string email_address;
            // public string age;
            // public int Gender;
            // public int Grade;
        }
    }
}