using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.Win32.SafeHandles;
using System.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using g12;

public class TestServer : MonoBehaviour
{
    public string path;
    public string savingpath;
    public int chars_len = 2048;
    int temp_len = 0;
    byte[] buffer = new byte[2048];
    byte[] temp_buffer = new byte[2048];
    public string Label_text = "";
    public bool isRead;
    public bool isCreate;
    private void Start()
    {
        temp_buffer = new byte[chars_len];
        if (isRead && isCreate) ReadCreate();
        else if (isRead) Read();
        else if (isCreate) UDPController.receiver.onUdpMessage += create;
    }
    private void ReadCreate()=> StartCoroutine(read(Create));
    private void Create() => StartCoroutine(create());
    private void Read() => StartCoroutine(read(null));
    IEnumerator read(Action onDone)
    {
        using (Stream source = File.OpenRead(path))
        {
            int len = (int)source.Length;
            buffer = new byte[len];
            int bytesRead = 0;
            while ((bytesRead = source.Read(temp_buffer, 0, temp_buffer.Length)) > 0)
            {
                UDPController.sender.sendUdpMessage(new UdpMessage(bytesRead, temp_buffer));
                //for (int i = 0; i < bytesRead; i++)
                //{
                //    buffer[temp_len + i] = temp_buffer[i];
                //}
                temp_len += bytesRead;
                Label_text = "Uploading: " + ((temp_len / (float)len) * 100) + "%";
                yield return null;
            }
            Debug.Log("Done uploading " + temp_len + "/" + len);
        }
        onDone?.Invoke();
        yield return null;
    }
    IEnumerator create()
    {
        // Create() creates a file at pathName 
        //FileStream fs = File.Create(savingpath);
        using (FileStream fs = File.Create(savingpath, buffer.Length))
        {
            int len = 0;
            while (len < buffer.Length)
            {
                int temp_chars = Mathf.Min(chars_len, buffer.Length - len);
                byte[] temp_arr = buffer.Skip(len).Take(temp_chars).ToArray();
                fs.Write(temp_arr, 0, temp_arr.Length);
                len += temp_chars;
                Label_text = "Downloading: " + (len / (float)buffer.Length) * 100 + "%";
                Debug.Log(temp_chars);
                yield return null;
            }
            Debug.Log("Done downloading " + len + "/" + buffer.Length);
        }
    }
    private void create(UdpMessage udpMessage)
    {
        byte[] arr = udpMessage.dataArr;
        using (FileStream fs = File.Create(savingpath, buffer.Length))
        {
            fs.Write(arr, 0, arr.Length);
            //Label_text = "Downloading: " + (len / (float)buffer.Length) * 100 + "%";
            //Debug.Log("Done downloading " + len + "/" + buffer.Length);
        }
    }
}

