using System.Collections.Generic;

public class NetMessageSyncProcessor
{
    protected Queue<NetMessage> recvQueue = new Queue<NetMessage>();
    private object server;
    private NetMessageHandler netMessagehandler;

    public NetMessageSyncProcessor(object svr, NetMessageHandler handler)
    {
        server = svr;
        netMessagehandler = handler;
        handler.Start();
    }

    public virtual void PutNetMessage(MsgCode code, NetMessage netMsg)
    {
        netMsg.code = code;

        lock (recvQueue)
        {
            recvQueue.Enqueue(netMsg);
        }
    }

    private NetMessage PeekMessage(out MsgCode code)
    {
        NetMessage msg = null;
        code = MsgCode.None;
        lock (recvQueue)
        {
            if (recvQueue.Count <= 0)
            {
                return null;
            }
            lock (recvQueue)
            {
                msg = recvQueue.Dequeue();
            }
            code = msg.code;
        }
        return msg;
    }

    public void OnRecvNetMessage()
    {
        NetMessage netMessage = null;
        do
        {
            MsgCode rstCode = MsgCode.None;
            netMessage = PeekMessage(out rstCode);
            if (netMessage == null)
            {
                return;
            }
            NetMessageHandler.MsgHandler handler = null;
            if (netMessagehandler.handlers.TryGetValue(rstCode, out handler))
            {
                handler(netMessage, server);
            }
        } while (netMessage != null);
        
    }
}