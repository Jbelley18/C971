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
    public partial class AssessmentEdit : ContentPage
    {

        public static Model.Assessment SelectedAssessment;

        public AssessmentEdit()
        {
            InitializeComponent();
        }

        public AssessmentEdit(Model.Assessment assess)
        {
            InitializeComponent();
            SelectedAssessment = assess;
        }
        protected override async void OnAppearing()
        {
            nameEntry.Text = SelectedAssessment.Name;
            startDate.Date = SelectedAssessment.StartDate;
            endDate.Date = SelectedAssessment.EndDate;
            type.SelectedItem = SelectedAssessment.Type;
            notification.IsToggled = SelectedAssessment.NotificationOn;

            base.OnAppearing();
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            SelectedAssessment.Name = nameEntry.Text;
            SelectedAssessment.StartDate = startDate.Date;
            SelectedAssessment.EndDate = endDate.Date;
            SelectedAssessment.Type = type.SelectedItem.ToString();
            SelectedAssessment.NotificationOn = notification.IsToggled;

            //validate no type conflicts with new assessment
            string assesstype = type.SelectedItem.ToString();
            int courseid = SelectedAssessment.CourseId;
            int assessId = SelectedAssessment.Id;
            var typeExists = await Services.Database.assessmentEditTypeExists(assesstype, courseid, assessId);
            if (typeExists)
            {
                DisplayAlert($"Alert", "A " + assesstype + " already exists for this Course", "Ok");
            }
            else if (Services.Database.assessmentBasicValidation(SelectedAssessment.Name, SelectedAssessment.StartDate.Date, SelectedAssessment.EndDate.Date))
            {
                DisplayAlert($"Alert", "Missing Name, Start Date, or End Date", "Ok");
            }
            else if (!Services.Database.checkDate(SelectedAssessment.StartDate, SelectedAssessment.EndDate))
            {
                DisplayAlert($"Alert", "Start date must be before End date", "Ok");
            }
            else
            {
            await Services.Database.db.UpdateAsync(SelectedAssessment);
            await Navigation.PopAsync();
            }

        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            await Services.Database.RemoveAssessment(SelectedAssessment.Id);
            await Navigation.PopAsync();
        }
    }
}