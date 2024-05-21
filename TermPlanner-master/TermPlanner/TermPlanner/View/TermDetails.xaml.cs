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
    public partial class TermDetails : ContentPage
    {
        public static Term SelectedTerm;

        public TermDetails()
        {
            InitializeComponent();
        }
        public TermDetails(Term term)
        {
            InitializeComponent();
            SelectedTerm = term;
        }

        protected override async void OnAppearing()
        {
            termName.Text = SelectedTerm.Name;
            termStartDate.Text = "Start: " + SelectedTerm.StartDate.ToString("MMMM dd, yyyy");
            termEndDate.Text = "End: " + SelectedTerm.EndDate.ToString("MMMM dd, yyyy");
            var courseList = await Services.Database.GetCourseList(SelectedTerm.Id);
            courseListView.ItemsSource = courseList;
            base.OnAppearing();
        }

        private async void DeleteTerm_Clicked(object sender, EventArgs e)
        {
            var id = SelectedTerm.Id;
            await Services.Database.RemoveTerm(id);
            await Navigation.PopAsync();
        }

        private async void EditTerm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TermEdit(SelectedTerm));

        }

        private async void AddCourse_Clicked(object sender, EventArgs e)
        {
            if (await Services.Database.courseMaxValidation(SelectedTerm.Id))
            {
                DisplayAlert($"Alert", "A Term can only have 6 courses.", "Ok");
            }
            else
            {
                await Navigation.PushAsync(new CourseAdd(SelectedTerm));
            }
        }

        private async void courseListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var course = (Course)e.Item;
            await Navigation.PushAsync(new View.CourseDetails(course));
        }
    }
}