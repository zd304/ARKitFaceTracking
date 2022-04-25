using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using ClientMsg;
using System.Collections.Generic;

public class FaceTrackingClinet : MonoBehaviour
{
    public string ip;
    public int port;

    private TcpClient tcp;
    private NetworkStream networkStream;

    private byte[] recvBuffer = new byte[204800];

    private AsyncCallback onRecv;
    private AsyncCallback onSend;

    NetMessageSyncProcessor netMessageSyncProcessor;

    public delegate void MessageHandler(MsgCmd cmd, byte[] data, FaceTrackingClinet client);
    private Dictionary<MsgCmd, MessageHandler> handlers = new Dictionary<MsgCmd, MessageHandler>();

    public bool IsConnect
    {
        private set;
        get;
    }

    void Awake()
    {
        tcp = new TcpClient();
        tcp.NoDelay = true;

        onRecv = OnRecieve;

        netMessageSyncProcessor = new NetMessageSyncProcessor(this, new ClientNetMessageHandler());
    }

    //private void Start()
    //{
    //    Connect(ip, port);
    //}

    public void Connect(string host, int port)
    {
        if (tcp == null)
        {
            return;
        }
        try
        {
            IPAddress address = IPAddress.Parse(host);
            var result = tcp.BeginConnect(address, port, OnConnect, this);
        }
        catch (Exception e)
        {
            Debug.LogError("tcp fail to connect: " + e.Message);
        }
    }

    private void BeginRecieve()
    {
        try
        {
            networkStream.BeginRead(recvBuffer, 0, recvBuffer.Length, this.onRecv, this);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("BeginRecieve() failed!\n" + e.Message);
        }
    }

    public void OnSuccessConnect()
    {
        IsConnect = true;

        networkStream = tcp.GetStream();

        BeginRecieve();
    }

    public void Disconnect()
    {
        IsConnect = false;
        try
        {
            if (tcp == null || tcp.Client == null)
            {
                return;
            }
            if (tcp.Client.Connected)
            {
                tcp.Client.Shutdown(SocketShutdown.Both);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("fail to disconnect：" + e.Message);
        }
        finally
        {
            if (networkStream != null)
            {
                networkStream.Dispose();
                networkStream = null;
            }
            if (tcp != null)
            {
                tcp.Close();
                tcp = null;
            }
        }
    }

    private void Send(byte[] buffer, int length)
    {
        if (!IsConnect)
        {
            return;
        }
        try
        {
            if (length < 0) length = buffer.Length;
            if (networkStream != null)
            {
                networkStream.Write(buffer, 0, length);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("fail to send：" + e.Message);
            Disconnect();
        }
    }

    public void Send<T>(MsgCmd cmd, T msg)
    {
        if (!IsConnect)
        {
            return;
        }

        byte[] msgData = SerializeHelper.Serialize<T>(msg);

        int headLength = sizeof(int) + sizeof(ushort) * 2;
        ushort length = (ushort)(headLength + msgData.Length);

        byte[] lengthData = System.BitConverter.GetBytes(length);
        byte[] cmdData = System.BitConverter.GetBytes((int)cmd);
        byte[] serializeData = System.BitConverter.GetBytes((ushort)0);

        byte[] data = new byte[length];
        int pos = 0;

        System.Buffer.BlockCopy(lengthData, 0, data, pos, lengthData.Length);
        pos += lengthData.Length;
        System.Buffer.BlockCopy(cmdData, 0, data, pos, cmdData.Length);
        pos += cmdData.Length;
        System.Buffer.BlockCopy(serializeData, 0, data, pos, serializeData.Length);
        pos += serializeData.Length;
        System.Buffer.BlockCopy(msgData, 0, data, pos, msgData.Length);

        Send(data, data.Length);
    }

    void Update()
    {
        netMessageSyncProcessor.OnRecvNetMessage();
    }

    private static void OnConnect(IAsyncResult r)
    {
        FaceTrackingClinet client = r.AsyncState as FaceTrackingClinet;
        if (client == null)
        {
            return;
        }
        try
        {
            client.tcp.EndConnect(r);

            if (client.tcp != null
                && client.tcp.Client != null)
            {
                Socket socket = client.tcp.Client;
                if (socket.Connected)
                {
                    ConnectMessage msg = new ConnectMessage()
                    {
                        client = client
                    };

                    client.netMessageSyncProcessor.PutNetMessage(MsgCode.ClientConnect, msg);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("tcp fail to connect: " + e.Message);

            DisonnectMessage msg = new DisonnectMessage();
            msg.client = client;

            client.netMessageSyncProcessor.PutNetMessage(MsgCode.ClientDisconnect, msg);
        }
    }

    private static void OnRecieve(IAsyncResult r)
    {
        FaceTrackingClinet client = r.AsyncState as FaceTrackingClinet;
        try
        {
            if (client.networkStream == null)
            {
                return;
            }

            int readSize = client.networkStream.EndRead(r);
            UnityEngine.Debug.Log("<><><> 接到一个" + readSize + "大小的包");
            if (readSize == 0)
            {
                UnityEngine.Debug.LogError("接到0字节消息，估计是被服务器踢了");
                return;
            }
            {
                byte[] buf = client.recvBuffer;

                RecieveMessage msg = new RecieveMessage();

                int pos = 0;
                msg.length = System.BitConverter.ToUInt16(buf, pos);
                pos += sizeof(ushort);
                msg.cmd = System.BitConverter.ToInt32(buf, pos);
                pos += sizeof(int);
                msg.serializeID = System.BitConverter.ToUInt16(buf, pos);
                pos += sizeof(ushort);

                int len = msg.length - 2 * sizeof(ushort) - sizeof(int);
                msg.packet = new byte[len];
                System.Buffer.BlockCopy(buf, pos, msg.packet, 0, len);
                pos += len;

                client.netMessageSyncProcessor.PutNetMessage(MsgCode.Recieve, msg);
            }

            client.BeginRecieve();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Remote computer shutdown: " + e.Message);

            DisonnectMessage msg = new DisonnectMessage()
            {
                client = client
            };

            client.netMessageSyncProcessor.PutNetMessage(MsgCode.Disconnect, msg);
        }
    }

    public void OnRecieveFromClient(int cmd, byte[] data)
    {
        MsgCmd msgCmd = (MsgCmd)cmd;
        MessageHandler handler;
        if (handlers.TryGetValue(msgCmd, out handler))
        {
            handler(msgCmd, data, this);
        }
    }

    public void RegistHandler(MsgCmd cmd, MessageHandler handler)
    {
        if (handlers.ContainsKey(cmd))
        {
            return;
        }
        handlers.Add(cmd, handler);
    }
}
