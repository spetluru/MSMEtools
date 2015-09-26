using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NirvanaApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page3 : Page
    {
        RadioButton option = null;
        public Page3()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string name = (string)e.Parameter;
            Data.Text = name;
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

            this.Frame.Navigate(typeof(Page4), Data.Text);
        }
    }
}
