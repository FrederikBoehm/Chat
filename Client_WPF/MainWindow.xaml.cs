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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client_WPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    
        
    public partial class MainWindow : Window
    {
        
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Application.Current.Shutdown();

            //Environment.Exit(0);

            try
            {
                Page1.GetRecThread.Abort();
                Page1.GetIniThread.Abort();
                Page1.GetSocket.Close();

                for (int i = App.Current.Windows.Count - 1; i >= 1; i--)
                {
                    App.Current.Windows[i].Close();
                }
            }
            catch
            {

            }
            

        }
        
    }
}
