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
    /// Interaktionslogik für Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        private const string IP = "192.168.178.37";
        private const int PORT = 8000;
        private const int BUFFER = 100;
        private static Socket server;
        //private string receive = "";
        //private bool isReceived = false;
        private static Thread t_rec;
        private static Thread ini;
        private Thread t_send;
        private string name;

        public Page1()
        {
            InitializeComponent();
            Eingabefeld.Focus();
            ini = new Thread(InitializeClient);
            ini.Start();
        }

        public void InitializeClient()
        {
            WriteMessage("Verbindung wird hergestellt...");

            try
            {

                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEo = new IPEndPoint(IPAddress.Parse(IP), PORT);
                server.Connect(ipEo);
                WriteMessage("Verbunden.");

                name = StartPage.GetName;
                _SendM(name);

                t_rec = new Thread(Rec);
                t_rec.Start();


                while (true)
                {
                    Thread.Sleep(500);
                }

            }
            catch
            {

                WriteMessage("Verbindung nicht möglich.");
            }
            Console.ReadLine();
        }


        void Rec()
        {
            while (true)
            {
                _RecM();

            }
        }

        void _RecM()
        {


            byte[] rcvBuffer = new byte[BUFFER];
            int rcvdBytes = 0;

            try
            {
                rcvdBytes = server.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None);

            }
            catch (System.Net.Sockets.SocketException ex)
            {
                t_rec.Abort();
                server.Close();
                WriteMessage("Verbindung zum Server verloren...");
            }

            if (rcvdBytes > 0)
            {
                string receive = Encoding.Unicode.GetString(rcvBuffer);

                WriteMessage(receive);
            }

        }

        void WriteMessage(string text)
        {
            text = text.Normalize();
            text = text.Trim('\0', '\n', ' ');

            Chatfenster.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                                (ThreadStart)delegate ()
                                                {
                                                    Chatfenster.AppendText(text + '\n');
                                                });
        }

        void WriteOwnMessage(string text)
        {
            text = text.Normalize();
            text = text.Trim('\0', '\n', ' ');

            if (text.Length > 0)
            {
                Chatfenster.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                                    (ThreadStart)delegate ()
                                                    {
                                                        Chatfenster.AppendText(name + ": " + text + '\n');
                                                    });
            }
        }

        void _SendM(string text)
        {
            byte[] sendbuffer = new byte[BUFFER];

            sendbuffer = Encoding.Unicode.GetBytes(text);
            server.Send(sendbuffer, 0, sendbuffer.Length, SocketFlags.None);

            
        }

        private void Absenden_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = Eingabefeld.Text;
                _SendM(message);

                WriteOwnMessage(message);
                
                Chatfenster.ScrollToLine(Chatfenster.LineCount-1);
                Eingabefeld.Clear();
            }
            catch
            {

            }
        }

        

        private void Eingabefeld_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    string message = Eingabefeld.Text;
                    _SendM(message);

                    WriteOwnMessage(message);
                    
                    Chatfenster.ScrollToLine(Chatfenster.LineCount-1);
                    Eingabefeld.Clear();
                }
            }
            catch
            {

            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("StartPage.xaml", UriKind.Relative));
        }
        public static Thread GetRecThread
        {
            get
            {
                return t_rec;
            }
        }

        public static Thread GetIniThread
        {
            get
            {
                return ini;
            }
        }

        public static Socket GetSocket
        {
            get
            {
                return server;
            }
        }
    }
}
