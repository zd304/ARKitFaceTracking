using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMsg
{
    public class ConnectMessage : NetMessage
    {
        public RemoteClient client;

        public ConnectMessage() : base(MsgCode.Connect) { }
    }

    public class RecieveMessage : NetMessage
    {
        public ushort length;
        public int cmd;
        public ushort serializeID;
        public int clientID;

        public byte[] packet;

        public RecieveMessage() : base(MsgCode.Recieve) { }
    }

    public class DisconnectMessage : NetMessage
    {
        public int clientID;

        public DisconnectMessage() : base(MsgCode.Disconnect) { }
    }
}