using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using TermPlanner.Model;
using Plugin.LocalNotifications;

namespace TermPlanner
{
    public partial class MainPage : ContentPage
    {

        private bool isStartup = true;
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await Services.Database.CreateDb();
            var terms = await Services.Database.GetTerms();
            if (isStartup && !terms.Any())
            {
                await Services.Database.CreateTables();
                await FireAlerts();
                isStartup = false;
            }
            else if (isStartup)
            {
                await FireAlerts();
                isStartup = false;
            }
            RefreshPage();
            base.OnAppearing();
        }

        private async Task FireAlerts()
        {
            var db = Services.Database.db;
            string notices = "";
            var today = DateTime.Today.ToString("MM/dd/yyyy");
            if (isStartup)
            {
                var terms = await Services.Database.GetTerms();
                foreach (Term term in terms)
                {
                    var courses = await Services.Database.GetCourseList(term.Id);
                    foreach (Course course in courses)
                    {
                        var start = course.StartDate.ToString("MM/dd/yyyy");
                        var end = course.EndDate.ToString("MM/dd/yyyy");
                        if (course.NotificationOn && today == start)
                        {
                            //notices += $"{course.Name} (Course Starting)\n";
                            //notices += $"{course.Name} starts today!\n";
                            notices += $"{course.Name} starts on {course.StartDate.ToString("MM/dd/yyyy")}\n";
                        }
                        if (course.NotificationOn && today == end)
                        {
                            //notices += $"{course.Name} (Course Ending)\n";
                            //notices += $"{course.Name} ends today!\n";
                            notices += $"{course.Name} ends on {course.EndDate.ToString("MM/dd/yyyy")}\n";
                        }

                        var assessments = await Services.Database.GetAssessementList(course.Id);
                        foreach (Assessment assessment in assessments)
                        {
                            var assessStart = assessment.StartDate.ToString("MM/dd/yyyy");
                            var assessEnd = assessment.EndDate.ToString("MM/dd/yyyy");
                            if (assessment.NotificationOn && today == assessStart)
                            {
                                //notices += $"{assessment.Name} (Assessment Starting)\n";
                                //notices += $"{assessment.Name} starts today!\n";
                                notices += $"{assessment.Name} starts on {assessment.StartDate.ToString("MM/dd/yyyy")}\n";
                            }
                            if (assessment.NotificationOn && today == assessEnd)
                            {
                                //notices += $"{assessment.Name} (Assessment Ending)\n";
                                //notices += $"{assessment.Name} ends today!\n";
                                notices += $"{assessment.Name} ends on {assessment.EndDate.ToString("MM/dd/yyyy")}\n";
                            }
                        }
                    }
                }
                if(!String.IsNullOrEmpty(notices))
                {
                    CrossLocalNotifications.Current.Show("Today's Notices", $"{notices}");
                }
            }
            isStartup = false;
        }

        private async void RefreshPage()
        {
            termListView.ItemsSource = await Services.Database.GetTerms();
        }

        private async void termListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var term = (Term)e.Item;
            await Navigation.PushAsync(new View.TermDetails(term));
        }

        private async void AddTerm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new View.TermAdd());
        }
    }
}
