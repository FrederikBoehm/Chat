using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Server
    {
        private const int BACKLOG = 5;
        private const int PORT = 8000;
        private static Socket servSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Thread> client_threads = new List<Thread>();
        private static List<Socket> client_sockets = new List<Socket>();
        private static List<string> client_names = new List<string>();
        //private static List<Thread> rec_threads = new List<Thread>();
        //private static List<Thread> _recm_threads = new List<Thread>();
        private const int BUFFERSIZE = 100;
        private static string message = "";
        private static int from = 0;

        static void Main(string[] args)
        {
            CreateServer();
            //AddClient();

            ThreadStart ts = new ThreadStart(Add);
            Thread t = new Thread(ts);
            t.Start();

            ThreadStart ts2 = new ThreadStart(Send);
            Thread t2 = new Thread(ts2);
            t2.Start();

                //_SendM();
            
            

        }

        static void CreateServer()
        {
            IPEndPoint ipEo = new IPEndPoint(IPAddress.Any, 8000);
            servSock.Bind(ipEo);
            servSock.Listen(BACKLOG);
            Console.WriteLine("Server erfolgreich erstellt");
        }

        static void Add()
        {
            while(true)
            {
                AddClient();
                Thread.Sleep(5);
            }
        }
        static void AddClient()
        {
            Console.WriteLine("Adding Client...");

            Socket c = servSock.Accept();
            client_sockets.Add(c);

            client_names.Add(GetName(c));
            SendGreeting(client_names.Count-1);

            ParameterizedThreadStart pts = new ParameterizedThreadStart(Rec);
            Thread t = new Thread(pts);
            //rec_threads.Add(t);
            
            //t.Start();
            client_threads.Add(t);
            client_threads[client_threads.Count-1].Start(client_sockets.Count-1);


            Console.WriteLine("Benutzer beigetreten");
        }

        private static string GetName(Socket c)
        {
            byte[] rcvBuffer = new byte[BUFFERSIZE];
            c.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None);

            string name = Encoding.Unicode.GetString(rcvBuffer);
            name = name.Normalize();
            name = name.Trim('\0');

            return name;
        }

        private static void SendGreeting(int n)
        {
            byte[] sendBuffer = new byte[BUFFERSIZE];
            sendBuffer = Encoding.Unicode.GetBytes("Willkommen auf dem Server " + client_names[n] +"!");
            client_sockets[n].Send(sendBuffer, 0, sendBuffer.Length, SocketFlags.None);
        }

        public static void Rec(object o)
        {
            //ParameterizedThreadStart pts = new ParameterizedThreadStart(_ReceiveM);
            //Thread t = new Thread(pts);
            //_recm_threads.Add(t);
            //_recm_threads[_recm_threads.Count-1].Start(o);
            //t.Start(o);

            while(true)
            {
                _ReceiveM(o);
            }
        }

        public static void _ReceiveM(object o)
        {
            int number = Convert.ToInt32(o);
            try
            {
                //int number = Convert.ToInt32(o);
                Console.WriteLine("Receiving Data from user {0}", number);
                byte[] rcvBuffer = new byte[BUFFERSIZE];
                client_sockets[number].Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None);

                if (message == "")
                {
                    message = Encoding.Unicode.GetString(rcvBuffer);
                    message = message.Normalize();
                    message = message.Trim('\0', '\n', ' ');
                    from = number;
                }
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Console.WriteLine(ex.ToString());
                client_sockets[number].Close();
                client_threads[number].Abort();
            }
        }

        public static void Send()
        {
            while(true)
            {
                if (message != "")
                {
                    _SendM();
                }
                Thread.Sleep(5);
            }
        }

        public static void _SendM()
        {
            Console.WriteLine("Sende an alle außer: {0}", from);
            for (int i = 0; i < client_sockets.Count; i++)
            {
                if (i != from && client_sockets[i].Connected)
                {
                    byte[] sendBuffer = new byte[BUFFERSIZE];
                    string nachricht = client_names[from] + ": " + message;
                    sendBuffer = Encoding.Unicode.GetBytes(nachricht);
                    client_sockets[i].Send(sendBuffer, 0, sendBuffer.Length, SocketFlags.None);
                }
            }

            message = "";
            Console.WriteLine("Senden erfolgreich.");
        }
    }
}
