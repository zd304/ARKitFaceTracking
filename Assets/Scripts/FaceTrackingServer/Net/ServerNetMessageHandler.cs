using System;
using System.Collections.Generic;
using ServerMsg;

public class ServerNetMessageHandler : NetMessageHandler
{
    public static void OnHandleConnectMsg(NetMessage msg, object server)
    {
        ConnectMessage connectMsg = msg as ConnectMessage;

        FaceTrackingServer svr = server as FaceTrackingServer;

        svr.AddClient(connectMsg.client);
    }

    public static void OnHandleRecieveMsg(NetMessage msg, object server)
    {
        RecieveMessage recvMessage = msg as RecieveMessage;

        FaceTrackingServer svr = server as FaceTrackingServer;

        svr.OnRecieveFromClient(recvMessage.clientID, recvMessage.cmd, recvMessage.packet);
    }

    public static void OnHandleDisconnectMsg(NetMessage msg, object server)
    {
        DisconnectMessage disMsg = msg as DisconnectMessage;

        FaceTrackingServer svr = server as FaceTrackingServer;

        RemoteClient client = svr.GetClientByID(disMsg.clientID);
        if (client == null)
        {
            return;
        }

    }

    ////////////////////////////////////////////////////////////////////////////////

    public override void Start()
    {
        handlers[MsgCode.Connect] = OnHandleConnectMsg;
        handlers[MsgCode.Recieve] = OnHandleRecieveMsg;
        handlers[MsgCode.Disconnect] = OnHandleDisconnectMsg;
    }
}