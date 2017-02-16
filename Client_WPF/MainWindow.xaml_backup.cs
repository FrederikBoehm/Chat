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
        private const string IP = "192.168.178.37";
        private const int PORT = 8000;
        private const int BUFFER = 100;
        private Socket server;
        private string receive = "";
        private Thread t_rec;
        private Thread t_send;

        public MainWindow()
        {
            InitializeComponent();
            Thread abc = new Thread(InitializeClient);
            abc.Start();
            //InitializeClient();
        }

        public void InitializeClient()
        {
            Chatfenster.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                                (ThreadStart)delegate ()
                                                {
                                                    Chatfenster.Text = "Verbindung wird hergestellt..." + "\n";
                                                });

            try
            {
                
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEo = new IPEndPoint(IPAddress.Parse(IP), PORT);
                server.Connect(ipEo);
                
                //ThreadStart ts_rec = new ThreadStart(Rec);
                t_rec = new Thread(Rec);
                t_rec.Start();

                //ThreadStart ts_send = new ThreadStart(Send);
                //t_send = new Thread(Send);
                //t_send.Start();
                
                while (true)
                {
                    Thread.Sleep(500);
                }

            }
            catch
            {
                //Console.WriteLine("Verbindung nicht möglich.");
                Chatfenster.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                                (ThreadStart)delegate ()
                                                {
                                                    Chatfenster.AppendText("Verbindung nicht möglich.");
                                                });
            }
            Console.ReadLine();
        }
        

        void Rec()
        {
            while (true)
            {
                _RecM();
                //Thread.Sleep(5);
            }
        }

        void _RecM()
        {
            //Console.WriteLine("Receive...");
            //Chatfenster.Text = "Receive...";
            byte[] rcvBuffer = new byte[BUFFER];
            int rcvdBytes = 0;
            rcvdBytes = server.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None);

            //t_rec.Priority = ThreadPriority.Highest;
            //t_send.Priority = ThreadPriority.Lowest;

            if (rcvdBytes > 0)
            {
                receive = Encoding.ASCII.GetString(rcvBuffer);
                
                WriteMessage();
            }

            //t_rec.Priority = ThreadPriority.Normal;
            //t_send.Priority = ThreadPriority.Normal;
        }

        void WriteMessage()
        {
            string text = receive.Normalize();
            text = text.Trim('\0');
            //Console.WriteLine(text);

            Chatfenster.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                                (ThreadStart)delegate ()
                                                {
                                                    Chatfenster.AppendText(text + "\n");
                                                });

            receive = "";
        }

        void Send()
        {
            while (true)
            {
                _SendM();
                Thread.Sleep(5);
            }
        }

        void _SendM()
        {
            //Console.WriteLine("Send...");
            //Chatfenster.Text = "Send...";
            byte[] sendbuffer = new byte[BUFFER];
            string message = Console.ReadLine();
            //Chatfenster.Text = message;

            t_send.Priority = ThreadPriority.Highest;
            t_rec.Priority = ThreadPriority.Lowest;

            sendbuffer = Encoding.ASCII.GetBytes(message);
            server.Send(sendbuffer, 0, sendbuffer.Length, SocketFlags.None);

            t_send.Priority = ThreadPriority.Normal;
            t_rec.Priority = ThreadPriority.Normal;
        }

        private void Absenden_Click(object sender, RoutedEventArgs e)
        {
            string message = Eingabefeld.Text;
            byte[] sendbuffer = new byte[BUFFER];

            sendbuffer = Encoding.ASCII.GetBytes(message);
            server.Send(sendbuffer, 0, sendbuffer.Length, SocketFlags.None);
            
        }
    }
}
