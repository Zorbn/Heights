using System.Text.Json.Serialization;

namespace Messaging;

public interface IData
{
}

public struct InitializeData : IData
{
    [JsonInclude] public int Id;
}

public struct SpawnPlayerData : IData
{
    [JsonInclude] public int Id;
    [JsonInclude] public float X;
    [JsonInclude] public float Y;
    [JsonInclude] public string Name;
    [JsonInclude] public int HighScore;
}

public struct DestroyPlayerData : IData
{
    [JsonInclude] public int Id;
}

public struct MovePlayerData : IData
{
    [JsonInclude] public int Id;
    [JsonInclude] public float X;
    [JsonInclude] public float Y;
    [JsonInclude] public byte Direction;
    [JsonInclude] public byte Animation;
    [JsonInclude] public bool Grounded;
}

public struct UpdateScoreData : IData
{
    [JsonInclude] public int Id;
    [JsonInclude] public int Score;
}

public struct UpdateHighScoreData : IData
{
    [JsonInclude] public int Id;
    [JsonInclude] public int HighScore;
}

public struct UpdateNameData : IData
{
    [JsonInclude] public int Id;
    [JsonInclude] public string Name;
}

public struct HeartbeatData : IData
{
}

public struct DisconnectData : IData
{
}

public readonly struct Message
{
    public enum MessageType
    {
        Initialize,
        SpawnPlayer,
        MovePlayer,
        DestroyPlayer,
        UpdateScore,
        UpdateHighScore,
        UpdateName,
        Heartbeat,
        Disconnect
    }

    private readonly IData data;

    private readonly MessageType messageType;

    public Message(MessageType messageType, IData data)
    {
        this.messageType = messageType;
        this.data = data;
    }

    public static Type ToDataType(MessageType messageType)
    {
        return messageType switch
        {
            MessageType.Initialize => typeof(InitializeData),
            MessageType.SpawnPlayer => typeof(SpawnPlayerData),
            MessageType.MovePlayer => typeof(MovePlayerData),
            MessageType.DestroyPlayer => typeof(DestroyPlayerData),
            MessageType.UpdateScore => typeof(UpdateScoreData),
            MessageType.UpdateHighScore => typeof(UpdateHighScoreData),
            MessageType.UpdateName => typeof(UpdateNameData),
            MessageType.Heartbeat => typeof(HeartbeatData),
            MessageType.Disconnect => typeof(DisconnectData),
            _ => throw new ArgumentOutOfRangeException($"No data type corresponds to {messageType}!")
        };
    }

    public byte[] ToByteArray()
    {
        List<byte> bytes = new();
        bytes.AddRange(ByteUtils.ObjectToByteArray(ToDataType(messageType), data));
        bytes.InsertRange(0, BitConverter.GetBytes((int)messageType));
        bytes.InsertRange(0, BitConverter.GetBytes(bytes.Count + sizeof(int))); // Add message length

        return bytes.ToArray();
    }
}