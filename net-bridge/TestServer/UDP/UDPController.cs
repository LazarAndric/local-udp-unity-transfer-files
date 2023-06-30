using System.Collections;
using System.Collections.Generic;

namespace g12
{
    public class UDPController
    {
        public byte[] testMsg;

        public static g12.UdpReceiver receiver;
        public static g12.UdpSender sender;

        public UDPController()
        {
            sender = new UdpSender("127.0.0.1",8001);
            sender.Run();
            receiver = new UdpReceiver(8000);
            receiver.onUdpMessage += onUdpMessage;
            receiver.Run();
        }

        public void Update()
        {
            receiver.Update();
        }
        void onUdpMessage(g12.UdpMessage msg)
        {
            sender.sendUdpMessage(msg);
        }

        public void OnDisable()
        {
            sender.Stop();
            receiver.Stop();
        }
    }
}