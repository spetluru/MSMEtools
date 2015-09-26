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
using NirvanaApp.ServiceReference1;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NirvanaApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page6 : Page
    {
        public Page6()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string name = (string)e.Parameter;
            Data.Text = name;
        }

        private void passParam_Click(object sender, RoutedEventArgs e)
        {

            Data.Text += this.comments.Text.Replace("\'", "").Replace("\"", "") + "\',";
            ServiceReference1.MyServiceClassSoapClient ws = new MyServiceClassSoapClient();
            ws.AddFeedbackAsync(Data.Text);
            this.Frame.Navigate(typeof(Page7), Data.Text);


        }
    }
}
