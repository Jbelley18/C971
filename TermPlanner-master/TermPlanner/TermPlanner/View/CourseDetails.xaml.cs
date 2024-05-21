using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace TermPlanner.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseDetails : ContentPage
    {
        public Model.Course SelectedCourse;

        public CourseDetails()
        {
            InitializeComponent();
        }
        public CourseDetails(Model.Course course)
        {
            InitializeComponent();
            SelectedCourse = course;
        }

        protected override async void OnAppearing()
        {
            header.Text = SelectedCourse.Name;
            nameEntry.Text = SelectedCourse.Name;
            startDate.Date = SelectedCourse.StartDate;
            endDate.Date = SelectedCourse.EndDate;
            status.SelectedItem = SelectedCourse.Status;
            instructorName.Text = SelectedCourse.InstructorName;
            instructorPhone.Text = SelectedCourse.InstructorPhone;
            instructorEmail.Text = SelectedCourse.InstructorEmail;
            notes.Text = SelectedCourse.Notes;
            notification.IsToggled = SelectedCourse.NotificationOn;

            base.OnAppearing();
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            SelectedCourse.Name = nameEntry.Text;
            SelectedCourse.StartDate = startDate.Date;
            SelectedCourse.EndDate = endDate.Date;
            SelectedCourse.Status = status.SelectedItem.ToString();
            SelectedCourse.InstructorName = instructorName.Text;
            SelectedCourse.InstructorPhone = instructorPhone.Text;
            SelectedCourse.InstructorEmail = instructorEmail.Text;
            SelectedCourse.Notes = notes.Text;
            SelectedCourse.NotificationOn = notification.IsToggled;


            if (Services.Database.courseBasicValidation(SelectedCourse.Name, SelectedCourse.InstructorName, SelectedCourse.InstructorPhone, SelectedCourse.InstructorEmail))
            {
                DisplayAlert($"Alert", "Some fields are missing input", "Ok");
            }
            else if (!Services.Database.checkDate(SelectedCourse.StartDate, SelectedCourse.EndDate))
            {
                DisplayAlert($"Alert", "Start date must be before End date", "Ok");
            }
            else if (!Services.Database.IsValidEmail(SelectedCourse.InstructorEmail))
            {
                DisplayAlert($"Alert", "Enter a valid email address", "Ok");
            }
            else
            {
                await Services.Database.db.UpdateAsync(SelectedCourse);
                await Navigation.PopAsync();
            }
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            await Services.Database.RemoveCourse(SelectedCourse.Id);
            await Navigation.PopAsync();
        }

        private async void AssessmentButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Assessment(SelectedCourse));
        }

        private async void ShareButton_Clicked(object sender, EventArgs e)
        {
            await ShareNote(SelectedCourse.Notes);
        }
        private async void SaveNoteButton_Clicked(object sender, EventArgs e)
        {
            SelectedCourse.Notes = notes.Text;
            await Services.Database.db.UpdateAsync(SelectedCourse);
        }

        public async Task ShareNote(string note)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = note,
                Title = "Share Text"
            });
        }

    }
}