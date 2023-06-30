using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

using System.Threading;
public class UDPLIstener : MonoBehaviour
{
    public const int listenPort = 56708;
    UdpClient listener;
    IPEndPoint groupEP;
    void Start()
    {
        listener = new UdpClient(listenPort);
        groupEP = new IPEndPoint(IPAddress.Any, listenPort);
        Thread thread = new Thread(Receive);
        thread.Priority = System.Threading.ThreadPriority.Highest;
        thread.Start();
    }

    private void Receive()
    {
        try
        {
            while (true)
            {
                Debug.Log("Waiting for broadcast");
                byte[] bytes = listener.Receive(ref groupEP);

                Debug.Log($"Received broadcast from {groupEP} :");
                Debug.Log($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");

            }
        }
        catch (SocketException e)
        {
            Debug.Log(e);
        }
    }
    private void OnDestroy()
    {
        listener.Close();
    }
}