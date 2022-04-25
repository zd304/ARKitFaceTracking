using System;
using System.Collections.Generic;
using ClientMsg;

public class ClientNetMessageHandler : NetMessageHandler
{
    public static void OnHandleConnectMsg(NetMessage msg, object client)
    {
        UnityEngine.Debug.Log("Success to connect to server!");

        ConnectMessage connectMsg = msg as ConnectMessage;

        connectMsg.client.OnSuccessConnect();
    }

    public static void OnHandleRecieveMsg(NetMessage msg, object client)
    {
        UnityEngine.Debug.Log("Success to recive from server!");

        RecieveMessage connectMsg = msg as RecieveMessage;

        connectMsg.client.OnRecieveFromClient(connectMsg.cmd, connectMsg.packet);
    }

    public static void OnHandleDisconnectMsg(NetMessage msg, object client)
    {
        UnityEngine.Debug.Log("Disconnect to server!");

        ConnectMessage connectMsg = msg as ConnectMessage;

        connectMsg.client.Disconnect();
    }

    public override void Start()
    {
        handlers[MsgCode.ClientConnect] = OnHandleConnectMsg;
        handlers[MsgCode.ClientRecieve] = OnHandleRecieveMsg;
        handlers[MsgCode.ClientDisconnect] = OnHandleDisconnectMsg;
    }
}