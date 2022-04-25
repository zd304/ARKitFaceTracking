using System;
using System.Collections.Generic;

public class NetStreamBuffer
{
    /// <summary>
    /// 数据buffer
    /// </summary>
    public byte[] Buffer
    {
        get;
        private set;
    }

    /// <summary>
    /// Buffer大小
    /// </summary>
    public int Size
    {
        get { return Buffer.Length; }
    }

    public int WritableSize
    {
        get { return Buffer.Length - Position; }
    }

    /// <summary>
    /// 当前数据大小索引值
    /// </summary>
    public int Position
    {
        set;
        get;
    }

    /// <summary>
    /// 已经读取的数据大小
    /// </summary>
    int readSize;
    /// <summary>
    /// 还没读取的数据大小
    /// </summary>
    int DataSize
    {
        get { return Position - readSize; }
    }

    /// <summary>
    /// 当前数据包的大小
    /// </summary>
    int PacketSize
    {
        get
        {
            int headLength = sizeof(int) + sizeof(ushort) * 2;
            ushort bodyLength = BitConverter.ToUInt16(Buffer, readSize);
            //if (!EndianHelper.IsBigEndian())
            //{
            //    bodyLength = EndianHelper.ReverseEndian(bodyLength);
            //}
            if (bodyLength > Buffer.Length ||
                Buffer.Length < headLength)
            {
                throw new Exception("PacketSize: " + (bodyLength + headLength));
            }
            return bodyLength + headLength;
        }
    }

    public NetStreamBuffer(int size = 204800)
    {
        Buffer = new byte[size];
        Position = 0;
        readSize = 0;
    }

    //返回实际push大小
    public int Push(byte[] data, int len)
    {
        if (WritableSize < len)
        {
            len = WritableSize;
        }
        System.Buffer.BlockCopy(data, 0, Buffer, Position, len);
        Position += len;
        return len;
    }

    public void Pop(out byte[] data)
    {
        int ps = PacketSize;
        data = new byte[ps]; // GC待优化
        System.Buffer.BlockCopy(Buffer, readSize, data, 0, ps);
        readSize += ps;
    }

    /// <summary>
    /// 检查是否收到完整包
    /// </summary>
    /// <returns></returns>
    public bool Check()
    {
        bool ret = DataSize >= PacketSize;
        if (!ret)
        {
            Position -= readSize;
            if (Position > 0)
            {
                System.Buffer.BlockCopy(Buffer, readSize, Buffer, 0, Position);
            }
            readSize = 0;
        }
        return ret;
    }
}