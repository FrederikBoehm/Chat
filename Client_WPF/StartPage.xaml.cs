using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_WPF
{
    /// <summary>
    /// Interaktionslogik für StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        static string name = "";

        public StartPage()
        {
            InitializeComponent();
            Name.Focus();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {

            name = Name.Text;
            if (name.Trim().Length == 0)
            {
                MessageBox.Show("Der eingegeben Name ist zu kurz");
            }
            else
            {
                this.NavigationService.Navigate(new Uri("Page1.xaml", UriKind.Relative));
            }
        }

        public static string GetName
        {
            get
            {
                return name;
            }
        }

        private void Name_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                name = Name.Text;
                if (name.Trim().Length == 0)
                {
                    MessageBox.Show("Der eingegeben Name ist zu kurz");
                }
                else
                {
                    this.NavigationService.Navigate(new Uri("Page1.xaml", UriKind.Relative));
                }
            }
        }
    }
}
