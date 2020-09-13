using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Teachers.Models
{
    public class EducatorsContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public EducatorsContext() : base("name=EducatorsContext")
        {
        }

        public System.Data.Entity.DbSet<Teachers.Models.Teacher> Teachers { get; set; }
        public System.Data.Entity.DbSet<Teachers.Models.Subject> Subjects { get; set; }
        public System.Data.Entity.DbSet<Teachers.Models.Assessment> Assessments { get; set; }
        public System.Data.Entity.DbSet<Teachers.Models.AssessmentResult> AssessmentResults { get; set; }
        public System.Data.Entity.DbSet<Teachers.Models.Class> Classes { get; set; }
        public System.Data.Entity.DbSet<Teachers.Models.Grade> Grades { get; set; }
        public System.Data.Entity.DbSet<Teachers.Models.Student> Students { get; set; }
        public System.Data.Entity.DbSet<Teachers.Models.StudentClass> StudentClasses { get; set; }
    }
}
