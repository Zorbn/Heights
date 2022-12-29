using System.Text.Json.Serialization;

namespace Messaging;

public interface Data
{
}

public struct InitializeData : Data
{
    [JsonInclude] public int Id;
}

public struct ExampleNotificationData : Data
{
    [JsonInclude] public string Text;
}

public struct SpawnPlayerData : Data
{
    [JsonInclude] public int Id;
    [JsonInclude] public float X;
    [JsonInclude] public float Y;
}

public struct DestroyPlayerData : Data
{
    [JsonInclude] public int Id;
}

public struct MovePlayerData : Data
{
    [JsonInclude] public int Id;
    [JsonInclude] public float X;
    [JsonInclude] public float Y;
    [JsonInclude] public byte Direction;
    [JsonInclude] public byte Animation;
}

public struct UpdateScoreData : Data
{
    [JsonInclude] public int Id;
    [JsonInclude] public int Score;
}

public class Message
{
    public enum MessageType
    {
        Initialize,
        ExampleNotification,
        SpawnPlayer,
        MovePlayer,
        DestroyPlayer,
        UpdateScore
    }
    
    public static Type ToDataType(MessageType messageType) => messageType switch
    {
        MessageType.Initialize => typeof(InitializeData),
        MessageType.ExampleNotification => typeof(ExampleNotificationData),
        MessageType.SpawnPlayer => typeof(SpawnPlayerData),
        MessageType.MovePlayer => typeof(MovePlayerData),
        MessageType.DestroyPlayer => typeof(DestroyPlayerData),
        MessageType.UpdateScore => typeof(UpdateScoreData),
        _ => throw new ArgumentOutOfRangeException($"No data type corresponds to {messageType}!")
    };

    private readonly MessageType messageType; 
    private readonly Data data;

    public Message(MessageType messageType, Data data)
    {
        this.messageType = messageType;
        this.data = data;
    }

    public byte[] ToByteArray()
    {
        List<byte> bytes = new();
        bytes.AddRange(ByteUtils.ObjectToByteArray(ToDataType(messageType), data));
        bytes.InsertRange(0, BitConverter.GetBytes((int) messageType));
        bytes.InsertRange(0, BitConverter.GetBytes(bytes.Count + sizeof(int))); // Add message length

        return bytes.ToArray();
    }
}