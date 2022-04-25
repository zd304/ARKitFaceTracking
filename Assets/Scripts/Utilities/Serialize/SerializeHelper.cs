using System;
using System.IO;
using ProtoBuf;

public static class SerializeHelper
{
    public static byte[] Serialize<T>(T instance)
    {
        byte[] bytes = null;
        using (var ms = new MemoryStream())
        {
            try
            {
                Serializer.Serialize<T>(ms, instance);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
                return new byte[0];
            }
            bytes = new byte[ms.Position]; // GC待优化
            var fullBytes = ms.GetBuffer();
            Buffer.BlockCopy(fullBytes, 0, bytes, 0, (int)ms.Position);
        }
        return bytes;
    }

    public static byte[] Serialize(Type type, object instance)
    {
        byte[] bytes = null;
        using (var ms = new MemoryStream())
        {
            try
            {
                Serializer.NonGeneric.Serialize(ms, instance);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
                return new byte[0];
            }
            bytes = new byte[ms.Position]; // GC待优化
            var fullBytes = ms.GetBuffer();
            Buffer.BlockCopy(fullBytes, 0, bytes, 0, (int)ms.Position);
        }
        return bytes;
    }

    public static T Deserialize<T>(byte[] bytes)
    {
        using (var ms = new MemoryStream(bytes))
        {
            T t;
            try
            {
                t = Serializer.Deserialize<T>(ms);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
                return default(T);
            }
            return t;
        }
    }

    public static object Deserialize(System.Type T, byte[] bytes)
    {
        using (var ms = new MemoryStream(bytes))
        {
            object t;
            try
            {
                t = Serializer.Deserialize(T, ms);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
                return /*(default(T)*/null;
            }
            return t;
        }
    }
}