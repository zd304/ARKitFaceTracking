using System;

namespace ClientMsg
{
    public class ConnectMessage : NetMessage
    {
        public FaceTrackingClinet client;

        public ConnectMessage() : base(MsgCode.ClientConnect) { }
    }

    public class RecieveMessage : NetMessage
    {
        public ushort length;
        public int cmd;
        public ushort serializeID;

        public byte[] packet;

        public FaceTrackingClinet client;

        public RecieveMessage() : base(MsgCode.Recieve) { }
    }

    public class DisonnectMessage : NetMessage
    {
        public FaceTrackingClinet client;

        public DisonnectMessage() : base(MsgCode.ClientDisconnect) { }
    }
}