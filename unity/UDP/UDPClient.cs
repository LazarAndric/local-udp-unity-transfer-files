using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using UnityEngine;
using System.Collections;

public class UDPClient : MonoBehaviour
{
    public int port=8000;
    public string ip = "192.168.0.14";
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        Send("test test test");
    }
    private void Send(string msg)
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        IPAddress targetAddress = IPAddress.Parse(ip);

        byte[] sendbuf = Encoding.ASCII.GetBytes(msg);
        IPEndPoint ep = new IPEndPoint(targetAddress, port);

        s.SendTo(sendbuf, ep); s.Close();

        print("Message sent");
    }
}