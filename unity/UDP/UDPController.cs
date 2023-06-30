using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace g12
{
    public class UDPController : MonoBehaviour
    {
        [Header("Output")]
        public byte[] testMsg;

        [Header("Config")]
        public static g12.UdpReceiver receiver;
        [Header("Config")]
        public static g12.UdpSender sender;

        void Awake()
        {
            if (receiver == null) receiver = GetComponent<UdpReceiver>();
            if (sender == null) sender = GetComponent<UdpSender>();
            if (receiver == null) Debug.LogError("UDPController failed to init, no UdpReceiver component.");
            if (receiver == null) Debug.LogError("UDPController failed to init, no UdpSender component.");

            //ExhibitConfig.instance.onConfigUpdate += onConfigUpdated;
            //onConfigUpdated();

            sender.Run();
            receiver.onUdpMessage += onUdpMessage;
            receiver.Run();
        }


        void onUdpMessage(g12.UdpMessage msg)
        {
            testMsg = msg.dataArr;
        }

        public void OnDisable()
        {
            sender.Stop();
            receiver.Stop();
        }

        void OnApplicationQuit()
        {
            sender.Stop();
            receiver.Stop();
        }
    }
}