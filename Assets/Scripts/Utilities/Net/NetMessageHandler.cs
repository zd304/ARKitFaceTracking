using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class NetMessageHandler
{
    public abstract void Start();

    public delegate void MsgHandler(NetMessage msg, object server);
    public Dictionary<MsgCode, MsgHandler> handlers = new Dictionary<MsgCode, MsgHandler>();
}