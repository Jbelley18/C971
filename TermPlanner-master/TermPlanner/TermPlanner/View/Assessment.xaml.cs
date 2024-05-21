using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TermPlanner.Model;

namespace TermPlanner.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Assessment : ContentPage
    {

        public static int SelectedCourseId;
        public static Course SelectedCourse;

        public Assessment()
        {
            InitializeComponent();
        }

        public Assessment(Course course)
        {
            InitializeComponent();
            SelectedCourseId = course.Id;
            SelectedCourse = course;
        }

        protected override async void OnAppearing()
        {
            var assessmentList = await Services.Database.GetAssessementList(SelectedCourse.Id);
            assessmentListView.ItemsSource = assessmentList;
            base.OnAppearing();
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            if (await Services.Database.assessmentMaxValidation(SelectedCourseId))
            {
                DisplayAlert($"Alert", "Objective and Performance assessments already exist. Delete or edit an existing assessment", "Ok");
            }
            else
            {
            await Navigation.PushAsync(new AssessmentAdd(SelectedCourse));
            }
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void assessmentListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var assessment = (Model.Assessment)e.Item;
            await Navigation.PushAsync(new AssessmentEdit(assessment));
        }
    }
}