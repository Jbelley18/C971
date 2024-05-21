using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TermPlanner.Model
{
    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
        public bool NotificationOn { get; set; }
    }
}
