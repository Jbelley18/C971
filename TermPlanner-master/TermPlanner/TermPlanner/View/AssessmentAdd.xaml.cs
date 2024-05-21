using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TermPlanner.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentAdd : ContentPage
    {

        public static Model.Course SelectedCourse;

        public AssessmentAdd()
        {
            InitializeComponent();
        }

        public AssessmentAdd(Model.Course course)
        {
            InitializeComponent();
            SelectedCourse = course;
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            int assessId = SelectedCourse.Id;
            int courseid = SelectedCourse.Id;
            string name = nameEntry.Text;
            DateTime start = startDate.Date;
            DateTime end = endDate.Date;
            string assesstype = type.SelectedItem.ToString();
            bool notify = notification.IsToggled;

            //validate no type conflicts and no null fields
            var typeExists = await Services.Database.assessmentAddTypeExists(assesstype, courseid);
            if (typeExists)
            {
                DisplayAlert($"Alert", "A " + assesstype + " already exists for this Course", "Ok");
            }
            else if (Services.Database.assessmentBasicValidation(name, start, end))
            {
                DisplayAlert($"Alert", "Missing Name, Start Date, or End Date", "Ok");
            }
            else if (!Services.Database.checkDate(start, end))
            {
                DisplayAlert($"Alert", "Start date must be before End date", "Ok");
            }
            else
            {
                await Services.Database.AddAssessment(courseid, name, start, end, assesstype, notify);
                await Navigation.PopAsync();
            }
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}