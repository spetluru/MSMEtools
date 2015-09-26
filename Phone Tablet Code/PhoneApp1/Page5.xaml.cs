using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp1.ServiceReference2;

namespace PhoneApp1
{
    public partial class Page5 : PhoneApplicationPage
    {
        public Page5()
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

        private void passParam_Click(object sender, RoutedEventArgs e)
        {
            Data.Text += this.comments.Text.Replace("\'", "").Replace("\"", "") + "\',";
            ServiceReference2.MyServiceClassSoapClient ws = new MyServiceClassSoapClient();
            ws.AddFeedbackAsync(Data.Text);
            NavigationService.Navigate(new Uri("/Page6.xaml?msg=" + Data.Text, UriKind.Relative));

        }
    }
}