using System.Text.Json;

namespace Messaging;

public static class ByteUtils
{
    public static byte[] ObjectToByteArray(Type dataType, object obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj, dataType);
    }

    public static object ByteArrayToObject(Type dataType, ReadOnlySpan<byte> arrBytes)
    {
        return JsonSerializer.Deserialize(arrBytes, dataType);
    }
}