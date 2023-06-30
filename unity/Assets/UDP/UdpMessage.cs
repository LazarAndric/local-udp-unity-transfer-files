using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.IO;

namespace g12
{
    public class UdpMessage
    {
        public int length;
        public byte[] dataArr;

        public UdpMessage(int length,byte[] dataArr)
        {
            this.length = length;
            this.dataArr = dataArr;
        }


        //theta:146.12
        //distance:00338.00
        //ID: 1

        //146.12:00448.00:1
        public static UdpMessage decode(BinaryReader br)
        {
            int l = br.ReadInt16();
            byte[] a = br.ReadBytes(l);
            return new UdpMessage(l, a);
        }

        public byte[] encode()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                using (BinaryWriter BW = new BinaryWriter(MS))
                {
                    // todo: write stuff here
                    BW.Write(length);
                    BW.Write(dataArr);
                    MS.Flush();
                    byte[] bytes = MS.ToArray();
                    return bytes;
                }
            }        
        }
    }
}
