using System.Net;
using System.Net.Sockets;

namespace Messaging;

public static class Client
{
    public delegate void OnConnect();

    public delegate void OnConnectFailed();

    public delegate void OnInitialized(int localId);

    private static TcpClient Socket;
    private static MessageStream MessageStream;

    private static bool IsInitialized;

    private static Dictionary<Message.MessageType, MessageStream.MessageHandler> MessageHandlers;

    private static readonly Dictionary<Message.MessageType, MessageStream.MessageHandler> DefaultMessageHandlers = new()
    {
        { Message.MessageType.Initialize, HandleInitialize }
    };

    private static MessageStream.OnDisconnect OnDisconnectCallback;
    private static OnConnect OnConnectCallback;
    private static OnConnectFailed OnConnectFailedCallback;
    private static OnInitialized OnInitializedCallback;

    public static void StartClient(string ip,
        Dictionary<Message.MessageType, MessageStream.MessageHandler> messageHandlers,
        MessageStream.OnDisconnect onDisconnect, OnConnect onConnect,
        OnConnectFailed onConnectFailed, OnInitialized onInitialized)
    {
        OnDisconnectCallback = onDisconnect;
        OnConnectCallback = onConnect;
        OnConnectFailedCallback = onConnectFailed;
        OnInitializedCallback = onInitialized;

        MessageHandlers = messageHandlers;

        foreach (KeyValuePair<Message.MessageType, MessageStream.MessageHandler> defaultMessageHandler in
                 DefaultMessageHandlers)
            if (!MessageHandlers.ContainsKey(defaultMessageHandler.Key))
                MessageHandlers.Add(defaultMessageHandler.Key, defaultMessageHandler.Value);

        Socket = new TcpClient
        {
            ReceiveBufferSize = MessageStream.DataBufferSize,
            SendBufferSize = MessageStream.DataBufferSize
        };

        Socket.BeginConnect(IpUtils.GetIp(ip), IpUtils.DefaultPort, ConnectCallback, Socket);
    }

    public static void StopClient()
    {
        MessageStream?.StopReading();
        Socket?.Close();
    }

    public static void RegisterHandler(Message.MessageType type, MessageStream.MessageHandler handler)
    {
        if (MessageHandlers is null) return;
        MessageHandlers[type] = handler;
    }

    private static void ConnectCallback(IAsyncResult result)
    {
        try
        {
            Socket.EndConnect(result);
        }
        catch
        {
            OnConnectFailedCallback();
        }

        if (!Socket.Connected) return;

        OnConnectCallback();

        MessageStream = new MessageStream(Socket, 0, MessageHandlers, OnDisconnectCallback);
        MessageStream.StartReading();
    }

    public static void SendMessage(Message.MessageType type, IData data)
    {
        if (!IsInitialized) return;

        MessageStream.SendMessage(type, data);
    }

    private static void HandleInitialize(int fromId, IData data)
    {
        if (data is not InitializeData initData) return;

        IsInitialized = true;
        MessageStream.Id = initData.Id;

        Console.WriteLine($"Initialized with id of: {initData.Id}.");
        OnInitializedCallback(initData.Id);
    }
}