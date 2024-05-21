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
    public partial class TermEdit : ContentPage
    {
        public Term SelectedTerm;
        public TermEdit()
        {
            InitializeComponent();
        }
        public TermEdit(Term term)
        {
            InitializeComponent();
            SelectedTerm = term;
        }

        protected override async void OnAppearing()
        {
            nameEntry.Placeholder = SelectedTerm.Name;
            startDate.Date = SelectedTerm.StartDate;
            endDate.Date = SelectedTerm.EndDate;
            base.OnAppearing();
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            SelectedTerm.Name = nameEntry.Text;
            SelectedTerm.StartDate = startDate.Date;
            SelectedTerm.EndDate = endDate.Date;
            if (!Services.Database.checkDate(SelectedTerm.StartDate, SelectedTerm.EndDate))
            {
                DisplayAlert($"Alert", "Start date must be before End date", "Ok");
            }
            else if (Services.Database.termBasicValidation(SelectedTerm.Name, SelectedTerm.StartDate, SelectedTerm.EndDate))
            {
                DisplayAlert($"Alert", "Some fields are missing input", "Ok");
            }
            else
            {
                await Services.Database.db.UpdateAsync(SelectedTerm);
                await Navigation.PopAsync();
            }
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}