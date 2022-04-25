using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MsgCode
{
    None,

    ServerCode = 1001,
    Connect,
    Recieve,
    Disconnect,

    ClientCode = 5001,
    ClientConnect,
    ClientRecieve,
    ClientDisconnect,
}

public class NetMessage
{
    public MsgCode code;

    protected NetMessage(MsgCode code) { this.code = code; }
}