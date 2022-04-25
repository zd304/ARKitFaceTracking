using System;

public static class EndianHelper
{
    static bool isBigEndian;
    static bool isEndianChecked = false;

    public static bool IsBigEndian()
    {
        if (!isEndianChecked)
        {
            isEndianChecked = true;
            int checkNum = 0x01aa;
            isBigEndian = (checkNum & 0xff) == 0x01;
        }
        return isBigEndian;
    }

    public static int ReverseEndian(int num)
    {
        int rst = (num & 0xff) << 24 |
            (num & 0xff00) << 8 |
            (num >> 8) & 0xff00 |
            (num >> 24) & 0xff;
        return rst;
    }

    public static ushort ReverseEndian(ushort num)
    {
        ushort rst = (ushort)(
            ((int)num & 0xff) << 8 |
            ((int)num >> 8) & 0xff
            );
        return rst;
    }

    public static byte[] ReverseEndian(byte[] data)
    {
        Array.Reverse(data);
        return data;
    }
}