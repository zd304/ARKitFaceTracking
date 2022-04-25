using System;
using System.Net.Sockets;
using ServerMsg;

public class RemoteClient
{
    public int id;
    public TcpClient client;
    public NetworkStream networkStream;

    //private NetStreamBuffer recvBuffer = new NetStreamBuffer();
    private byte[] recvBuffer = new byte[204800];

    private AsyncCallback onRecv;

    private FaceTrackingServer server;

    public RemoteClient(FaceTrackingServer svr)
    {
        onRecv = OnRecieve;
        server = svr;
    }

    public void BeginRecieve()
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

    private static void OnRecieve(IAsyncResult r)
    {
        RemoteClient client = r.AsyncState as RemoteClient;
        try
        {
            if (client.networkStream == null)
            {
                return;
            }

            int readSize = client.networkStream.EndRead(r);
            //UnityEngine.Debug.Log("<><><> 接到一个" + readSize + "大小的包");
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

                msg.clientID = client.id;

                client.server.PutNetMessage(MsgCode.Recieve, msg);
            }

            client.BeginRecieve();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Remote computer shutdown: " + e.Message);

            DisconnectMessage msg = new DisconnectMessage()
            {
                clientID = client.id
            };

            client.server.PutNetMessage(MsgCode.Disconnect, msg);
        }
    }

    public void Disconnect()
    {
        try
        {
            if (client == null || client.Client == null)
            {
                return;
            }
            if (client.Client.Connected)
            {
                client.Client.Shutdown(SocketShutdown.Both);
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("fail to disconnect：" + e.Message);
        }
        finally
        {
            if (networkStream != null)
            {
                networkStream.Dispose();
                networkStream = null;
            }
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }
    }

    private void Send(byte[] buffer, int length)
    {
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
            UnityEngine.Debug.LogError("fail to send：" + e.Message);
            Disconnect();
        }
    }

    public void Send<T>(MsgCmd cmd, T msg)
    {
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
}