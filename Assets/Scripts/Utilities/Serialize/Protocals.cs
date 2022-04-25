using ProtoBuf;
using System.Collections.Generic;

public enum MsgCmd
{
    HandShake,
    CoefficientChg,
    BlendShapesChg,
}

[ProtoContract]
public class HandShake
{
    [ProtoMember(1)]
    public float clientTime;
}

[ProtoContract]
public class CoefficientChg
{
    [ProtoMember(1)]
    public int location;
    [ProtoMember(2)]
    public float coefficient;
    [ProtoMember(3)]
    public float time;
}

[ProtoContract]
public class BlendShapesChg
{
    [ProtoMember(1)]
    public float time;
    [ProtoMember(2, OverwriteList = true)]
    public float[] blendShapes = new float[52];
}