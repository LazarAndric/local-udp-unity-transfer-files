using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;

namespace g12
{
    public class UdpSender
    {
        public int Port;
        public string Address;

        public UdpSender(string addr, int port)
        {
            Address = addr;
            Port = port;
        }
        private Queue outgoing = Queue.Synchronized(new Queue());
        public void sendUdpMessage(UdpMessage msg)
        {
            outgoing.Enqueue(msg);
        }

        private Thread thread = null;
        public void Run()
        {
            if (thread == null)
            {
                thread = new Thread(ThreadLoop);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public void Stop()
        {
            if (thread != null)
            {
                bQuit = true;
                s.Close();
                thread.Join();
                thread = null;
            }
        }

        private bool bQuit;
        private Socket s;

        private void ThreadLoop()
        {
            bQuit = false;
            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            if (s == null)
            {
                Console.WriteLine("Cant create UDP broadcast receiver socket");
                return;
            }

            //s.Blocking = false;
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Address), Port);

            while (!bQuit)
            {
                //Thread.Sleep(TimeoutMS);
                if (outgoing.Count > 0)
                {
                    try
                    {
                        UdpMessage msg = (UdpMessage)outgoing.Dequeue();
                        if (msg != null)
                        {
                            byte[] data = msg.encode();
                            Console.WriteLine("Send to " + ip);
                            s.SendTo(data, 0, data.Length, SocketFlags.None, ip);
                        }
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }

            s.Close();
        }

        public void OnDisable()
        {
            Stop();
        }

        void OnApplicationQuit()
        {
            Stop();
        }

    }


}
