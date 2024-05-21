using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xamarin.Essentials;
using SQLite;
using System.Threading.Tasks;
using TermPlanner.Model;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Net.Mail;

namespace TermPlanner.Services
{
    public static class Database
    {
        public static SQLiteAsyncConnection db;
        public static bool Firstrun = true;

        //Create the DB
        public static async Task CreateDb()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TermPlanner_db.db");
            db = new SQLiteAsyncConnection(databasePath);
        }

        // Create Tables
        public static async Task CreateTables()
        {
            await db.DropTableAsync<Term>();
            await db.DropTableAsync<Course>();
            await db.DropTableAsync<Assessment>();

            await db.CreateTableAsync<Term>();
            await db.CreateTableAsync<Course>();
            await db.CreateTableAsync<Assessment>();
            //fill the tables with dummy data if the DB was just created or if reset button was pressed
            if (Firstrun)
            {
                await FillDummyData();
                Firstrun = false;
            }

        }
        //Populate dummy data into tables 
        public static async Task FillDummyData()
        {
            //Term Data
            var term1 = new Term
            {
                Name = "Term 1",
                StartDate = DateTime.Today.AddDays(-5),
                EndDate = DateTime.Today.AddDays(+10),
            };
            await db.InsertAsync(term1);

            //Course Data
            var course = new Course()
            {
                TermId = 1,
                Name = "Mobile App Dev",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(+9),
                Status = "In-Progress",
                InstructorName = "Zach Cabrera",
                InstructorPhone = "770-655-8985",
                InstructorEmail = "zcabre1@wgu.edu",
                Notes = "This is a note",
                NotificationOn = true
            };
            await db.InsertAsync(course);


            //Assessment Data
            var perfAssess = new Assessment
            {
                CourseId = 1,
                Name = "Mobile Application Project",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(+6),
                Type = "Performance Assessment",
                NotificationOn = true
            };
            var objAssess = new Assessment
            {
                CourseId = 1,
                Name = "Pre-Test",
                StartDate = DateTime.Today.AddDays(+2),
                EndDate = DateTime.Today.AddDays(+3),
                Type = "Objective Assessment",
                NotificationOn = true
            };
            await db.InsertAsync(perfAssess);
            await db.InsertAsync(objAssess);
        }

        //----------------
        //CRUD methods
        //----------------

        //---Term---
        //Add Term
        public static async Task AddTerm(string name, DateTime startDate, DateTime endDate)
        {
            var term = new Term
            {
                Name = name,
                StartDate = startDate,
                EndDate = endDate
            };
            var id = await db.InsertAsync(term);
        }
        //Remove Term
        public static async Task RemoveTerm(int id)
        {
            await db.DeleteAsync<Term>(id);
        }
        //Get Terms
        public static async Task<IEnumerable<Term>> GetTerms()
        {
            await db.CreateTableAsync<Term>();
            var termsFilter = await db.QueryAsync<Term>("SELECT * FROM Terms");
            var terms = new ObservableCollection<Term>(termsFilter);
            return terms;
        }

        //---Course---
        //Add Course
        public static async Task AddCourse(int termId, string name, DateTime startDate, DateTime endDate, string status, string instructorName, string instructorPhone, string instructorEmail, string notes, bool notificationOn)
        {
            var course = new Course()
            {
                TermId = termId,
                Name = name,
                StartDate = startDate,
                EndDate = endDate,
                Status = status,
                InstructorName = instructorName,
                InstructorPhone = instructorPhone,
                InstructorEmail = instructorEmail,
                Notes = notes,
                NotificationOn = notificationOn
            };
            var id = await db.InsertAsync(course);
        }
        //Remove Course
        public static async Task RemoveCourse(int id)
        {
            await db.DeleteAsync<Course>(id);
        }
        //Get Courses
        public static async Task<IEnumerable<Course>> GetCourseList(int termId)
        {
            await db.CreateTableAsync<Course>();
            var coursesFiltered = await db.QueryAsync<Course>($"SELECT * FROM COURSES WHERE TermId = '{termId}'");
            var courseList = new ObservableCollection<Course>(coursesFiltered);
            return courseList;
        }

        //---Assessment---
        //Add Assessment
        public static async Task AddAssessment(int courseId, string name, DateTime start, DateTime end, string type, bool notify)
        {
            var assessment = new Assessment()
            {
                CourseId = courseId,
                Name = name,
                StartDate = start,
                EndDate = end,
                Type = type,
                NotificationOn = notify
            };
            var id = await db.InsertAsync(assessment);
        }
        //Remove Assessment
        public static async Task RemoveAssessment(int id)
        {
            await db.DeleteAsync<Assessment>(id);
        }
        //Get Assessments
        public static async Task<IEnumerable<Assessment>> GetAssessementList(int courseId)
        {
            await db.CreateTableAsync<Model.Assessment>();
            var assessmentFiltered = await db.QueryAsync<Assessment>($"SELECT * FROM Assessments WHERE CourseId = '{courseId}'");
            var assessmentList = new ObservableCollection<Assessment>(assessmentFiltered);
            return assessmentList;
        }


        //----------------
        //Validation
        //----------------

        //Date Validation
        public static bool checkDate(DateTime start, DateTime end)
        {
            bool isValid = true;
            int compare = DateTime.Compare(start, end);
            if (compare > 0 || compare == 0)
            {
                isValid = false;
            }
            return isValid;

        }
        //Assessment Validations------
        //Assessment Add Validation - Only one Assessment type "Performance" and one Assessment type "Objective"
        public static async Task<bool> assessmentAddTypeExists(string type, int courseId)
        {
            bool exists = false;
            var currentAssessments = await db.QueryAsync<Assessment>($"SELECT * FROM Assessments WHERE CourseId = '{courseId}' AND Type = '{type}'");
            if (currentAssessments.Count > 0)
            {
                exists = true;
            }
            return exists;
        }
        //Assessment Edit Validation - Only one Assessment type "Performance" and one Assessment type "Objective"
        public static async Task<bool> assessmentEditTypeExists(string type, int courseId, int assessId)
        {
            bool exists = false;
            var currentAssessments = await db.QueryAsync<Assessment>($"SELECT * FROM Assessments WHERE CourseId = '{courseId}' AND Type = '{type}' AND Id != '{assessId}'");
            if (currentAssessments.Count > 0)
            {
                exists = true;
            }
            return exists;
        }
        //Assessment basic validation - Name, StartDate, EndDate not null
        public static bool assessmentBasicValidation(string name, DateTime start, DateTime end)
        {
            bool valid = false;
            if (String.IsNullOrEmpty(name))
            {
                valid = true;

            }
            return valid;
        }
        //Check if Objective and Performance Assessments already exist
        public static async Task<bool> assessmentMaxValidation(int courseId)
        {
            bool bothAssessmentsExist = false;
            var currentAssessments = await db.QueryAsync<Assessment>($"SELECT * FROM Assessments WHERE (CourseId = '{courseId}') AND (Type LIKE '%Perf%' OR Type LIKE '%Obj%')");
            if (currentAssessments.Count == 2)
            {
                bothAssessmentsExist = true;
            }
            return bothAssessmentsExist;
        }

        //Course Validations------
        //Basic Course Validation for non-null strings
        public static bool courseBasicValidation(string name, string instN, string instP, string instE)
        {
            bool valid = false;
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(instN) || String.IsNullOrEmpty(instP) || String.IsNullOrEmpty(instE))
            {
                valid = true;
            }
            return valid;
        }
        //Check for max (6) courses and deny creating a new one
        public static async Task<bool> courseMaxValidation(int termId)
        {
            bool isMax = false;
            var courseCount = await db.QueryAsync<Course>($"SELECT * FROM Courses WHERE TermId = '{termId}'");
            if (courseCount.Count >= 6)
            {
                isMax = true;
            }
            return isMax;
        }
        //Course Instructor Email validation
        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        //Term Validations------
        //Term Basic Validation for non-null strings
        public static bool termBasicValidation(string name, DateTime start, DateTime end)
        {
            bool valid = false;
            if (String.IsNullOrEmpty(name) || start.Date == null || end.Date == null)
            {
                valid = true;
            }
            return valid;
        }
    }
}
