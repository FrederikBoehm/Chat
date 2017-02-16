using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class Client
    {
        private const string IP = "192.168.178.37";
        private const int PORT = 8000;
        private const int BUFFER = 100;
        private static Socket server;
        private static string receive = "";
        private static Thread t_rec;
        private static Thread t_send;

        static void Main(string[] args)
        {
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEo = new IPEndPoint(IPAddress.Parse(IP), PORT);
                server.Connect(ipEo);

                ThreadStart ts_rec = new ThreadStart(Rec);
                t_rec = new Thread(ts_rec);
                t_rec.Start();

                ThreadStart ts_send = new ThreadStart(Send);
                t_send = new Thread(ts_send);
                t_send.Start();
                

                /*
                while (true)
                {
                    byte[] sendbuffer = new byte[1024];
                    string message = Console.ReadLine();
                    sendbuffer = Encoding.ASCII.GetBytes(message);
                    server.Send(sendbuffer, 0, sendbuffer.Length, SocketFlags.None);

                    string receive = "";

                    while (receive == "")
                    {
                        byte[] rcvBuffer = new byte[1024];
                        server.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None);
                        receive = Encoding.ASCII.GetString(rcvBuffer);

                        if (receive != "")
                        {
                            Console.WriteLine(receive);
                        }
                    }
                }
                */
                while(true)
                {
                    Thread.Sleep(500);
                }

            }
            catch
            {
                Console.WriteLine("Verbindung nicht möglich.");
            }
            Console.ReadLine();
            
        }

        static void Rec()
        {
            while(true)
            {
                _RecM();
                Thread.Sleep(5);
            }
        }

        static void _RecM()
        {
            Console.WriteLine("Receive...");
            byte[] rcvBuffer = new byte[BUFFER];
            int rcvdBytes = 0;
            rcvdBytes = server.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None);

            t_rec.Priority = ThreadPriority.Highest;
            t_send.Priority = ThreadPriority.Lowest;

            if(rcvdBytes > 0)
            {
                receive = Encoding.ASCII.GetString(rcvBuffer);
                WriteMessage();
            }

            t_rec.Priority = ThreadPriority.Normal;
            t_send.Priority = ThreadPriority.Normal;
        }

        static void WriteMessage()
        {
            string text = receive.Normalize();
            text = text.Trim();
            Console.WriteLine(text);
            receive = "";
        }

        static void Send()
        {
            while(true)
            {
                _SendM();
                Thread.Sleep(5);
            }
        }

        static void _SendM()
        {
            Console.WriteLine("Send...");
            byte[] sendbuffer = new byte[BUFFER];
            string message = Console.ReadLine();

            t_send.Priority = ThreadPriority.Highest;
            t_rec.Priority = ThreadPriority.Lowest;

            sendbuffer = Encoding.ASCII.GetBytes(message);
            server.Send(sendbuffer, 0, sendbuffer.Length, SocketFlags.None);

            t_send.Priority = ThreadPriority.Normal;
            t_rec.Priority = ThreadPriority.Normal;
        }
        
    }
}
