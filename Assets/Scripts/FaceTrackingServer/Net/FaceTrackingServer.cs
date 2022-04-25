using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using ServerMsg;

public class FaceTrackingServer : MonoBehaviour
{
    TcpListener tcp;
    int currentLinked;
    Dictionary<int, RemoteClient> clients = new Dictionary<int, RemoteClient>();

    NetMessageSyncProcessor netMessageSyncProcessor;

    public delegate void MessageHandler(MsgCmd cmd, byte[] data, RemoteClient client);
    private Dictionary<MsgCmd, MessageHandler> handlers = new Dictionary<MsgCmd, MessageHandler>();

    void Awake()
    {
        netMessageSyncProcessor = new NetMessageSyncProcessor(this, new ServerNetMessageHandler());
    }

    public void StartServer(string host, int port)
    {
        tcp = new TcpListener(IPAddress.Parse(host), port);
        tcp.Start(100);

        IAsyncResult result = tcp.BeginAcceptTcpClient(new AsyncCallback(OnAcceptTcpClient), this);
    }

    private void OnAcceptTcpClient(IAsyncResult o)
    {
        FaceTrackingServer server = o.AsyncState as FaceTrackingServer;
        if (tcp == null)
        {
            return;
        }

        TcpClient tcpClient = null;
        try
        {
            tcpClient = server.tcp.EndAcceptTcpClient(o);
            System.Threading.Interlocked.Increment(ref currentLinked);
        }
        catch (Exception e)
        {
            Debug.LogError("OnAcceptTcpClient Exception: " + e.Message);
        }

        IAsyncResult result = server.tcp.BeginAcceptTcpClient(new AsyncCallback(OnAcceptTcpClient), server);

        NetworkStream stream = tcpClient.GetStream();

        RemoteClient client = new RemoteClient(server)
        {
            id = currentLinked,
            client = tcpClient,
            networkStream = stream
        };

        netMessageSyncProcessor.PutNetMessage(MsgCode.Connect, new ConnectMessage() { client = client });
    }

    void Update()
    {
        netMessageSyncProcessor.OnRecvNetMessage();
    }

    private void OnApplicationQuit()
    {
        lock (tcp)
        {
            tcp.Stop();
            tcp = null;
        }

        foreach (var p in clients)
        {
            p.Value.Disconnect();
        }
        clients.Clear();
    }

    public void AddClient(RemoteClient client)
    {
        if (clients.ContainsKey(client.id))
        {
            Debug.LogError("have same client id already");
            return;
        }
        clients.Add(client.id, client);
        client.BeginRecieve();
    }

    public RemoteClient GetClientByID(int clientID)
    {
        RemoteClient rst = null;
        if (clients.TryGetValue(clientID, out rst))
        {
            return rst;
        }
        return null;
    }

    public void PutNetMessage(MsgCode code, NetMessage netMsg)
    {
        netMessageSyncProcessor.PutNetMessage(code, netMsg);
    }

    public void OnRecieveFromClient(int clientID, int cmd, byte[] data)
    {
        RemoteClient client = GetClientByID(clientID);
        if (client == null)
        {
            return;
        }

        MsgCmd msgCmd = (MsgCmd)cmd;
        MessageHandler handler;
        if (handlers.TryGetValue(msgCmd, out handler))
        {
            handler(msgCmd, data, client);
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
