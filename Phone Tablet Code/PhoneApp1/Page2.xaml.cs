using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PhoneApp1
{
    public partial class Page2 : PhoneApplicationPage
    {
        RadioButton option = null;
        public Page2()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string msg = "";

            if (NavigationContext.QueryString.TryGetValue("msg", out msg))

                Data.Text = msg;
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            option = sender as RadioButton;

        }
        private void passParam_Click(object sender, RoutedEventArgs e)
        {

            if (option.Name == "r1")
            {
                Data.Text += "Excellent\',\'";
            }
            else if (option.Name == "r2")
            {
                Data.Text += "Good\',\'";
            }
            else if (option.Name == "r3")
            {
                Data.Text += "Satisfactory\',\'";
            }
            else if (option.Name == "r4")
            {
                Data.Text += "Unsatisfactory\',\'";
            }
            else if (option.Name == "r5")
            {
                Data.Text += "Poor\',\'";
            }

            NavigationService.Navigate(new Uri("/Page3.xaml?msg=" + Data.Text, UriKind.Relative));
        }

    }
}