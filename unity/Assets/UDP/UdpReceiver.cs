using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace g12
{
    public class UdpReceiver : MonoBehaviour
    {
        public int Port;
        Socket s;
        private Queue incoming = Queue.Synchronized(new Queue());

        public delegate void dgOnUdpMessage(UdpMessage msg);
        public dgOnUdpMessage onUdpMessage;

        // Update is called once per frame
        public void Update()
        {
            //Debug.Log(incoming.Count);
            while (incoming.Count > 0)
            {
                UdpMessage x = (UdpMessage)incoming.Dequeue();
                if (x != null)
                {
                    onUdpMessage?.Invoke(x);
                }
            }
        }

        private Thread thread = null;

        public void Run()
        {
            thread = new Thread(ThreadLoop);
            thread.IsBackground = true;
            thread.Start();
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

//         private float[] ConvertByteArrayToFloat(byte[] bytes, int len)
//         {
//             if (len % 4 != 0) throw new ArgumentException();
// 
//             float[] floats = new float[len / 4];
//             for (int i = 0; i < floats.Length; i++)
//             {
//                 floats[i] = BitConverter.ToSingle(bytes, i * 4);
//             }
// 
//             return floats;
//         }

        private bool bQuit;
        private void ThreadLoop()
        {
            Debug.Log("UDP Thread start...");
            bQuit = false;
            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            if (s == null)
            {
                Debug.Log("Cant create UDP broadcast receiver socket");
                return;
            }

            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            IPEndPoint iep = new IPEndPoint(IPAddress.Any, Port);
            EndPoint sender = new IPEndPoint(IPAddress.Any, Port);
            s.Bind(iep);

            while (!bQuit)
            {
                try
                {
                    byte[] buffer = new byte[1500];
                    int recv = s.ReceiveFrom(buffer, SocketFlags.None, ref sender);
                    MemoryStream MS = new MemoryStream(buffer, 0, recv);
                    BinaryReader BR = new BinaryReader(MS);
                    UdpMessage msg = UdpMessage.decode(BR);
                    incoming.Enqueue(msg);
                }
                catch (SocketException e)
                {
                    Debug.Log("Error: " + e.Message);
                    break;
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
